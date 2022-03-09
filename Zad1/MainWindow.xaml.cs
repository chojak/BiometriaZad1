using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zad1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
    public enum ActualMode
    {
        Blue,
        Red,
        Green,
        Mean
    }

    public partial class MainWindow : Window
    {
        public string Source = "";
        public Bitmap ImageBitmap;
        public Bitmap HistogramBitmap;
        public ActualMode ActualMode = ActualMode.Mean;

        // https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource
        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            Image.MaxHeight = 500;
            Image.MaxWidth = 800;

            //Histogram.MaxHeight = 500;
            //Histogram.MaxWidth = 800;

            foreach (var x in Canvas.Children)
            {
                if (x.GetType() == typeof(RadioButton))
                {
                    (x as RadioButton).Checked += RadioButton_Checked;
                }

                ThresholdLevelTextBox.Text = "Threshold level: 0";
            }
        }
        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            Image.Visibility = Image.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            Histogram.Visibility = Histogram.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

            SwapButton.Content = SwapButton.Content == "BinaryThreshold" ? "Histogram" : "BinaryThreshold";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Source != "")
            {
                ImageBitmap = Algorithm.BinaryThreshold(new Bitmap(Source), (byte)Slider.Value, ActualMode);
                Image.Source = BitmapToImageSource(ImageBitmap);
                ThresholdLevelTextBox.Text = "Threshold level: " + Math.Round(Slider.Value, 0);
            }
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            if (openFileDialog.ShowDialog() == true)
            {
                Source = openFileDialog.FileName;
                ImageBitmap = new Bitmap(Source);
                Image.Source = BitmapToImageSource(ImageBitmap);
                HistogramBitmap = Algorithm.Histogram(new Bitmap(Source), ActualMode);
                Histogram.Source = BitmapToImageSource(HistogramBitmap);
            }

        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            
            // czemu == nie dziala?!?!?
            if (SwapButton.Content.Equals("Histogram"))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog.Title = "Save an Image File";
                if (saveFileDialog.ShowDialog() == true)
                {
                    ImageBitmap.Save(saveFileDialog.FileName);
                }
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog.Title = "Save a histogram";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.ShowDialog() == true)
                {
                    HistogramBitmap.Save(saveFileDialog.FileName);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            switch((sender as RadioButton).Content)
            {
                case "Red":
                    ActualMode = ActualMode.Red;
                    break;
                case "Green":
                    ActualMode = ActualMode.Green;
                    break;
                case "Blue":
                    ActualMode = ActualMode.Blue;
                    break;
                default:
                    ActualMode = ActualMode.Mean;
                    break;
            }

            Image.Source = BitmapToImageSource(ImageBitmap);
            HistogramBitmap = Algorithm.Histogram(new Bitmap(Source), ActualMode);
            Histogram.Source = BitmapToImageSource(HistogramBitmap);
        }
    }
}
