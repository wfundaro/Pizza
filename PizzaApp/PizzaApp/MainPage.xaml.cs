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
            SORT_PRICE,
            SORT_FAV
        }
        E_sort sortListPizza = E_sort.SORT_NONE;
        List<Pizza> pizzas;
        List<string> favoryPizzas = new List<string>();
        const string KEY_SORT = "sortListPizza";
        const string KEY_FAV = "fav";
        string tempFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");
        string jsonFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pizzas.json");
        public MainPage()
        {
            InitializeComponent();
            // read favory
            LoadFavList();
            // check sorting mode
            if (Application.Current.Properties.ContainsKey(KEY_SORT))
            {
                sortListPizza = (E_sort)Application.Current.Properties[KEY_SORT];
                sortButton.Source = GetImageSourceFromSort(sortListPizza);
            }
            // Command for favories buttons
            listViewPizza.RefreshCommand = new Command((obj) =>
            {
                DownloadData((pizzas) =>
                {
                    if(pizzas != null)
                    {
                        listViewPizza.ItemsSource = GetPizzaCells(GetPizzasFromSort(sortListPizza, pizzas), favoryPizzas);
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
                    listViewPizza.ItemsSource = GetPizzaCells(GetPizzasFromSort(sortListPizza, pizzas), favoryPizzas);
                    listViewPizza.IsVisible = true;
                    waitLayout.IsVisible = false;
                }
            }
            DownloadData((pizzas) =>
            {
                if(pizzas != null)
                {
                    listViewPizza.ItemsSource = GetPizzaCells(GetPizzasFromSort(sortListPizza, pizzas), favoryPizzas);
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
            listViewPizza.ItemsSource = GetPizzaCells(GetPizzasFromSort(sortListPizza, pizzas), favoryPizzas);
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
                case E_sort.SORT_FAV:
                    ret = "sort_fav.png";
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
                case E_sort.SORT_FAV:
                    ret.Sort((p1, p2) => { return p1.titre.CompareTo(p2.titre); });
                    break;
                case E_sort.SORT_PRICE:
                    ret.Sort((p1, p2) => { return p2.prix.CompareTo(p1.prix); });
                    break;
            }
            return ret;
        }

        private List<PizzaCell> GetPizzaCells(List<Pizza> p, List<string> f)
        {
            List<PizzaCell> ret = new List<PizzaCell>();
            if (p == null)
            {
                return ret;
            }
            foreach(Pizza pizza in p)
            {
                bool isFav = f.Contains(pizza.nom);
                if(sortListPizza == E_sort.SORT_FAV)
                {
                    if (isFav)
                    {
                        ret.Add(new PizzaCell { pizza = pizza, isFavorite = isFav, favChangedAction = OnFavPizzaChanged });
                    }
                } else
                {
                    ret.Add(new PizzaCell { pizza = pizza, isFavorite = isFav, favChangedAction = OnFavPizzaChanged });
                }
            }
            return ret;
        }

        private void OnFavPizzaChanged(PizzaCell pizzacell)
        {
            bool isInFavList = favoryPizzas.Contains(pizzacell.pizza.nom);
            if(pizzacell.isFavorite && !isInFavList)
            {
                favoryPizzas.Add(pizzacell.pizza.nom);
                SaveFavList();
            } 
            else if(!pizzacell.isFavorite && isInFavList)
            {
                favoryPizzas.Remove(pizzacell.pizza.nom);
                SaveFavList();
            }
        }
        private void SaveFavList()
        {
            string json = JsonConvert.SerializeObject(favoryPizzas);
            Application.Current.Properties[KEY_FAV] = json;
            Application.Current.SavePropertiesAsync();
        }
        private void LoadFavList()
        {
            if (Application.Current.Properties.ContainsKey(KEY_FAV))
            {
                string json = Application.Current.Properties[KEY_FAV].ToString();
                favoryPizzas = JsonConvert.DeserializeObject<List<string>>(json);
            }
        }
    }
}
