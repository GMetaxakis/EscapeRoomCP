using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EscapeRoomCP
{
    /// <summary>
    /// Interaction logic for SecondWindows.xaml
    /// </summary>
    public partial class SecondWindow : Window
    {
        System.Windows.Threading.DispatcherTimer fileTimer, messageTimer;
        string audioFileLocation;

        public SecondWindow()
        {
            InitializeComponent();
            loadSettings();

        }

        private bool _inStateChange;

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Maximized && !_inStateChange)
            {
                _inStateChange = true;
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                _inStateChange = false;
            }
            base.OnStateChanged(e);
        }

        internal void loadSettings()
        {

            try
            {
                FontFamily selectedFont = new FontFamily(Properties.Settings.Default.timerFont.Name);
                Brush selectedColor = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(Properties.Settings.Default.timerColor.Name);

                timerLbl.FontFamily = selectedFont;
                timerLbl.Foreground = selectedColor;
                timerLbl.FontSize = Properties.Settings.Default.timerSize;

                timerSmallLbl.Foreground = selectedColor;
                timerSmallLbl.FontFamily = selectedFont;
                timerSmallLbl.FontSize = Properties.Settings.Default.timer2Size;

                messageTextBlock.Foreground = selectedColor;
                messageTextBlock.FontFamily = selectedFont;
                messageTextBlock.FontSize = Properties.Settings.Default.textSize;
                          
                audioFileLocation = Properties.Settings.Default.beepFileLocation;
                
                if (Properties.Settings.Default.backgroundImageSetted==true)
                {
                    ImageBrush myBrush = new ImageBrush();
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(Properties.Settings.Default.backgroundImageLocation));
                    myBrush.ImageSource = image.Source;
                    Background = myBrush;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("load settinsg error (sc) " + ex.Message);
                //Close();
            }
        }

        private void beep()
        {
            try
            {
                if (Properties.Settings.Default.beepOnUpdate)
                {
                    if (audioFileLocation == null || audioFileLocation.Equals(""))
                        return;
                    
                    //System.Media.SystemSounds.Beep.Play();

                    MediaPlayer beepSound = new MediaPlayer();
                    
                    beepSound.Open(new Uri(audioFileLocation));
                    beepSound.Volume = 100;
                    beepSound.Play(); //Play the media
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("error with audio file");
            }
        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length >= 2)
            {
                //System.Drawing.Rectangle screenBounds = System.Windows.Forms.Screen.AllScreens[2].Bounds;

                //this.Left = screenBounds.X;
                //this.Top = screenBounds.Y;
                //this.Width = screenBounds.Width;
                //this.Height = screenBounds.Height;
            }
            //this.WindowState = WindowState.Maximized;
        }


        internal void timer_timerDecreased(object sender, EventArgs e)
        {
            string time = (e as MyEventArgs).ToString();
            timerLbl.Content = time;
            timerSmallLbl.Content = time;
        }

        private void hideViews()
        {
            image.Visibility = Visibility.Hidden;
            video.Visibility = Visibility.Hidden;
            timerLbl.Visibility = Visibility.Hidden;
            messageTextBlock.Visibility = Visibility.Hidden;
            timerSmallLbl.Visibility = Visibility.Hidden;
        }

        internal void showMessageForTime(string message, int seconds)
        {
            hideViews();
            messageTextBlock.Visibility = Visibility.Visible;
            timerSmallLbl.Visibility = Visibility.Visible;

            messageTextBlock.Text = message;

            if (messageTimer != null)
            {
                messageTimer.Stop();
                messageTimer = null;
            }

            messageTimer = new System.Windows.Threading.DispatcherTimer();
            messageTimer.Tick += new EventHandler(OnMessageTimer);
            messageTimer.Interval = new TimeSpan(0, 0, seconds);
            messageTimer.Start();

            beep();
        }

        internal void showFile(Uri fileUri, string tag, int seconds)
        {
            hideViews();
            timerSmallLbl.Visibility = Visibility.Visible;

            if (tag == "image")
            {
                image.Visibility = Visibility.Visible;
                BitmapImage bitmapImage = new BitmapImage(fileUri);
                image.Source = bitmapImage;
            }
            else if (tag == "video")
            {
                video.Visibility = Visibility.Visible;
                video.Source = fileUri;
                video.Play();
            }

            if (seconds != 0)
                startMediaTimer(seconds);

            beep();
        }

        private void startMediaTimer(int seconds)
        {
            if (fileTimer != null)
            {
                fileTimer.Stop();
                fileTimer = null;
            }
            fileTimer = new System.Windows.Threading.DispatcherTimer();
            fileTimer.Tick += new EventHandler(OnFileTimer);
            fileTimer.Interval = new TimeSpan(0, 0, seconds);
            fileTimer.Start();
        }

        internal void showTimer()
        {
            hideViews();
            timerLbl.Visibility = Visibility.Visible;
        }

        private void OnFileTimer(object sender, EventArgs e)
        {
            if (video.HasVideo)
                video.Stop();
            showTimer();
        }

        private void OnMessageTimer(object sender, EventArgs e)
        {
            showTimer();
        }




        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
