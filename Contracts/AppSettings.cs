using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public static class AppSettings
    {
        public static Values Settings = new Values();

        public class Values
        {
            public string AlpacaKeyId { get; set; }
            public string AlpacaSecret { get; set; }
            public string AlpacaApiUrl = "https://paper-api.alpaca.markets";
            public ulong DiscordChannel { get; set; }
            public string DiscordToken { get; set; }
            public bool ServiceHasBeenDisabled = false;
        }
    }
}
