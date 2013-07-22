using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;

namespace InAppService
{
	[Activity (Label = "InAppService", MainLauncher = true)]
	public class MainActivity : Activity, AdapterView.IOnItemSelectedListener
	{
		InAppBillingHelper billingHelper;
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

		InAppBillingServiceConnection serviceConnection;

		protected override void OnCreate (Bundle bundle)
		{

			StartSetup (CreationFinished);

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

		protected override void OnDestroy ()
		{
			if (serviceConnection != null) {
				UnbindService (serviceConnection);
			}
		}

		void OnBuyButtonClick (object sender, EventArgs e)
		{
			billingHelper.BuyItem (_selectedProduct, "none");
		}

		public void StartSetup(Action<bool> setupFinished){

			serviceConnection = new InAppBillingServiceConnection(this, setupFinished);

			var serviceIntent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
			int count = PackageManager.QueryIntentServices(serviceIntent, 0).Count;
			if (count != 0) {
				BindService(serviceIntent, serviceConnection, Bind.AutoCreate);
			}
			else {
				// no service available to handle that Intent
				if (setupFinished != null) {
					setupFinished(false); 
				}
			}
		}

		void CreationFinished (bool isServiceCreated)
		{
				if (isServiceCreated) {
					billingHelper = new InAppBillingHelper (this, serviceConnection.Service);
				}

				//Get available products
				billingHelper.QueryInventoryAsync (new List<string> { "product1", "product2" }, ItemType.InApp)
					.ContinueWith(GetInventory, 
					              TaskScheduler.FromCurrentSynchronizationContext());

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{

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


