using Android.OS;
using Xamarin.InAppBilling.Utilities;
using Android.Content;

namespace Xamarin.InAppBilling.Utilities
{
	public static class Extensions
	{
		public static int GetResponseCodeFromBundle (this Bundle bunble)
		{
			object response = bunble.Get (Response.Code);
			if (response == null) {
				//Bundle with null response code, assuming OK (known issue)
				return BillingResult.OK;
			}
			if (response is Java.Lang.Number) {
				return ((Java.Lang.Number)response).IntValue ();
			}
			return BillingResult.Error;
		}

		public static int GetReponseCodeFromIntent (this Intent intent)
		{
			object response = intent.Extras.Get (Response.Code);
			if (response == null) {
				//Bundle with null response code, assuming OK (known issue)
				return BillingResult.OK;
			}
			if (response is Java.Lang.Number) {
				return ((Java.Lang.Number)response).IntValue ();
			}
			return BillingResult.Error;
		}
	}
}
