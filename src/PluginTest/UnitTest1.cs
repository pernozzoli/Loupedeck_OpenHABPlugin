namespace PluginTest;

using System.Net.Http.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OHCommandItem
{
    public string? Label { get; set; }
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? Group { get; set; }
}

[TestClass]
public class UnitTest1
{
    protected HttpClient? _client = null;
    protected string baseUrl = "http://192.168.40.15:8080/rest/";

    protected List<OHCommandItem> _items = new List<OHCommandItem>();

    [TestMethod]
    public void TestMethod1()
    {
        _client = new HttpClient();
        // Read all openHAB items available
        String ohUrl = "http://192.168.40.15:8080/rest/items?recursive=false";
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        var response = _client.GetAsync(ohUrl).Result;
        //// Read as json
        string jsonString = response.Content.ReadAsStringAsync().Result;
        JArray? data = JsonConvert.DeserializeObject<JArray>(jsonString);
        Console.WriteLine(data);
        if (data != null)
        {
            int maxItems = int.MaxValue;
            int itemNo = 0;
            while ((itemNo < data.Count) && (itemNo < maxItems))
            {
                var item = data[itemNo];
                string? itemType = item["type"]?.ToString();
                // Get the group and select the first one as sub-group
                string? itemGroup = item["groupNames"]?.FirstOrDefault()?.ToString();

                if (itemGroup == null)
                {
                    itemGroup = "No group";
                }

                if (itemType == "Switch")
                {
                    string? itemLabel = item["label"]?.ToString();
                    string? itemName = item["name"]?.ToString();
                    string? itemLink = item["link"]?.ToString();
                    if ((itemLabel != null) && (itemName != null) && (itemLink != null))
                    {
                        _items.Add(new OHCommandItem
                        {
                            Name = itemName,
                            Label = itemLabel,
                            Link = itemLink,
                            Group = itemGroup,
                        });
                        //this.AddParameter(itemName, itemLabel, "Items");
                    }
                }
                itemNo++;
            }
            Console.WriteLine(_items);
        }
    }
}
