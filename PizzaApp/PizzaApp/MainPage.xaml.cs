using PizzaApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace PizzaApp
{
    public partial class MainPage : ContentPage
    {
        List<Pizza> pizzas;
        public MainPage()
        {
            InitializeComponent();

            pizzas = new List<Pizza>();
            pizzas.Add(new Pizza { nom = "végétarienne", prix = 7, ingredients = new string[] {"tomate", "poivrons", "oignons" }, imageUrl = "https://cac.img.pmdstatic.net/fit/http.3A.2F.2Fprd2-bone-image.2Es3-website-eu-west-1.2Eamazonaws.2Ecom.2Fcac.2F2018.2F09.2F25.2Ff0f33831-f80e-45b3-9032-593ada3ace5f.2Ejpeg/750x562/quality/80/crop-from/center/cr/wqkgUGF1bGluYSBKQUtPQklFQy9QUklTTUFQSVggLyBDdWlzaW5lIEFjdHVlbGxl/pizza-vegetarienne.jpeg" });
            pizzas.Add(new Pizza { nom = "montagnarde", prix = 11, ingredients = new string[] { "reblochon", "pomme de terre", "lardons", "oignons", "crème" }, imageUrl = "https://dam.savencia-fromagedairy.com/m/78cb7a72755f6137/TH04_320x320-TH04_pizza-au-reblochon.jpg" });
            pizzas.Add(new Pizza { nom = "carnivore", prix = 14, ingredients = new string[] {"tomate", "viande hachée", "mozarella" }, imageUrl = "https://assets.tmecosys.com/image/upload/t_web767x639/img/recipe/vimdb/83535.jpg" });
            
            listViewPizza.ItemsSource = pizzas;
        }
    }
}
