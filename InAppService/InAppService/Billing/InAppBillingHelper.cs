using Android.Content;
using System.Collections.Generic;
using Android.OS;
using Com.Android.Vending.Billing;
using Android.App;
using System.Threading.Tasks;

namespace InAppService
{
	/// <summary>
	/// In app billing service helper.
	/// </summary>
	public class InAppBillingHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InAppService.InAppBillingHelper"/> class.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="billingService">Billing service.</param>
		public InAppBillingHelper (Activity activity, IInAppBillingService billingService)
		{
			_billingService = billingService;
			_activity = activity;
		}

		/// <summary>
		/// Queries the inventory asynchronously.
		/// </summary>
		/// <returns>List of strings</returns>
		/// <param name="skuList">Sku list.</param>
		/// <param name="itemType">Item type.</param>
		public Task<IList<string>> QueryInventoryAsync (List<string> skuList, string itemType)
		{

			var getSkuDetailsTask = Task.Factory.StartNew<IList<string>> (() => {

				Bundle querySku = new Bundle ();
				querySku.PutStringArrayList (Constants.ItemIdList, skuList);


				Bundle skuDetails = _billingService.GetSkuDetails (Constants.APIVersion, _activity.PackageName, itemType, querySku);
				
				if (skuDetails.ContainsKey (Constants.SkuDetailsList)) {
					return skuDetails.GetStringArrayList (Constants.SkuDetailsList);
				}

				return null;
			});

			return getSkuDetailsTask;
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
			var buyIntentBundle = _billingService.GetBuyIntent (Constants.APIVersion, _activity.PackageName, sku, itemType, payload);
			var pendingIntent = buyIntentBundle.GetParcelable ("BUY_INTENT") as PendingIntent;
			if (pendingIntent != null) {
				_activity.StartIntentSenderForResult (pendingIntent.IntentSender, 1001, new Intent (), 0, 0, 0);
			}
		}

		Activity _activity;
		IInAppBillingService _billingService;
	}
}

