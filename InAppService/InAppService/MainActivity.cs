using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.InAppBilling.Model;
using Xamarin.InAppBilling.Utilities;
using Xamarin.InAppBilling;

namespace InAppService
{
	[Activity (Label = "InAppService", MainLauncher = true)]
	public class MainActivity : Activity, AdapterView.IOnItemSelectedListener, AdapterView.IOnItemClickListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			StartSetup ();

			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			_produtctSpinner = FindViewById<Spinner> (Resource.Id.productSpinner);

			var buyButton = FindViewById<Button> (Resource.Id.buyButton);
			buyButton.Click += OnBuyButtonClick;

			_lvPurchsedItems = FindViewById<ListView> (Resource.Id.purchasedItemsList);
			_lvPurchsedItems.OnItemClickListener = this;

			//Disable until we get the items
			_produtctSpinner.Enabled = false;
			_produtctSpinner.OnItemSelectedListener = this;

			_products = new List<Product> ();

		}

		protected override void OnDestroy ()
		{
			if (_serviceConnection != null) {
				_serviceConnection.Disconnected ();
			}

			base.OnDestroy ();
		}

		void LoadPurchasedItems ()
		{
			var purchases = _billingHelper.GetPurchases (ItemType.InApp);

			_purchasesAdapter = new PurchaseAdapter (this, purchases);
			_lvPurchsedItems.Adapter = _purchasesAdapter;
		}

		void UpdatePurchasedItems ()
		{
			var purchases = _billingHelper.GetPurchases (ItemType.InApp);
			if (_purchasesAdapter != null) {
				foreach (var item in purchases) {
					_purchasesAdapter.Items.Add (item);
				}

				_purchasesAdapter.NotifyDataSetChanged ();
			}
		}

		void OnBuyButtonClick (object sender, EventArgs e)
		{
			//_billingHelper.LaunchPurchaseFlow ("android.test.purchased", ItemType.InApp, Guid.NewGuid().ToString());
			_billingHelper.LaunchPurchaseFlow (_selectedProduct);
		}

		public void StartSetup ()
		{
			_serviceConnection = new InAppBillingServiceConnection (this, string.Empty);
			_serviceConnection.OnConnected += HandleOnConnected; 

			_serviceConnection.Connect ();
		}

		void HandleOnConnected (object sender, EventArgs e)
		{
			_billingHelper = _serviceConnection.BillingHelper;
			GetInventory ();

			LoadPurchasedItems ();
		}

		async Task GetInventory ()
		{
			//Get available products
			_products = await _billingHelper.QueryInventoryAsync (new List<string> {
				"product1",
				"product2",
				"android.test.purchased"
			}, ItemType.InApp);

			if (_products == null) {
				return;
			}

			_produtctSpinner.Enabled = (_products.Count > 0);

			var items = _products.Select (p => p.Title).ToList ();

			_produtctSpinner.Adapter = 
				new ArrayAdapter<string> (this, 
			                           Android.Resource.Layout.SimpleSpinnerItem,
			                           items);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_billingHelper.HandleActivityResult (requestCode, resultCode, data);

			//TODO: Use a call back to update the purchased items
			UpdatePurchasedItems ();
		}

		public void OnItemSelected (AdapterView parent, Android.Views.View view, int position, long id)
		{
			_selectedProduct = _products [position];
		}

		public void OnNothingSelected (AdapterView parent)
		{
			//throw new NotImplementedException ();
		}

		#region IOnItemClickListener implementation

		public void OnItemClick (AdapterView parent, Android.Views.View view, int position, long id)
		{
			string productid = ((TextView)view).Text;
			var purchases = _purchasesAdapter.Items;
			var purchasedItem = purchases.FirstOrDefault (p => p.ProductId == productid);
			if (purchasedItem != null) {
				bool result = _billingHelper.ConsumePurchase (purchasedItem);
				if (result) {
					_purchasesAdapter.Items.Remove (purchasedItem);
					_purchasesAdapter.NotifyDataSetChanged ();
				}
			}
		}

		#endregion

		IInAppBillingHelper _billingHelper;
		Spinner _produtctSpinner;
		Product _selectedProduct;
		IList<Product> _products;
		InAppBillingServiceConnection _serviceConnection;
		ListView _lvPurchsedItems;
		PurchaseAdapter _purchasesAdapter;
	}
}


