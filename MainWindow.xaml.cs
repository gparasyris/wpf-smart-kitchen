using AmlProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
   

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        Environment env;
        ISwitchable currentView;
        public void Navigate(UserControl view)
        {
            //Dispose resources from previous screen
            if (currentView != null)
                currentView.Destroy();
            currentView = view as ISwitchable;
            //Clear the contents of the main window and add the new screen
            mainContent.Children.Clear();
            mainContent.Children.Add(view);
        }

        public void Navigate(UserControl view, object state)
        {
            //Call basic Navigate method
            Navigate(view);
            //Pass the state to the new screen
            if (currentView != null)
                currentView.UtilizeState(state);
        }


        public MainWindow()
        {
            InitializeComponent();

            env = new Environment();
            env.using_Kinect = false;
            env.using_Sensors = false;
            env.user1 = new Person("John");
            env.user2 = new Person("Emily");
            env.user1.addAllergie("Leek");
            env.user2.addAllergie("Egg");
            env.fridge = new Fridge();
            env.removedItems = new Stack<Product>();
            env.scannedProducts = new Queue<Product>();
            env.recipebook = new RecipeBook();
            env.allProducts = new AllProducts();

            env.removedItems.Push(new Product("Milk", "any"));
//            env.removedItems.Push(new Product("Butter", "any"));
            env.removedItems.Push(new Product("Egg", "any"));
            env.shoppingLists = new Stack<Dictionary<int, Product>>(); 


            ViewSwitcher.SetMainWindow(this);
            Navigate(new login(env));      
        }
    }
}

