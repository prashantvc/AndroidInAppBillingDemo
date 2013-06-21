using System;
using Android.Content;
using System.Collections.Generic;
using Xamarin.GoogleInAppBillig;
using Android.OS;
using Com.Android.Vending.Billing;
using Newtonsoft.Json;

namespace InAppService
{
	public class InAppBillingHelper
	{
		Context appContext;
		IInAppBillingService billingService;
		List<Product> products;

		public InAppBillingHelper (Context context, IInAppBillingService billingService)
		{
			this.billingService = billingService;
			appContext = context.ApplicationContext;
			products = new List<Product> ();
		}

		public void QueryInventory (List<string> skuList, string itemType)
		{
			Bundle querySku = new Bundle ();
			querySku.PutStringArrayList (Constants.ItemIdList, skuList);

			Bundle skuDetails = billingService.GetSkuDetails (3, appContext.PackageName, itemType, querySku);

			if (skuDetails.ContainsKey (Constants.SkuDetailsList)) {

				var items = skuDetails.GetStringArrayList (Constants.SkuDetailsList);

				foreach (var item in items) {
					var product = JsonConvert.DeserializeObject<Product> (item);
					Console.WriteLine (product);
					products.Add (product);
				}
			}

		}
	}
}

