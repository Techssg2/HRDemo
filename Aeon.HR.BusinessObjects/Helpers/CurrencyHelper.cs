using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Helpers
{
	public static class CurrencyHelper
	{
		public static string ToCurrencyFormat(this decimal value)
		{
			//return string.Format(new CultureInfo("vi-VN"), "{0:#,##0.00}", decimal.Round(value, 0));
			return string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", decimal.Round(value, 0));
		}

		public static string ToCurrencyFormat(this int value)
		{
			//return string.Format(new CultureInfo("vi-VN"), "{0:#,##0.00}", decimal.Round(value, 0));
			return string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", decimal.Round(value, 0));
		}
	}
}
