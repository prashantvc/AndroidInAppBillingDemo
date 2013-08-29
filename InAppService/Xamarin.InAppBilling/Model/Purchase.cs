
namespace Xamarin.InAppBilling.Model
{
	/// <summary>
	/// Purchase model
	/// </summary>
	public class Purchase
	{
		public string PackageName { get; set; }
		public string OrderId { get; set; }
		public string ProductId { get; set; }
		public string DeveloperPayload { get; set; }
		public int PurchaseTime { get; set; }
		public int PurchaseState { get; set; }
		public string PurchaseToken { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Purchase: PackageName={0}, OrderId={1}, ProductId={2}, DeveloperPayload={3}, PurchaseTime={4}, PurchaseState={5}, PurchaseToken={6}]", PackageName, OrderId, ProductId, DeveloperPayload, PurchaseTime, PurchaseState, PurchaseToken);
		}
	}
}
