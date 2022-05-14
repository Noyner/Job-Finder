using Newtonsoft.Json;

namespace CRM.DAL.Models.ResponseModels.Odata
{
    public class OData<T> 
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }
        public T value { get; set; }
    }
}