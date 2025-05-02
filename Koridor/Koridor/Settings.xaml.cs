using System;
using System.Windows;
using System.Windows.Controls;

namespace Koridor
{
    public partial class SettingsWindow : Window
    {
        public int SelectedBoardSize { get; set; } = 9;
        private bool _isBotSelected = true;

        private double _masterVolume = 0.5;
        private double _musicVolume = 0.5;
        private double _effectsVolume = 0.5;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettingsFromProperties();
            BotButton.IsChecked = _isBotSelected;
            FriendButton.IsChecked = !_isBotSelected;
            this.Closing += SettingsWindow_Closing;

            MasterVolumeSlider.Value = _masterVolume;
            MusicVolumeSlider.Value = _musicVolume;
            EffectsVolumeSlider.Value = _effectsVolume;

            foreach (ComboBoxItem item in BoardSizeComboBox.Items)
            {
                if (item.Content.ToString().StartsWith(SelectedBoardSize.ToString()))
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void OpponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button)
            {
                if (button.Name == "BotButton")
                {
                    _isBotSelected = true;
                }
                else if (button.Name == "FriendButton")
                {
                    _isBotSelected = false;
                }
            }
        }

        private void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _masterVolume = MasterVolumeSlider.Value;
            // Логика изменения общей громкости (если необходимо)
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _musicVolume = MusicVolumeSlider.Value;
            // Логика изменения громкости музыки
        }

        private void EffectsVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _effectsVolume = EffectsVolumeSlider.Value;
            // Логика изменения громкости эффектов. победа-проигрыш
        }
        private void BoardSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoardSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string sizeString = selectedItem.Content.ToString();
                SelectedBoardSize = int.Parse(sizeString.Split('x')[0]);
                Console.WriteLine($"Выбран размер поля: {SelectedBoardSize}x{SelectedBoardSize}");
            }
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettingsToProperties();
        }

        private void SaveSettingsToProperties()
        {
            Application.Current.Properties["BoardSize"] = SelectedBoardSize;
            Application.Current.Properties["IsBotSelected"] = _isBotSelected;
            Application.Current.Properties["MasterVolume"] = _masterVolume;
            Application.Current.Properties["MusicVolume"] = _musicVolume;
            Application.Current.Properties["EffectsVolume"] = _effectsVolume;
        }

        private void LoadSettingsFromProperties()
        {
            //Загружаем размер доски
            if (Application.Current.Properties["BoardSize"] is int boardSize)
            {
                SelectedBoardSize = boardSize;
            }
            else
            {
                SelectedBoardSize = 9;
            }

            //Загружаем состояние бота
            if (Application.Current.Properties["IsBotSelected"] is bool isBot)
            {
                _isBotSelected = isBot;
            }
            else
            {
                _isBotSelected = true;
            }

            //Настройки громкости
            if (Application.Current.Properties["MasterVolume"] is double masterVolume)
            {
                _masterVolume = masterVolume;
            }
            else
            {
                _masterVolume = 0.5;
            }

            if (Application.Current.Properties["MusicVolume"] is double musicVolume)
            {
                _musicVolume = musicVolume;
            }
            else
            {
                _musicVolume = 0.5;
            }

            if (Application.Current.Properties["EffectsVolume"] is double effectsVolume)
            {
                _effectsVolume = effectsVolume;
            }
            else
            {
                _effectsVolume = 0.5;
            }
        }
    }
}