using System;
using System.Collections.Generic;
using System.Configuration;
using System.Media;
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
    /// Interaction logic for FirstWindow.xaml
    /// </summary>
    public partial class FirstWindow : Window
    {
        Time time;
        SecondWindow secondWindow;
        public FirstWindow()
        {
            try
            {
                InitializeComponent();
                loadSettings();


                time = new Time();
                
                secondWindow = new SecondWindow();
                secondWindow.Show();

                time.timerDecreased += time_timerDecreased;
                time.timerDecreased += secondWindow.timer_timerDecreased;

                this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("MyHandler error " + e.Message);
        }
        
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.ThreeDBorderWindow;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            secondWindow.Close();
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

        void loadSettings()
        {

            try
            {
                startValueOfTimeTB.Text = Properties.Settings.Default.defaultStartValueOfTimerInMinutes + "";
                miniTimerTB.Text = Properties.Settings.Default.defaultStartValueOfSecondTimerInSeconds + "";

                colorCB.SelectedValue = Properties.Settings.Default.timerColor.Name;
                //fcolorCB.SelectedValue = Properties.Settings.Default.timerFinishedColor.Name;

                fontCB.SelectedValue = new FontFamily(Properties.Settings.Default.timerFont.Name);

                Brush selectedColor = (Brush)(new System.Windows.Media.BrushConverter()).ConvertFromString(Properties.Settings.Default.timerColor.Name);
                FontFamily selectedFont = fontCB.SelectedValue as FontFamily;

                scTimer.FontFamily = selectedFont;
                scTimer.Foreground = selectedColor;
                scTimer.FontSize = Properties.Settings.Default.timerSize / 3;

                scSmallTimer.FontFamily = selectedFont;
                scSmallTimer.Foreground = selectedColor;
                scSmallTimer.FontSize = Properties.Settings.Default.timer2Size / 3;


                scMessage.Foreground = selectedColor;
                scMessage.FontFamily = selectedFont;
                scMessage.FontSize = Properties.Settings.Default.textSize / 3;

                timerSizeTB.Text = Properties.Settings.Default.timerSize + "";
                timer2SizeTB.Text = Properties.Settings.Default.timer2Size + "";
                textSizeTB.Text = Properties.Settings.Default.textSize + "";

                audioFileLocation = Properties.Settings.Default.beepFileLocation;

                textIpCam1.Text = Properties.Settings.Default.cameraIp1;
                textIpCam2.Text = Properties.Settings.Default.cameraIp2;

               
                if (Properties.Settings.Default.backgroundImageSetted == true)
                {
                    ImageBrush myBrush = new ImageBrush();
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(Properties.Settings.Default.backgroundImageLocation));
                    myBrush.ImageSource = image.Source;
                    secondWindowFrame.Background = myBrush;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("app.config file error " + ex.Message);
                //Close();
            }
        }

        void time_timerDecreased(object sender, EventArgs e)
        {
            scTimer.Content = (e as MyEventArgs).ToString();
            scSmallTimer.Content = (e as MyEventArgs).ToString();
            currTimeLeftLbl.Content = (e as MyEventArgs).ToString();
        }

        string fileToSentTag;
        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            if (video.HasVideo)
                video.Stop();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "All Files|*.jpg;*.jpeg;*.png;*.avi;*.mp4;*.wmv;";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                fileLocationTxt.Text = filename;
                Uri fileUri = new Uri(filename, UriKind.Absolute);
                string extension = System.IO.Path.GetExtension(filename);
                Console.WriteLine("extension : " + extension);

                if (extension.Equals(".png") || extension.Equals(".jpeg") || extension.Equals(".jpg"))
                {
                    image.Visibility = Visibility.Visible;
                    video.Visibility = Visibility.Hidden;
                    fileToSentTag = "image";

                    BitmapImage bitmapImage = new BitmapImage(fileUri);
                    image.Source = bitmapImage;
                }
                else if (extension.Equals(".avi") || extension.Equals(".mp4") || extension.Equals(".wmv"))
                {
                    image.Visibility = Visibility.Hidden;
                    video.Visibility = Visibility.Visible;

                    fileToSentTag = "video";

                    video.Source = fileUri;
                    video.Play();
                }
            }
        }

        private void StartPauseTime(object sender, RoutedEventArgs e)
        {
            try
            {

                time.init(startValueOfTimeTB.Text);
                //time.init(hour, minutes);
                time.startpause();
                //secondWindow.showTimer();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error with minutes " + ex.Message);
            }
        }

        private void StopTime(object sender, RoutedEventArgs e)
        {
            time.stop();
        }

        private void sentToSecondScreen(object sender, RoutedEventArgs e)
        {
            if (video.HasVideo)
                video.Stop();

            try
            {
                int second = Int32.Parse(miniTimerTB.Text);
                string selectTabHeader = (tabControl.SelectedItem as TabItem).Header.ToString();
                if (selectTabHeader.Equals("Πολυμέσα"))
                {
                    Uri fileUri = new Uri(fileLocationTxt.Text, UriKind.Absolute);
                    showFile(fileUri, fileToSentTag, second);
                    secondWindow.showFile(fileUri, fileToSentTag, second);
                }

                else if (selectTabHeader.Equals("Μήνυμα"))
                {
                    if (dontTouchRB.IsChecked == true)
                    {
                        sentMessage("Don't Touch", second);
                        secondWindow.showMessageForTime("Don't Touch", second);
                    }
                    else if (customMsgRB.IsChecked == true)
                    {
                        sentMessage(customMessageTB.Text, second);
                        secondWindow.showMessageForTime(customMessageTB.Text, second);
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("error with secs");
            }
        }

        private void saveSettings(object sender, RoutedEventArgs e)
        {

            try
            {
                Properties.Settings.Default.defaultStartValueOfTimerInMinutes = Int32.Parse(startValueOfTimeTB.Text);
                Properties.Settings.Default.defaultStartValueOfSecondTimerInSeconds = Int32.Parse(miniTimerTB.Text);
                Properties.Settings.Default.timerColor = System.Drawing.Color.FromName(colorCB.SelectedValue.ToString());
                Properties.Settings.Default.timerFont = new System.Drawing.Font(fontCB.SelectedValue.ToString(), 10);

                if (bgImagePreview.Source != null)
                {
                    Properties.Settings.Default.backgroundImageLocation = bgImagePreview.Source.ToString();
                    Properties.Settings.Default.backgroundImageSetted = true;
                }

                Properties.Settings.Default.timerSize = Int32.Parse(timerSizeTB.Text);
                Properties.Settings.Default.timer2Size = Int32.Parse(timer2SizeTB.Text);
                Properties.Settings.Default.textSize = Int32.Parse(textSizeTB.Text);

                Properties.Settings.Default.beepFileLocation = audioFileLocation;

                Properties.Settings.Default.cameraIp1 = textIpCam1.Text;
                Properties.Settings.Default.cameraIp2 = textIpCam2.Text;


                Properties.Settings.Default.Save();




            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error saving settings \n" + ex);
            }

            secondWindow.loadSettings();
            loadSettings();
        }


        #region sc

        System.Windows.Threading.DispatcherTimer fileTimer, messageTimer;

        private void hideViews()
        {
            scImage.Visibility = Visibility.Hidden;
            scVideo.Visibility = Visibility.Hidden;
            scTimer.Visibility = Visibility.Hidden;
            scMessage.Visibility = Visibility.Hidden;
            scSmallTimer.Visibility = Visibility.Hidden;
        }

        private void sentMessage(string message, int seconds)
        {
            hideViews();
            scMessage.Visibility = Visibility.Visible;
            scSmallTimer.Visibility = Visibility.Visible;

            scMessage.Text = message;

            if (messageTimer != null)
            {
                messageTimer.Stop();
                messageTimer = null;
            }

            messageTimer = new System.Windows.Threading.DispatcherTimer();
            messageTimer.Tick += new EventHandler(OnMessageTimer);
            messageTimer.Interval = new TimeSpan(0, 0, seconds);
            messageTimer.Start();
        }

        internal void showFile(Uri fileUri, string tag, int seconds)
        {
            hideViews();
            scSmallTimer.Visibility = Visibility.Visible;

            if (tag == "image")
            {
                scImage.Visibility = Visibility.Visible;
                BitmapImage bitmapImage = new BitmapImage(fileUri);
                scImage.Source = bitmapImage;
            }
            else if (tag == "video")
            {
                scVideo.Visibility = Visibility.Visible;
                scVideo.Source = fileUri;
                scVideo.Play();
            }

            if (seconds != 0)
                startMediaTimer(seconds);
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
            scTimer.Visibility = Visibility.Visible;
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
        #endregion

        private void SelectBgImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                Uri fileUri = new Uri(filename, UriKind.Absolute);
                string extension = System.IO.Path.GetExtension(filename);

                if (extension.Equals(".png") || extension.Equals(".jpeg") || extension.Equals(".jpg"))
                {
                    BitmapImage bitmapImage = new BitmapImage(fileUri);
                    bgImagePreview.Height = 140;
                    bgImagePreview.Source = bitmapImage;
                }
            }

        }
        string audioFileLocation;
        private void SelectBeepFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".wav";
            dlg.Filter = "Audio Files|*.wav;*.mp3;";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                Uri fileUri = new Uri(filename, UriKind.Absolute);
                string extension = System.IO.Path.GetExtension(filename);

                if (extension.Equals(".wav") || extension.Equals(".mp3"))
                {
                    audioFileLocation = filename;
                }
            }
        }

        private void StartCapture(object sender, RoutedEventArgs e)
        {
            try
            {
                if (streamPlayerControl1.IsPlaying)
                    streamPlayerControl1.Stop();
                else
                    streamPlayerControl1.Play(textIpCam1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("streamPlayerControl1 : " + ex.Message + " connection string : " + textIpCam1.Text);
            }

            try
            {
                if (streamPlayerControl2.IsPlaying)
                    streamPlayerControl2.Stop();
                else
                    streamPlayerControl2.Play(textIpCam2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("streamPlayerControl2 : " + ex.Message + " connection string : " + textIpCam2.Text);
            }

        }
    }
}
