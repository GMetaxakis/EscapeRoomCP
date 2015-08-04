using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EscapeRoomCP
{
    class Time
    {
        DateTime value;
        System.Windows.Threading.DispatcherTimer timer;
        public event EventHandler timerDecreased;
        Boolean started = false;
        Boolean stopped = true;
        protected virtual void onTimerDecreased(MyEventArgs mea)
        {
            if (timerDecreased != null)
                timerDecreased(this, mea);
        }

        internal void init(string minutesStr)
        {

            if (started)
                return;
            if (!stopped)
                return;

            int minutes = Int32.Parse(minutesStr);
            int hour = 0;
            while (minutes >= 60)
            {
                minutes = minutes - 60;
                hour++;
            }

            value = new DateTime(1, 1, 1, hour, minutes, 0);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Interval = new TimeSpan(0, 0, 1);

            stopped = false;
        }

        internal void init(int hour, int minutes)
        {
            if (!stopped)
                return;

            value = new DateTime(1, 1, 1, hour, minutes, 0);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Interval = new TimeSpan(0, 0, 1);
        }
        public void startpause()
        {
            if (started)
            {
                timer.Stop();
                started = false;
            }
            else
            {
                timer.Start();
                started = true;
            }
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            try {
                value = value.AddSeconds(-1);

                onTimerDecreased(new MyEventArgs(getTime()));

                Console.WriteLine(value.ToString("mm:ss"));
            }
            catch (Exception)
            {
                timer.Stop();
                MessageBox.Show("FINISHED!");
            }
        }
        
        string getTime()
        {
            if (value.Hour > 0)
                return value.ToString("HH:mm:ss");
            else
                return value.ToString("mm:ss");
        }

        internal void stop()
        {
            timer.Stop();
            stopped = true;
            started = false;
        }
    }

    public class MyEventArgs : System.EventArgs
    {
        String value;
        public MyEventArgs(String value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
