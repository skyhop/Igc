using Xunit;

namespace Skyhop.Igc.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ParseSelfGeneratedFile()
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
    }
}
