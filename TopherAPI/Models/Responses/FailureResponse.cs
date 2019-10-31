using Newtonsoft.Json;

namespace TopherAPI.Models.Responses
{
    public class FailureResponse
    {
        public ErrorInformation Error { get; set; }

        [JsonIgnore]
        public string Json
        {
            get { return JsonConvert.SerializeObject(this, Formatting.Indented); }
        }

        public FailureResponse()
        {
            Error = new ErrorInformation();
        }

        public FailureResponse(string type, string message)
        {
            Error = new ErrorInformation
            {
                Type = type,
                Message = message
            };
        }
    }

    public class ErrorInformation
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
