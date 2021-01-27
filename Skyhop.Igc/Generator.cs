using System;
using System.Text;
using UnitsNet;

namespace Skyhop.Igc
{
    /*
     * This thing is just an improvisation in order to get some python integration working.
     * Do not use in production for anything else!
     * 
     * See http://vali.fai-civl.org/documents/IGC-Spec_v1.00.pdf for more info
     */
    public class Generator
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public Generator(
            DateTime date,
            string firstPilotName = null,
            string secondPilotName = null,
            string gliderType = null,
            string gliderRegistration = null,
            string gliderCallsign = null)
        {
            _sb.AppendLine($"AXXX https://skyhop.org");
            _sb.AppendLine($"HFDTE{date:ddMMyy}");
            _sb.AppendLine($"HFPLTPILOTINCHARGE: {firstPilotName ?? "-"}");
            _sb.AppendLine($"HFCM2CREW2: {secondPilotName ?? "-"}");
            _sb.AppendLine($"HFGTYGLIDERTYPE: {gliderType ?? "-"}");
            _sb.AppendLine($"HFGIDGLIDERID: {gliderRegistration ?? "-"}");
            _sb.AppendLine($"HFDAT100GPSDATUM: WGS-84");
            _sb.AppendLine($"HFRFWFIRMWAREVERSION: -");
            _sb.AppendLine($"HFRHWHARDWAREVERSION: FLARM");
            _sb.AppendLine($"HFFTYFRTYPE: https://skyhop.org");
            _sb.AppendLine($"HFGPSRECEIVER: -");
            _sb.AppendLine($"HFPRSPRESSALTSENSOR: -");
            _sb.AppendLine($"HFCIDCOMPETITIONID: {gliderCallsign ?? "-"}");
        }

        public void AddFix(DateTime timestamp, double latitude, double longitude, int altitude)
        {
            var latDMS = SexagesimalAngle.FromDouble(latitude);
            var lonDMS = SexagesimalAngle.FromDouble(longitude);

            // ToDo: Ensure the length of the numbers does not exceed the format

            _sb.Append($"B{timestamp:HHmmss}");
            _sb.Append(string.Format(
                "{0:00}{1:00}{2:000}{3}",
                latDMS.Degrees,
                latDMS.Minutes,
                ((double)latDMS.Seconds / 60 + (double)latDMS.Milliseconds / 60000) * 1000,
                latDMS.IsNegative ? 'S' : 'N'));

            _sb.Append(string.Format(
                "{0:000}{1:00}{2:000}{3}",
                lonDMS.Degrees,
                lonDMS.Minutes,
                ((double)lonDMS.Seconds / 60 + (double)lonDMS.Milliseconds / 60000) * 1000,
                lonDMS.IsNegative ? 'W' : 'E'));

            _sb.AppendLine($"A{0:D5}{(int)Length.FromFeet(altitude).Meters:D5}002");
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
