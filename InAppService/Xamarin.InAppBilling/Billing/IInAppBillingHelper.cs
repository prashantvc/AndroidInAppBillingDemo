using System.Threading.Tasks;
using System.Collections.Generic;

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
		Task<IList<string>> QueryInventoryAsync (List<string> skuList, string itemType);
	}
}

