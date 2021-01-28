using System.IO;
using System.Reflection;

namespace Skyhop.Igc.Tests
{
    internal static class Common
    {
        internal static string ReadFile(string filename)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return File.ReadAllText(Path.Combine(path, "Resources", filename));
        }
    }
}
