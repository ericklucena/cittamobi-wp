using CittaMobiWP.Common;
using CittaMobiWP.Models;
using CittaMobiWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CittaMobiWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Vehicles : Page
    {
        Service Service { get; set; }
        int StopId { get; set; }
        ObservableCollection<ServiceVehicle> VehiclesList { get; set; }
        ObservableCollection<Stop> StopsList { get; set; }
        List<BasicGeoposition> Lines;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private DispatcherTimer predictionsTimer;


        public Vehicles()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            VehiclesList = new ObservableCollection<ServiceVehicle>();
            StopsList = new ObservableCollection<Stop>();
            Lines = new List<BasicGeoposition>();
            MapVehicles.ItemsSource = VehiclesList;
            MapStops.ItemsSource = StopsList;
        }

        private void StartTimers()
        {
            predictionsTimer = new DispatcherTimer();
            predictionsTimer.Tick += predictionsTimer_Tick;
            predictionsTimer.Interval = new TimeSpan(0, 0, 15);
            predictionsTimer.Start();
        }

        private void predictionsTimer_Tick(object sender, object e)
        {
            getPredictionsInfo();
            getVehiclesInfo();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void getVehiclesInfo()
        {
            string errorMessage = null;
            try
            {
                string webresponse = await Network.ServiceVehiclesByService(Service.ServiceId);

                List<ServiceVehicle> rootObject = JsonConvert.DeserializeObject<List<ServiceVehicle>>(webresponse);

                VehiclesList.Clear();
                foreach(ServiceVehicle sv in rootObject)
                {
                    VehiclesList.Add(sv);
                }

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

        }

        private async void getStopsInfo()
        {
            string errorMessage = null;

            try
            {
                double minLat = 90;
                double maxLat = -90;
                double minLng = 180;
                double maxLng = -180;

                string webresponse = await Network.StopsByService(Service.ServiceId);

                List<Stop> rootObject = JsonConvert.DeserializeObject<List<Stop>>(webresponse);

                foreach (Stop s in rootObject)
                {
                    StopsList.Add(s);
                    Lines.Add(new BasicGeoposition(){
                        Latitude = s.Location.Lat,
                        Longitude = s.Location.Lng
                    });

                    if (s.Location.Lat > maxLat)
                        maxLat = s.Location.Lat;
                    if (s.Location.Lat < minLat)
                        minLat = s.Location.Lat;
                    if (s.Location.Lng > maxLng)
                        maxLng = s.Location.Lng;
                    if (s.Location.Lng < minLng)
                        minLng = s.Location.Lng;
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapPolyline CourseLines = new MapPolyline();
                    CourseLines.Path = new Geopath(Lines);
                    CourseLines.StrokeColor = Colors.DodgerBlue;
                    CourseLines.StrokeThickness = 5;
                    MapControl.MapElements.Add(CourseLines);
                });

                BasicGeoposition nw = new BasicGeoposition(){
                        Latitude = maxLat,
                        Longitude = minLng
                    };
                BasicGeoposition se = new BasicGeoposition(){
                        Latitude = minLat,
                        Longitude = maxLng
                    };

                GeoboundingBox box = new GeoboundingBox(nw, se);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await MapControl.TrySetViewBoundsAsync(box, new Thickness(2,2,2,2), MapAnimationKind.Bow);
                });

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
        }

        private async void getPredictionsInfo()
        {
            string errorMessage = null;

            PredictionProgressBar.Visibility = Visibility.Visible;

            try
            {
                string webresponse = await Network.PredictionsByServiceAndStop(Service.ServiceId, StopId);

                Prediction predictions = JsonConvert.DeserializeObject<Prediction>(webresponse);

                Service.Vehicles.Clear();

                foreach(Vehicle v in predictions.Services.First().Vehicles)
                {
                    Service.Vehicles.Add(v);
                }

                //this.DataContext = Service;
                Predictions.ItemsSource = predictions.Services.First().Vehicles;
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

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VehiclesPayload vp = (VehiclesPayload) e.Parameter;
            Service = vp.Service;
            StopId = vp.StopId;
            this.DataContext = Service;

            getVehiclesInfo();
            getStopsInfo();
            getPredictionsInfo();

            StartTimers();

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DataContext = null;
            VehiclesList.Clear();
            VehiclesList = null;
            StopsList.Clear();
            StopsList = null;
            Lines.Clear();
            Lines = null;

            predictionsTimer.Stop();
            predictionsTimer = null;

            this.navigationHelper.OnNavigatedFrom(e);
        }


        #endregion
    }
}
