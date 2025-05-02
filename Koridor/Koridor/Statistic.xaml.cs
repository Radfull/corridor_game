using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Koridor
{
    /// <summary>
    /// Interaction logic for StatisticWindow.xaml
    /// </summary>
    public partial class StatisticWindow : Window
    {
        public ObservableCollection<GameStatistic> GameStatistics { get; set; }

        public StatisticWindow()
        {
            InitializeComponent();
            GameStatistics = new ObservableCollection<GameStatistic>();
            StatisticsDataGrid.ItemsSource = GameStatistics;
        }

        void StatisticsButtonClick(object sender, RoutedEventArgs e)
        {
            StatisticWindow statisticWindow = new StatisticWindow();
            statisticWindow.Show();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class GameStatistic
    {
        public DateTime GameDate { get; set; }
        public string WinnerName { get; set; }
        public int MoveCount { get; set; }
        public TimeSpan GameDuration { get; set; }
    }
}