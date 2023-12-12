using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for DetailsMenu.xaml
    /// </summary>
    public partial class DetailsMenu : UserControl
    {
        private DispatcherTimer Timer;
        private TimeSpan Time;
        public DetailsMenu()
        {
            InitializeComponent();
        }

        public void InitializeTime(TimeSpan time)
        {
            Time = time;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
            TimeDisplay.Text = Time.ToString(@"mm\:ss");
        }

        public void StartTime()
        {
            Timer.Start();
        }

        public void StopTime()
        {
            Timer.Stop();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Time = Time.Subtract(TimeSpan.FromSeconds(1));
            TimeDisplay.Text = Time.ToString(@"mm\:ss");

            if (Time.TotalSeconds <= 0)
            {
                // Timer 1 expired, handle accordingly
                Timer.Stop();
            }
        }
    }
}
