using AmlProject;
using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class screen_2_recipes_grid : UserControl, ISwitchable
    {
        Environment env;
        private BarcodeReader barcodeReader;
        List<Recipe> currentRecipes;
        public screen_2_recipes_grid(Environment env)
        {
            InitializeComponent();

            this.env = env;
            string users = "";
            if (env.user1.isActive())
            {
                users = env.user1.getUserName();
            }
            if (env.user2.isActive())
            {
                if (env.user1.isActive())
                {
                    users = users + " and " + env.user2.getUserName();
                }
                else
                {
                    users = env.user2.getUserName();
                }
            }
            Dispatcher.BeginInvoke((Action)(() => { top.Text = users; }));



            if (env.using_Sensors == true)
            {
                env.intf[0].SensorChange += Fridge_SensorChange;
            }

            if (env.using_Kinect == true)
            {
                var regionSensorBinding = new Binding("Kinect")
                {
                    Source =
                        HY564KinectManager.Instance().GetKinectSensorChooser()
                };
                BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

                HY564KinectManager.Instance().GetKinectSensorChooser().Kinect.ColorFrameReady += NewSensor_ColorFrameReady;
                barcodeReader = new BarcodeReader();
                barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE };
            }
            logic();
            
        }



        public void logic()
        {
            getAllRecipes();
            switch (env.recipeMode)
            {
                case 1:
                    arrangeRecipesByIngredientAvailability();
                    break;
                case 2:
                    arrangeRecipesByCookingTime();
                    break;
            }
            reorderRecipesByPickedUpIngredients();
            allergiesStuff();
            refreshData();
        }


        public void arrangeRecipesByIngredientAvailability()
        {
            HashSet<String> availableIngredients = new HashSet<string>();
            for (int i = 0; i < env.fridge.getSize(); i++){
                String name = env.fridge.checkSlot(i).getProductName();
                if (name != "empty"){
                    availableIngredients.Add(name);
                }
            }
            for (int i = 0; i < env.removedItems.Count; i++)
            {
                availableIngredients.Add(env.removedItems.ElementAt(i).getProductName());
            }

            List<CompoundRecipe> recipesToOrder = new List<CompoundRecipe>();
            for (int i = 0; i < currentRecipes.Count; i++)
            {
                int match = 0;
                for (int j = 0; j < availableIngredients.Count; j++)
                {
                    if (currentRecipes.ElementAt(i).getIngredients().Keys.Contains(availableIngredients.ElementAt(j)))
                        match++;
                }
                recipesToOrder.Add(new CompoundRecipe(currentRecipes.ElementAt(i), match));
            }
            recipesToOrder = recipesToOrder.OrderBy(o => o.value).Reverse().ToList();
            currentRecipes = new List<Recipe>();
            for (int i = 0; i < recipesToOrder.Count; i++)
            {
                currentRecipes.Add(recipesToOrder.ElementAt(i).recipe); 
            }
        }


        private void arrangeRecipesByCookingTime()
        {
            currentRecipes = currentRecipes.OrderBy(o => o.cookingTime).ToList();
        }

        private void getAllRecipes()
        {
            currentRecipes = new List<Recipe>();
            for (int i = 0; i < env.recipebook.amountOfRecipes(); i++)
            {
                currentRecipes.Add(env.recipebook.getRecipe(i));
            }
        }


        public void reorderRecipesByPickedUpIngredients(){
            HashSet<String> availableIngredients = new HashSet<string>();
            for (int i = 0; i < env.removedItems.Count; i++)
            {
                availableIngredients.Add(env.removedItems.ElementAt(i).getProductName());
            }
            List<CompoundRecipe> recipesToOrder = new List<CompoundRecipe>();
            for (int i = 0; i < currentRecipes.Count; i++)
            {
                int match = 0;
                for (int j = 0; j < availableIngredients.Count; j++)
                {
                    if (currentRecipes.ElementAt(i).getIngredients().Keys.Contains(availableIngredients.ElementAt(j)))
                    {
                        match++;
                    }
                }
                recipesToOrder.Add(new CompoundRecipe(currentRecipes.ElementAt(i), match));
                Console.WriteLine("INGREDIENTS: " + match + " RECIPE = " + currentRecipes.ElementAt(i).getName());
            }

            CompoundRecipe whatever;
            int removed;
            for (int j = 1; j <= availableIngredients.Count; j++)
            {
                Console.WriteLine("AvailableIngredients: " + j);
                removed = 0;
                for (int i = 0; i < recipesToOrder.Count; i++)
                {
                    if (recipesToOrder.ElementAt(i - removed).value == j)
                    {
                        whatever = recipesToOrder.ElementAt(i - removed);
                        Console.WriteLine("whatever: " + whatever.recipe.getName());
                        recipesToOrder.RemoveAt(i - removed);
                        recipesToOrder.Insert(0, whatever);
                        if (i != 0)
                        {
                            removed++;
                        }
                    }
                }
            }
            currentRecipes = new List<Recipe>();
            for (int i = 0; i < recipesToOrder.Count; i++)
            {
                currentRecipes.Add(recipesToOrder.ElementAt(i).recipe);
            }
        }

        public void allergiesStuff()
        {
            List<int> toRemove = new List<int>();
            HashSet<String> allergies = new HashSet<string>();
            if (env.user1.isActive())
            {
                allergies.UnionWith(env.user1.getAllergies());
            }
            if (env.user2.isActive())
            {
                allergies.UnionWith(env.user2.getAllergies());
            }

            for (int i = 0; i < currentRecipes.Count; i++)
            {
                for (int j = 0; j < allergies.Count; j++)
                {
                    if (currentRecipes.ElementAt(i).getIngredients().Keys.Contains(allergies.ElementAt(j)))
                    {
                        toRemove.Add(i);
                    }
                }
            }

            int removed = 0;

            Recipe whatever;
            for (int i = 0; i < toRemove.Count; i++)
            {
                whatever = currentRecipes.ElementAt(toRemove.ElementAt(i) - removed);
                currentRecipes.RemoveAt(toRemove.ElementAt(i) - removed);
                currentRecipes.Add(whatever);
                removed++;
            }
        }

        public void refreshData()
        {
            if (currentRecipes.Count > 0)
            {
                Dispatcher.BeginInvoke((Action)(() => {rec1.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(0).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec1.Label = currentRecipes.ElementAt(0).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br1.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 1)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec2.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(1).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec2.Label = currentRecipes.ElementAt(1).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br2.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 2)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec3.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(2).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec3.Label = currentRecipes.ElementAt(2).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br3.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 3)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec4.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(3).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec4.Label = currentRecipes.ElementAt(3).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br4.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 4)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec5.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(4).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec5.Label = currentRecipes.ElementAt(4).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br5.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 5)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec6.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(5).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec6.Label = currentRecipes.ElementAt(5).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br6.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 6)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec7.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(6).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec7.Label = currentRecipes.ElementAt(6).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => {br7.Visibility = Visibility.Hidden; })); }
            if (currentRecipes.Count > 7)
            {
                Dispatcher.BeginInvoke((Action)(() => { rec8.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), currentRecipes.ElementAt(7).getPicturePath()))); }));
                Dispatcher.BeginInvoke((Action)(() => { rec8.Label = currentRecipes.ElementAt(7).getName(); }));
            }
            else { Dispatcher.BeginInvoke((Action)(() => { br8.Visibility = Visibility.Hidden; })); }
        }


        void NewSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    //UpdateKinectFrame(colorFrame);
                    var qr_Result = barcodeReader.Decode(colorFrame.GetRawPixelData(), colorFrame.Width, colorFrame.Height,
                                            ZXing.RGBLuminanceSource.BitmapFormat.RGB32);
                    if (qr_Result != null)
                    {
                        Console.WriteLine("DETECTED QR = " + qr_Result.Text);
                        // this.ScreenLabel.Content = qr_Result.Text;
                        string[] words = qr_Result.Text.Split(' ');

                        if (words[0] == "User:")
                        {
                            if (words[1] == env.user1.getUserName())
                            {
                                if (!env.user1.isActive())
                                {
                                    env.speech.SpeakAsync("Welcome " + env.user1.getUserName());
                                    env.user1.setActive(true);
                                    logic();
                                }

                            }
                            if (words[1] == env.user2.getUserName())
                            {
                                if (!env.user2.isActive())
                                {
                                    env.speech.SpeakAsync("Welcome " + env.user2.getUserName());
                                    env.user2.setActive(true);
                                    logic();
                                }
                            }
                            string users = "";
                            if (env.user1.isActive())
                            {
                                users = env.user1.getUserName();
                            }
                            if (env.user2.isActive())
                            {
                                if (env.user1.isActive())
                                {
                                    users = users + " and " + env.user2.getUserName();
                                }
                                else
                                {
                                    users = env.user2.getUserName();
                                }
                            }
                            Dispatcher.BeginInvoke((Action)(() => { top.Text = users; }));
                        }
                    }
                }

            }
        }


        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            if (env.using_Sensors == true)
            {
                env.intf[0].SensorChange -= Fridge_SensorChange;
            }

            if (env.using_Kinect == true)
            {
                HY564KinectManager.Instance().GetKinectSensorChooser().Kinect.ColorFrameReady -= NewSensor_ColorFrameReady;
                BindingOperations.ClearBinding(this.kinectRegion, KinectRegion.KinectSensorProperty);
            }
        }


        private void KinectPressButton(object sender, RoutedEventArgs e)
        {
            env.speech.SpeakAsyncCancelAll();
            String name = ((Microsoft.Kinect.Toolkit.Controls.KinectTileButton)e.OriginalSource).Name;
            switch (name)
            {
                case "home":
                    ViewSwitcher.Switch(new screen_1_home_logged_in(env));
                    break;
                case "recipes":
                    ViewSwitcher.Switch(new screen_2_recipes(env));
                    break;
                case "shopping":
                    ViewSwitcher.Switch(new screen_3_shopping_List(env));
                    break;
                case "schedule":
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "fridge":
                    ViewSwitcher.Switch(new screen_5_fridge(env));
                    break;
                case "alarms":
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "preferences":
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "logout":
                    ViewSwitcher.Switch(new login(env));
                    break;

                //single
                case "rec1":
                    env.cookingRecipe = currentRecipes.ElementAt(0);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;

                case "rec2":
                    env.cookingRecipe = currentRecipes.ElementAt(1);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec3":
                    env.cookingRecipe = currentRecipes.ElementAt(2);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec4":
                    env.cookingRecipe = currentRecipes.ElementAt(3);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec5":
                    env.cookingRecipe = currentRecipes.ElementAt(4);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec6":
                    env.cookingRecipe = currentRecipes.ElementAt(5);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec7":
                    env.cookingRecipe = currentRecipes.ElementAt(6);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
                case "rec8":
                    env.cookingRecipe = currentRecipes.ElementAt(7);
                    Console.WriteLine("RECIPE PRESSED = " + env.cookingRecipe.getName());
                    ViewSwitcher.Switch(new screen_2_recipes_single(env));
                    break;
            }

        }

        void Fridge_SensorChange(object sender, Phidgets.Events.SensorChangeEventArgs e)
        {
            int index = e.Index;
            int value = e.Value;
            int fridgeSlot = index - 3;
            if (index > 2 && index < 8)
            {
                Product product = new Product("empty", "no");
                if (value < 900)
                {
                    Console.WriteLine("PRESSED SENSOR = " + index);
                    if (env.fridge.checkSlot(fridgeSlot).getProductName() == "empty")
                    {
                        if (env.scannedProducts.Count > 0)
                        {
                            product = env.scannedProducts.Dequeue();
                        }
                        else
                        {
                            if (env.removedItems.Count > 0)
                            {
                                product = env.removedItems.Pop();
                            }
                        }
                        env.fridge.addProductToFridge(product, fridgeSlot);
                    }
                }
                else
                {
                    if (env.fridge.checkSlot(index - 3).getProductName() != "empty")
                    {
                        product = env.fridge.removeProductFromFridge(fridgeSlot);
                        if (product.getProductName() != "empty")
                        {
                            env.removedItems.Push(product);
                        }
                    }
                }

                logic();
            }
            Thread.Sleep(100);
        }

    private class CompoundRecipe{
        public Recipe recipe;
        public int value;

        public CompoundRecipe(Recipe recipe, int value)
        {
            this.recipe = recipe;
            this.value = value;
        }
    }

    }
}
