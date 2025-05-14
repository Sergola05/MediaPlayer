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
using Microsoft.Win32;
using System.IO;
using NReco.VideoConverter;




namespace MediaPlayer
{
    public partial class ConvertWindow : Window
    {
        private string selectedFilePath;

        public ConvertWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Media files|*.mp4;*.avi;*.mkv;*.flv;*.wmv;*.mov;*.mp3;*.wav;*.aac;*.ogg;*.webm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                FilePathText.Text = selectedFilePath;
            }
        }
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Сначала выберите файл!", "Ошибка");
                return;
            }
            //получает текст в который надо конверировать от Comobox например mp4 
            string format = ((ComboBoxItem)FormatComboBox.SelectedItem).Content.ToString().ToLower();
            //Меняет расширение файла например если Video.mp4 то получает Video.avi
            string outputFile = System.IO.Path.ChangeExtension(selectedFilePath, format);

            var converter = new FFMpegConverter();
            //Сама конвертация со всеми данными исходный файл куда сохранить и целовой формат

            converter.ConvertMedia(selectedFilePath, outputFile, format);

            MessageBox.Show($"Конвертация завершена!\nФайл сохранён как: {outputFile}", "Готово");
        }


    }
}