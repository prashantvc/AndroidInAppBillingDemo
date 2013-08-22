using System;
using Android.Content;
using Com.Android.Vending.Billing;
using Android.Util;
using Xamarin.InAppBilling.Utilities;
using Android.App;

namespace Xamarin.InAppBilling
{
	public class InAppBillingServiceConnection: Java.Lang.Object, IServiceConnection
	{
		public InAppBillingServiceConnection (Activity activity)
		{
			_activity = activity;
		}

		public IInAppBillingService Service {
			get;
			private set;
		}

		public IInAppBillingHelper Helper {
			get;
			private set;
		}

		public void Connect ()
		{
			var serviceIntent = new Intent ("com.android.vending.billing.InAppBillingService.BIND");
			int intentServicesCount = _activity.PackageManager.QueryIntentServices (serviceIntent, 0).Count;
			if (intentServicesCount != 0) {
				_activity.BindService (serviceIntent, this, Bind.AutoCreate);
			}
		}

		public void Disconnected ()
		{
			_activity.UnbindService (this);
		}
		#region IServiceConnection implementation
		public void OnServiceConnected (ComponentName name, Android.OS.IBinder service)
		{
			LogDebug ("Billing service connected.");
			Service = IInAppBillingServiceStub.AsInterface (service);

			string packageName = _activity.PackageName;

			try {
				LogDebug ("Checking for in-app billing V3 support");

				int response = Service.IsBillingSupported (Constants.APIVersion, packageName, ItemType.InApp);
				if (response != BillingResult.OK) {
					Connected = false;
				}

				LogDebug ("In-app billing version 3 supported for " + packageName);

				// check for v3 subscriptions support
				response = Service.IsBillingSupported (Constants.APIVersion, packageName, ItemType.Subscription);
				if (response == BillingResult.OK) {
					LogDebug ("Subscriptions AVAILABLE.");
					Connected = true;
					RaiseOnConnected (Connected);

					return;
				} else {
					LogDebug ("Subscriptions NOT AVAILABLE. Response: " + response);
					Connected = false;
				}

			} catch (Exception ex) {
				LogDebug (ex.ToString ());
				Connected = false;
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			Connected = false;
			Service = null;
			Helper = null;

			RaiseOnDisconnected ();
		}
		#endregion
		public bool Connected {
			get;
			private set;
		}

		protected virtual void RaiseOnConnected (bool connected)
		{
			if (!connected) {
				return;
			}

			Helper = new InAppBillingHelper (_activity, this);

			var handler = OnConnected;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}

		protected virtual void RaiseOnDisconnected ()
		{
			var handler = OnDisconnected;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}

		public static void LogDebug (string msg)
		{
			Log.Debug (Tag, "In-app billing error: " + msg);
		}

		public static void LogError (string msg)
		{
			Log.Error (Tag, "In-app billing error: " + msg);
		}

		public static void LogWarn (string msg)
		{
			Log.Warn (Tag, "In-app billing warning: " + msg);
		}

		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;

		Activity _activity;
		const string Tag = "Iab Helper";
	}
}

