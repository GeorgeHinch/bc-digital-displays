﻿using BC_Digital_Displays.Classes;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BC_Digital_Displays
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReciprocalClubPage : Page
    {
        public static ReciprocalClubPage reciprocalClubPage;
        public ReciprocalClubPage()
        {
            this.InitializeComponent();
            reciprocalClubPage = this;

            this.Loaded += UserControl_Loaded;

            BasicGeoposition snPosition = new BasicGeoposition() { Latitude = 47.605484, Longitude = -122.189529 };
            Geopoint snPoint = new Geopoint(snPosition);

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = snPoint;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "Bellevue Club";
            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            clubMap.MapElements.Add(mapIcon1);

            // Center the map over the POI.
            clubMap.Center = snPoint;
            clubMap.ZoomLevel = 3;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = Task.Run(async () => { await loadReciprocalClubs(); });
                task.Wait();

                IEnumerable<bcReciprocalClubs> itemsControl = items;

                foreach (bcReciprocalClubs i in items)
                {
                    // Create map geoposition from lat and long
                    BasicGeoposition clubPosition = new BasicGeoposition() { Latitude = Convert.ToDouble(i.addressLat), Longitude = Convert.ToDouble(i.addressLong) };
                    Geopoint clubPoint = new Geopoint(clubPosition);

                    // Create a MapIcon.
                    MapIcon mapIcon = new MapIcon();
                    mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    mapIcon.Location = clubPoint;
                    mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    mapIcon.Title = i.clubName;
                    mapIcon.ZIndex = 0;

                    clubMap.MapElementClick += ClubMap_MapElementClick;
                    // Add the MapIcon to the map.
                    clubMap.MapElements.Add(mapIcon);
                }
                
                clubListView.Tag = items;
                clubListView.loadContent();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message + " | ");
            }
        }

        public static bool iconClicked = false;
        private void ClubMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            Debug.WriteLine("Sender is: " + sender.GetType().ToString() + "|");
            
            if (!iconClicked)
            {
                iconClicked = true;

                MapIcon mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;

                foreach (bcReciprocalClubs club in items)
                {
                    if (mapIcon.Title == club.clubName)
                    {
                        clubMap.Visibility = Visibility.Collapsed;
                        clubList.Visibility = Visibility.Visible;

                        clubListView.returnList.Visibility = Visibility.Collapsed;
                        clubListView.returnMap.Visibility = Visibility.Visible;

                        TextBlock tb = new TextBlock();
                        tb.Tag = club;

                        clubListView.moreInfo_Tapped(tb, null);
                    }
                }
            }
        }

        #region Load reciprocal clubs
        private List<bcReciprocalClubs> items = new List<bcReciprocalClubs>();
        private IMobileServiceTable<bcReciprocalClubs> bcReciprocalClubsTable = App.MobileService.GetTable<bcReciprocalClubs>();
        public async Task loadReciprocalClubs()
        {
        MobileServiceInvalidOperationException exception = null;
            try
            {
                var rowsNum = await bcReciprocalClubsTable
                    .Where(aClub => aClub.deleted == false)
                    .IncludeTotalCount().ToCollectionAsync();

                int loopTimes = ((int)rowsNum.TotalCount - 1) / 50 + 1;
                int skipNum = 0;

                for (int i = 0; i < loopTimes; i++)
                {
                    IList<bcReciprocalClubs> tempItems = await bcReciprocalClubsTable
                        .Where(aClub => aClub.deleted == false)
                        .OrderBy(aClub => aClub.sortCountry)
                        .ThenBy(aClub => aClub.sortState)
                        .ThenBy(aClub => aClub.sortCity)
                        .ThenBy(aClub => aClub.clubName)
                        .Skip(skipNum)
                        .Take(50)
                        .ToCollectionAsync();
                    
                    items.AddRange(tempItems);

                    skipNum = skipNum + 50;
                }
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
                Debug.WriteLine("Exception: " + exception.Message + " | ");
                GoogleAnalytics.EasyTracker.GetTracker().SendException(e.Message, false);
            }
        }
        #endregion

        private void GoBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainMenu.mainMenu.menuFlipView.Visibility = Visibility.Visible;
            MainMenu.mainMenu.mainFrame.Navigate(typeof(Page));
        }

        private void mapRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clubMap.Visibility = Visibility.Visible;
            clubList.Visibility = Visibility.Collapsed;

            clubListView.moreInfoGrid.Visibility = Visibility.Collapsed;
        }

        private void listRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clubMap.Visibility = Visibility.Collapsed;
            clubList.Visibility = Visibility.Visible;
        }
    }
}
