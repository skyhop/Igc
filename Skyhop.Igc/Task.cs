using System;
using System.Collections.Generic;

namespace Skyhop.Igc
{
    public class Task
    {
        public string DeclarationDate { get; set; }
        public string DeclarationTime { get; set; }
        public DateTime DeclarationTimestamp { get; set; }
        public string FlightDate { get; set; }
        public int? TaskNumber { get; set; }
        public int NumTurnpoints { get; set; }
        public string Comment { get; set; }
        public List<TaskPoint> Points { get; set; }
    }
}
