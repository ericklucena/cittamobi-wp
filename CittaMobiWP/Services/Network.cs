using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CittaMobiWP.Services
{
    class Network
    {
        private const string PREDICTIONS_BY_STOP = "API CALL GOES HERE";
        private const string PREDICTIONS_BY_SERVICE_AND_STOP = "API CALL GOES HERE";
        private const string VEHICLES_BY_SERVICE = "API CALL GOES HERE";
        private const string STOPS_BY_SERVICE = "API CALL GOES HERE";
        private const string STOP_KINDS = "API CALL GOES HERE";
        private const string AGENCY_BY_REGION = "API CALL GOES HERE";
        private const string STOPS_BY_VIEWPORT = "API CALL GOES HERE";
        private const string VALIDATE_CARD = "API CALL GOES HERE";
        private const string QUERY_CARD = "API CALL GOES HERE";
        private const string SEARCH_BY_STRING = "API CALL GOES HERE";

        public static async Task<string> SearchByString(string query, Geopoint location)
        {
            string url = SEARCH_BY_STRING + "q?q=" + query + "&a=202"+
                        "&lat=" + location.Position.Latitude.ToString(CultureInfo.InvariantCulture) +
                        "&lng=" + location.Position.Longitude.ToString(CultureInfo.InvariantCulture);
            return await GetResponse(url);
        }

        public static async Task<string> StopsByViewport(Geopoint sw, Geopoint ne)
        {
            return await GetResponse(StopsByViewportUrl(sw, ne));
        }

        public static async Task<string> PredictionsByServiceAndStop(int serviceId, int stopId)
        {
            return await GetResponse(PredictionsByServiceAndStopUrl(serviceId, stopId));
        }

        public static async Task<string> PredictionsByStop(int stopId)
        {
            return await GetResponse(PredictionsByStopUrl(stopId));
        }

        public static async Task<string> ServiceVehiclesByService(int serviceId)
        {
            return await GetResponse(ServiceVehiclesByServiceUrl(serviceId));
        }

        public static async Task<string> StopsByService(int serviceId)
        {
            return await GetResponse(StopsByServiceUrl(serviceId));
        }

        public static async Task<string> EletronicTicketById(string ticketId)
        {
            string json = "{\"cardexternalnumber\":\""+ticketId+"\",\"cityid\":6}";

            return await PostResponse(QUERY_CARD, json);
        }

        public static async Task<string> IsValidEletronicTicket(string ticketId)
        {
            string json = "{\"cardexternalnumber\":\"" + ticketId + "\",\"cityid\":6}";

            return await PostResponse(VALIDATE_CARD, json);
        }

        private static async Task<string> PostResponse(string url, string data)
        {
            string webResponse = null;

            try
            {
                HttpClient client = new System.Net.Http.HttpClient();
                //client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                webResponse = await response.Content.ReadAsStringAsync();
                //Debug.Assert(response.StatusCode == System.Net.HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                throw e;
            }

            return webResponse;
        }

        private static async Task<string> GetResponse(string url)   
        {
            string webresponse = null;
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                HttpResponseMessage response = await http.GetAsync(url);
                webresponse = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                throw e;
            }

            return webresponse;
        }

        private static string StopsByViewportUrl(Geopoint sw, Geopoint ne)
        {
            string result = STOPS_BY_VIEWPORT;

            result += "swlat=" + sw.Position.Latitude.ToString(CultureInfo.InvariantCulture);
            result += "&swlng=" + sw.Position.Longitude.ToString(CultureInfo.InvariantCulture);
            result += "&nelat=" + ne.Position.Latitude.ToString(CultureInfo.InvariantCulture);
            result += "&nelng=" + ne.Position.Longitude.ToString(CultureInfo.InvariantCulture);

            return result;
        }

        private static string PredictionsByServiceAndStopUrl(int serviceId, int stopId)
        {
            return PREDICTIONS_BY_SERVICE_AND_STOP + serviceId + "/" + stopId;
        }

        private static string PredictionsByStopUrl(int stopId)
        {
            return PREDICTIONS_BY_STOP + stopId;
        }

        private static string ServiceVehiclesByServiceUrl(int serviceId)
        {
            return VEHICLES_BY_SERVICE + serviceId;
        }

        private static string StopsByServiceUrl(int serviceId)
        {
            return STOPS_BY_SERVICE + serviceId;
        }
    }
}
