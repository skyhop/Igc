using System;
using System.Collections.Generic;

namespace Skyhop.Igc
{
    public class IgcFile
    {
        public string Date { get; set; }
        public int? NumFlight { get; set; }
        public string Pilot { get; set; }
        public string Copilot { get; set; }
        public string GliderType { get; set; }
        public string Registration { get; set; }
        public string Callsign { get; set; }
        public string CompetitionClass { get; set; }
        public string LoggerId { get; set; }
        public string LoggerManufacturer { get; set; }
        public string LoggerType { get; set; }
        public string FirmwareVersion { get; set; }
        public string HardwareVersion { get; set; }

        public Task Task { get; set; }
        public List<BRecord> Fixes { get; set; } = new List<BRecord>();
        public List<KRecord> DataRecords { get; set; } = new List<KRecord>();
        public string Security { get; set; }
    }
}
