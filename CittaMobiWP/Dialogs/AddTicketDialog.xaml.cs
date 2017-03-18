using CittaMobiWP.Models;
using CittaMobiWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CittaMobiWP.Dialogs
{
    public sealed partial class AddTicketDialog : ContentDialog
    {
        public AddTicketDialog()
        {
            this.InitializeComponent();
            IsPrimaryButtonEnabled = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AppSettings.Instance.AddFavouriteTicket(ticketId.Text, ticketNickname.Text);
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private async void ticketId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ticketId.Text.Length >12)
            {
                ValidationProgressBar.Visibility = Visibility.Visible;
                try
                {
                    string json = await Network.IsValidEletronicTicket(ticketId.Text);

                    EletronicTicket eTicket = JsonConvert.DeserializeObject<EletronicTicket>(json);

                    IsPrimaryButtonEnabled = eTicket.Numero != null;
                }
                catch (Exception ex)
                {

                }
                ValidationProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                IsPrimaryButtonEnabled = false;
            }
        }
    }
}
