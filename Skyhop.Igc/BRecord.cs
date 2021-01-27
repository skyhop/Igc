using System;
using System.Collections.Generic;

namespace Skyhop.Igc
{
    public class BRecord
    {
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// UTC time of the GPS fix in ISO 8601 format 
        /// </summary>
        public string Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Valid { get; set; }
        public double? PressureAltitude { get; set; }
        public double? GpsAltitude { get; set; }
        public Dictionary<string, string> Extensions { get; set; }
        public double? FixAccuracy { get; set; }
        /// <summary>
        /// Engine Noise Level from 0.0 to 1.0
        /// </summary>
        public double? Enl { get; set; }
    }
}
