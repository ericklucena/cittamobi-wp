using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;
using CittaMobiWP.Models;
using CittaMobiWP.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CittaMobiWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int currentStopId;
        private string currentStopMnemonic;

        private Geopoint lastBottomLeft;
        private Geopoint lastTopRight;

        private double SearchTime;

        private bool stopsLoadingFailed;

        private DateTime LastBusStopUpdate { get; set; }
        public ObservableCollection<Stop> CurrentStopList { get; set; }
        public ObservableCollection<Service> CurrentPredictionList { get; set; }
        public ObservableCollection<UserLocation> CurrentCoordinate { get; set; }

        private DispatcherTimer predictionsTimer;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //busLines.ItemsSource = Util.LoadLines();

            CreateLists();
            currentStopId = -1;
            stopsLoadingFailed = false;

            favoriteStops.ItemsSource = AppSettings.Instance.FavoriteStops;

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            //MyAdRotatorControl.PlatformAdProviderComponents.Add(AdRotator.Model.AdType.PubCenter, typeof(AdDuplex.Universal.Controls.WinPhone.XAML.AdControl));
            //MyAdRotatorControl.PlatformAdProviderComponents.Add(AdRotator.Model.AdType.AdDuplex, typeof(AdDuplex.Universal.Controls.WinPhone.XAML.AdControl));
            //MyAdRotatorControl.PlatformAdProviderComponents.Add(AdRotator.Model.AdType.AdMob, typeof(GoogleAds.AdView));
        }

        private void StartTimers()
        {
            predictionsTimer = new DispatcherTimer();
            predictionsTimer.Tick += predictionsTimer_Tick;
            predictionsTimer.Interval = new TimeSpan(0, 0, 30);
            predictionsTimer.Start();
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else if(MainPivot.SelectedItem != MapPivot)
            {
                MainPivot.SelectedItem = MapPivot;
            }
            else
            {
                var msg = new MessageDialog("Deseja fechar o aplicativo?");
                var okBtn = new UICommand("Sim");
                var cancelBtn = new UICommand("Não");
                msg.Commands.Add(okBtn);
                msg.Commands.Add(cancelBtn);
                IUICommand result = await msg.ShowAsync();

                if (result != null && result.Label == "Sim")
                {
                    Application.Current.Exit();
                }
            }
        }

        private void CreateLists()
        {
            CurrentPredictionList = new ObservableCollection<Service>();
            CurrentStopList = new ObservableCollection<Stop>();
            CurrentCoordinate = new ObservableCollection<UserLocation>();
        }

        private void BindLists()
        {
            predictions.ItemsSource = CurrentPredictionList;
            MapStops.ItemsSource = CurrentStopList;
            UserLocation.ItemsSource = CurrentCoordinate;
        }

        private void predictionsTimer_Tick(object sender, object e)
        {
            getPredictionsInfo();
            getStopsInfo();
        }

        private void busStopsTimer_Tick(object sender, object e)
        {
            getStopsInfo();
        }

        // Handle selection changed on ListBox
        private void busLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(Vehicles), e.AddedItems.First());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CreateLists();
            BindLists();
            CenterMap();
            getStopsInfo();
            getPredictionsInfo();
            StartTimers();
            SearchTime = 0;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            predictionsTimer.Stop();
            predictionsTimer = null;
            base.OnNavigatedFrom(e);
        }

        private async void getStopsInfo()
        {
            string errorMessage = null;

            Geopoint bottomLeft = MapExtensions.GetBottomLeftCorner(mapControl);
            Geopoint topRight = MapExtensions.GetTopRightCorner(mapControl);

            if (mapControl.ZoomLevel >= 16 && 
                (stopsLoadingFailed || lastTopRight == null || lastBottomLeft == null ||
                 bottomLeft.Position.Latitude != lastBottomLeft.Position.Latitude ||
                 bottomLeft.Position.Longitude != lastBottomLeft.Position.Longitude ||
                 topRight.Position.Latitude != lastTopRight.Position.Latitude ||
                 topRight.Position.Longitude != lastTopRight.Position.Longitude))
            {
                lastBottomLeft = bottomLeft;
                lastTopRight = topRight;

                MapProgressBar.Visibility = Visibility.Visible;

                try
                {
                    string webresponse = await Network.StopsByViewport(bottomLeft, topRight);

                    List<Stop> rootObject = JsonConvert.DeserializeObject<List<Stop>>(webresponse);

                    CurrentStopList.Clear();

                    foreach (Stop s in rootObject)
                    {
                        CurrentStopList.Add(s);
                    }
                    LastBusStopUpdate = DateTime.Now;
                    stopsLoadingFailed = false;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    stopsLoadingFailed = true;
                }

                if (errorMessage != null)
                {
                    MessageDialog msg = new MessageDialog("Houve algum problema durante a requisição ao servidor. Por favor, verifique se o aparelho está conectado à internet e tente novamente.", "Você está conectado à internet?");
                    await msg.ShowAsync();
                }

                MapProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void getPredictionsInfo()
        {
            string errorMessage = null;

            if (currentStopId < 0)
            {
                return;
            }

            PredictionProgressBar.Visibility = Visibility.Visible;
            try
            {
                string webresponse = await Network.PredictionsByStop(currentStopId);

                Prediction predictions = JsonConvert.DeserializeObject<Prediction>(webresponse);
                List<Service> filtered = new List<Service>();
                foreach (Service s in predictions.Services)
                {
                    if (s.Vehicles != null && s.Vehicles.Count > 0)
                    {
                        filtered.Add(s);
                    }
                }
                filtered.Sort();

                CurrentPredictionList.Clear();

                foreach(Service s in filtered)
                {
                    CurrentPredictionList.Add(s);
                }

                if (CurrentPredictionList.Count == 0)
                {
                    EmptyPredictionsList.Visibility = Visibility.Visible;
                }
                else
                {
                    EmptyPredictionsList.Visibility = Visibility.Collapsed;
                }

                LastUpdate.Text = "Ultima atualização: " + DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (errorMessage != null)
            {
                MessageDialog msg = new MessageDialog("Houve algum problema durante a requisição ao servidor. Por favor, verifique se o aparelho está conectado à internet e tente novamente.", "Você está conectado à internet?");
                await msg.ShowAsync();
            }

            PredictionProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CenterMap()
        {
            string errorMessage = null;

            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Geolocator geolocator = new Geolocator();
                    Geoposition geoposition = null;
                    geoposition = await geolocator.GetGeopositionAsync();
                    CurrentCoordinate.Clear();
                    CurrentCoordinate.Add(new UserLocation(geoposition.Coordinate));

                    await mapControl.TrySetViewAsync(geoposition.Coordinate.Point, 16, 0, 0, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    //mapControl.Center = geoposition.Coordinate.Point;
                    //mapControl.ZoomLevel = 16;
                    getStopsInfo();                   
                });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (errorMessage != null)
            {
                MessageDialog msg = new MessageDialog("Não foi possível localizar o seu dispositivo. Por favor verifique se o sistema de localização de seu telefone está ativado para uma melhor experiência de uso.", "Seu GPS está ativado?");
                await msg.ShowAsync();
            }


        }

        private void MapControl_CenterChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {
            if (mapControl.LoadingStatus == MapLoadingStatus.Loaded)
            {
                getStopsInfo();
            }
        }

        private void busIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int newStopId = int.Parse(((Image)sender).Tag.ToString());

            if (newStopId != currentStopId)
            {
                CurrentPredictionList.Clear();
                currentStopId = newStopId;

            }

            foreach(Stop s in CurrentStopList)
            {
                if(s.Id == currentStopId)
                {
                    SelectedStopName.Text = s.Mnemonic;
                    this.currentStopMnemonic = s.Mnemonic;
                }
            }

            getPredictionsInfo();
            MainPivot.SelectedItem = PredictionPivot;
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPivot.SelectedItem == MapPivot)
            {
                AppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                CenterMapButton.Visibility = Visibility.Visible;
                FavoriteButton.Visibility = Visibility.Collapsed;
                UnFavoriteButton.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
                EditFavoriteStopsButton.Visibility = Visibility.Collapsed;
            }
            else if (MainPivot.SelectedItem == PredictionPivot)
            {
                AppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                CenterMapButton.Visibility = Visibility.Collapsed;
                UpdateFavoriteButtons();
                SearchButton.Visibility = Visibility.Visible;
                EditFavoriteStopsButton.Visibility = Visibility.Collapsed;
            }
            else if (MainPivot.SelectedItem == FavoriteStopsPivot)
            {
                AppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                CenterMapButton.Visibility = Visibility.Collapsed;
                FavoriteButton.Visibility = Visibility.Collapsed;
                UnFavoriteButton.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
                EditFavoriteStopsButton.Visibility = Visibility.Visible;
                favoriteStops.ItemsSource = AppSettings.Instance.FavoriteStops;
            }
            else if (MainPivot.SelectedItem == SearchPivot)
            {
                AppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                CenterMapButton.Visibility = Visibility.Collapsed;
                FavoriteButton.Visibility = Visibility.Collapsed;
                UnFavoriteButton.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Collapsed;
                EditFavoriteStopsButton.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateFavoriteButtons()
        {
            if (currentStopId > 0)
            {
                if (AppSettings.Instance.IsFavouritedStop(currentStopId))
                {
                    FavoriteButton.Visibility = Visibility.Collapsed;
                    UnFavoriteButton.Visibility = Visibility.Visible;
                }
                else
                {
                    FavoriteButton.Visibility = Visibility.Visible;
                    UnFavoriteButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.AddFavouriteStop(currentStopId, currentStopMnemonic);
            UpdateFavoriteButtons();
        }

        private void UnFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.RemoveFavouriteStop(currentStopId);
            UpdateFavoriteButtons();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        private void TicketsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Tickets));
        }

        private void predictions_ItemClick(object sender, ItemClickEventArgs e)
        {
            VehiclesPayload v = new VehiclesPayload(currentStopId, (Service) e.ClickedItem);
            Frame.Navigate(typeof(Vehicles), v);
        }

        private void CenterMapButton_Click(object sender, RoutedEventArgs e)
        {
            CenterMap();
        }

        private void favoriteStops_ItemClick(object sender, ItemClickEventArgs e)
        {
            int newStopId = (e.ClickedItem as FavoriteStop).Id;

            if (newStopId != currentStopId)
            {
                CurrentPredictionList.Clear();
                currentStopId = newStopId;

            }
            this.currentStopMnemonic = (e.ClickedItem as FavoriteStop).Mnemonic;
            SelectedStopName.Text = currentStopMnemonic;
            getPredictionsInfo();
            MainPivot.SelectedItem = PredictionPivot;
        }

        private void EditFavoriteStopsButton_Checked(object sender, RoutedEventArgs e)
        {
            editableFavoriteStops.ItemsSource = favoriteStops.ItemsSource;
            favoriteStops.Visibility = Visibility.Collapsed;
            editableFavoriteStops.Visibility = Visibility.Visible;
        }

        private void EditFavoriteStopsButton_Unchecked(object sender, RoutedEventArgs e)
        {
            List<FavoriteStop> stops = editableFavoriteStops.ItemsSource as List<FavoriteStop>;

            foreach (FavoriteStop stop in stops)
            {
                AppSettings.Instance.AddFavouriteStop(stop.Id, stop.Mnemonic);
            }

            favoriteStops.ItemsSource = AppSettings.Instance.FavoriteStops;

            favoriteStops.Visibility = Visibility.Visible;
            editableFavoriteStops.Visibility = Visibility.Collapsed;
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            double time = t.TotalMilliseconds;

            SearchProgressBar.Visibility = Visibility.Visible;
            try
            {
                string webResponse = await Network.SearchByString(SearchBox.Text, mapControl.Center);

                SearchResult results = JsonConvert.DeserializeObject<SearchResult>(webResponse);

                if (time > SearchTime)
                {
                    searchResults.ItemsSource = results.Services;
                    SearchTime = time;
                }

            }
            catch (Exception ex)
            {

            }
            
            SearchProgressBar.Visibility = Visibility.Collapsed;
        }

        private void searchResults_ItemClick(object sender, ItemClickEventArgs e)
        {
            VehiclesPayload v = new VehiclesPayload(currentStopId, (Service)e.ClickedItem);
            Frame.Navigate(typeof(VehiclesNoStop), v);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MainPivot.SelectedItem = SearchPivot;
        }
    }
}

