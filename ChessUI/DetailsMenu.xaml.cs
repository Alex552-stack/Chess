using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChessUI;

/// <summary>
///     Interaction logic for DetailsMenu.xaml
/// </summary>
public partial class DetailsMenu : UserControl
{
    private TimeSpan _time;
    private DispatcherTimer _timer;

    public DetailsMenu()
    {
        InitializeComponent();
    }

    public void InitializeTime(TimeSpan time)
    {
        _time = time;
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
        TimeDisplay.Text = _time.ToString(@"mm\:ss");
    }

    public void StartTime()
    {
        _timer.Start();
    }

    public void StopTime()
    {
        _timer.Stop();
    }


    private void Timer_Tick(object sender, EventArgs e)
    {
        _time = _time.Subtract(TimeSpan.FromSeconds(1));
        TimeDisplay.Text = _time.ToString(@"mm\:ss");

        if (_time.TotalSeconds <= 0)
            // Timer 1 expired, handle accordingly
            _timer.Stop();
    }
}