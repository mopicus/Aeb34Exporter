using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeb34Exporter
{
	/// <summary>
	/// AEB34 Issuer data
	/// </summary>
	public sealed class Aeb34Issuer
	{
		/// <summary>
		/// Legal ID
		/// </summary>
		public string LegalID { get; set; }

		/// <summary>
		/// Issuer name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Address
		/// </summary>
		public string Address1 { get; set; }

		/// <summary>
		/// Postal code + City
		/// </summary>
		public string Address2 { get; set; }

		/// <summary>
		/// State
		/// </summary>
		public string Address3 { get; set; }

		/// <summary>
		/// Issuer country
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Issuer bank account
		/// </summary>
		public string BankAccount { get; set; }
	}
}
