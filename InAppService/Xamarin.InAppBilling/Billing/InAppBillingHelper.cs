using Android.Content;
using System.Collections.Generic;
using Android.OS;
using Com.Android.Vending.Billing;
using Android.App;
using System.Threading.Tasks;
using System;
using Xamarin.InAppBilling.Utilities;
using Xamarin.InAppBilling.Model;
using Newtonsoft.Json;
using System.Linq;

namespace Xamarin.InAppBilling
{
	/// <summary>
	/// In app billing service helper.
	/// </summary>
	public class InAppBillingHelper : IInAppBillingHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InAppService.InAppBillingHelper"/> class.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="billingService">Billing service.</param>
		public InAppBillingHelper (Activity activity, IInAppBillingService billingService, string publicKey)
		{
			_billingService = billingService;
			_activity = activity;
			_publicKey = publicKey;
		}

		public Task<IList<Product>> QueryInventoryAsync (IList<string> skuList, string itemType)
		{
			var getSkuDetailsTask = Task.Factory.StartNew<IList<Product>> (() => {

				Bundle querySku = new Bundle ();
				querySku.PutStringArrayList (Billing.ItemIdList, skuList);


				Bundle skuDetails = _billingService.GetSkuDetails (Billing.APIVersion, _activity.PackageName, itemType, querySku);
				
				if (!skuDetails.ContainsKey (Billing.SkuDetailsList)) {
					return null;
				}
					 
				var products = skuDetails.GetStringArrayList (Billing.SkuDetailsList);

				return (products == null) ? null
					: products.Select (JsonConvert.DeserializeObject<Product>).ToList ();
			});

			return getSkuDetailsTask;
		}

		public void LaunchPurchaseFlow (Product product)
		{
			_payload = Guid.NewGuid ().ToString ();
			LaunchPurchaseFlow (product.ProductId, product.Type, _payload);
		}

		public void LaunchPurchaseFlow (string sku, string itemType, string payload)
		{

#if DEBUG
			var consume = _billingService.ConsumePurchase (Billing.APIVersion, _activity.PackageName, "inapp:com.xamarin.InAppService:android.test.purchased");
			Console.WriteLine ("Consumed: {0}", consume);
#endif

			Bundle buyIntentBundle = _billingService.GetBuyIntent (Billing.APIVersion, _activity.PackageName, sku, itemType, payload);
			var response = buyIntentBundle.GetResponseCodeFromBundle ();

			if (response != BillingResult.OK) {
				return;
			}

			var pendingIntent = buyIntentBundle.GetParcelable (Response.BuyIntent) as PendingIntent;
			if (pendingIntent != null) {
				_activity.StartIntentSenderForResult (pendingIntent.IntentSender, PurchaseRequestCode, new Intent (), 0, 0, 0);
			}
		}

		public IList<Purchase> GetPurchases (string itemType)
		{
			string continuationToken = string.Empty;
			var purchases = new List<Purchase> ();

			do {

				Logger.Debug ("ContinuationTocken {0}", continuationToken);

				Bundle ownedItems = _billingService.GetPurchases (Billing.APIVersion, _activity.PackageName, itemType, null);
				var response = ownedItems.GetResponseCodeFromBundle ();

				if (response != BillingResult.OK) {
					break;
				}

				if (!ValidOwnedItems (ownedItems)) {
					Logger.Debug("Invalid purchases");
					return purchases;
				}

				var items = ownedItems.GetStringArrayList (Response.InAppPurchaseItemList);
				var dataList = ownedItems.GetStringArrayList (Response.InAppPurchaseDataList);
				var signatures = ownedItems.GetStringArrayList (Response.InAppDataSignatureList);

				for (int i = 0; i < items.Count; i++) {
					string data = dataList [i];
					string sign = signatures [i];
				
					if (Security.VerifyPurchase (_publicKey, data, sign)) {
						var purchase = JsonConvert.DeserializeObject<Purchase> (data);
						purchases.Add (purchase);
					}
				}

				continuationToken = ownedItems.GetString (Response.InAppContinuationToken);

				Console.WriteLine (items);

			} while(!string.IsNullOrWhiteSpace(continuationToken));

			return purchases;
		}

		static bool ValidOwnedItems (Bundle purchased)
		{
			return	purchased.ContainsKey (Response.InAppPurchaseItemList)
				&& purchased.ContainsKey (Response.InAppPurchaseDataList)
				&& purchased.ContainsKey (Response.InAppDataSignatureList);
		}

		public void HandleActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (PurchaseRequestCode != requestCode || data == null) {
				return;
			}

			int response = data.GetReponseCodeFromIntent ();
			string purchaseData = data.GetStringExtra (Response.InAppPurchaseData);
			string purchaseSign = data.GetStringExtra (Response.InAppDataSignature);
		}

		Activity _activity;
		string _payload;
		IInAppBillingService _billingService;
		const int PurchaseRequestCode = 1001;
		readonly string _publicKey;
	}
}

