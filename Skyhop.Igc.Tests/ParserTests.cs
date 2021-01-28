using Xunit;

namespace Skyhop.Igc.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Parse_2009191731_PH975()
        {
            var file = Common.ReadFile("20-09-19 17_31 PH-975.igc");

            var igcFile = Parser.Parse(file);

            Assert.True(igcFile != null);
            Assert.True(igcFile.Registration == "PH-975");
            Assert.True(igcFile.Callsign == "W6");
            Assert.True(igcFile.Security == null);
            Assert.True(igcFile.Date == "2020-09-19");
            Assert.True(igcFile.Fixes.Count == 162);
            Assert.True(igcFile.GliderType == "ASK-21");
            Assert.True(igcFile.FirmwareVersion == "-");
            Assert.True(igcFile.LoggerType == "https://skyhop.org");
            Assert.True(igcFile.LoggerManufacturer == "XXX");
            Assert.True(igcFile.Task == null);
            Assert.True(igcFile.HardwareVersion == "FLARM");
            Assert.True(igcFile.Pilot == "Corstian Boerman");
            Assert.True(igcFile.Copilot == "-");
        }

        [Fact]
        public void ParseFile_1G_77fv6m71()
        {
            var file = Common.ReadFile("1G_77fv6m71.igc");

            var igcFile = Parser.Parse(file);

            Assert.True(igcFile.Callsign == "1G");
            Assert.True(igcFile.CompetitionClass == "Club");
            Assert.True(igcFile.Copilot == null);
            Assert.True(igcFile.DataRecords.Count == 80);
            Assert.True(igcFile.Date == "2017-07-15");
            Assert.True(igcFile.FirmwareVersion == "6.0rc6");
            Assert.True(igcFile.Fixes.Count == 4047);
            Assert.True(igcFile.GliderType == "ASW 19");
            Assert.True(igcFile.HardwareVersion == "23");
            Assert.True(igcFile.LoggerId == "6M7");
            Assert.True(igcFile.LoggerManufacturer == "LXNAV");
            Assert.True(igcFile.LoggerType == "LXNAV,LX8080");
            Assert.True(igcFile.NumFlight == null);
            Assert.True(igcFile.Pilot == "Florian Graf");
            Assert.True(igcFile.Registration == "D-2019");
            Assert.True(igcFile.Security == "440FA49446EB4CDA7E1CB4B7D1F33A33F421E7A92656F2AEAE0A18356531A5B1E7C45C72F5C819E94A5DB32640019DC09BAB2EE64BE9FD1B558B9538308598C71F34C38E008D1AB9B859C762747F6B862B52D377C1D1C4D2BFDE801B63B9013AB53F4DF1A5DA6B1D8BCEF04ECF0D54D5895FF1C00DF131F7030241CB7E9C3FAEC1B8D");
            Assert.True(igcFile.Task.Comment == "");
            Assert.True(igcFile.Task.DeclarationDate == "2017-07-15");
            Assert.True(igcFile.Task.DeclarationTime == "08:57:20");
            Assert.True(igcFile.Task.FlightDate == null);
            Assert.True(igcFile.Task.NumTurnpoints == 4);
            Assert.True(igcFile.Task.Points.Count == 8);
            Assert.True(igcFile.Task.TaskNumber == 2);
        }
    }
}
