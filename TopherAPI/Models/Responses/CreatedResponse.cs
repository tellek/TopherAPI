using Newtonsoft.Json;

namespace TopherAPI.Models.Responses
{
    public class CreatedResponse
    {
        public CreatedResourceInformation Created { get; set; }

        [JsonIgnore]
        public string Json
        {
            get { return JsonConvert.SerializeObject(this, Formatting.Indented); }
        }

        public CreatedResponse()
        {
            Created = new CreatedResourceInformation();
        }

        public CreatedResponse(long id, string href)
        {
            Created = new CreatedResourceInformation
            {
                Id = id,
                Href = href
            };
        }
    }

    public class CreatedResourceInformation
    {
        public string Href { get; set; }
        public long Id { get; set; }
    }
}
