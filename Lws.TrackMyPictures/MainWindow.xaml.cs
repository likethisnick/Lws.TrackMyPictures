using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using ExifLib;
using System.Threading;
using Microsoft.Win32;
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
        // marker
        public PhotoMarker ChoosenOne;
        GeoCoderStatusCode status;
        List<string> myAdressList = new List<string>();
        List<PhotoMarker> myphotoMarkerList = new List<PhotoMarker>();
        int photoCounter;
        public int i;

        public string pathh
        {
            get; set;
        }
        public string path
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
            MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.MapProvider = GMapProviders.YandexMap;
            status = GeoCoderStatusCode.Unknow;
            PointLatLng? pos = GMapProviders.GoogleMap.GetPoint("Ukraine, Kyiv", out status);
            MainMap.Position = new PointLatLng(pos.Value.Lat, pos.Value.Lng);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 9;
            photoCounter = 0;
            i = 0;

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
 
            if (i == 0)
            {
                ShowHideInfo("sbShowInfo", btnInfoHide, btnInfoShow, PhotoInfoPanel);
                i++;
            }
        }

        private void btnInfoHide_Click(object sender, RoutedEventArgs e)
        {
        
            if (i == 1)
            {
                ShowHideInfo("sbHideInfo", btnInfoHide, btnInfoShow, PhotoInfoPanel);
                i--;
            }
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

        /* LATER MAKE CHOOSEN ITEM DISSAPEAR FROM LIST

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i <= MapComboBox.Items.Count - 1; i++)
            {
                if (((ComboBoxItem)(MapComboBox.Items[i])).Content == ((ComboBoxItem)MapComboBox.SelectedItem).Content)
                {
                    ((ComboBoxItem)(MapComboBox.Items[i])).Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                    ((ComboBoxItem)(MapComboBox.Items[i])).Visibility = System.Windows.Visibility.Visible;
            }
        } */

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {

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
                    string pT = string.Format(PhotoDateTime+"");

                    exifInfo.photoTime = pT;
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
            /*
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            ofd.Title = "Выберите фото, местонахождения которого вам необходимо";
            
            if (ofd.ShowDialog() == true)
            {
                path = ofd.FileName;
            }*/

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int SameFile = 0;
            for (int i = 0; i < files.Length; i++)
            {
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
                        exInfo.photoPlace = pl.CountryName +" "+ pl.AdministrativeAreaName;
                    }
                }

                PhotoMarker A = new PhotoMarker(this, it, files[i], photoCounter, exInfo);
                it.Shape = A;
                myphotoMarkerList.Add(A);
                it.Tag = photoCounter;
                MainMap.Markers.Add(it);
                photoCounter++;
                MainMap.Position = LLTpointer;
            }
            if(SameFile!=0)
            MessageBox.Show(SameFile + " - number of files already noticed on map", "Similar files found");

        }

        private void btnBackToMap_Click(object sender, RoutedEventArgs e)
        {
            MapGrid.Visibility = Visibility.Visible;
            MarkerGrid.Visibility = Visibility.Hidden;
            MainMap.CanDragMap = true;
        }


        private void btnToLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null || MainMap == null || myphotoMarkerList.Count<2)
                return;

            if (ChoosenOne.Number<MainMap.Markers.Count&&ChoosenOne.Number>0)
            {
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == (ChoosenOne.Number-1));
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblCommentary.Content = ChoosenOne.Number;
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                return;
            }

            if (ChoosenOne.Number == 0)
            {
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == myphotoMarkerList.Count - 1);
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblCommentary.Content = ChoosenOne.Number;
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
            }


        }

        private void btnToRight_Click(object sender, RoutedEventArgs e)
        {
            if (ChoosenOne == null||MainMap==null || myphotoMarkerList.Count<2)
                return;
            
                if(ChoosenOne.Number<MainMap.Markers.Count-1&&ChoosenOne.Number>-1)
            {
                ChoosenOne = myphotoMarkerList.Find(meh=>meh.Number==(ChoosenOne.Number+1));
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblCommentary.Content = ChoosenOne.Number;
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
                return;
            }

            if (ChoosenOne.Number == MainMap.Markers.Count - 1)
            {
                ChoosenOne = myphotoMarkerList.Find(meh => meh.Number == 0);
                lblFileName.Content = Path.GetFileName(ChoosenOne.path);
                ImgRect.Fill = new ImageBrush(new BitmapImage(new Uri(ChoosenOne.path)));

                lblCommentary.Content = ChoosenOne.Number;
                lblDateTime.Text = ChoosenOne.Date;
                lblPhotoPlace.Text = ChoosenOne.Place;
                lblPhotoExtension.Content = ChoosenOne.Extension + ", " + Math.Round(ChoosenOne.fllength, 2) + " MB";
                lblRes.Content = ChoosenOne.photoWidth + "x" + ChoosenOne.photoHeight;
                lblCameraInfo.Content = ChoosenOne.CameraInfo;
                lblCameraAddInfo.Content = ChoosenOne.CameraInfoAdd;
            }


        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            MainMap.Position = new PointLatLng(ChoosenOne.Lat, ChoosenOne.Long);
            MapGrid.Visibility = Visibility.Visible;
            MarkerGrid.Visibility = Visibility.Hidden;
            MainMap.CanDragMap = true;
        }

        private void btnFullScr_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void btnMinScr_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

        }
    }


    }
