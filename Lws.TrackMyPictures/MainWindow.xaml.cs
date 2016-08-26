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
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;
            PointLatLng? pos = GMapProviders.GoogleMap.GetPoint("Ukraine, Kyiv", out status);
            MainMap.Position = new PointLatLng(pos.Value.Lat, pos.Value.Lng);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 9;
            i = 0;


            
            MapComboBox.DisplayMemberPath = "Name";
            MapComboBox.Items[0] = GMapProviders.GoogleMap;
            MapComboBox.Items[1] = GMapProviders.YandexMap;
            MapComboBox.Items[2] = GMapProviders.OpenStreetMap;
            MapComboBox.Items[3] = GMapProviders.BingMap;
            MapComboBox.SelectedItem = MainMap.MapProvider;
        }

        public int i;


        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {
            if (i==0)
            {
                ShowHideMenu("sbShowLeftMenu", "LblLeft",btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu, StackLabel);
                i++;
                this.DataContext = i;
            }
           
        }

        private void btnLeftMenuHide_Click(object sender, RoutedEventArgs e)
        {
            if (i == 1)
            {
                ShowHideMenu("sbHideLeftMenu", "LblRight", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu, StackLabel);
                i--;
                this.DataContext = i;
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
    }
}
