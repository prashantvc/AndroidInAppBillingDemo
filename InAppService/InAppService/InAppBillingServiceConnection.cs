using System;
using Android.Content;
using Com.Android.Vending.Billing;
using Android.Util;

namespace InAppService
{
	public class InAppBillingServiceConnection: Java.Lang.Object, IServiceConnection
	{
		Action<bool> setupFinished;
		Context context;

		public InAppBillingServiceConnection (Context context, Action<bool> setupFinished)
		{
			this.context = context;
			this.setupFinished = setupFinished;
		}

		public IInAppBillingService Service {
			get;
			private set;
		} 

		#region IServiceConnection implementation

		public void OnServiceConnected (ComponentName name, Android.OS.IBinder service)
		{
			LogDebug ("Billing service connected.");
			Service = IInAppBillingServiceStub.AsInterface (service);

			string packageName = context.PackageName;

			try {
				LogDebug ("Checking for in-app billing V3 support");

				int response = Service.IsBillingSupported (3, packageName, ItemType.InApp);
				if (response != 0) {
					if (setupFinished != null) 
					{
						setupFinished(false);
						//setupFinished(new IabResult(response,
						  //                          "Error checking for billing v3 support."));
					}
				}

				LogDebug("In-app billing version 3 supported for " + packageName);

				// check for v3 subscriptions support
				response = Service.IsBillingSupported(3, packageName, ItemType.Subscription);
				if (response == 0) {
					LogDebug("Subscriptions AVAILABLE.");
					setupFinished(true);
				}
				else {
					LogDebug("Subscriptions NOT AVAILABLE. Response: " + response);
				}


			} catch (Exception ex) {
				if (setupFinished != null) {
					setupFinished (false);
				}
			}

			if (setupFinished != null) {
				setupFinished (false);
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			Service = null;
		}

		#endregion

		bool debugLog = true;
		string Tag = "Iab Helper";

		void LogDebug (String msg)
		{
			if (debugLog)
				Log.Debug (Tag, msg);
		}

		void LogError (String msg)
		{
			Log.Error (Tag, "In-app billing error: " + msg);
		}

		void LogWarn (String msg)
		{
			Log.Warn (Tag, "In-app billing warning: " + msg);
		}
	}
}

