﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;

namespace Koridor
{
    public partial class StatsWindow : Window
    {
        public StatsWindow(List<GameStats> stats)
        {
            InitializeComponent();
            StatsDataGrid.ItemsSource = stats.OrderByDescending(s => s.GameDate).ToList();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}