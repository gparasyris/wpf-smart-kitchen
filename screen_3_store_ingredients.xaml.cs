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
    public partial class screen_3_store_ingredients : UserControl, ISwitchable
    {
        Environment env;
        Phidgets.Manager PM;
        Phidgets.InterfaceKit[] intf;
        private BarcodeReader barcodeReader;
        Queue<Product> productsScanned;
        Dictionary<int,Product> shoppingList;
        ImageBrush greenBrush;
        public screen_3_store_ingredients(Environment env)
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

            //populate the screen with the products available

            List<Product> products = new List<Product>();
            products = env.allProducts.getProducts();
            Dispatcher.BeginInvoke((Action)(() => { ing1.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[0].getPicturePath()))); 
                                                    ing2.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[1].getPicturePath()))); 
                                                    ing3.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[2].getPicturePath()))); 
                                                    ing4.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[3].getPicturePath()))); 
                                                    ing5.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[4].getPicturePath())));
                                                    ing6.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[5].getPicturePath())));
                                                    ing7.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[6].getPicturePath())));
                                                    ing8.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[7].getPicturePath()))); 
                                                    ing9.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[8].getPicturePath()))); 
                                                    ing10.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[9].getPicturePath())));
                                                    ing11.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[10].getPicturePath()))); 
                                                    ing12.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[11].getPicturePath())));
                                                    ing13.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[12].getPicturePath()))); 
                                                    ing14.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[13].getPicturePath())));
                                                    ing15.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[14].getPicturePath()))); 
                                                    ing16.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), products[15].getPicturePath()))); }));
                                                    

            //initiallize greenBrush
            greenBrush =new ImageBrush( new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),"Resources\\colors\\darkgreen.png" ))); 

             
            //create an empty shopping list to add products   
            shoppingList = new Dictionary<int, Product>();
            
            productsScanned = new Queue<Product>();

            if (env.using_Sensors == true)
            {
            //    intf[0].SensorChange += MainWindow_SensorChange;
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
                            productsScanned.Enqueue(new Product(words[1], words[2]));
                        }
                    }
                }
            }
        }

        private WriteableBitmap _outputBitmap = null;
        private byte[] _pixelData = new byte[0];

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {


        }

        void MainWindow_SensorChange(object sender, Phidgets.Events.SensorChangeEventArgs e)
        {

            if (e.Index == 0 && e.Value < 500)
            {

                if (productsScanned.Count > 0 && env.fridge.checkSlot(e.Index).getProductName() == "empty")
                {
                    env.fridge.addProductToFridge(productsScanned.Dequeue(), e.Index);

                }
            }

            if (e.Index == 1 && e.Value < 500)
            {
                if (productsScanned.Count > 0 && env.fridge.checkSlot(e.Index).getProductName() == "empty")
                {
                    env.fridge.addProductToFridge(productsScanned.Dequeue(), e.Index);

                }
            }

            //    switch (e.Index)
            //  {
            //    case 0: Dispatcher.BeginInvoke((Action)(() => { touchValue.Content = e.Value; }));
            //      break;

            //            }

        }


        private void KinectPressButton(object sender, RoutedEventArgs e)
        {
            String name = ((Microsoft.Kinect.Toolkit.Controls.KinectTileButton)e.OriginalSource).Name;
            int b=0;
            if (name.Contains("ing") && name.Length < 6){
                b = Convert.ToInt32(name.Substring(3));
                name = "ing";
            }
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
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "saveList":
                    env.shoppingLists.Push(shoppingList);
                    ViewSwitcher.Switch(new screen_3_shopping_List(env));
                    break;

                case "ing":
                    switch (b)
                    {
                        case 1:
                            if (bor1.Background == Brushes.Yellow)
                            {
                                bor1.Background = greenBrush;
                                bor1.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor1.Background = Brushes.Yellow;
                                bor1.Padding = new Thickness(8);
                            }

                            break;
                        case 2:
                            if (bor2.Background == Brushes.Yellow)
                            {
                                bor2.Background = greenBrush;
                                bor2.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor2.Background = Brushes.Yellow;
                                bor2.Padding = new Thickness(8);
                            }

                            break;
                        case 3:
                            if (bor3.Background == Brushes.Yellow)
                            {
                                bor3.Background = greenBrush;
                                bor3.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor3.Background = Brushes.Yellow;
                                bor3.Padding = new Thickness(8);
                            }

                            break;
                        case 4:
                            if (bor4.Background == Brushes.Yellow)
                            {
                                bor4.Background = greenBrush;
                                bor4.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor4.Background = Brushes.Yellow;
                                bor4.Padding = new Thickness(8);
                            }

                            break;
                        case 5:
                            if (bor5.Background == Brushes.Yellow)
                            {
                                bor5.Background = greenBrush;
                                bor5.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor5.Background = Brushes.Yellow;
                                bor5.Padding = new Thickness(8);
                            }

                            break;
                        case 6:
                            if (bor6.Background == Brushes.Yellow)
                            {
                                bor6.Background = greenBrush;
                                bor6.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor6.Background = Brushes.Yellow;
                                bor6.Padding = new Thickness(8);
                            }

                            break;
                        case 7:
                            if (bor7.Background == Brushes.Yellow)
                            {
                                bor7.Background = greenBrush;
                                bor7.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor7.Background = Brushes.Yellow;
                                bor7.Padding = new Thickness(8);
                            }

                            break;
                        case 8:
                            if (bor8.Background == Brushes.Yellow)
                            {
                                bor8.Background = greenBrush;
                                bor8.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor8.Background = Brushes.Yellow;
                                bor8.Padding = new Thickness(8);
                            }

                            break;
                        case 9:
                            if (bor9.Background == Brushes.Yellow)
                            {
                                bor9.Background = greenBrush;
                                bor9.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor9.Background = Brushes.Yellow;
                                bor9.Padding = new Thickness(8);
                            }

                            break;
                        case 10:
                            if (bor10.Background == Brushes.Yellow)
                            {
                                bor10.Background = greenBrush;
                                bor10.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor10.Background = Brushes.Yellow;
                                bor10.Padding = new Thickness(8);
                            }

                            break;
                        case 11:
                            if (bor11.Background == Brushes.Yellow)
                            {
                                bor11.Background = greenBrush;
                                bor11.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor11.Background = Brushes.Yellow;
                                bor11.Padding = new Thickness(8);
                            }

                            break;
                        case 12:
                            if (bor12.Background == Brushes.Yellow)
                            {
                                bor12.Background = greenBrush;
                                bor12.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor12.Background = Brushes.Yellow;
                                bor12.Padding = new Thickness(8);
                            }

                            break;
                        case 13:
                            if (bor13.Background == Brushes.Yellow)
                            {
                                bor13.Background = greenBrush;
                                bor13.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor13.Background = Brushes.Yellow;
                                bor13.Padding = new Thickness(8);
                            }

                            break;
                        case 14:
                            if (bor14.Background == Brushes.Yellow)
                            {
                                bor14.Background = greenBrush;
                                bor14.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor14.Background = Brushes.Yellow;
                                bor14.Padding = new Thickness(8);
                            }

                            break;
                        case 15:
                            if (bor15.Background == Brushes.Yellow)
                            {
                                bor15.Background = greenBrush;
                                bor15.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor15.Background = Brushes.Yellow;
                                bor15.Padding = new Thickness(8);
                            }

                            break;
                        case 16:
                            if (bor16.Background == Brushes.Yellow)
                            {
                                bor16.Background = greenBrush;
                                bor16.Padding = new Thickness(2);
                                shoppingList.Remove(b);
                            }
                            else
                            {
                                shoppingList.Add(b, env.allProducts.getProducts()[b - 1]);
                                bor16.Background = Brushes.Yellow;
                                bor16.Padding = new Thickness(8);
                            }

                            break;
                       
                    }

                    break;

            }

        }





    }
}
