using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Microsoft.Win32;
using NReco.VideoConverter;

namespace MediaPlayer
{
    public partial class ConvertWindow : Window
    {
        private string selectedFilePath;
        private FFMpegConverter converter;

        public ConvertWindow()
        {
            InitializeComponent();
            converter = new FFMpegConverter();
            StatusText.Text = "Готов к работе";
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Media files|*.mp4;*.avi;*.mkv;*.flv;*.wmv;*.mov;*.mp3;*.wav;*.aac;*.ogg;*.webm",
                Title = "Выберите файл для конвертации"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                FilePathText.Text = selectedFilePath;
                StatusText.Text = $"Выбран файл: {Path.GetFileName(selectedFilePath)}";
                FormatComboBox.IsEnabled = true;
                ConvertButton.IsEnabled = true;
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Сначала выберите файл!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(selectedFilePath))
            {
                MessageBox.Show("Исходный файл не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string format = ((ComboBoxItem)FormatComboBox.SelectedItem).Content.ToString().ToLower();
                string outputFile = Path.Combine(
                    Path.GetDirectoryName(selectedFilePath),
                    $"{Path.GetFileNameWithoutExtension(selectedFilePath)}_converted.{format}");

                if (File.Exists(outputFile))
                {
                    if (MessageBox.Show("Файл с таким именем уже существует. Перезаписать?",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    File.Delete(outputFile);
                }

                StatusText.Text = "Конвертация началась...";
                ConvertButton.IsEnabled = false;
                BrowseButton.IsEnabled = false;
                FormatComboBox.IsEnabled = false;

                converter.LogReceived += (o, args) => {
                    Dispatcher.Invoke(() => {
                        StatusText.Text = args.Data;
                    });
                };

                await Task.Run(() => {
                    converter.ConvertMedia(
                        selectedFilePath,
                        outputFile,
                        FormatToFFmpegFormat(format));
                });

                MessageBox.Show($"Конвертация завершена успешно!\nФайл сохранён как: {outputFile}",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FFMpegException ex)
            {
                MessageBox.Show($"Ошибка FFmpeg: {ex.Message}\nКод ошибки: {ex.ErrorCode}",
                    "Ошибка конвертации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConvertButton.IsEnabled = true;
                BrowseButton.IsEnabled = true;
                FormatComboBox.IsEnabled = true;
                StatusText.Text = "Готов к новой конвертации";
            }
        }

        private string FormatToFFmpegFormat(string format)
        {
            switch (format.ToLower())
            {
                case "mp4": return "mp4";
                case "avi": return "avi";
                case "mkv": return "matroska";
                case "flv": return "flv";
                case "wmv": return "asf";
                case "mov": return "mov";
                case "mp3": return "mp3";
                case "wav": return "wav";
                case "aac": return "aac";
                case "ogg": return "ogg";
                case "webm": return "webm";
                default: return format;
            }
        }

    }
}