using AmlProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    public class AllProducts
    {

        List<Product> products = new List<Product>();
        
		public List<Product> getProducts(){
		return products;
		}

        public AllProducts()
        {
            products.Add(new Product("Egg","12/25/2014", "dairy", "Resources\\Products\\egg.jpg"));
            products.Add(new Product("Olive oil", "12/25/2014", "oil", "Resources\\Products\\oil.jpg"));
            products.Add(new Product("Garlic", "12/25/2014", "vegetable/fruit", "Resources\\Products\\garlic.jpg"));
            products.Add(new Product("Potato", "12/25/2014", "vegetable/fruit", "Resources\\Products\\potato.jpg"));
            products.Add(new Product("Rosemary", "12/25/2014", "spice", "Resources\\Products\\rosemary.jpg"));
            products.Add(new Product("Onion", "12/25/2014", "vegetable/fruit", "Resources\\Products\\onion.jpg"));
            products.Add(new Product("Celery", "12/25/2014", "vegetable/fruit", "Resources\\Products\\celery.jpg"));
            products.Add(new Product("Chicken stock", "12/25/2014", "dairy", "Resources\\Products\\chicken-stock.jpg"));
            products.Add(new Product("Peas", "12/25/2014", "vegetable/fruit", "Resources\\Products\\peas.jpg"));
            products.Add(new Product("Leek", "12/25/2014", "vegetable/fruit", "Resources\\Products\\leek.jpg"));
            products.Add(new Product("Vinegar", "12/25/2014", "oil", "Resources\\Products\\vinegar.jpg"));
            products.Add(new Product("Watercress", "12/25/2014", "vegetable/fruit", "Resources\\Products\\watercress.jpg"));
            products.Add(new Product("Sour cream", "12/25/2014", "dairy", "Resources\\Products\\sourcream.jpg"));
            products.Add(new Product("Salt", "12/25/2014", "spice", "Resources\\Products\\salt.jpg"));
            products.Add(new Product("Black pepper", "12/25/2014", "spice", "Resources\\Products\\blackpepper.jpg"));
            products.Add(new Product("Thyme", "12/25/2014", "spice", "Resources\\Products\\thyme.jpg"));
            products.Add(new Product("Creme fraiche", "12/25/2014", "dairy", "Resources\\Products\\cremefraiche.jpg"));
            products.Add(new Product("Vanilla", "12/25/2014", "spice", "Resources\\Products\\vanilla.jpg"));
            products.Add(new Product("Pineapple", "12/25/2014", "vegetable/fruit", "Resources\\Products\\pineapple.jpg"));
            products.Add(new Product("Sugar", "12/25/2014", "spice", "Resources\\Products\\sugar.jpg"));
            products.Add(new Product("Mint", "12/25/2014", "vegetable/fruit", "Resources\\Products\\mint.jpg"));
            products.Add(new Product("Chicken", "12/25/2014", "meat", "Resources\\Products\\chicken.jpg"));
            products.Add(new Product("Carrot", "12/25/2014", "vegetable/fruit", "Resources\\Products\\carrot.jpg"));
            products.Add(new Product("Lemon", "12/25/2014", "vegetable/fruit", "Resources\\Products\\lemon.jpg"));


        }
    }
}