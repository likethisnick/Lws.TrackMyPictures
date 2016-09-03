using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GMap.NET.WindowsPresentation;
using System.Diagnostics;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Lws.TrackMyPictures
{
    /// <summary>
    /// Interaction logic for PhotoMarker.xaml
    /// </summary>
    public partial class PhotoMarker
    {
        Popup Popup;
        Label Label;
        GMapMarker Marker;
        MainWindow MainWindow;
        public int Number;
        public double fllength;
        public int photoWidth;
        public int photoHeight;
        public string Place;
        public double Lat;
        public double Long;
        public string Date;
        public string CameraInfo;
        public string CameraInfoAdd;
        public string Extension;
        public string path
        { get; set; }

        public PhotoMarker(MainWindow window, GMapMarker marker, string Adress, int PhotoCounter, ExifInfo exifInfo)
        {
            this.InitializeComponent();
            DataContext = this;
            this.Marker = marker;
            this.MainWindow = window;
            Extension = exifInfo.Extension;
            photoWidth = exifInfo.width;
            photoHeight = exifInfo.heigh;
            CameraInfo = exifInfo.CameraInfo;
            CameraInfoAdd = exifInfo.CameraInfoAdd;
            Place = exifInfo.photoPlace;
            Date = exifInfo.photoTime;
            Lat = exifInfo.Lat;
            Long = exifInfo.Long;



            fllength = new System.IO.FileInfo(Adress).Length;
            // ???????????????? в одном МБ 1048576 байт
            fllength = fllength / 1048576;


            Number = PhotoCounter;
            
            path = Adress;
            Popup = new Popup();
            Label = new Label();

            this.Unloaded += new RoutedEventHandler(PhotoMarker_Unloaded);
            this.SizeChanged += new SizeChangedEventHandler(PhotoMarker_SizeChanged);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(PhotoMarker_MouseLeftButtonDown);
            this.MouseEnter += new MouseEventHandler(PhotoMarker_MouseEnter);
            this.MouseLeave += new MouseEventHandler(PhotoMarker_MouseLeave);
            this.MouseMove += new MouseEventHandler(PhotoMarker_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(PhotoMarker_MouseLeftButtonDown);
        }

        private void PhotoMarker_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(PhotoMarker_Unloaded);
            this.SizeChanged -= new SizeChangedEventHandler(PhotoMarker_SizeChanged);
            this.MouseLeftButtonDown -= new MouseButtonEventHandler(PhotoMarker_MouseLeftButtonDown);
            this.MouseEnter -= new MouseEventHandler(PhotoMarker_MouseEnter);
            this.MouseLeave -= new MouseEventHandler(PhotoMarker_MouseLeave);
            this.MouseMove -= new MouseEventHandler(PhotoMarker_MouseMove);
            this.MouseLeftButtonDown -= new MouseButtonEventHandler(PhotoMarker_MouseLeftButtonDown);

            Marker.Shape = null;
            Popup = null;
            Label = null;
        }

        public void PhotoMarker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.MainMap.CanDragMap = false; // fix of map bug

            MainWindow.pathh = path;
              MainWindow.MapGrid.Visibility = Visibility.Hidden;
               
                MainWindow.MarkerGrid.Visibility = Visibility.Visible;
            MainWindow.ChoosenOne = this;
            MainWindow.lblFileName.Content = Path.GetFileName(path);
            MainWindow.ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(path)));
                ImageBrush[] a = new ImageBrush[MainWindow.MainMap.Markers.Count];

            MainWindow.lblPhotoPlace.Text = Place;
            MainWindow.lblDateTime.Text = Date;
            MainWindow.lblPhotoExtension.Content = Extension + ", " + Math.Round(fllength, 2) + " MB";
            MainWindow.lblRes.Content = photoWidth + "x" + photoHeight;
            MainWindow.lblCameraInfo.Content = CameraInfo;
            MainWindow.lblCameraAddInfo.Content = CameraInfoAdd;


        }


        public void ass()
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }


        void PhotoMarker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Marker.Offset = new Point(-e.NewSize.Width / 2, -e.NewSize.Height);
        }

        void PhotoMarker_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                Point p = e.GetPosition(MainWindow.MainMap);
                Marker.Position = MainWindow.MainMap.FromLocalToLatLng((int)(p.X), (int)(p.Y));
            }
        }

        void PhotoMarker_MouseLeave(object sender, MouseEventArgs e)
        {
            Marker.ZIndex -= 10000;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        void PhotoMarker_MouseEnter(object sender, MouseEventArgs e)
        {
            Marker.ZIndex += 10000;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
        }
    }
}