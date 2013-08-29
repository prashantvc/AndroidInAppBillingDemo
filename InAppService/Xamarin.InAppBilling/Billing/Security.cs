using System.Text;
using Java.Security;
using Android.Util;
using Java.Security.Spec;
using Java.Lang;
using Xamarin.InAppBilling.Utilities;

namespace Xamarin.InAppBilling
{
	/// <summary>
	/// Utility security class to verify the purchases
	/// </summary>
	public sealed class Security
	{
		/// <summary>
		/// Verifies the purchase.
		/// </summary>
		/// <returns><c>true</c>, if purchase was verified, <c>false</c> otherwise.</returns>
		/// <param name="publicKey">Public key.</param>
		/// <param name="signedData">Signed data.</param>
		/// <param name="signature">Signature.</param>
		public static bool VerifyPurchase (string publicKey, string signedData, string signature)
		{
			if (signedData == null) {
				Logger.Error ("Security. data is null");
				return false;
			}

			if (!string.IsNullOrEmpty (signature)) {
				var key = Security.GeneratePublicKey (publicKey);
				bool verified = Security.Verify (key, signedData, signature);

				if (!verified) {
					Logger.Error ("Security. Signature does not match data.");
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Generates the public key.
		/// </summary>
		/// <returns>The public key.</returns>
		/// <param name="encodedPublicKey">Encoded public key.</param>
		public static IPublicKey GeneratePublicKey (string encodedPublicKey)
		{
			try {
				var keyFactory = KeyFactory.GetInstance (KeyFactoryAlgorithm);
				return keyFactory.GeneratePublic (new X509EncodedKeySpec (Base64.Decode (encodedPublicKey, 0)));
			} catch (NoSuchAlgorithmException e) {
				Logger.Error (e.Message);
				throw new RuntimeException (e);
			} catch (Exception e) {
				Logger.Error (e.Message);
				throw new IllegalArgumentException ();
			}
		}

		/// <summary>
		/// Verify the specified publicKey, signedData and signature.
		/// </summary>
		/// <param name="publicKey">Public key.</param>
		/// <param name="signedData">Signed data.</param>
		/// <param name="signature">Signature.</param>
		public static bool Verify (IPublicKey publicKey, string signedData, string signature)
		{
			Logger.Debug ("Signature: {0}", signature);
			try {
				var sign = Signature.GetInstance (SignatureAlgorithm);
				sign.InitVerify (publicKey);
				sign.Update (Encoding.UTF8.GetBytes (signedData));

				if (!sign.Verify (Base64.Decode (signature, 0))) {
					Logger.Error ("Security. Signature verification failed.");
					return false;
				}

				return true;
			} catch (Exception e) {
				Logger.Error (e.Message);
			}

			return false;
		}

		const string KeyFactoryAlgorithm = "RSA";
		const string SignatureAlgorithm = "SHA1withRSA";

	}
}
