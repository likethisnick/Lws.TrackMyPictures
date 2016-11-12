using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExifLib;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Net;

namespace Lws.TrackMyPictures
{

    public partial class MainWindow : Window
    {
        public PhotoMarker ChoosenOne;
        GeoCoderStatusCode status;
        List<string> myAdressList = new List<string>();
        List<PhotoMarker> myphotoMarkerList = new List<PhotoMarker>();
        int photoCounter;
        public double zoom;
        public int i;

        public string pathh
        {
            get; set;
        }
        public string path
        {
            get; set;
        }

        public int ColumnsNumber;

        public System.Windows.Controls.Image Imagessss { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            // Set the view to show details.
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            DataContext = this;
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
            MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.MapProvider = GMapProviders.GoogleMap;
            status = GeoCoderStatusCode.Unknow;
            PointLatLng? pos = GMapProviders.GoogleMap.GetPoint("Ukraine, Kyiv", out status);
            MainMap.Position = new PointLatLng(pos.Value.Lat, pos.Value.Lng);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 9;
            photoCounter = 0;
            i = 0;

        //    ColumnsNumber = (int)this.Width/120;

            zoom = 1;
            MapComboBox.DisplayMemberPath = "Name";
            MapComboBox.Items[0] = GMapProviders.GoogleMap;
            MapComboBox.Items[1] = GMapProviders.YandexMap;
            MapComboBox.Items[2] = GMapProviders.OpenStreetMap;
            MapComboBox.Items[3] = GMapProviders.BingMap;
            MapComboBox.SelectedItem = MainMap.MapProvider;

            

        }



        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {

            if (i==0)
            {
                ShowHideMenu("sbShowLeftMenu", "LblLeft",btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu, StackLabel);
                i++;
                
            }
        }

        private void btnLeftMenuHide_Click(object sender, RoutedEventArgs e)
        {
            if (i == 1)
            {
                ShowHideMenu("sbHideLeftMenu", "LblRight", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu, StackLabel);
                i--;
            }
        }

        private void btnInfoShow_Click(object sender, RoutedEventArgs e)
        {
 
                ShowHideInfo("sbShowInfo", btnInfoHide, btnInfoShow, PhotoInfoPanel);

        }

        private void btnInfoHide_Click(object sender, RoutedEventArgs e)
        {

                ShowHideInfo("sbHideInfo", btnInfoHide, btnInfoShow, PhotoInfoPanel);

        }





        private void ShowHideMenu(string Storyboard, string lblSb, Button btnHide, Button btnShow, StackPanel pnl, StackPanel label)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            Storyboard sbb = Resources[lblSb] as Storyboard;
            sb.Begin(pnl);
            sbb.Begin(label);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ShowHideInfo(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }

        }


        private void MainMap_MouseEnter(object sender, MouseEventArgs e)
        {
            btnLeftMenuHide_Click(sender, e);
        }


        // Exif

        private static string RenderTag(object tagValue)
        {
            // Arrays don't render well without assistance.
            var array = tagValue as Array;
            if (array != null)
            {
                // Hex rendering for really big byte arrays (ugly otherwise)
                if (array.Length > 20 && array.GetType().GetElementType() == typeof(byte))
                    return "0x" + string.Join("", array.Cast<byte>().Select(x => x.ToString("X2")).ToArray());

                return string.Join("; ", array.Cast<object>().Select(x => x.ToString()).ToArray());
            }

            return tagValue.ToString();
        }



        public ExifInfo OF(object filename)
        {
            

            ExifInfo exifInfo = new ExifInfo();

            path = (string)filename;

            // File Extension
            exifInfo.Extension = Path.GetExtension(path).Replace(".", "");
            try
            {
                using (var reader = new ExifReader(path))
                {
                    // File Lattitude
                    object GPSLatitude;
                    if (reader.GetTagValue(ExifTags.GPSLatitude, out GPSLatitude))
                    {
                        string GPSlat = string.Format("{0}: {1}", Enum.GetName(typeof(ExifTags), ExifTags.GPSLatitude), RenderTag(GPSLatitude));

                        string[] GPSlats = GPSlat.Split(' ', ';');
                        GPSlats = GPSlats.Where(n => !string.IsNullOrEmpty(n)).ToArray();
                        double[] myLats = new double[3];
                        myLats[0] = Convert.ToDouble(GPSlats[1]);
                        myLats[1] = Convert.ToDouble(GPSlats[2]);
                        myLats[2] = Convert.ToDouble(GPSlats[3]);
                        exifInfo.Lat = myLats[0] + myLats[1] / 60 + myLats[2] / 3600;
                    }
                    // File Longtitude
                    object GPSLongitude;
                    if (reader.GetTagValue(ExifTags.GPSLongitude, out GPSLongitude))
                    {
                        string GPSlat = string.Format("{0}: {1}", Enum.GetName(typeof(ExifTags), ExifTags.GPSLongitude), RenderTag(GPSLongitude));

                        string[] GPSLongs = GPSlat.Split(' ', ';');
                        GPSLongs = GPSLongs.Where(n => !string.IsNullOrEmpty(n)).ToArray();
                        double[] myLongs = new double[3];
                        myLongs[0] = Convert.ToDouble(GPSLongs[1]);
                        myLongs[1] = Convert.ToDouble(GPSLongs[2]);
                        myLongs[2] = Convert.ToDouble(GPSLongs[3]);
                        exifInfo.Long = myLongs[0] + myLongs[1] / 60 + myLongs[2] / 3600;
                    }
                    // File Photo Taken DataTime
                    object PhotoDateTime;
                    if (reader.GetTagValue(ExifTags.DateTime, out PhotoDateTime))
                    {
                        string pT = string.Format(PhotoDateTime + "");

                        String s = (string)PhotoDateTime;
                        String[] DatePart = s.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        exifInfo.photoTime = new DateTime(int.Parse(DatePart[0]), int.Parse(DatePart[1]), int.Parse(DatePart[2]));
                    }
                    // File Width
                    object PixelXDimension;
                    if (reader.GetTagValue(ExifTags.PixelXDimension, out PixelXDimension))
                    {
                        UInt32 a = (UInt32)PixelXDimension;
                        exifInfo.width = (int)a;
                    }
                    // File Heigh
                    object PixelYDimension;
                    if (reader.GetTagValue(ExifTags.PixelYDimension, out PixelYDimension))
                    {
                        UInt32 a = (UInt32)PixelYDimension;
                        exifInfo.heigh = (int)a;
                    }
                    // File Camera Info
                    object CameraInfo;
                    if (reader.GetTagValue(ExifTags.Model, out CameraInfo))
                    {
                        exifInfo.CameraInfo = (string)CameraInfo;
                    }

                    object CameraInfoAdd;
                    if (reader.GetTagValue(ExifTags.DeviceSettingDescription, out CameraInfoAdd))
                    {
                        exifInfo.CameraInfoAdd = (string)CameraInfoAdd;
                    }

                    return exifInfo;
                }
            }
            catch
            {
                return null;
            }   
            }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MainMap.Zoom = ((int)MainMap.Zoom) + 1;
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MainMap.Zoom = ((int)(MainMap.Zoom + 0.99)) - 1;
        }

        private void myWindow_Drop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int SameFile = 0;
            int ExifNull = 0;
            for (int i = 0; i < files.Length; i++)
            {
                ExifInfo exInfo = new ExifInfo();
                exInfo = OF(files[i]);

                if(exInfo==null)
                {
                    ExifNull++;
                    continue;
                }

                if (myAdressList.Contains(files[i]))
                {
                    SameFile++;
                    continue;
                }
                myAdressList.Add(files[i]);
               
                PointLatLng LLTpointer = new PointLatLng();
                LLTpointer.Lat = exInfo.Lat;
                LLTpointer.Lng = exInfo.Long;
                GMapMarker it = new GMapMarker(LLTpointer);
                it.ZIndex = 5;
                List<Placemark> plc = null;
                var st = GMapProviders.GoogleMap.GetPlacemarks(new PointLatLng(exInfo.Lat, exInfo.Long), out plc);
                
                foreach (var pl in plc)
                {
                    if (!string.IsNullOrEmpty(pl.PostalCodeNumber))
                    {
                        exInfo.photoPlace = pl.CountryName +" "+ pl.AdministrativeAreaName;
                    }
                }

                PhotoMarker A = new PhotoMarker(this, it, files[i], photoCounter, exInfo);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.DecodePixelWidth = 240;
                bi.DecodePixelHeight = 220;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(files[i]);
                bi.EndInit();

                ListViewItemsCollections.Add(new ListViewItemsData()
                {
                    GridViewColumnName_ImageSource = files[i],
                    GridViewDate = exInfo.photoTime.ToShortDateString(),
                    GridImageBrush = bi
                });
                ListView1.ItemsSource = ListViewItemsCollections;
                it.Shape = A;
                myphotoMarkerList.Add(A);
                it.Tag = photoCounter;
                MainMap.Markers.Add(it);
                photoCounter++;
                MainMap.Position = LLTpointer;
            }

            if (SameFile != 0)
            {
                MessageBox.Show(SameFile + " - number of files already noticed on map", "Similar files found");
            }
            if (ExifNull != 0)
            {
                MessageBox.Show(ExifNull + " files couldn't be found on map (no GPS Exif info found)", "No Exif");
            }
            CheckBorderScrollViewer();

        
            ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(ListView1.ItemsSource);
            
            view.GroupDescriptions.Clear();

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("GridViewDate");
            view.GroupDescriptions.Add(groupDescription);
            view.CustomSort = new DateTimeComparer();
        }

        private void btnBackToMap_Click(object sender, RoutedEventArgs e)
        {
            RotateBack();
            MapGrid.Visibility = Visibility.Visible;
            MapShow();
            MarkerGrid.Visibility = Visibility.Hidden;
            MainMap.CanDragMap = true;
        }


        private void btnToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null || MainMap == null || myphotoMarkerList.Count<2)
                return;
            RotateBack();
            
           
            

            if (ChoosenOne.Number<MainMap.Markers.Count&&ChoosenOne.Number>0)
            {
                btnScaleOut_Click(this, e);
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == (ChoosenOne.Number-1));
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                lblFileAdress.Content = ChoosenOne.path;
            }
            else if(ChoosenOne.Number == 0)
            {
                btnScaleOut_Click(this, e);
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == myphotoMarkerList.Count - 1);
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                lblFileAdress.Content = ChoosenOne.path;
            }
            TurnZoomToOne();
            ChoosenOne.photoHeight = ChoosenOne.photoHeightT;
            ChoosenOne.photoWidth = ChoosenOne.photoWidthT;
            
        }

        private void btnToRight_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null||MainMap==null || myphotoMarkerList.Count<2)
                return;
            RotateBack();
            TurnZoomToOne();
            ChoosenOne.photoHeight = ChoosenOne.photoHeightT;
            ChoosenOne.photoWidth = ChoosenOne.photoWidthT;

            if (ChoosenOne.Number<MainMap.Markers.Count-1&&ChoosenOne.Number>-1)
            {
                btnScaleOut_Click(this, e);
                ChoosenOne = myphotoMarkerList.Find(meh=>meh.Number==(ChoosenOne.Number+1));
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

            
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                lblFileAdress.Content = ChoosenOne.path;
            }
            else if (ChoosenOne.Number == MainMap.Markers.Count - 1)
            {
                if (ChoosenOne == null)
                    return;
                btnScaleOut_Click(this, e);
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == 0);
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

               
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                lblFileAdress.Content = ChoosenOne.path;
            }
            TurnZoomToOne();
            ChoosenOne.photoHeight = ChoosenOne.photoHeightT;
            ChoosenOne.photoWidth = ChoosenOne.photoWidthT;

        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (MainMap==null)
                return;

            MainMap.Position = new PointLatLng(ChoosenOne.Lat, ChoosenOne.Long);
            MapGrid.Visibility = Visibility.Visible;
            MapShow();
            MarkerGrid.Visibility = Visibility.Hidden;
            MainMap.CanDragMap = true;
        }

        private void btnFullScr_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            if (ImgRect == null || ChoosenOne == null)
            {
                return;
            }

            btnScaleOut_Click(this, e);
        }

        private void btnMinScr_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            if (ChoosenOne == null)
                return;
            TurnZoomToOne();
            if (ChoosenOne.photoHeight > ChoosenOne.photoWidth)
            {
                ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
                ImgRect.VerticalAlignment = VerticalAlignment.Center;

                button6_Click(this, e);
            }
            else
            {
                ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
                ImgRect.VerticalAlignment = VerticalAlignment.Center;

                button6_Click(this, e);
            }
           
        }

        private void btnOneToOne_Click(object sender, RoutedEventArgs e)
        {
         
            ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
            ImgRect.VerticalAlignment = VerticalAlignment.Center;
            MyScrollViewer.Width = this.Width;
            MyScrollViewer.Height = this.Height - 80;

            double wd;
            double ht;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;

            zoom = ChoosenOne.photoWidth / ImgRect.Width;
            zoom = Math.Round(zoom, 1);
            ScaleTransform scaleTransform1 = new ScaleTransform(zoom, zoom);
            ImgRect.LayoutTransform = scaleTransform1;
        }

        
        private void btnScaleOut_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
           
            zoom = zoom - 0.3;
            zoom = Math.Round(zoom, 1);
            if (zoom <= 1)
            {
                preventImageEscape();
                TurnZoomToOne();
                return;
            }

            ScaleTransform scaleTransform2 = new ScaleTransform(1 * zoom, 1 * zoom);
            ImgRect.LayoutTransform = scaleTransform2;

            CheckBorderScrollViewer();
        }

        private void btnScaleIn_Click(object sender, RoutedEventArgs e)
        {
            if (zoom >= 4||ChoosenOne==null)
            {
                return;
            }

            ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
            ImgRect.VerticalAlignment = VerticalAlignment.Center;
            MyScrollViewer.Width = this.Width;
            MyScrollViewer.Height = this.Height - 80;

            double wd ;
            double ht ;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;

            zoom = zoom + 0.3;
            zoom = Math.Round(zoom, 1);
            ScaleTransform scaleTransform1 = new ScaleTransform(zoom, zoom);
            ImgRect.LayoutTransform = scaleTransform1;
        }

        private void myWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TurnZoomToOne();

            if (this.WindowState == WindowState.Maximized)
            {
                btnFullScr_Click(this, e);
                return;
            }

            ColumnsNumber = (int)this.Width / 120;

        }


        int Angle = 0;


        private void btnRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
            CheckBorderScrollViewer();
            preventImageEscape();
            TurnZoomToOne();
            Angle = Angle - 90;
            if (Angle == 360 || Angle == -360)
                Angle = 0;
            Image rotated90 = new Image();
            TransformedBitmap tb = new TransformedBitmap();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(ChoosenOne.path, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            // Properties must be set between BeginInit and EndInit calls.
            tb.BeginInit();
            tb.Source = bi;
            // Set image rotation.
            RotateTransform transform = new RotateTransform(Angle);
            tb.Transform = transform;
            tb.EndInit();
            // Set the Image source.
            rotated90.Source = tb;
            ImgRect.Fill = new ImageBrush(rotated90.Source);
            ChoosenOne.photoWidth = (rotated90.Source as BitmapSource).PixelWidth;
            ChoosenOne.photoHeight = (rotated90.Source as BitmapSource).PixelHeight;
            double wd;
            double ht;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;
        }

        private void RotateBack()
        {
            if (ChoosenOne == null)
                return;
            CheckBorderScrollViewer();
            preventImageEscape();
            TurnZoomToOne();
            Angle = 0;
            Image rotated90 = new Image();
            TransformedBitmap tb = new TransformedBitmap();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(ChoosenOne.path, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            // Properties must be set between BeginInit and EndInit calls.
            tb.BeginInit();
            tb.Source = bi;
            // Set image rotation.
            RotateTransform transform = new RotateTransform(Angle);
            tb.Transform = transform;
            tb.EndInit();
            // Set the Image source.
            rotated90.Source = tb;
            ImgRect.Fill = new ImageBrush(rotated90.Source);
            ChoosenOne.photoWidth = (rotated90.Source as BitmapSource).PixelWidth;
            ChoosenOne.photoHeight = (rotated90.Source as BitmapSource).PixelHeight;
            double wd;
            double ht;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;
        }

        private void preventImageEscape()
        {
            Matrix m = ImgRect.RenderTransform.Value;
            m.OffsetX = 0;
            m.OffsetY = 0;
            ImgRect.RenderTransform = new MatrixTransform(m);
        }

        private void btnRotateRight_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
            CheckBorderScrollViewer();
            preventImageEscape();
            TurnZoomToOne();
            Angle = Angle +90;
            if (Angle == 360 || Angle == -360)
                Angle = 0;
            Image rotated90 = new Image();
            TransformedBitmap tb = new TransformedBitmap();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(ChoosenOne.path, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            // Properties must be set between BeginInit and EndInit calls.
            tb.BeginInit();
            tb.Source = bi;
            // Set image rotation.
            RotateTransform transform = new RotateTransform(Angle);
            tb.Transform = transform;
            tb.EndInit();
            // Set the Image source.
            rotated90.Source = tb;
            ImgRect.Fill = new ImageBrush(rotated90.Source);
             ChoosenOne.photoWidth = (rotated90.Source as BitmapSource).PixelWidth;
            ChoosenOne.photoHeight = (rotated90.Source as BitmapSource).PixelHeight;

            double wd = 1;
            double ht = 1;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;
        }

        public void TurnZoomToOne()
        {
            if (ChoosenOne==null)
                return;

            zoom = 1;
            ScaleTransform scaleTransform1 = new ScaleTransform(1, 1);
            ImgRect.LayoutTransform = scaleTransform1;
            ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
            ImgRect.VerticalAlignment = VerticalAlignment.Center;
            MyScrollViewer.Width = this.Width;
            MyScrollViewer.Height = this.Height - 80;

            double wd = 1;
            double ht = 1;
            wd = ChoosenOne.photoWidth;
            ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;
            ImgRect.Height = this.Height - 80;
            ImgRect.Width = ImgRect.Height * coefe;
            return;
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
            preventImageEscape();
            TurnZoomToOne();
            ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
            ImgRect.VerticalAlignment = VerticalAlignment.Center;

            double wd = ChoosenOne.photoWidth;
            double ht = ChoosenOne.photoHeight;
            double coefe = wd/ht;
            ImgRect.Width = ImgRect.Height*coefe;
            this.Width= ImgRect.Width;
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
            preventImageEscape();
            TurnZoomToOne();
            ImgRect.HorizontalAlignment = HorizontalAlignment.Center;
            ImgRect.VerticalAlignment = VerticalAlignment.Center;

            double wd = ChoosenOne.photoWidth;
            double ht = ChoosenOne.photoHeight;
            double coefe = wd / ht;

            ImgRect.Width = this.Width;

            ImgRect.Height = ImgRect.Width / coefe;
            this.Height = ImgRect.Height+80;
        }


        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null)
                return;
            string folder = Path.GetDirectoryName(ChoosenOne.path);
            Process.Start(folder);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
           OpenFileDialog ofd = new OpenFileDialog();
           ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
           ofd.Title = "Выберите фото, местонахождения которого вам необходимо";
            ofd.Multiselect = true;

           if (ofd.ShowDialog() == true)
           {
                string[] files = ofd.FileNames;
                int SameFile = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    ExifInfo preExInfo = new ExifInfo();
                    preExInfo = OF(files[i]);


                    ExifInfo exInfo = new ExifInfo();
                    exInfo = OF(files[i]);

                    if (myAdressList.Contains(files[i]))
                    {
                        SameFile++;
                        continue;
                    }
                    myAdressList.Add(files[i]);

                    PointLatLng LLTpointer = new PointLatLng();
                    LLTpointer.Lat = exInfo.Lat;
                    LLTpointer.Lng = exInfo.Long;
                    GMapMarker it = new GMapMarker(LLTpointer);
                    it.ZIndex = 5;
                    List<Placemark> plc = null;
                    var st = GMapProviders.GoogleMap.GetPlacemarks(new PointLatLng(exInfo.Lat, exInfo.Long), out plc);

                    foreach (var pl in plc)
                    {
                        if (!string.IsNullOrEmpty(pl.PostalCodeNumber))
                        {
                            exInfo.photoPlace = pl.CountryName + " " + pl.AdministrativeAreaName;
                        }
                    }

                    PhotoMarker A = new PhotoMarker(this, it, files[i], photoCounter, exInfo);

                    // ADD FILE TO PHOTO VIEW
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.DecodePixelWidth = 240;
                    bi.DecodePixelHeight = 220;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(files[i]);
                    bi.EndInit();

                    ListViewItemsCollections.Add(new ListViewItemsData()
                    {
                        GridViewColumnName_ImageSource = files[i],
                        GridViewDate = exInfo.photoTime.ToShortDateString(),
                        GridImageBrush = bi
                    }); 

                    ListView1.ItemsSource = ListViewItemsCollections;


                    ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(ListView1.ItemsSource);

                    view.GroupDescriptions.Clear();

                    PropertyGroupDescription groupDescription = new PropertyGroupDescription("GridViewDate");
                    view.GroupDescriptions.Add(groupDescription);
                    view.CustomSort = new DateTimeComparer();

                    it.Shape = A;
                    myphotoMarkerList.Add(A);
                    it.Tag = photoCounter;
                    MainMap.Markers.Add(it);
                    photoCounter++;
                    MainMap.Position = LLTpointer;
                }
                if (SameFile != 0)
                    MessageBox.Show(SameFile + " - number of files already noticed on map", "Similar files found");
                CheckBorderScrollViewer();
            }

        }

        Point start;
        Point origin;

        private void ImgRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImgRect.IsMouseCaptured) return;
            ImgRect.CaptureMouse();
            start = e.GetPosition(MyScrollViewer);
            origin.X = ImgRect.RenderTransform.Value.OffsetX;
            origin.Y = ImgRect.RenderTransform.Value.OffsetY;
            
        }

        const double minimum = 0;
        public double maximumX;
        public double maximumY;
        private void ImgRect_MouseMove(object sender, MouseEventArgs e)
        {
            if (MyScrollViewer.Width - ImgRect.Width * zoom > 0)
                maximumX = 0;
            else
                maximumX = (zoom * ImgRect.Width) - MyScrollViewer.Width;

            if (MyScrollViewer.Height - ImgRect.Height * zoom > 0)
                maximumY = 0;
            else
                maximumY = (zoom * ImgRect.Height) - MyScrollViewer.Height;

            if (!ImgRect.IsMouseCaptured||zoom<=1)
                return;

            Point p = e.GetPosition(MyScrollViewer);
            Matrix m = ImgRect.RenderTransform.Value;

            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);
            

            if (m.OffsetX > - minimum)
                m.OffsetX = - minimum;

            if (-m.OffsetX > maximumX)
                m.OffsetX = -maximumX;

            if (m.OffsetY > - minimum)
                m.OffsetY = - minimum;

            if (-m.OffsetY > maximumY)
                m.OffsetY = -maximumY;

            ImgRect.RenderTransform = new MatrixTransform(m);

        }

        void CheckBorderScrollViewer()
        {
            maximumX = (zoom * ImgRect.Width) - MyScrollViewer.Width;
            maximumY = (zoom * ImgRect.Height) - MyScrollViewer.Height;

            Matrix m = ImgRect.RenderTransform.Value;

            if (m.OffsetX > -minimum)
                m.OffsetX = -minimum;

            if (-m.OffsetX > maximumX)
                m.OffsetX = -maximumX;

            if (m.OffsetY > -minimum)
                m.OffsetY = -minimum;

            if (-m.OffsetY > maximumY)
                m.OffsetY = -maximumY;

            ImgRect.RenderTransform = new MatrixTransform(m);
        }

        private void ImgRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ImgRect.ReleaseMouseCapture();
        }


        private void MyScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta>0)
            {
                btnScaleIn_Click(this, e);
            }
            else 
            {
                btnScaleOut_Click(this, e);
            }
            e.Handled = true;
        }

        private void btnPhotos_Checked(object sender, RoutedEventArgs e)
        {
            MapGrid.Visibility = Visibility.Hidden;
            FoldersGrid.Visibility = Visibility.Hidden;
            PhotosGrid.Visibility = Visibility.Visible;
            OptionsGrid.Visibility = Visibility.Hidden;
        }

        private void btnMap_Checked(object sender, RoutedEventArgs e)
        {
            if (MapGrid == null || FoldersGrid == null || PhotosGrid == null || OptionsGrid == null)
                return;
            MapGrid.Visibility = Visibility.Visible;
            FoldersGrid.Visibility = Visibility.Hidden;
            PhotosGrid.Visibility = Visibility.Hidden;
            OptionsGrid.Visibility = Visibility.Hidden;
        }

        private void btnFolders_Checked(object sender, RoutedEventArgs e)
        {
            MapGrid.Visibility = Visibility.Hidden;
            FoldersGrid.Visibility = Visibility.Hidden;
            PhotosGrid.Visibility = Visibility.Hidden;
            OptionsGrid.Visibility = Visibility.Hidden;
        }

        private void btnOptions_Checked(object sender, RoutedEventArgs e)
        {
            MapGrid.Visibility = Visibility.Hidden;
            FoldersGrid.Visibility = Visibility.Hidden;
            PhotosGrid.Visibility = Visibility.Hidden;
            OptionsGrid.Visibility = Visibility.Visible;
        }


        public void MapHide()
        {
            btnMap1.Visibility = Visibility.Hidden;
            btnMap1Grd.Visibility = Visibility.Hidden;

            btnFolders1.Visibility = Visibility.Hidden;
            btnFolders1Grd.Visibility = Visibility.Hidden;

            btnPhotos1.Visibility = Visibility.Hidden;
            btnPhotos1Grd.Visibility = Visibility.Hidden;

            btnOptions1.Visibility = Visibility.Hidden;
            btnOptions1Grd.Visibility = Visibility.Hidden;

            btnLeftMenuHide.Visibility = Visibility.Hidden;
            btnLeftMenuShow.Visibility = Visibility.Hidden;

            StackLabel.Visibility = Visibility.Hidden;
            panelRect.Visibility = Visibility.Hidden;

            pnlLeftMenu.Visibility = Visibility.Hidden;
          
        }

        public void MapShow()
        {
           
            btnMap1.Visibility = Visibility.Visible;
            btnMap1Grd.Visibility = Visibility.Visible;
            pnlLeftMenu.Visibility = Visibility.Visible;


            // Not Done Yet
            btnFolders1.Visibility = Visibility.Hidden;
            btnFolders1Grd.Visibility = Visibility.Hidden;

            btnPhotos1.Visibility = Visibility.Visible;
            btnPhotos1Grd.Visibility = Visibility.Visible;

            // Not Done Yet
            btnOptions1.Visibility = Visibility.Hidden;
            btnOptions1Grd.Visibility = Visibility.Hidden;

            btnLeftMenuHide.Visibility = Visibility.Visible;
            btnLeftMenuShow.Visibility = Visibility.Visible;

            StackLabel.Visibility = Visibility.Visible;
            panelRect.Visibility = Visibility.Visible;

        }



        // LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT LISTVIEW CODE NEXT


        public ObservableCollection<ListViewItemsData> ListViewItemsCollections { get { return _ListViewItemsCollections; } }
        ObservableCollection<ListViewItemsData> _ListViewItemsCollections = new ObservableCollection<ListViewItemsData>();

        

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListView1.SelectedItem == null)
                return;
            ListViewItemsData A = (ListViewItemsData)ListView1.SelectedItem;
            ChoosenOne = myphotoMarkerList.Find(meh => meh.path == A.GridViewColumnName_ImageSource);
            ChoosenOne.PhotoMarker_MouseLeftButtonDown(this, e);

        }

    }


    public class ListViewItemsData
    {
        public string GridViewColumnName_ImageSource { get; set; }
        public string GridViewDate { get; set; }
        public ImageSource GridImageBrush { get; set; }
    }


}
