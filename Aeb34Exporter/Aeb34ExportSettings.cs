using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeb34Exporter
{
	/// <summary>
	/// Main class for AEB34 export data and settings
	/// </summary>
	public sealed class Aeb34ExportSettings
	{
		/// <summary>
		/// Operations execution date
		/// </summary>
		public DateTime ExecutionDate { get; set; }

		/// <summary>
		/// Issuer data
		/// </summary>
		public Aeb34Issuer Issuer { get; set; }

		/// <summary>
		/// Issuer Reference
		/// </summary>
		public string Reference { get; set; }

		/// <summary>
		/// Transfer concept
		/// </summary>
		public string Concept { get; set; }

		/// <summary>
		/// List of recipients and amounts
		/// </summary>
		public List<Aeb34Recipient> Recipients { get; set; }

	}
}
