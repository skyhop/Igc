namespace Skyhop.Igc
{
    internal static class Constants
    {
        internal const int ONE_HOUR = 60 * 60 * 1000;
        internal const int ONE_DAY = 24 * 60 * 60 * 1000;
        
        internal const string RE_A = @"^A(\w{3})(\w{3,}?)(?:FLIGHT:(\d+)|\:(.+))?$";
        internal const string RE_HFDTE = @"^HFDTE(?:DATE:)?(\d{2})(\d{2})(\d{2})(?:,?(\d{2}))?";
        internal const string RE_PLT_HEADER = @"^H[FOP]PLT(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_CM2_HEADER = @"^H[FOP]CM2(?:.{0,}?:(.*)|(.*))$"; // P is used by some broken Flarms
        internal const string RE_GTY_HEADER = @"^H[FOP]GTY(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_GID_HEADER = @"^H[FOP]GID(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_CID_HEADER = @"^H[FOP]CID(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_CCL_HEADER = @"^H[FOP]CCL(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_FTY_HEADER = @"^H[FOP]FTY(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_RFW_HEADER = @"^H[FOP]RFW(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_RHW_HEADER = @"^H[FOP]RHW(?:.{0,}?:(.*)|(.*))$";
        internal const string RE_B = @"^B(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{3})([NS])(\d{3})(\d{2})(\d{3})([EW])([AV])(-\d{4}|\d{5})(-\d{4}|\d{5})";
        internal const string RE_K = @"^K(\d{2})(\d{2})(\d{2})";
        internal const string RE_IJ = @"^[IJ](\d{2})(?:\d{2}\d{2}[A-Z]{3})+";
        internal const string RE_TASK = @"^C(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})(\d{4})([-\d]{2})(.*)";
        internal const string RE_TASKPOINT = @"^C(\d{2})(\d{2})(\d{3})([NS])(\d{3})(\d{2})(\d{3})([EW])(.*)";
    }
}
