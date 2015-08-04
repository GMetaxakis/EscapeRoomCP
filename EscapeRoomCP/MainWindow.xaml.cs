using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace EscapeRoomCP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SecondWindow secondWindow;
        Time time;
        public MainWindow()
        {
            InitializeComponent();
            
            time = new Time();

            time.timerDecreased += time_timerDecreased;

            secondWindow = new SecondWindow();
            secondWindow.Show();
            time.timerDecreased += secondWindow.timer_timerDecreased;
        }

        void time_timerDecreased(object sender, EventArgs e)
        {
            currTimeTxt.Content = (e as MyEventArgs).ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            secondWindow.Close();
        }

        private void StartTimer(object sender, RoutedEventArgs e)
        {
            try
            {
                int minutes = Int32.Parse(timeTxt.Text);

                int hour = 0;
                while (minutes >= 60)
                {
                    minutes = minutes - 60;
                    hour++;
                }

                time.init(hour, minutes);
                time.startpause();
                secondWindow.showTimer();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("error with minutes");
            }
        }
        
        private void StopTimer(object sender, RoutedEventArgs e)
        {
            time.stop();
        }

        private void ShowTimer(object sender, RoutedEventArgs e)
        {
            secondWindow.showTimer() ;
        }

        private void ShowMessage(object sender, RoutedEventArgs e)
        {
            try
            {
                int second = Int32.Parse(messageTimeTxt.Text);
                //secondWindow.showMessageForTime(messageTxt.Text, second);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error with secs : " + ex.Message);

            }
        }

        private void IncreaseFontSize(object sender, RoutedEventArgs e)
        {
            //secondWindow.increaseFontSizeForMessage();
        }

        private void DecreaseFontSize(object sender, RoutedEventArgs e)
        {
            //secondWindow.decreaseFontSizeForMessage();
        }

        private void ShowNoTouchMessage(object sender, RoutedEventArgs e)
        {
            try
            {
                int second = Int32.Parse(messageNTTimeTxt.Text);
                //secondWindow.showMessageForTime("DON'T TOUCH", second);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("error with secs");

            }
        }
        String tag;
        Boolean deleteVideo=false;

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            if (video.HasVideo)
                video.Stop();
            if (deleteVideo)
                Utils.deleteVideoFromPpt();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "All Files|*.jpg;*.jpeg;*.png;*.avi;*.mp4;*.wmv;"; //|Image Files|*.jpg;*.jpeg;*.png;|video|*.wmv;*.avi;*.mp4;|powerpoint|*.ppt
            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                string filename = dlg.FileName;
                fileLocationTxt.Text = filename;
                Uri fileUri = new Uri(filename, UriKind.Absolute);
                string extension = Path.GetExtension(filename);
                Console.WriteLine("extension : "+extension);

                if (extension.Equals(".png") || extension.Equals(".jpeg") || extension.Equals(".jpg"))
                {
                    image.Visibility = Visibility.Visible;
                    video.Visibility = Visibility.Hidden;
                    tag = "image";

                    BitmapImage bitmapImage = new BitmapImage(fileUri);
                    image.Source = bitmapImage;
                }
                else if (extension.Equals(".avi") || extension.Equals(".mp4") || extension.Equals(".wmv"))
                {
                    image.Visibility = Visibility.Hidden;
                    video.Visibility = Visibility.Visible;

                    tag = "video";

                    video.Source = fileUri;
                    video.Play();
                }
            }
        }

        private void showFile(object sender, RoutedEventArgs e)
        {
            try
            {
                int seconds = Int32.Parse(fileTimeTxt.Text);
                Uri fileUri = new Uri(fileLocationTxt.Text, UriKind.Absolute);
                if(fileUri!=null && tag!=null)
                    secondWindow.showFile(fileUri, tag, seconds);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("error with secs");

            }
        }
        
    }
}
