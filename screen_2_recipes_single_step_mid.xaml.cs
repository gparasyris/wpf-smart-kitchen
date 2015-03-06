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
using System.Speech.Synthesis;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class screen_2_recipes_single_step_mid : UserControl, ISwitchable
    {
        Environment env;
        private BarcodeReader barcodeReader;
        public screen_2_recipes_single_step_mid(Environment env)
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
                // intf[0].SensorChange += MainWindow_SensorChange;
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
            env.skipIngredients = false;
            refreshData();
        }


        public void refreshData()
        {
            if (env.recipeStep < env.cookingRecipe.getSteps().Count)
            {
                Dispatcher.BeginInvoke((Action)(() => { StepNumber.Content = "Step " + (env.recipeStep + 1); }));
                Dispatcher.BeginInvoke((Action)(() => { i0.Text = env.cookingRecipe.getSteps().ElementAt(env.recipeStep); }));
                env.speech.SpeakAsync("Step " + (env.recipeStep + 1) + ". " + env.cookingRecipe.getSteps().ElementAt(env.recipeStep));


            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() => { StepNumber.Content = "Recipe Completed"; }));
                Dispatcher.BeginInvoke((Action)(() => { i0.Text = "Congratulations, you have now finished cooking this recipe"; }));
                env.speech.SpeakAsync("Recipe Completed. Congratulations, you have now finished cooking this recipe");
            }

            string temp = env.cookingRecipe.getAssistance().ElementAt(0);
            // VideoAssistance.Source = new Uri(temp); 
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
                case "Next":
                    if (env.recipeStep == env.cookingRecipe.getSteps().Count)
                    {
                        ViewSwitcher.Switch(new screen_1_home_logged_in(env));
                    }
                    else
                    {
                        env.recipeStep = env.recipeStep + 1;
                        refreshData();
                    }
                    break;
                case "Previous":
                    if (env.recipeStep == 0)
                    {
                        ViewSwitcher.Switch(new screen_2_recipes_single_step_1(env));
                    }
                    else
                    {
                        env.recipeStep = env.recipeStep - 1;
                        refreshData();
                    }
                    break;
            }

        }





    }
}
