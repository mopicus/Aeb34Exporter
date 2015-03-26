using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeb34Exporter
{
	/// <summary>
	/// AEB34 recipient data
	/// </summary>
	public sealed class Aeb34Recipient
	{
		public string LegalID { get; set; }

		public string Name { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string Country { get; set; }

		public double Amount { get; set; }

		public string BIC { get; set; }
		public string BankAccount { get; set; }
	}

}
