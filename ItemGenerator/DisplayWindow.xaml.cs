using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace ItemGenerator
{
    

    public partial class DisplayWindow : Window
    {

        public string currentObject;
        public List<string> objects;
        public int prevIndex;
        private int currentIndex;
        private int timer;
        private bool trueRandom;
        
        public DisplayWindow(List<string> inputs, bool rand, int seconds)
        {
            InitializeComponent();
            objects = inputs;
            trueRandom = rand;
            timer = seconds;
        }


        private void StartTimer(object sender, RoutedEventArgs e)
        {
            Timer t = new Timer {Interval = timer};
            t.Elapsed += GetNextItem;
            t.Start();
        }

        private void GetNextItem(object sender, ElapsedEventArgs e)
        {
            Random rnd = new Random();
            var nextIndex = rnd.Next(0, objects.Count);
            if (!trueRandom)
                while (nextIndex != prevIndex)
                    nextIndex = rnd.Next(0, objects.Count);

            currentIndex = nextIndex;
            currentObject = objects[currentIndex];
        }
    }
}