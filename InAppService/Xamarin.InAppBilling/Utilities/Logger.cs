using Android.Util;

namespace Xamarin.InAppBilling.Utilities
{
	public sealed class Logger
	{
		const string Tag = "InApp-Billing";

		public static void Debug (string format, params object[] args)
		{
			Log.Debug (Tag, string.Format (format, args));
		}

		public static void Info (string format, params object[] args)
		{
			Log.Info (Tag, string.Format (format, args));
		}

		public static void Error (string format, params object[] args)
		{
			Log.Error (Tag, string.Format (format, args));
		}
	}
}

