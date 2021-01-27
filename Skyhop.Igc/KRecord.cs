using System;
using System.Collections.Generic;

namespace Skyhop.Igc
{
    public class KRecord
    {
        public DateTime Timestamp { get; set; }

        public string Time { get; set; }
        public Dictionary<string, string> RecordExtensions { get; set; }
    }
}
