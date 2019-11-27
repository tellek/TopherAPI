using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Contracts.AppSettings;

namespace StocksMonitor.Processes
{
    public partial class AssetCollection
    {
        /// <summary>
        /// The minimum volume for the last entry. Note: The higher the limit, the less this should be. [default: 500000]
        /// </summary>
        public decimal MinLastVolume = 500000;

        /// <summary>
        /// Minimum price of the asset to even be considered. [default: 5.00]
        /// </summary>
        public decimal MinPrice = 5.00m;

        /// <summary>
        /// Maximum price of the asset to even be considered. [default: 10.00]
        /// </summary>
        public decimal MaxPrice = 10.00m;

        /// <summary>
        /// Time frame to use in your query. (Day, Fifteen Minutes, Five Minutes, Minute) [default: Day]
        /// </summary>
        public TimeFrame TimeBasis = TimeFrame.Day;

        /// <summary>
        /// The maximum records for each asset to return. [default: 2]
        /// </summary>
        public int Limit = 2;

        /// <summary>
        /// The amount of records to signify as a good asset. Any less records returned and the asset will be filtered. [default: 2]
        /// </summary>
        public int GoodAmount = 2;

        /// <summary>
        /// This appears to be the date time from for capturing records, although it seems inconsistant. [default: null]
        /// </summary>
        public DateTime? TimeFrom = null;

        /// <summary>
        /// This appears to be the date time to for capturing records, although it seems inconsistant. [default: null]
        /// </summary>
        public DateTime? TimeTo = null;

        /// <summary>
        /// If the first record is higher than the last value then the record will be filtered. [default: true]
        /// </summary>
        public bool FilterIfLosingValue = true;

        /// <summary>
        /// If the record count is less than the good amount then the record will be filtered. [default: true]
        /// </summary>
        public bool FilterIfBadCount = true;

        /// <summary>
        /// If the last volume is less than the MinLastVolume then the record will be filtered. [default: true]
        /// </summary>
        public bool FilterIfLowVolume = true;

        /// <summary>
        /// If last close price is greater than MaxPrice then filter. [default: true]
        /// </summary>
        public bool FilterIfTooExpensive = true;

        /// <summary>
        /// If last close price is less than MinPrice then filter. [default: true]
        /// </summary>
        public bool FilterIfTooCheap = true;
    }
}
