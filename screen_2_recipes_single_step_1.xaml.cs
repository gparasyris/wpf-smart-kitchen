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
    public partial class screen_2_recipes_single_step_1 : UserControl, ISwitchable
    {
        Environment env;
        private BarcodeReader barcodeReader;
        public screen_2_recipes_single_step_1(Environment env)
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

            refreshData();
            lightControl();
            if (env.skipIngredients == true)
            {
                ingredientsPicked();
            }
        }


        public void refreshData()
        {
            Dictionary<String, String> ingredients = env.cookingRecipe.getIngredients();
            HashSet<String> remIngr = new HashSet<string>();
            for (int i = 0; i < env.removedItems.Count; i++)
            {
                remIngr.Add(env.removedItems.ElementAt(i).getProductName());

            } 
            if (ingredients.Count > 0)
            {
                Dispatcher.BeginInvoke((Action)(() => { i0.Text = ingredients.ElementAt(0).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(0).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i0.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i0.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 1)
            {
                Dispatcher.BeginInvoke((Action)(() => { i1.Text = ingredients.ElementAt(1).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(1).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i1.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i1.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 2)
            {
                Dispatcher.BeginInvoke((Action)(() => { i2.Text = ingredients.ElementAt(2).Value; }));
            }
                if (remIngr.Contains(ingredients.ElementAt(2).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i2.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i2.TextDecorations = null; }));
                }
            if (ingredients.Count > 3)
            {
                Dispatcher.BeginInvoke((Action)(() => { i3.Text = ingredients.ElementAt(3).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(3).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i3.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i3.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 4)
            {
                Dispatcher.BeginInvoke((Action)(() => { i4.Text = ingredients.ElementAt(4).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(4).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i4.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i4.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 5)
            {
                Dispatcher.BeginInvoke((Action)(() => { i5.Text = ingredients.ElementAt(5).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(5).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i5.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i5.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 6)
            {
                Dispatcher.BeginInvoke((Action)(() => { i6.Text = ingredients.ElementAt(6).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(6).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i6.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i6.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 7)
            {
                Dispatcher.BeginInvoke((Action)(() => { i7.Text = ingredients.ElementAt(7).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(7).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i7.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i7.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 8)
            {
                Dispatcher.BeginInvoke((Action)(() => { i8.Text = ingredients.ElementAt(8).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(8).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i8.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i8.TextDecorations = null; }));
                }
            }
            if (ingredients.Count > 9)
            {
                Dispatcher.BeginInvoke((Action)(() => { i9.Text = ingredients.ElementAt(9).Value; }));
                if (remIngr.Contains(ingredients.ElementAt(9).Key))
                {
                    Dispatcher.BeginInvoke((Action)(() => { i9.TextDecorations = TextDecorations.Strikethrough; }));
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() => { i9.TextDecorations = null; }));
                }
            }

            env.speech.SpeakAsync("Gather the ingredients below.");
            for (int i = 0; i < ingredients.Count; i++){
                env.speech.SpeakAsync(ingredients.ElementAt(i).Value + ".");
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
                    lightsOff();
                    ViewSwitcher.Switch(new screen_1_home_logged_in(env));
                    break;
                case "recipes":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_2_recipes(env));
                    break;
                case "shopping":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_3_shopping_List(env));
                    break;
                case "schedule":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "fridge":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_5_fridge(env));
                    break;
                case "alarms":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "preferences":
                    lightsOff();
                    ViewSwitcher.Switch(new screen_0_notInIntermediate(env));
                    break;
                case "logout":
                    lightsOff();
                    ViewSwitcher.Switch(new login(env));
                    break;
                case "Next":
                    lightsOff();
                    env.recipeStep = 0;
                    ViewSwitcher.Switch(new screen_2_recipes_single_step_mid(env));
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
                            if (env.removedItems.Count > 0)
                            {
                                product = env.removedItems.Pop();
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
                lightControl();
                refreshData();
                ingredientsPicked();
            }
            Thread.Sleep(100);
        }


        public void ingredientsPicked()
        {
            HashSet<String> ingredientsLeft = new HashSet<string>();
            ingredientsLeft.UnionWith(env.cookingRecipe.getIngredients().Keys);
            Console.WriteLine("AMOUNT OF INGREDIENTS NEEDED = " + ingredientsLeft.Count);
            for (int i = 0; i < env.removedItems.Count; i++ )
            {
                if (ingredientsLeft.Contains(env.removedItems.ElementAt(i).getProductName()))
                {
                    ingredientsLeft.Remove(env.removedItems.ElementAt(i).getProductName());
                }
            }
            Console.WriteLine("AMOUNT OF INGREDIENTS LEFT = " + ingredientsLeft.Count);

            if (ingredientsLeft.Count == 0)
            {
                Dispatcher.BeginInvoke((Action)(() => {
                    env.speech.SpeakAsyncCancelAll();
                    lightsOff();
                    env.recipeStep = 0;
                    Thread.Sleep(100);
                    ViewSwitcher.Switch(new screen_2_recipes_single_step_mid(env));
                }));

            }
        }


        public void lightControl()
        {
            if (env.using_Sensors == true)
            {
                if (env.cookingRecipe.getIngredients().Keys.Contains(env.fridge.checkSlot(0).getProductName()))
                    env.intf[0].outputs[3] = true;
                else
                    env.intf[0].outputs[3] = false;

                if (env.cookingRecipe.getIngredients().Keys.Contains(env.fridge.checkSlot(1).getProductName()))
                    env.intf[0].outputs[4] = true;
                else
                    env.intf[0].outputs[4] = false;

                if (env.cookingRecipe.getIngredients().Keys.Contains(env.fridge.checkSlot(2).getProductName()))
                    env.intf[0].outputs[6] = true;
                else
                    env.intf[0].outputs[6] = false;

                if (env.cookingRecipe.getIngredients().Keys.Contains(env.fridge.checkSlot(3).getProductName()))
                    env.intf[0].outputs[5] = true;
                else
                    env.intf[0].outputs[5] = false;

                if (env.cookingRecipe.getIngredients().Keys.Contains(env.fridge.checkSlot(4).getProductName()))
                    env.intf[0].outputs[7] = true;
                else
                    env.intf[0].outputs[7] = false;
            }
        }


        public void lightsOff()
        {
            if (env.using_Sensors == true)
            {
                    env.intf[0].outputs[3] = false;
                    env.intf[0].outputs[4] = false;
                    env.intf[0].outputs[6] = false;
                    env.intf[0].outputs[5] = false;
                    env.intf[0].outputs[7] = false;
            }
        }

    }
}
