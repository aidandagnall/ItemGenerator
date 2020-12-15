using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ItemSelector
{
    

    public partial class DisplayWindow : INotifyPropertyChanged
    {

        private string _currentObject;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly List<string> _items;
        private int _currIndex;
        private int _repeats;
        private readonly int _timer;
        private readonly bool _trueRandom;
        private readonly bool _showTotal;

        public string CurrentObject
        {
            get => _currentObject;
            set
            {
                _currentObject = value;
                OnPropertyChanged("CurrentObject");
            }
        }

        public DisplayWindow(List<string> inputs, bool rand, int seconds, int repeats, bool showTotal)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.KeyDown += new KeyEventHandler(KeyPressHandler);
            _items = inputs;
            _trueRandom = rand;
            _showTotal = showTotal;
            _timer = seconds;
            _repeats = repeats == 0 ? -1 : repeats;
            CurrentObject = "Ready?";
            Title = "Picker";
        }

        private void KeyPressHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (_timer > 0)
            {
                _t.Stop();
                GetNextItem();
                StartTimer(_timer);
                UpdateTimerDisplay();
            }
            else
                GetNextItem();
        }


        private void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }


        private void StartButtonHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_repeats > 0 && _showTotal)
                StartTotalTimer();
            if (_timer > 0)
                StartTimer(5);
            else
                GetNextItem();
            StartButton.Visibility = Visibility.Hidden;
        }

        private void UpdateTimerDisplay()
        {
            TimerDisplay.Content = "Next item in: " + (_ts.Seconds != 0 ? _ts.Seconds.ToString() : _timer.ToString()) + "s";
        }

        private void StartTotalTimer()
        {
            var ts = TimeSpan.FromSeconds(_timer * _repeats + 5);
            var t = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (ts == TimeSpan.Zero)
                    TotalDisplay.Visibility = Visibility.Hidden;
                TotalDisplay.Content = "Total time remaining: " + ts.Minutes.ToString() + ':' + ts.Seconds.ToString("00");
                ts = ts.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            t.Start();
        }

        private DispatcherTimer _t;
        private TimeSpan _ts;

        private void StartTimer(int duration)
        {
            _ts = TimeSpan.FromSeconds(duration);
            _t = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                UpdateTimerDisplay();
                if (_ts == TimeSpan.Zero)
                {
                    _t.Stop();
                    
                    if (_repeats == 0)
                    {
                        TimerDisplay.Content = "";
                        CurrentObject = "Done 🎉";
                        return;
                    }
                    else
                    {
                        GetNextItem();
                        StartTimer(_timer);
                        _repeats--;
                    }
                }

                _ts = _ts.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _t.Start();
        }

        private void GetNextItem()
        {
            Console.WriteLine(@"Getting element");
            var rnd = new Random();
            var nextIndex = rnd.Next(0, _items.Count);
            if (!_trueRandom)
                while (nextIndex == _currIndex)
                    nextIndex = rnd.Next(0, _items.Count);

            _currIndex = nextIndex;
            CurrentObject = _items[nextIndex];
        }

    }
}