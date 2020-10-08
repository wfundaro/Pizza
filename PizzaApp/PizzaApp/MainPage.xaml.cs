using Newtonsoft.Json;
using PizzaApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace PizzaApp
{
    public partial class MainPage : ContentPage
    {
        enum E_sort
        {
            SORT_NONE,
            SORT_NAME,
            SORT_PRICE
        }
        E_sort sortListPizza = E_sort.SORT_NONE;
        List<Pizza> pizzas;
        const string KEY_SORT = "sortListPizza";
        string tempFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");
        string jsonFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pizzas.json");
        public MainPage()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey(KEY_SORT))
            {
                sortListPizza = (E_sort)Application.Current.Properties[KEY_SORT];
                sortButton.Source = GetImageSourceFromSort(sortListPizza);
            }
            listViewPizza.RefreshCommand = new Command((obj) =>
            {
                DownloadData((pizzas) =>
                {
                    if(pizzas != null)
                    {
                        listViewPizza.ItemsSource = GetPizzasFromSort(sortListPizza, pizzas);
                    }
                });
                listViewPizza.IsRefreshing = false;
            });
            listViewPizza.IsVisible = false;
            waitLayout.IsVisible = true;
            if (File.Exists(jsonFileName))
            {
                string pizzasJson = File.ReadAllText(jsonFileName);
                if (!String.IsNullOrEmpty(pizzasJson))
                {
                    pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);
                    listViewPizza.ItemsSource = GetPizzasFromSort(sortListPizza, pizzas);
                    listViewPizza.IsVisible = true;
                    waitLayout.IsVisible = false;
                }
            }
            DownloadData((pizzas) =>
            {
                if(pizzas != null)
                {
                    listViewPizza.ItemsSource = GetPizzasFromSort(sortListPizza, pizzas);
                    listViewPizza.IsVisible = true;
                    waitLayout.IsVisible = false;
                }
            });
        }

        public void DownloadData(Action<List<Pizza>> action)
        {
            const string URL = "https://drive.google.com/uc?export=download&id=1VnDkqQpYj7vWl32bfX20LIDoAeXg1FdO";
            using (var webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                {
                    Exception ex = e.Error;
                    if (ex ==null)
                    {
                        File.Copy(tempFileName, jsonFileName, true);
                        string pizzasJson = File.ReadAllText(jsonFileName);
                        pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            action.Invoke(pizzas);
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Erreur", ex.Message, "OK");
                            action.Invoke(null);
                        });
                    }
                    //try
                    //{
                    //    string pizzasJson = File.ReadAllText(jsonFileName);
                    //    pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);
                    //    Device.BeginInvokeOnMainThread(() =>
                    //    {
                    //        action.Invoke(pizzas);
                    //    });
                    //}
                    //catch (Exception ex)
                    //{
                    //    Device.BeginInvokeOnMainThread(() =>
                    //    {
                    //        _ = DisplayAlert("Erreur", ex.Message, "OK");
                    //        action.Invoke(null);
                    //    });

                    //}
                };
                //webClient.DownloadStringAsync(new Uri(URL));
                webClient.DownloadFileAsync(new Uri(URL), tempFileName);
            }
        }
        public void SortButtonClick(object sender, System.EventArgs args)
        {
            sortListPizza++;
            int lastEnumIndex = Enum.GetNames(typeof(E_sort)).Length - 1;
            if ( (int)sortListPizza > lastEnumIndex)
            {
                sortListPizza = E_sort.SORT_NONE;
            }
            sortButton.Source = GetImageSourceFromSort(sortListPizza);
            listViewPizza.ItemsSource = GetPizzasFromSort(sortListPizza, pizzas);
            Application.Current.Properties[KEY_SORT] = (int)sortListPizza;
            Application.Current.SavePropertiesAsync();
        }
        private string GetImageSourceFromSort(E_sort sort)
        {
            string ret;
            switch (sort) {
                case E_sort.SORT_NAME:
                    ret = "sort_nom.png";
                    break;
                case E_sort.SORT_PRICE:
                    ret = "sort_prix.png";
                    break;
                default:
                    ret = "sort_none.png";
                    break;
            }
            return ret;
        }
        private List<Pizza> GetPizzasFromSort(E_sort sort, List<Pizza> pizzas)
        {
            if(pizzas == null)
            {
                return null;
            }
            List<Pizza> ret = new List<Pizza>(pizzas);
            switch (sort)
            {
                case E_sort.SORT_NAME:
                    ret.Sort((p1, p2) => { return p1.titre.CompareTo(p2.titre); });
                    break;
                case E_sort.SORT_PRICE:
                    ret.Sort((p1, p2) => { return p2.prix.CompareTo(p1.prix); });
                    break;
            }
            return ret;
        }
    }
}
