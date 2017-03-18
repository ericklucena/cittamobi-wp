using CittaMobiWP.Dialogs;
using CittaMobiWP.Models;
using CittaMobiWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CittaMobiWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tickets : Page
    {
        public Tickets()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadFavoriteTickets();
        }

        private async void LoadFavoriteTickets()
        {

            ObservableCollection<FavoriteTicket> tickets = new ObservableCollection<FavoriteTicket>(AppSettings.Instance.FavoriteTickets);
            
            TicketsProgressBar.Visibility = Visibility.Visible;
            string errorMessage = null;

            foreach (FavoriteTicket ticket in tickets)
            {
                try
                {
                    string json = await Network.EletronicTicketById(ticket.Id);
                    EletronicTicket eTicket = JsonConvert.DeserializeObject<EletronicTicket>(json);
                    if  (eTicket.Numero != null)
                    {
                        ticket.Balance = eTicket.Saldo;
                    }
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
            }

            if (errorMessage != null)
            {
                MessageDialog msg = new MessageDialog("Houve algum problema durante a requisição ao servidor. Por favor, verifique se o aparelho está conectado à internet e tente novamente.", "Você está conectado à internet?");
                await msg.ShowAsync();
            }
            LastUpdate.Text = "Ultima atualização: " + DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
            favoriteTickets.ItemsSource = tickets;
            TicketsProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void AddTicketButton_Click(object sender, RoutedEventArgs e)
        {
            AddTicketDialog dialog = new AddTicketDialog();
            ContentDialogResult result = await dialog.ShowAsync();
            LoadFavoriteTickets();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFavoriteTickets();
        }

        private void ManageButton_Checked(object sender, RoutedEventArgs e)
        {
            RemoveTicketsButton.Visibility = Visibility.Visible;
            favoriteTickets.SelectionMode = ListViewSelectionMode.Multiple;
        }

        private void ManageButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RemoveTicketsButton.Visibility = Visibility.Collapsed;
            favoriteTickets.SelectionMode = ListViewSelectionMode.None;
        }

        private void RemoveTicketsButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<FavoriteTicket> tickets = favoriteTickets.ItemsSource as ObservableCollection<FavoriteTicket>;

            foreach (FavoriteTicket ticket in favoriteTickets.SelectedItems)
            {
                tickets.Remove(ticket);
                AppSettings.Instance.RemoveFavouriteTicket(ticket.Id);
            }

            LoadFavoriteTickets();
        }
    }
}
