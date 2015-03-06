using AmlProject;
using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections;
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
    public partial class screen_3_shopping_List : UserControl, ISwitchable
    {
        Environment env;
        private BarcodeReader barcodeReader;
        List<Recipe> currentRecipes = new List<Recipe>();
        Dictionary<int,Product> shoppingList;

        public screen_3_shopping_List(Environment env)
        {
            InitializeComponent();

            this.env = env;
            string users = "";
            if (env.user1.isActive())
            {
                users = users + env.user1.getUserName() + " ";
            }
            if (env.user2.isActive())
            {
                users = users + env.user2.getUserName();
            }
            Dispatcher.BeginInvoke((Action)(() => { top.Text = users; }));
            

            if (env.using_Sensors == true)
            {
                //   intf[0].SensorChange += MainWindow_SensorChange;
                //    intf[1].SensorChange += MainWindow_SensorChange2;
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


            for (int i = 0; i < env.recipebook.amountOfRecipes(); i++)
            {
                currentRecipes.Add(env.recipebook.getRecipe(i));
            }

            if (env.recipeMode == 2)
            {
                currentRecipes = currentRecipes.OrderBy(o => o.cookingTime).ToList();
            }

            //refreshData();
            

            if (env.shoppingLists.Count!=0)
            {
                shoppingList=env.shoppingLists.Peek();
                Product[] products = new Product[16];
                shoppingList.Values.CopyTo(products, 0);  

                if (products[0] != null)
                {
                    ingre1.Content = products[0].getProductName();
                }
                if (products[1] != null)
                {
                    ingre2.Content = products[1].getProductName();
                }
                if (products[2] != null)
                {
                    ingre3.Content = products[2].getProductName();
                }
                if (products[3] != null)
                {
                    ingre4.Content = products[3].getProductName();
                }
                if (products[4] != null)
                {
                    ingre5.Content = products[4].getProductName();
                } if (products[5] != null)
                {
                    ingre6.Content = products[5].getProductName();
                } 
                if (products[6] != null)
                {
                    ingre7.Content = products[6].getProductName();
                }
                if (products[7] != null)
                {
                    ingre8.Content = products[7].getProductName();
                }
                if (products[8] != null)
                {
                    ingre9.Content = products[8].getProductName();
                }
                if (products[9] != null)
                {
                    ingre10.Content = products[9].getProductName();
                }
           

            }

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
                        // this.ScreenLabel.Content = qr_Result.Text;

                        string[] words = qr_Result.Text.Split(' ');

                        if (words[0] == "Product:")
                        {
                            //   productsScanned.Enqueue(new Product(words[1], words[2]));
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
                // env.intf[0].SensorChange -= Fridge_SensorChange;
            }

            if (env.using_Kinect == true)
            {
                HY564KinectManager.Instance().GetKinectSensorChooser().Kinect.ColorFrameReady -= NewSensor_ColorFrameReady;
                BindingOperations.ClearBinding(this.kinectRegion, KinectRegion.KinectSensorProperty);
            }
        }

        void MainWindow_SensorChange(object sender, Phidgets.Events.SensorChangeEventArgs e)
        {

        }


        private void KinectPressButton(object sender, RoutedEventArgs e)
        {
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
                    env.user1.setActive(false);
                    env.user2.setActive(false);
                    ViewSwitcher.Switch(new login(env));
                    break;
                case "ToIngredients":
                    ViewSwitcher.Switch(new screen_3_store_ingredients(env));
                    break;
            }

        }





    }
}
