using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MediaPlayer;

public partial class MainWindow : Window
{
    DispatcherTimer timer;
    public MainWindow()
    {
        InitializeComponent();
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (myMediaElement.Source != null && myMediaElement.NaturalDuration.HasTimeSpan)
        {
            mediaSlider.Value = myMediaElement.Position.TotalMilliseconds;
        }
    }
    #region Управление видео
    //Кнопка открывание медиа
    private void ButtonOpenMediaClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Multiselect = false;
        openFileDialog.Filter = "Media files|*.mp4;*.avi;*.mkv;*.flv;*.wmv;*.mov;*.mp3;*.wav;*.aac;*.ogg;*.webm";
        if (openFileDialog.ShowDialog() == true)
        {
            myMediaElement.Source = null;
            myMediaElement.Source = new Uri(openFileDialog.FileName);
            myMediaElement.Play();
            mediaStatusTextBlock.Text = Statuses.Play.ToString();
        }
    }
    
    //Кнопка проигрыватель
    private void ButtonPlayClick(object sender, RoutedEventArgs e)
    {
        InitializeValuesOfVideo();
        myMediaElement.Play();
        mediaStatusTextBlock.Text = Statuses.Play.ToString();
    }

    
    //Иницализация свойств медиа
    private void InitializeValuesOfVideo()
    {
        myMediaElement.Volume = (double)volumeMediaSlider.Value;
        myMediaElement.SpeedRatio = (double)speedMediaSlider.Value;
    }

    //кнопка паузы
    private void ButtonPauseClick(object sender, RoutedEventArgs e)
    {
        myMediaElement.Pause();
        mediaStatusTextBlock.Text= Statuses.Pause.ToString();
    }

    //кнопка стоп
    private void ButtonStopClick(object sender, RoutedEventArgs e)
    {
        myMediaElement.Stop();
        mediaStatusTextBlock.Text = Statuses.Stop.ToString();
    }
    //cобытие отркытия видео
    private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
    {
        InitializeValuesOfVideo();
        if (myMediaElement.NaturalDuration.HasTimeSpan)
        {
            mediaSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            timer.Start(); 
        }
    }
    
    //событие закончилсоь
    private void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
    {
        myMediaElement.Stop();
        mediaStatusTextBlock.Text = Statuses.Stop.ToString();
    }
    //событие изменниние свойств громкости
    private void volumeMediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        myMediaElement.Volume = (double)volumeMediaSlider.Value;
    }
    //событие изменниние свойств скорости
    private void speedMediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        myMediaElement.SpeedRatio = (double)speedMediaSlider.Value;
    }
    #endregion
    //мышка отпустиоа  слайдер медиа
    private void mediaSlider_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
    {
        myMediaElement.Position = TimeSpan.FromMilliseconds(mediaSlider.Value);
        timer.Start();
    }
    //мышка нажимает на слайдер медиа
    private void mediaSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        timer.Stop();
    }
    //нажатие на чек бокс показа статус бара
    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            if (checkBox.IsChecked == true)
            {
                myStatusbar.Visibility = Visibility.Visible;
            }
            else
            {
                myStatusbar.Visibility = Visibility.Collapsed;
            }
        }
    }
    //загрузка окна
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        dateStatusTextBlock.Text = DateTime.Now.ToShortDateString();
    }
    //действие закрытие окна About
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
       var result = MessageBox.Show("Уверены?","Потверждение выхода",
           MessageBoxButton.YesNo,MessageBoxImage.Asterisk);
       if (result == MessageBoxResult.No)
       {
            e.Cancel = true;
       }
    }

    private void MenuItemExitClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void MenuItemAboutClick(object sender, RoutedEventArgs e)
    {
        foreach(Window item in this.OwnedWindows)
        {
            return;
        }
        AboutWindow aboutWindow = new AboutWindow();
        aboutWindow.Owner =  this;
        aboutWindow.Show();
    }

    private void MenuItemConvertClick(object sender, RoutedEventArgs e)
    {
        foreach (Window item in this.OwnedWindows)
        {
            return;
        }
        ConvertWindow convertWindow = new ConvertWindow();
        convertWindow.Owner = this;
        convertWindow.Show();
    }
}