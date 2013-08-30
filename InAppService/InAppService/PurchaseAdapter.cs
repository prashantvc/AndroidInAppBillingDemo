using System;
using Xamarin.InAppBilling.Model;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using Android.App;

namespace InAppService
{
	public class PurchaseAdapter : BaseAdapter<Purchase>
	{
		public PurchaseAdapter (Activity context, IList<Purchase> purchases) : base()
		{
			_context = context;
			_purchases = purchases;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Purchase this [int position] {
			get {
				return _purchases [position];
			}
		}

		public IList<Purchase> Items {
			get {
				return _purchases;
			}
		}

		public override int Count {
			get {
				return _purchases.Count;
			}
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var view = convertView;

			if (view == null) {
				view = _context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem1, null);
			}

			view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = _purchases [position].ProductId;
			return view;
		}

		Activity _context;
		IList<Purchase> _purchases;
	}
}

