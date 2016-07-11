using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ATTS
{
    public static class CurrencyCode
    {
        private static IDictionary<string, string> map;
        static CurrencyCode()
        {
            map = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture => {
                    try
                    {
                        return new RegionInfo(culture.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null)
                .GroupBy(ri => ri.ISOCurrencySymbol)
                .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
        }
        public static bool TryGetCurrencySymbol(
                              string ISOCurrencySymbol,
                              out string symbol)
        {
            return map.TryGetValue(ISOCurrencySymbol, out symbol);
        }

    }
}