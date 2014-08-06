﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace nexMuni
{
    public class RouteData : INotifyPropertyChanged
    {
        private string predictions1 = String.Empty;
        private string predictions2 = String.Empty;
        private string temp;

        public string RouteNum { get; set; }
        public string RouteName { get; set; }
        public string Dir1 { get; set; }
        public string Dir2 { get; set; }

        public string Times1 {
            get
            {
                return this.predictions1;
            }

            set
            {
                if (value != this.predictions1)
                {
                    this.predictions1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Times2 {
            get
            {
                return this.predictions2;
            }

            set
            {
                if (value != this.predictions2)
                {
                    this.predictions2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public RouteData() { }

        public RouteData(string name, string num, string dir, string [] _times)
        {
            int i = 0;
            RouteName = name;
            RouteNum = num;
            Dir1 = dir;
            if (Dir1.Contains("via Downtown")) Dir1 = Dir1.Replace("via Downtown", "");
            if (Dir1.Contains("&amp;")) Dir1 = Dir1.Replace("&amp;", "&");
            if (Dir1.Contains("Daly City BART Station")) Dir1 = Dir1.Replace("( Daly City BART Station 4pm-7pm)", "BART");

            if (_times.Length > 1)
            {
                while (i < _times.Length - 1)
                {
                    temp = temp + _times[i] + ", ";
                    i++;
                }
                Times1 = temp + _times[i] + " mins";
                temp = string.Empty;
            }
            else Times1 = _times + " mins";
        }

        public void AddDir2(string _inTitle, string [] _times)
        {
            int i= 0;
            Dir2 = _inTitle;
            if (Dir2.Contains("via Downtown")) Dir2 = Dir2.Replace("via Downtown", "");
            if (Dir2.Contains("&amp;")) Dir2 = Dir2.Replace("&amp;", "&");
            if (Dir2.Contains("Daly City BART Station")) Dir2 = Dir2.Replace("( Daly City BART Station 4pm-7pm)", "BART");

            if (_times.Length > 1)
            {
                while (i < _times.Length - 1)
                {
                    temp = temp + _times[i] + ", ";
                    i++;
                }
                Times2 = temp + _times[i] + " mins";
                temp = string.Empty;
            }
            else Times2 = _times[0] + " mins";  
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}