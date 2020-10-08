using Newtonsoft.Json;
using PizzaApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public MainPage()
        {
            InitializeComponent();
            listViewPizza.RefreshCommand = new Command((obj) =>
            {
                DownloadData((pizzas) =>
                {
                    listViewPizza.ItemsSource = pizzas;
                    listViewPizza.IsRefreshing = false;
                });
                listViewPizza.IsRefreshing = false;
            });
            listViewPizza.IsVisible = false;
            waitLayout.IsVisible = true;
            DownloadData((pizzas) =>
            {
                listViewPizza.ItemsSource = pizzas;
                listViewPizza.IsVisible = true;
                waitLayout.IsVisible = false;
            });
        }

        public void DownloadData(Action<List<Pizza>> action)
        {
            const string URL = "https://drive.google.com/uc?export=download&id=1VnDkqQpYj7vWl32bfX20LIDoAeXg1FdO";
            using (var webClient = new WebClient())
            {
                webClient.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                {
                    try
                    {
                        string pizzasJson = e.Result;
                        List<Pizza> pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            action.Invoke(pizzas);
                        });
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _ = DisplayAlert("Erreur", ex.Message, "OK");
                            action.Invoke(null);
                        });
                    }
                };
                webClient.DownloadStringAsync(new Uri(URL));
            }
        }
    }
}
