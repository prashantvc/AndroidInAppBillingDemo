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

		protected override void OnCreate (Bundle bundle)
		{

			StartSetup (SetupFinished);

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			_produtctSpinner = FindViewById<Spinner> (Resource.Id.productSpinner);

			var buyButton = FindViewById<Button> (Resource.Id.buyButton);
			buyButton.Click += OnBuyButtonClick;


			//Disable until we get the items
			_produtctSpinner.Enabled = false;
			_produtctSpinner.OnItemSelectedListener = this;

			_products = new List<Product> ();

		}

		protected override void OnDestroy ()
		{
			if (_serviceConnection != null) {
				UnbindService (_serviceConnection);
			}
		}

		void OnBuyButtonClick (object sender, EventArgs e)
		{
			_billingHelper.BuyItem (_selectedProduct, "none");
		}

		public void StartSetup(Action<bool> setupFinished){

			_serviceConnection = new InAppBillingServiceConnection(this, setupFinished);

			var serviceIntent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
			int count = PackageManager.QueryIntentServices(serviceIntent, 0).Count;
			if (count != 0) {
				BindService(serviceIntent, _serviceConnection, Bind.AutoCreate);
			}
			else {
				// no service available to handle that Intent
				if (setupFinished != null) {
					setupFinished(false); 
				}
			}
		}

		void SetupFinished (bool isServiceCreated)
		{
			if (isServiceCreated) {
				_billingHelper = new InAppBillingHelper (this, _serviceConnection.Service);
			}

			//Get available products
			_billingHelper.QueryInventoryAsync (new List<string> { "product1", "product2" }, ItemType.InApp)
				.ContinueWith(GetInventory, 
				              TaskScheduler.FromCurrentSynchronizationContext());

		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{

		}

		public void OnItemSelected (AdapterView parent, Android.Views.View view, int position, long id)
		{
			_selectedProduct = _products [position];
		}

		public void OnNothingSelected (AdapterView parent)
		{
			//throw new NotImplementedException ();
		}

		void GetInventory (Task<IList<string>> task)
		{
			if (task.Result == null) {
				return;
			}

			foreach (var item in task.Result) {
				var product = JsonConvert.DeserializeObject<Product> (item);
				_products.Add (product);
				Console.WriteLine (product);
			}

			if (_products.Count > 0) {
				_produtctSpinner.Enabled = true;
			}

			var items = _products.Select (p => p.Title).ToList();

			_produtctSpinner.Adapter = new ArrayAdapter<string> (this, 
			                                                    Android.Resource.Layout.SimpleSpinnerItem,
			                                                    items);
		}

		
		InAppBillingHelper _billingHelper;
		Spinner _produtctSpinner;
		Product _selectedProduct;
		IList<Product> _products;
		InAppBillingServiceConnection _serviceConnection;

	}
}


