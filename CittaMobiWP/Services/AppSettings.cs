using CittaMobiWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace CittaMobiWP.Services
{
    class AppSettings
    {

        private static AppSettings instance;
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppSettings();
                }

                return instance;
            }
        }

        private const string STOP_PREFIX = "stop.";
        private const string FAVORITE_STOPS = "favorite_stops";
        public List<int> FavoriteStopsIds {get; set;}
        private const string FAVORITE_SERVICES = "favorite_services";
        public List<int> FavoriteServices { get; set; }
        private const string TIMES_OPEN = "times_open";
        public int TimesOpen { get; set; }
        private const string TICKET_PREFIX = "ticket."; 
        private const string FAVORITE_TICKETS = "favorite_tickets";
        public List<string> FavoriteTicketsIds { get; set; }

        public List<FavoriteStop> FavoriteStops
        {
            get
            {
                List<FavoriteStop> stopsList = new List<FavoriteStop>();
                foreach(int stopId in FavoriteStopsIds)
                {
                    stopsList.Add(new FavoriteStop(stopId, GetStopMnemonic(stopId)));
                }

                return stopsList;
            }
        }

        public List<FavoriteTicket> FavoriteTickets
        {
            get
            {
                List<FavoriteTicket> stopsList = new List<FavoriteTicket>();
                foreach (string ticketId in FavoriteTicketsIds)
                {
                    stopsList.Add(new FavoriteTicket(ticketId, GetTicketNickname(ticketId)));
                }

                return stopsList;
            }
        }

        public AppSettings()
        {
            LoadServices();
            LoadStops();
            LoadTickets();
            LoadTimesOpen();
        }

        public void IncreaseTimesOpen()
        {
            LoadTimesOpen();
            TimesOpen++;
            StoreTimesOpen();
        }

        public bool IsFavouritedStop(int stopId)
        {
            foreach (int id in FavoriteStopsIds)
            {
                if(id == stopId)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddFavouriteStop(int stopId, string stopMnemonic)
        {
            if (!IsFavouritedStop(stopId))
            {
                FavoriteStopsIds.Add(stopId);
                ApplicationData.Current.LocalSettings.Values[STOP_PREFIX + stopId] = stopMnemonic;
                StoreStops();
            }
            else
            {
                RemoveFavouriteStop(stopId);
                AddFavouriteStop(stopId, stopMnemonic);
            }
        }

        public void RemoveFavouriteStop(int stopId)
        {
            FavoriteStopsIds.Remove(stopId);
            ApplicationData.Current.LocalSettings.Values.Remove(STOP_PREFIX + stopId);
            StoreStops();
        }

        public string GetStopMnemonic(int stopId)
        {
            return (string)ApplicationData.Current.LocalSettings.Values[STOP_PREFIX + stopId];
        }

        public void StoreStops()
        {
            if (this.FavoriteStopsIds != null && this.FavoriteStopsIds.Count > 0)
            {
                ApplicationData.Current.LocalSettings.Values[FAVORITE_STOPS] = this.FavoriteStopsIds.ToArray();
            }
        }

        public void LoadStops()
        {
            if (ApplicationData.Current.LocalSettings.Values[FAVORITE_STOPS] != null)
            {
                this.FavoriteStopsIds = ((int[])ApplicationData.Current.LocalSettings.Values[FAVORITE_STOPS]).ToList();
            }
            else
            {
                this.FavoriteStopsIds = new List<int>();
                StoreStops();
            }
            
        }

        public bool IsFavouritedTicket(string ticketId)
        {
            foreach (string id in FavoriteTicketsIds)
            {
                if (id == ticketId)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddFavouriteTicket(string ticketId, string ticketNickname)
        {
            if (!IsFavouritedTicket(ticketId))
            {
                FavoriteTicketsIds.Add(ticketId);
                ApplicationData.Current.LocalSettings.Values[TICKET_PREFIX + ticketId] = ticketNickname;
                StoreTickets();
            }
            else
            {
                RemoveFavouriteTicket(ticketId);
                AddFavouriteTicket(ticketId, ticketNickname);
            }
        }

        public void RemoveFavouriteTicket(string ticketId)
        {
            FavoriteTicketsIds.Remove(ticketId);
            ApplicationData.Current.LocalSettings.Values.Remove(TICKET_PREFIX + ticketId);
            StoreTickets();
        }

        public string GetTicketNickname(string ticketId)
        {
            return (string)ApplicationData.Current.LocalSettings.Values[TICKET_PREFIX + ticketId];
        }

        public void StoreTickets()
        {
            if (this.FavoriteTicketsIds != null && this.FavoriteTicketsIds.Count > 0)
            {
                ApplicationData.Current.LocalSettings.Values[FAVORITE_TICKETS] = this.FavoriteTicketsIds.ToArray();
            }
        }

        public void LoadTickets()
        {
            if (ApplicationData.Current.LocalSettings.Values[FAVORITE_TICKETS] != null)
            {
                this.FavoriteTicketsIds = ((string[])ApplicationData.Current.LocalSettings.Values[FAVORITE_TICKETS]).ToList();
            }
            else
            {
                this.FavoriteTicketsIds = new List<string>();
                StoreTickets();
            }

        }

        public void StoreServices()
        {
            if(this.FavoriteServices != null && this.FavoriteServices.Count > 0)
            {
                ApplicationData.Current.LocalSettings.Values[FAVORITE_SERVICES] = this.FavoriteServices.ToArray();
            }
        }

        public void LoadServices()
        {
            if (ApplicationData.Current.LocalSettings.Values[FAVORITE_SERVICES] != null)
            {
                this.FavoriteServices = ((int[])ApplicationData.Current.LocalSettings.Values[FAVORITE_SERVICES]).ToList();
            }
            else
            {
                this.FavoriteServices = new List<int>();
                StoreServices();
            }
        }

        public void StoreTimesOpen()
        {
            ApplicationData.Current.LocalSettings.Values[TIMES_OPEN] = this.TimesOpen;
        }

        public void LoadTimesOpen()
        {
            if (ApplicationData.Current.LocalSettings.Values[TIMES_OPEN] != null)
            {
                this.TimesOpen = (int)ApplicationData.Current.LocalSettings.Values[TIMES_OPEN];
            }
            else
            {
                this.TimesOpen = 0;
                StoreTimesOpen();
            }
            
        }
    }
}
