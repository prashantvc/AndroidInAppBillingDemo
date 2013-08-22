using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.InAppBilling.Model;

namespace Xamarin.InAppBilling
{
	public interface IInAppBillingHelper
	{
		
		/// <summary>
		/// Queries the inventory asynchronously.
		/// </summary>
		/// <returns>List of strings</returns>
		/// <param name="skuList">Sku list.</param>
		/// <param name="itemType">Item type.</param>
		Task<IList<Product>> QueryInventoryAsync (IList<string> skuList, string itemType);
	}
}

