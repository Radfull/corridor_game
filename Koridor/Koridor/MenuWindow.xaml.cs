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
using System.Windows.Shapes;

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

    }
}
