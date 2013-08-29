using System;
using Android.Content;
using Com.Android.Vending.Billing;
using Xamarin.InAppBilling.Utilities;
using Android.App;

namespace Xamarin.InAppBilling
{
	public class InAppBillingServiceConnection: Java.Lang.Object, IServiceConnection
	{
		public InAppBillingServiceConnection (Activity activity, string publicKey)
		{
			_activity = activity;
			_publicKey = publicKey;
		}

		public IInAppBillingService Service {
			get;
			private set;
		}

		public IInAppBillingHelper BillingHelper {
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
			Logger.Debug ("Billing service connected.");
			Service = IInAppBillingServiceStub.AsInterface (service);

			string packageName = _activity.PackageName;

			try {
				Logger.Debug ("Checking for in-app billing V3 support");

				int response = Service.IsBillingSupported (Billing.APIVersion, packageName, ItemType.InApp);
				if (response != BillingResult.OK) {
					Connected = false;
				}

				Logger.Debug ("In-app billing version 3 supported for {0}", packageName);

				// check for v3 subscriptions support
				response = Service.IsBillingSupported (Billing.APIVersion, packageName, ItemType.Subscription);
				if (response == BillingResult.OK) {
					Logger.Debug ("Subscriptions AVAILABLE.");
					Connected = true;
					RaiseOnConnected (Connected);

					return;
				} else {
					Logger.Debug ("Subscriptions NOT AVAILABLE. Response: {0}", response);
					Connected = false;
				}

			} catch (Exception ex) {
				Logger.Debug (ex.ToString ());
				Connected = false;
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			Connected = false;
			Service = null;
			BillingHelper = null;

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

			BillingHelper = new InAppBillingHelper (_activity, Service, _publicKey);

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

		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;

		Activity _activity;
		const string Tag = "Iab Helper";
		readonly string _publicKey;
	}
}

