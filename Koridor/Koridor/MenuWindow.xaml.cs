using Newtonsoft.Json;
﻿using System;
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
using System.Windows.Shapes;
using System.IO;

namespace Koridor
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
       

        public MenuWindow()
        {
            InitializeComponent();
            LoadGameStats();
        }

        private void GameButtonClick(object sender, RoutedEventArgs e)
        {
            var gameWindow = new MainWindow();
            gameWindow.Show();
            this.Close();
        }

        private void RulesButtonClick(object sender, RoutedEventArgs e)
        {
            var rulesWindow = new RulesWindow();
            rulesWindow.Show();
            this.Close();
        }
        void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingWindow = new SettingsWindow();
            settingWindow.Show();

        }
        private void StatisticsButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = "game_stats.json";
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var allStats = JsonConvert.DeserializeObject<List<GameStats>>(json)
                                  ?? new List<GameStats>();

                    // Сортируем по дате (новые сверху)
                    allStats = allStats.OrderByDescending(s => s.GameDate).ToList();

                    var statsWindow = new StatsWindow(allStats);
                    statsWindow.ShowDialog();
        }
                else
                {
                    MessageBox.Show("Статистика игр пока отсутствует.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}");
            }
        }

        private List<GameStats> gameHistory = new List<GameStats>();

        private void LoadGameStats()
        {
            string filePath = "game_stats.json";
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    gameHistory = JsonConvert.DeserializeObject<List<GameStats>>(json)
                                  ?? new List<GameStats>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}");
                gameHistory = new List<GameStats>();
            }
        }
    }
}
