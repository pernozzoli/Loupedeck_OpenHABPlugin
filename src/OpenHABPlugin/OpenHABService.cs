using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Linq;

#nullable enable
namespace Loupedeck.OpenHABPlugin
{
    public class OpenHABService
    {
        protected HttpClient? _client = null;

        public List<OpenHABCommandItem> Items { get; } = new List<OpenHABCommandItem>();

        protected String _baseUrl;

        public IEnumerable<OpenHABCommandItem> Switches => Items.Where(item => item.Type == "Switch");

        public IEnumerable<OpenHABCommandItem> Dimmer => Items.Where(item => item.Type == "Dimmer");

        public OpenHABService(String baseUrl)
        {
            _client = new HttpClient();
            _baseUrl = baseUrl;
            ReadOpenHABItems();
        }

        /// <summary>
        /// Read the openHAB items to be added as a selection for the command profile actions
        /// </summary>
        protected void ReadOpenHABItems()
        {

            String ohUrl = $"{_baseUrl}/rest/items?recursive=false";
            _client!.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = _client!.GetAsync(ohUrl).Result;


            //// Read as json
            if (response != null)
            {
                var data = DeserializeResponse(response);
                if (data != null)
                {
                    int maxItems = int.MaxValue;
                    int itemNo = 0;
                    while ((itemNo < data.Count) && (itemNo < maxItems))
                    {
                        var item = data[itemNo];
                        // Get the group and select the first one as sub-group
                        string? itemGroup = item["groupNames"]?.FirstOrDefault()?.ToString();

                        if (itemGroup == null)
                        {
                            itemGroup = "No group";
                        }

                        string? itemType = item["type"]?.ToString();
                        string? itemLabel = item["label"]?.ToString();
                        string? itemName = item["name"]?.ToString();
                        string? itemLink = item["link"]?.ToString();
                        string? itemCategory = item["category"]?.ToString();
                        string? itemState = item["state"]?.ToString();
                        if ((itemLabel != null) && (itemName != null) && (itemLink != null))
                        {
                            Items!.Add(new OpenHABCommandItem
                            {
                                Type = itemType,
                                Name = itemName,
                                Label = itemLabel,
                                Link = itemLink,
                                Group = itemGroup,
                                Category = itemCategory,
                                State = itemState
                            });
                        }
                        itemNo++;
                    }
                }
            }
            else
            {
                Console.WriteLine("openHAB service could not be contacted");
            }
        }

        /// <summary>
        /// Deserializes an API response to JArray
        /// </summary>
        /// <param name="response">API response</param>
        /// <returns></returns>
        public static JArray? DeserializeResponse(HttpResponseMessage response)
        {
            if (response == null)
            {
                return null;
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            JArray? data = JsonConvert.DeserializeObject<JArray>(jsonString);
            return data;
        }

        /// <summary>
        /// Returns byte array of icon defined in openHAB for given item category and state
        /// </summary>
        /// <param name="state">Item state</param>
        /// <param name="itemCategory">Item category as defined in openHAB</param>
        /// <returns>Byte array of img</returns>
        public Byte[]? GetItemIconForState(String state, String? itemCategory)
        {
            string imageUrl = _baseUrl + "/icon/" + itemCategory + "?anyFormat=true&format=png&state=" + state;
            var response = _client?.GetAsync(imageUrl).Result;
            byte[]? imgBytes = response?.Content.ReadAsByteArrayAsync().Result;
            return imgBytes;
        }

        /// <summary>
        /// Toggles an openHAB switch
        /// </summary>
        /// <param name="itemUrl">Url of the item</param>
        /// <returns>Item state</returns>
        public String? ToggleItem(String itemUrl)
        {
            HttpContent body = new StringContent("TOGGLE");
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            var response = _client?.PostAsync(itemUrl, body).Result;
            String? state = GetItemState(itemUrl);
            return state;
        }

        /// <summary>
        /// Gets the state of the given item
        /// </summary>
        /// <param name="itemUrl">Url of the item</param>
        /// <returns>Item state</returns>
        public String? GetItemState(String itemUrl)
        {
            var response = _client?.GetAsync(itemUrl + "/state").Result;
            var state = response?.Content.ReadAsStringAsync().Result;
            return state;
        }

        /// <summary>
        /// Set an openHAB item state
        /// </summary>
        /// <param name="itemUrl">Url of the item</param>
        /// <returns>Item state</returns>
        public String? SetItemState(String itemUrl, string value)
        {
            HttpContent body = new StringContent(value);
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            var response = _client?.PostAsync(itemUrl, body).Result;
            response = _client?.GetAsync(itemUrl + "/state").Result;
            string? state = response?.Content.ReadAsStringAsync().Result;
            return state;
        }

    }
}
#nullable restore

