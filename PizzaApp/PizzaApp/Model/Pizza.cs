using PizzaApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaApp.Model
{
    public class Pizza
    {
        public string nom { get; set; }
        public int prix { get; set; }
        public string[] ingredients { get; set; }
        public string imageUrl { get; set; }

        public string prixEuros { get { return prix + "€"; } }
        public string ingredientsStr { get { return String.Join(", ", ingredients); } }
        public string titre { get { return nom.Capitalize(); } }
        public Pizza()
        {

        }

    }
}
