using System;
using System.Collections.Generic;
using System.Text;

namespace Instapaper.Core
{
    public static class ConvertExtensions
    {
        public static string ToStringInvariant(this int value)
        {
            return value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
