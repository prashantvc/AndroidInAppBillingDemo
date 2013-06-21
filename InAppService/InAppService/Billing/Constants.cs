using System;

namespace InAppService
{
	public sealed class BillingResult{
		private BillingResult ()
		{
			
		}

		public static int OK{
			get{
				return 0;
			}
		}
	}

	public sealed class ItemType{
		public static string InApp{
			get{
				return "inapp";
			}
		}

		
		public static string Subscription{
			get{
				return "subs";
			}
		}
	}

	public sealed class Constants{
		private Constants() {}

		public static string SkuDetailsList{
			get{
				return "DETAILS_LIST";
			}
		}

		public static string ItemIdList{
			get{
				return "ITEM_ID_LIST";
			}
		}
	}
}
