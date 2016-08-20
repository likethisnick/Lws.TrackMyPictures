using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
        public MainWindow()
        {
            InitializeComponent();
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
            MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            MainMap.IgnoreMarkerOnMouseWheel = true;
            MainMap.ShowCenter = false;
            MainMap.MapProvider = GMapProviders.GoogleMap;
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;
            PointLatLng? pos = GMapProviders.GoogleMap.GetPoint("Ukraine, Kyiv", out status);
            MainMap.Position = new PointLatLng(pos.Value.Lat, pos.Value.Lng);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 9;
        }

        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }

        private void btnLeftMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }

        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
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

        
    }
}
