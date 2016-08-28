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
        GMapMarker currentMarker;
        GeoCoderStatusCode status;
        List<string> myAdressList = new List<string>();
        int photoCounter;
        public int i;

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

            currentMarker = new GMapMarker(MainMap.Position);
            {
                currentMarker.Offset = new System.Windows.Point(-15, -15);
                currentMarker.ZIndex = int.MaxValue;
                MainMap.Markers.Add(currentMarker);
            }

            

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

        double TenLats;
        double TenLongs;

        public double[] OF(object filename)
        {
            path = (string)filename;
                    using (var reader = new ExifReader(path))
                    {
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
                            TenLats = myLats[0] + myLats[1] / 60 + myLats[2] / 3600;
                        }

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
                            TenLongs = myLongs[0] + myLongs[1] / 60 + myLongs[2] / 3600;
                        }

                        DateTime datePictureTaken;
                        reader.GetTagValue(ExifTags.DateTimeOriginal, out datePictureTaken);
                double[] Coordinates = { TenLats, TenLongs };
                return Coordinates;
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
                
                double[] coords = OF(files[i]);

                if (myAdressList.Contains(files[i]))
                {
                    SameFile++;
                    continue;
                }
                myAdressList.Add(files[i]);
               
                PointLatLng LLTpointer = new PointLatLng();
                LLTpointer.Lat = coords[0];
                LLTpointer.Lng = coords[1];
                GMapMarker it = new GMapMarker(LLTpointer);
                it.ZIndex = 5;
                it.Shape = new PhotoMarker(this, it, files[i]);
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
    }


    }
