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
        private int _prevIndex;
        private int _currIndex;
        private readonly int _timer;
        private readonly bool _trueRandom;

        public string CurrentObject
        {
            get => _currentObject;
            set
            {
                _currentObject = value;
                OnPropertyChanged("CurrentObject");
            }
        }
        public DisplayWindow(List<string> inputs, bool rand, int seconds)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.KeyDown += new KeyEventHandler(KeyPressHandler);
            _items = inputs;
            _trueRandom = rand;
            _timer = seconds;
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
            if (_timer > 0)
                StartTimer(5);
            else
                GetNextItem();
            StartButton.Visibility = Visibility.Hidden;
        }

        private void UpdateTimerDisplay()
        {
            TimerDisplay.Content = _ts.Seconds != 0 ? _ts.Seconds.ToString() : _timer.ToString();
        }

        private void StartTimer(int duration)
        {
            _ts = TimeSpan.FromSeconds(duration);
            _t = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                UpdateTimerDisplay();
                if (_ts == TimeSpan.Zero)
                {
                    _t.Stop();
                    GetNextItem();
                    StartTimer(_timer);
                }

                _ts = _ts.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _t.Start();
        }

        private DispatcherTimer _t;
        private TimeSpan _ts;

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