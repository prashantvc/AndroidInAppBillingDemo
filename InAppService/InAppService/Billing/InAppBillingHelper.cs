using System;
using Android.Content;
using System.Collections.Generic;
using Xamarin.GoogleInAppBillig;
using Android.OS;
using Com.Android.Vending.Billing;
using Newtonsoft.Json;
using Android.App;

namespace InAppService
{
	public class InAppBillingHelper
	{
		Activity appContext;
		IInAppBillingService billingService;
		List<Product> products;
		const int ApiVersion = 3;

		public InAppBillingHelper (Activity context, IInAppBillingService billingService)
		{
			this.billingService = billingService;
			appContext = context;
			products = new List<Product> ();
		}

		public void QueryInventory (List<string> skuList, string itemType)
		{
			Bundle querySku = new Bundle ();
			querySku.PutStringArrayList (Constants.ItemIdList, skuList);

			Bundle skuDetails = billingService.GetSkuDetails (ApiVersion, appContext.PackageName, itemType, querySku);

			if (skuDetails.ContainsKey (Constants.SkuDetailsList)) {

				var items = skuDetails.GetStringArrayList (Constants.SkuDetailsList);

				foreach (var item in items) {
					var product = JsonConvert.DeserializeObject<Product> (item);
					Console.WriteLine (product);
					products.Add (product);
				}
			}

		}

		public void BuyItem (string sku, string itemType, string payload)
		{
			var buyIntentBundle = billingService.GetBuyIntent (ApiVersion, appContext.PackageName,sku, itemType, payload);
			var pendingIntent = buyIntentBundle.GetParcelable ("BUY_INTENT") as PendingIntent;
			if (pendingIntent != null) {
				appContext.StartIntentSenderForResult (pendingIntent.IntentSender, 1001, new Intent (), 0, 0, 0);
			}
		}
	}
}

