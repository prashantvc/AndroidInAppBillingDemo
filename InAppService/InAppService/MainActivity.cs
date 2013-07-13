using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.GoogleInAppBillig;
using Android.Content;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace InAppService
{
	[Activity (Label = "InAppService", MainLauncher = true)]
	public class MainActivity : BillingActivity, AdapterView.IOnItemSelectedListener
	{
		InAppBillingHelper billingHelper;
		ConnectionSetupListener listener;
		Spinner produtctSpinner;
		Product _selectedProduct;

		IList<Product> products;


		void GetInventory (Task<IList<string>> task)
		{
			if (task.Result == null) {
				return;
			}

			foreach (var item in task.Result) {
				var product = JsonConvert.DeserializeObject<Product> (item);
				products.Add (product);
				Console.WriteLine (product);
			}

			if (products.Count > 0) {
				produtctSpinner.Enabled = true;
			}

			var items = products.Select (p => p.Title).ToList();

			produtctSpinner.Adapter = new ArrayAdapter<string> (this, 
			                                                    Android.Resource.Layout.SimpleSpinnerItem,
			                                                    items);
		}


		protected override void OnCreate (Bundle bundle)
		{
			//This should be called before base.OnCreate
			//find a way to handle it better
			listener = new ConnectionSetupListener (CreationFinished);
			SetupBillingServiceConnection (listener);

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			produtctSpinner = FindViewById<Spinner> (Resource.Id.productSpinner);

			var buyButton = FindViewById<Button> (Resource.Id.buyButton);
			buyButton.Click += OnBuyButtonClick;


			//Disable until we get the items
			produtctSpinner.Enabled = false;
			produtctSpinner.OnItemSelectedListener = this;

			products = new List<Product> ();

		}

		void OnBuyButtonClick (object sender, EventArgs e)
		{
			billingHelper.BuyItem (_selectedProduct, "none");
		}

		void CreationFinished (bool isServiceCreated)
		{
			try {
				if (isServiceCreated) {
					billingHelper = new InAppBillingHelper (this, BillingService);
				}

				//Get available products
				billingHelper.QueryInventoryAsync (new List<string> { "product1", "product2" }, ItemType.InApp)
					.ContinueWith(GetInventory, 
					              TaskScheduler.FromCurrentSynchronizationContext());

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


		public void OnItemSelected (AdapterView parent, Android.Views.View view, int position, long id)
		{
			_selectedProduct = products [position];
		}

		public void OnNothingSelected (AdapterView parent)
		{
			//throw new NotImplementedException ();
		}

	}
}


