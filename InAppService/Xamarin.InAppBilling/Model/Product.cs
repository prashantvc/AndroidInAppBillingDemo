
namespace Xamarin.InAppBilling.Model
{
	public class Product
	{
		public string Title { get; set; }
		public string Price { get; set; }
		public string Type { get; set; }
		public string Description { get; set; }
		public string ProductId { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Product: Title={0}, Price={1}, Type={2}, Description={3}, ProductId={4}]", Title, Price, Type, Description, ProductId);
		}
	}
}

