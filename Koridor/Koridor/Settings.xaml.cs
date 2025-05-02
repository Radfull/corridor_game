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
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public enum OpponentType { Friend, Bot }

        public OpponentType SelectedOpponent { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();

            BotButton.IsChecked = true; //по умолчанию поставила игру с ботом, если кайф можно наоборот 
            SelectedOpponent = OpponentType.Bot;
            BotButton.Background = Brushes.DarkGray;
        }

        private void OpponentButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton? radioButton = sender as RadioButton;

            if (radioButton != null)
            {
                FriendButton.Background = Brushes.LightGray;
                BotButton.Background = Brushes.LightGray;

                radioButton.Background = Brushes.DarkGray;

                if (radioButton == FriendButton)
                {
                    SelectedOpponent = OpponentType.Friend;
                }
                else if (radioButton == BotButton)
                {
                    SelectedOpponent = OpponentType.Bot;
                }


            }
        }

        public OpponentType GetSelectedOpponent()  //надеюсь так удобнее принимать значение выбора, если нет - сори,
                                                   //как переделать не знаю, надо смотреть как вам это передоваться будет,
                                                   //а я хоть и гений програмирования, но все мы не совершенны
        {
            return SelectedOpponent;
        }

    }
}