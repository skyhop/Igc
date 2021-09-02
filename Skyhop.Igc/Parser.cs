using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyhop.Igc
{
    public class Parser
    {
        private Parser()
        {

        }

        private readonly IgcFile _result = new IgcFile();
        private List<RecordExtension> _fixExtensions = new List<RecordExtension>();
        private List<RecordExtension> _dataExtensions = new List<RecordExtension>();

        private int _lineNumber = 0;
        private DateTime? _prevTimestamp;
        private readonly List<(string name, string longId, string shortId)> _manufacturers = new List<(string, string, string)>
        {
            { ("Aircotec", "ACT", "I") },
            { ("Cambridge Aero Instruments", "CAM", "C") },
            { ("ClearNav Instruments", "CNI", null) },
            { ("Data Swan/DSX", "DSX", "D") },
            { ("EW Avionics", "EWA", "E") },
            { ( "Filser", "FIL", "F" ) },
            { ("Flarm", "FLA", "G") },
            { ("Flytech", "FLY", null) },
            { ("Garrecht", "GCS", "A") },
            { ("IMI Gliding Equipment", "IMI", "M") },
            { ("Logstream", "LGS", null) },
            { ("LX Navigation", "LXN", "L") },
            { ("LXNAV", "LXV", "V") },
            { ("Naviter", "NAV", null) },
            { ("New Technologies", "NTE", "N") },
            { ("Nielsen Kellerman", "NKL", "K") },
            { ("Pesches", "PES", "P") },
            { ("PressFinish Electronics", "PFE", null) },
            { ("Print Technik", "PRT", "R") },
            { ("Scheffel", "SCH", "H") },
            { ("Streamline Data Instruments", "SDI", "S") },
            { ("Triadis Engineering GmbH", "TRI", "T") },
            { ("Zander", "ZAN", "Z") },
            { ("XCSoar", "XCS", null) },
            { ("LK8000", "XLK", null) },
            { ("GpsDump", "XGD", null) },
            { ("SeeYou Recorder", "XCM", null) }
        };

        public static IgcFile Parse(string file)
        {
            var parser = new Parser();

            foreach (var line in file.Split('\n'))
            {
                try
                {
                    parser._processLine(line.Trim());
                } catch(Exception)
                {
                    throw;
                }
            }

            return parser._result;
        }

        private void _processLine(string line)
        {
            _lineNumber += 1;

            if (line.Length == 0) return;

            var recordType = line[0];

            switch (recordType)
            {
                case 'B':
                    var fix = _parseBRecord(line);
                    _prevTimestamp = fix.Timestamp;
                    _result.Fixes.Add(fix);
                    break;
                case 'K':
                    var data = _parseKRecord(line);
                    _prevTimestamp = data.Timestamp;
                    _result.DataRecords.Add(data);
                    break;
                case 'H':
                    _processHeader(line);
                    break;
                case 'C':
                    _processTaskLine(line);
                    break;
                case 'A':
                    var record = _parseARecord(line);

                    _result.LoggerId = record.LoggerId;
                    _result.LoggerManufacturer = record.Manufacturer;

                    if (record.NumFlight != null)
                    {
                        _result.NumFlight = record.NumFlight;
                    }
                    break;
                case 'I':
                    _fixExtensions = _parseIJRecord(line);
                    break;
                case 'J':
                    _dataExtensions = _parseIJRecord(line);
                    break;
                case 'G':
                    _result.Security = ( _result.Security ?? "" ) + line.Substring(1);
                    break;
            }

        }

        private void _processHeader(string line)
        {
            var headerType = line.Substring(2, 3);

            switch (headerType)
            {
                case "DTE":
                    var (date, numFlight) = _parseDateHeader(line);

                    _result.Date = date;
                    _result.NumFlight = numFlight;
                    break;
                case "PLT":
                    _result.Pilot = _parsePilot(line);
                    break;
                case "CM2":
                    _result.Copilot = _parseCopilot(line);
                    break;
                case "GTY":
                    _result.GliderType = _parseGliderType(line);
                    break;
                case "GID":
                    _result.Registration = _parseRegistration(line);
                    break;
                case "CID":
                    _result.Callsign = _parseCallsign(line);
                    break;
                case "CCL":
                    _result.CompetitionClass = _parseCompetitionClass(line);
                    break;
                case "FTY":
                    _result.LoggerType = _parseLoggerType(line);
                    break;
                case "RFW":
                    _result.FirmwareVersion = _parseFirmwareVersion(line);
                    break;
                case "RHW":
                    _result.HardwareVersion = _parseHardwareVersion(line);
                    break;
            }
        }

        private ARecord _parseARecord(string line) {
            var match = Regex.Match(line, Constants.RE_A);
            if (match.Success) {
              var manufacturer = _lookupManufacturer(match.Groups[1].Value);

                var loggerId = match.Groups[2].Value;
                var numFlight = !string.IsNullOrWhiteSpace(match.Groups[3]?.Value)
                    ? (int?)Convert.ToInt32(match.Groups[3].Value)
                    : null;

                var additionalData = match.Groups[4].Value;
                return new ARecord
                {
                    Manufacturer = manufacturer,
                    LoggerId = loggerId,
                    NumFlight = numFlight,
                    AdditionalData = additionalData
                };
            }

            match = Regex.Match(line, @"^A(\w{3})(.+)?$");
        
            if (match.Success)
            {
                var manufacturer = _lookupManufacturer(match.Groups[1].Value);
                var additionalData = match.Groups[2]?.Value?.Trim();

                return new ARecord
                {
                    Manufacturer = manufacturer,
                    LoggerId = null,
                    NumFlight = null,
                    AdditionalData = additionalData
                };
            }

            throw new Exception($"Invalid A record at line { _lineNumber }: {line}");
        }

        private string _lookupManufacturer(string id)
        {
            var isShort = id.Length == 1;

            id = id.ToUpperInvariant();

            var (name, longId, shortId) = _manufacturers.FirstOrDefault(q => q.shortId == id || q.longId == id);

            return name ?? id;
        }

        private (string date, int? numFlight) _parseDateHeader(string line) {
            var match = Regex.Match(line, Constants.RE_HFDTE);
            if (!match.Success) {
              throw new Exception($"Invalid DTE header at line {_lineNumber}: {line}");
            }

            bool lastCentury = match.Groups[3].Value[0] == '8' || match.Groups[3].Value[0] == '9';
            var date = $"{(lastCentury ? "19" : "20")}{ match.Groups[3].Value}-{ match.Groups[2].Value}-{ match.Groups[1].Value}";

            var numFlight = !string.IsNullOrWhiteSpace(match.Groups[4]?.Value)
                ? (int?)Convert.ToInt32(match.Groups[4].Value)
                : null;

            return (date, numFlight);
        }

        private string _parseTextHeader(string headerType, string regex, string line, string underscoreReplacement = " ")
        {
            var match = Regex.Match(line, regex);
            if (!match.Success) {
                throw new Exception($"Invalid {headerType} header at line {_lineNumber}: {line}");
            }

            return ( match.Groups[1].Value ?? match.Groups[2].Value ?? "" )
                .Replace("_", underscoreReplacement)
                .Trim();
        }

        private string _parsePilot(string line) {
            return _parseTextHeader("PLT", Constants.RE_PLT_HEADER, line);
        }

        private string _parseCopilot(string line) {
            return _parseTextHeader("CM2", Constants.RE_CM2_HEADER, line);
        }

        private string _parseGliderType(string line) {
            return _parseTextHeader("GTY", Constants.RE_GTY_HEADER, line);
        }

        private string _parseRegistration(string line) {
            return _parseTextHeader("GID", Constants.RE_GID_HEADER, line, "-");
        }

        private string _parseCallsign(string line) {
            return _parseTextHeader("GTY", Constants.RE_CID_HEADER, line);
        }

        private string _parseCompetitionClass(string line) {
            return _parseTextHeader("GID", Constants.RE_CCL_HEADER, line);
        }

        private string _parseLoggerType(string line) {
            return _parseTextHeader("FTY", Constants.RE_FTY_HEADER, line);
        }

        private string _parseFirmwareVersion(string line) {
            return _parseTextHeader("RFW", Constants.RE_RFW_HEADER, line);
        }

        private string _parseHardwareVersion(string line) {
            return _parseTextHeader("RHW", Constants.RE_RHW_HEADER, line);
        }

        private void _processTaskLine(string line)
        {
            if (_result.Task == null)
            {
                _result.Task = _parseTask(line);
            }
            else
            {
                _result.Task.Points.Add(_parseTaskPoint(line));
            }
        }

        private Task _parseTask(string line) {
            var match = Regex.Match(line, Constants.RE_TASK);
            if (!match.Success) {
                throw new Exception($"Invalid task declaration at line {_lineNumber}: {line}");
            }

            var lastCentury = match.Groups[3].Value[0] == '8' || match.Groups[3].Value[0] == '9';
            var declarationDate = $"{(lastCentury ? "19" : "20")}{ match.Groups[3].Value}-{ match.Groups[2].Value}-{ match.Groups[1].Value}";
            var declarationTime = $"{match.Groups[4].Value}:{ match.Groups[5].Value}:{ match.Groups[6].Value}";
            var declarationTimestamp = DateTime.Parse($"{declarationDate}T{declarationTime}Z");

            string flightDate = null;
            if (match.Groups[7].Value != "00" || match.Groups[8].Value != "00" || match.Groups[9].Value != "00")
            {
                lastCentury = match.Groups[9].Value[0] == '8' || match.Groups[9].Value[0] == '9';
                flightDate = $"{(lastCentury ? "19" : "20")}{match.Groups[9].Value}-{match.Groups[8].Value}-{match.Groups[7].Value}";
            }

            var taskNumber = ( match.Groups[10].Value != "0000" )
                ? (int?)Convert.ToInt32(match.Groups[10].Value)
                : null;

            var numTurnpoints = Convert.ToInt32(match.Groups[11].Value);
            var comment = match.Groups[12]?.Value ?? null;

            return new Task
            {
                DeclarationDate = declarationDate,
                DeclarationTime = declarationTime,
                DeclarationTimestamp = declarationTimestamp,
                FlightDate = flightDate,
                TaskNumber = taskNumber,
                NumTurnpoints = numTurnpoints,
                Comment = comment,
                Points = new List<TaskPoint>()
            };
        }

        private TaskPoint _parseTaskPoint(string line) {
            var match = Regex.Match(line, Constants.RE_TASKPOINT);
            if (!match.Success) {
                throw new Exception($"Invalid task point declaration at line {_lineNumber}: {line}");
            }

            var latitude = _parseLatitude(
                match.Groups[1].Value,
                match.Groups[2].Value,
                match.Groups[3].Value,
                match.Groups[4].Value);

            var longitude = _parseLongitude(
                match.Groups[5].Value,
                match.Groups[6].Value,
                match.Groups[7].Value,
                match.Groups[8].Value);

            var name = match.Groups[9]?.Value ?? null;

            return new TaskPoint
            {
                Latitude = latitude,
                Longitude = longitude,
                Name = name
            };
        }

        private BRecord _parseBRecord(string line) {
            if (string.IsNullOrWhiteSpace(_result.Date)) {
                throw new Exception("Missing HFDTE record before first B record");
            }

            var match = Regex.Match(line, Constants.RE_B);

            if (!match.Success) {
                throw new Exception($"Invalid B record at line {_lineNumber}: {line}");
            }

            var time = $"{match.Groups[1].Value}:{match.Groups[2].Value}:{match.Groups[3].Value}";
            var timestamp = _calcTimestamp(time);

            var latitude = _parseLatitude(
                match.Groups[4].Value,
                match.Groups[5].Value,
                match.Groups[6].Value,
                match.Groups[7].Value);

            var longitude = _parseLongitude(
                match.Groups[8].Value,
                match.Groups[9].Value,
                match.Groups[10].Value,
                match.Groups[11].Value);

            var valid = match.Groups[12].Value == "A";

            var pressureAltitude = match.Groups[13].Value == "00000"
                ? null
                : (int?)Convert.ToInt32(match.Groups[13].Value);

            var gpsAltitude = match.Groups[14].Value == "00000"
                ? null
                : (int?)Convert.ToInt32(match.Groups[14].Value);

            var extensions = new Dictionary<string, string>();

            if (_fixExtensions?.Any() ?? false)
            {
                foreach (var extension in _fixExtensions)
                {
                    extensions[extension.Code] = line.Substring(extension.Start, extension.Length);
                }
            }

            double? engineNoiseLevel = null;

            if (extensions.TryGetValue("ENL", out string enl))
            {
                var enlLength = _fixExtensions.FirstOrDefault(it => it.Code == "ENL").Length;
                var enlMax = Math.Pow(10, enlLength);

                engineNoiseLevel = Convert.ToInt32(enl) / enlMax;
            }

            var fixAccuracy = extensions.TryGetValue("FXA", out string fxa)
                ? (int?)Convert.ToInt32(fxa)
                : null;

            return new BRecord
            {
                Timestamp = timestamp,
                Time = time,
                Latitude = latitude,
                Longitude = longitude,
                Valid = valid,
                PressureAltitude = pressureAltitude,
                GpsAltitude = gpsAltitude,
                Extensions = extensions,
                Enl = engineNoiseLevel,
                FixAccuracy = fixAccuracy
            };
        }

        private KRecord _parseKRecord(string line) {
            if (string.IsNullOrWhiteSpace(_result.Date)) {
                throw new Exception("Missing HFDTE record before first K record");
            }

            if (!_dataExtensions?.Any() ?? false) {
                throw new Exception("Missing J record before first K record");
            }

            var match = Regex.Match(line, Constants.RE_K);
            if (!match.Success) {
                throw new Exception($"Invalid K record at line {_lineNumber}: {line}");
            }

            var time = $"{match.Groups[1].Value}:{match.Groups[2].Value}:{match.Groups[3].Value}";
            var timestamp = _calcTimestamp(time);

            var extensions = new Dictionary<string, string>();

            if (_dataExtensions?.Any() ?? false)
            {
                foreach (var extension in _dataExtensions)
                {
                    extensions[extension.Code] = line.Substring(extension.Start, extension.Length);
                }
            }

            return new KRecord
            {
                RecordExtensions = extensions,
                Time = time,
                Timestamp = timestamp
            };
        }

        private List<RecordExtension> _parseIJRecord(string line) {
            var match = Regex.Match(line, Constants.RE_IJ);
            if (!match.Success) {
              throw new Exception($"Invalid {line[0]} record at line {_lineNumber}: {line}");
            }

            var num = Convert.ToInt32(match.Groups[1].Value);
            if (line.Length < 3 + (num * 7))
            {
                throw new Exception($"Invalid {line[0]} record at line {_lineNumber}: {line}");
            }

            var extensions = new List<RecordExtension>(num);


            for (var i = 0; i < num; i++)
            {
                var offset = 3 + (i * 7);
                var start = Convert.ToInt32(line.Substring(offset, 2));
                var end = Convert.ToInt32(line.Substring(offset + 2, 2));
                var length = end - start;
                var code = line.Substring(offset + 4, 3);

                extensions.Add(new RecordExtension
                {
                    Code = code,
                    Start = start,
                    Length = length
                });
            }

            return extensions;
        }

        private double _parseLatitude(string dd, string mm, string mmm, string ns) {
            var degrees = Convert.ToInt32(dd) + (double.Parse($"{mm}.{mmm}", CultureInfo.InvariantCulture) / 60);
            return ( ns == "S" ) ? -degrees : degrees;
        }

        private double _parseLongitude(string ddd, string mm, string mmm, string ew) {
            var degrees = Convert.ToInt32(ddd) + (double.Parse($"{mm}.{mmm}", CultureInfo.InvariantCulture) / 60);
            return ( ew == "W" ) ? -degrees : degrees;
        }

          /**
           * Figures out a Unix timestamp in milliseconds based on the
           * date header value, the time field in the current record and
           * the previous timestamp.
           */
        private DateTime _calcTimestamp(string time) {
            var timestamp = DateTime.Parse($"{_result.Date}T{time}Z");

            // allow timestamps one hour before the previous timestamp,
            // otherwise we assume the next day is meant
            while (_prevTimestamp != null && timestamp < _prevTimestamp.Value.AddHours(-1))
            {
                timestamp = timestamp.AddDays(1);
            }

            return timestamp;
        }
    }
}
