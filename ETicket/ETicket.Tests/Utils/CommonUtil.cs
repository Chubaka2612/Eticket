using Newtonsoft.Json;
using System.Net.Http;
using System.Text;


namespace ETicket.Tests.Utils
{
    public class CommonUtil
    {
        public static string WrapToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static T PopulateFromJson<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        }

        public static HttpContent SerializeToHttpContent<T>(T data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            return new StringContent(jsonContent, Encoding.UTF8, mediaType: "application/json");
        }
    }
}
