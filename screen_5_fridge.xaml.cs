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
    public partial class screen_5_fridge : UserControl, ISwitchable
    {


        Environment env;
        BarcodeReader barcodeReader;

        public screen_5_fridge(Environment env)
        {
            InitializeComponent();

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
                } else{
                    users = env.user2.getUserName();
                }
            }
            Dispatcher.BeginInvoke((Action)(() => { top.Text = users; }));

            refreshData();

            if (env.using_Sensors == true)
            {
                env.intf[0].SensorChange += Fridge_SensorChange;
            }
        }


        public void refreshData()
        {
            Dispatcher.BeginInvoke((Action)(() => { N0.Text = env.fridge.checkSlot(0).getProductName(); }));
            Dispatcher.BeginInvoke((Action)(() => { V0.Text = env.fridge.checkSlot(0).getProductExpDate(); }));
            Dispatcher.BeginInvoke((Action)(() => { N1.Text = env.fridge.checkSlot(1).getProductName(); }));
            Dispatcher.BeginInvoke((Action)(() => { V1.Text = env.fridge.checkSlot(1).getProductExpDate(); }));
            Dispatcher.BeginInvoke((Action)(() => { N2.Text = env.fridge.checkSlot(2).getProductName(); }));
            Dispatcher.BeginInvoke((Action)(() => { V2.Text = env.fridge.checkSlot(2).getProductExpDate(); }));
            Dispatcher.BeginInvoke((Action)(() => { N3.Text = env.fridge.checkSlot(3).getProductName(); }));
            Dispatcher.BeginInvoke((Action)(() => { V3.Text = env.fridge.checkSlot(3).getProductExpDate(); }));
            Dispatcher.BeginInvoke((Action)(() => { N4.Text = env.fridge.checkSlot(4).getProductName(); }));
            Dispatcher.BeginInvoke((Action)(() => { V4.Text = env.fridge.checkSlot(4).getProductExpDate(); }));

            if (env.scannedProducts.Count > 0)
                Dispatcher.BeginInvoke((Action)(() => { S0.Text = env.scannedProducts.ElementAt(0).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { S0.Text = ""; }));
            if (env.scannedProducts.Count > 1)
                Dispatcher.BeginInvoke((Action)(() => { S1.Text = env.scannedProducts.ElementAt(1).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { S1.Text = ""; }));
            if (env.scannedProducts.Count > 2)
                Dispatcher.BeginInvoke((Action)(() => { S2.Text = env.scannedProducts.ElementAt(2).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { S2.Text = ""; }));
            if (env.scannedProducts.Count > 3)
                Dispatcher.BeginInvoke((Action)(() => { S3.Text = env.scannedProducts.ElementAt(3).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { S3.Text = ""; }));
            if (env.scannedProducts.Count > 4)
                Dispatcher.BeginInvoke((Action)(() => { S4.Text = env.scannedProducts.ElementAt(4).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { S4.Text = ""; }));

            if (env.removedItems.Count > 0)
                Dispatcher.BeginInvoke((Action)(() => { R0.Text = env.removedItems.ElementAt(0).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { R0.Text = ""; }));
            if (env.removedItems.Count > 1)
                Dispatcher.BeginInvoke((Action)(() => { R1.Text = env.removedItems.ElementAt(1).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { R1.Text = ""; }));
            if (env.removedItems.Count > 2)
                Dispatcher.BeginInvoke((Action)(() => { R2.Text = env.removedItems.ElementAt(2).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { R2.Text = ""; }));
            if (env.removedItems.Count > 3)
                Dispatcher.BeginInvoke((Action)(() => { R3.Text = env.removedItems.ElementAt(3).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { R3.Text = ""; }));
            if (env.removedItems.Count > 4)
                Dispatcher.BeginInvoke((Action)(() => { R4.Text = env.removedItems.ElementAt(4).getProductName(); }));
            else
                Dispatcher.BeginInvoke((Action)(() => { R4.Text = ""; }));

        //    lightHandling();
        }

        public void lightHandling(){
            if (env.using_Sensors == true)
            {
                if (env.fridge.checkSlot(0).getProductName() == "empty")
                    env.intf[0].outputs[3] = true;
                else
                    env.intf[0].outputs[3] = false;

                if (env.fridge.checkSlot(1).getProductName() == "empty")
                    env.intf[0].outputs[4] = true;
                else
                    env.intf[0].outputs[4] = false;

                if (env.fridge.checkSlot(2).getProductName() == "empty")
                    env.intf[0].outputs[6] = true;
                else
                    env.intf[0].outputs[6] = false;

                if (env.fridge.checkSlot(3).getProductName() == "empty")
                    env.intf[0].outputs[5] = true;
                else
                    env.intf[0].outputs[5] = false;

                if (env.fridge.checkSlot(4).getProductName() == "empty")
                    env.intf[0].outputs[7] = true;
                else
                    env.intf[0].outputs[7] = false;
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
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ViewSwitcher.Switch(new fscreen());
        }

        void NewSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    var qr_Result = barcodeReader.Decode(colorFrame.GetRawPixelData(), colorFrame.Width, colorFrame.Height,
                                                       ZXing.RGBLuminanceSource.BitmapFormat.RGB32);

                    if (qr_Result != null)
                    {
                        Console.WriteLine("DETECTED QR = " + qr_Result.Text);
                        // this.ScreenLabel.Content = qr_Result.Text;
                        string[] words = qr_Result.Text.Split(' ');

                        //   Dispatcher.BeginInvoke((Action)(() => { LogInf.Content = qr_Result.Text; }));

                        if (words[0] == "Product:")
                        {
                            bool change = true;
                            Product product = new Product(words[1], words[2]);
                            if (env.scannedProducts.Count > 0)
                            {
                                for (int i = 0; i < env.scannedProducts.Count; i++)
                                {
                                    if (env.scannedProducts.ElementAt(i).getProductName() == product.getProductName())
                                    {
                                        change = false;
                                    }
                                }
                            }
                            if (change)
                            {
                                env.scannedProducts.Enqueue(product);
                                refreshData();
                            }

                        }
                        if (words[0] == "User:")
                        {
                            if (words[1] == env.user1.getUserName())
                            {
                                if (!env.user1.isActive())
                                {
                                    env.speech.SpeakAsync("Welcome " + env.user1.getUserName());
                                    env.user1.setActive(true);
                                }

                            }
                            if (words[1] == env.user2.getUserName())
                            {
                                if (!env.user2.isActive())
                                {
                                    env.speech.SpeakAsync("Welcome " + env.user2.getUserName());
                                    env.user2.setActive(true);
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

                refreshData();
            }
            Thread.Sleep(100);
        }

    }
}
