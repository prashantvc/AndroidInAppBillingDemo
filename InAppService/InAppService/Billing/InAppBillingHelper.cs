using Android.Content;
using System.Collections.Generic;
using Android.OS;
using Com.Android.Vending.Billing;
using Android.App;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace InAppService
{
	public class InAppBillingHelper
	{
		Activity appContext;
		IInAppBillingService billingService;
		const int ApiVersion = 3;

		public InAppBillingHelper (Activity context, IInAppBillingService billingService)
		{
			this.billingService = billingService;
			appContext = context;
		}

		public Task<IList<string>> QueryInventoryAsync (List<string> skuList, string itemType)
		{

			var getSkuDetailsTask = Task.Factory.StartNew<IList<string>> (() => {

				Bundle querySku = new Bundle ();
				querySku.PutStringArrayList (Constants.ItemIdList, skuList);


				Bundle skuDetails = billingService.GetSkuDetails (ApiVersion, appContext.PackageName, itemType, querySku);
				
				if (skuDetails.ContainsKey (Constants.SkuDetailsList)) {
					return skuDetails.GetStringArrayList (Constants.SkuDetailsList);
				}
				return null;
				
			});

			return getSkuDetailsTask;
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
				}
			}

		}

		/// <summary>
		/// Buys an items
		/// </summary>
		/// <param name="product">Product.</param>
		/// <param name="payload">Payload.</param>
		public void BuyItem(Product product, string payload)
		{
			BuyItem (product.ProductId, product.Type, payload);
		}

		/// <summary>
		/// Buys an item.
		/// </summary>
		/// <param name="sku">Sku.</param>
		/// <param name="itemType">Item type.</param>
		/// <param name="payload">Payload.</param>
		public void BuyItem (string sku, string itemType, string payload)
		{
			var buyIntentBundle = billingService.GetBuyIntent (ApiVersion, appContext.PackageName, sku, itemType, payload);
			var pendingIntent = buyIntentBundle.GetParcelable ("BUY_INTENT") as PendingIntent;
			if (pendingIntent != null) {
				appContext.StartIntentSenderForResult (pendingIntent.IntentSender, 1001, new Intent (), 0, 0, 0);
			}
		}
	}
}

