using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.GoogleInAppBillig;
using Android.Content;
using System.Collections.Generic;
using System;

namespace InAppService
{
	[Activity (Label = "InAppService", MainLauncher = true)]
	public class MainActivity : BillingActivity
	{
		InAppBillingHelper billingHelper;
		ConnectionSetupListener listener;

		protected override void OnCreate (Bundle bundle)
		{
			//This should be called before base.OnCreate
			//find a way to handle it better
			listener = new ConnectionSetupListener (CreationFinished);
			SetupBillingServiceConnection (listener);

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);

			button.Click += delegate {
				billingHelper.QueryInventory (new List<string> { "product1", "product2" }, ItemType.InApp);
			};
		}

		void CreationFinished (bool isServiceCreated)
		{
			try {
				if (isServiceCreated) {
					billingHelper = new InAppBillingHelper (this, BillingService);
				}

			} finally {
				listener.Dispose ();
				listener = null;
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{

		}

		class ConnectionSetupListener : Java.Lang.Object, IOnServiceCreatedListener
		{

			Action<bool> creationFinished;

			public ConnectionSetupListener (Action<bool> creationFinished)
			{
				this.creationFinished = creationFinished;
			}

			public void OnServiceCreationFinished (bool isServiceCreated)
			{
				creationFinished (isServiceCreated);
			}
		}
	}
}


