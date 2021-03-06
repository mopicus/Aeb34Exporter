﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeb34Exporter
{
	/// <summary>
	/// Main AEB34 file exporter
	/// </summary>
    public sealed class Exporter
    {	
		//ISSUER RECORD
			//HEADER RECORD SEPA TRANSFERS 
				//RECIPIENT RECORD 1
				//RECIPIENT RECORD N
			//TOTALS RECORD SEPA TRANSFERS

			//HEADER RECORD OTHER TRANSFERS
				//RECIPIENT RECORD 1
				//RECIPIENT RECORD N
			//TOTALS RECORD OTHER TRANSFERS

			//HEADER RECORD CHECKS/CHEQUE
				//RECIPIENT RECORD 1
				//RECIPIENT RECORD N
			//TOTALS RECORD CHECKS/CHEQUE

		private Aeb34ExportSettings _Settings;


		//TOTALS FOR SEPA SECTION
		private int _SepaTotalRecipients = 0;
		private int _SepaTotalRecords = 0;
		private double _SepaTotalAmount = 0;


		//GLOBAL TOTALS
		private int _GlobalTotalRecipients = 0;
		private int _GlobalTotalRecords = 0;
		private double _GlobalTotalAmount = 0;


		private Stream _Stream;


		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="settings">AEB34 settings containing recipients</param>
		/// <param name="stream">Stream to write the contents</param>
		public Exporter(Aeb34ExportSettings settings, Stream stream)
		{
			if (settings == null)
				throw new NullReferenceException("AEB34 settings cannot be null");
			if (stream == null)
				throw new NullReferenceException("Stream for AEB34 export cannot be null");

			_Settings = settings;
			_Stream = stream;
		}


		/// <summary>
		/// Write string value, with optional padding
		/// </summary>
		private void Write(string value, int padding = 0)
		{
			if (value == null)
				value = "";		

			if (padding > 0)
				value = value.PadRight(padding);

			byte[] bs = Encoding.Default.GetBytes(value);
			_Stream.Write(bs, 0, bs.Length);
		}
		

		/// <summary>
		/// Write blank spaces to the stream
		/// </summary>
		private void WriteBlanks(int numberOfBlanks)
		{
			if (numberOfBlanks <= 0)
				throw new InvalidOperationException("Blanks to write must be greater than zero"); 
			Write(new string(' ', numberOfBlanks));
		}


		/// <summary>
		/// Writes date in YYYYMMDD
		/// </summary>
		private void WriteDate(DateTime date)
		{
			Write(date.ToString("yyyyMMdd"));
		}


		/// <summary>
		/// Write currency value to the stream, with given padding
		/// </summary>
		private void WriteCurrency(double amount, int padding)
		{
			Write(amount.ToString("f2", CultureInfo.InvariantCulture).Replace(".","").PadLeft(padding, '0'));
		}


		/// <summary>
		/// Writes Issuer record 
		/// </summary>
		private void WriteIssuerRecord()
		{
			//RECORD CODE
			Write("01");

			//OPERATION TYPE
			Write("ORD");

			//VERSION
			Write("34145");

			//RECORD NUMBER
			Write("001");

			//Legal ID
			if (string.IsNullOrEmpty(_Settings.Issuer.LegalID) || _Settings.Issuer.LegalID.Length != 9)
				throw new FormatException("LegalID must be 9 length");
			Write(_Settings.Issuer.LegalID);

			//TODO : Legal ID suffix
			WriteBlanks(3);

			//creation date
			WriteDate(DateTime.Now);

			//Execution date for the orders
			WriteDate(_Settings.ExecutionDate);

			//Bank account type, A = IBAN
			Write("A");

			//Bank account
			if (string.IsNullOrEmpty(_Settings.Issuer.BankAccount))
				throw new InvalidOperationException("Issuer bank account is required");
			Write(_Settings.Issuer.BankAccount, 34);

			//0 = single charge for all operations, 1= with detail, single charge for each operation
			Write("0");

			//Name, address, country, only address is required
			if (string.IsNullOrEmpty(_Settings.Issuer.Name))
				throw new InvalidOperationException("Issuer name is required");
			Write(_Settings.Issuer.Name, 70);
			Write(_Settings.Issuer.Address1, 50);
			Write(_Settings.Issuer.Address2, 50);
			Write(_Settings.Issuer.Address3, 40);
			Write(_Settings.Issuer.Country, 2);

			//free
			WriteBlanks(311);

			//increase global total records
			_GlobalTotalRecords++;
		}


		/// <summary>
		/// Write sepa transfer recipients header record
		/// </summary>
		private void WriteSepaTransferRecipientsHeader()
		{
			//RECORD CODE
			Write("02");

			//OPERATION TYPE
			Write("SCT");

			//VERSION
			Write("34145");

			//Legal ID
			if (string.IsNullOrEmpty(_Settings.Issuer.LegalID) || _Settings.Issuer.LegalID.Length != 9)
				throw new FormatException("LegalID must be 9 length");
			Write(_Settings.Issuer.LegalID);

			//TODO : Legal ID suffix
			WriteBlanks(3);

			//free
			WriteBlanks(578);

			//increase SEPA total records
			_SepaTotalRecords++;

			//increase global total records
			_GlobalTotalRecords++;
		}


		/// <summary>
		/// Writes all recipients records
		/// </summary>
		private void WriteSepaTransferRecipientsRecords()
		{ 
			//foreach recipient
			foreach(Aeb34Recipient recipient in _Settings.Recipients)
			{
				//RECORD CODE
				Write("03");

				//OPERATION TYPE
				Write("SCT");

				//VERSION
				Write("34145");

				//RECORD NUMBER
				Write("002");

				//ISSUER REFERENCE, optional
				Write(_Settings.Reference, 35);

				//Bank account type, A = IBAN
				Write("A");

				//Bank account
				if (string.IsNullOrEmpty(recipient.BankAccount))
					throw new InvalidOperationException("Recipient bank account is required");
				Write(recipient.BankAccount, 34);

				//amount
				if (recipient.Amount == 0)
					throw new InvalidOperationException("Recipient amount cannot be zero");
				WriteCurrency(recipient.Amount, 11);

				//increase sepa total amount
				_SepaTotalAmount += recipient.Amount;

				//shared expenses
				Write("3");

				//BIC Code
				if (string.IsNullOrEmpty(recipient.BIC))
					throw new InvalidOperationException("Recipient BIC code is required");
				Write(recipient.BIC, 11);

				//recipient name, address, postalcode state, country
				Write(recipient.Name, 70);
				Write(recipient.Address1, 50);
				Write(recipient.Address2, 50);
				Write(recipient.Address3, 40);
				Write(recipient.Country);

				//transfer concept
				Write(_Settings.Concept);

				//reserved
				WriteBlanks(35);

				//transfer type AT-45 SEPA RB
				WriteBlanks(4);

				//transfer reason AT-44 SEPA RB
				WriteBlanks(4);

				//free
				WriteBlanks(99);

				//increase SEPA total records
				_SepaTotalRecords++;

				//increase SEPA total recipients
				_SepaTotalRecipients++;

				//increase GLOBAL total recipients
				_GlobalTotalRecipients++;

				//increase global total records
				_GlobalTotalRecords++;
			}
		}


		/// <summary>
		/// Writes totals for sepa transfers
		/// </summary>
		private void WriteSepaTransferTotalsRecord()
		{
			//RECORD CODE
			Write("04");

			//OPERATION TYPE
			Write("SCT");

			//Amount total
			WriteCurrency(_SepaTotalAmount, 17);
			
			//increase global amount
			_GlobalTotalAmount += _SepaTotalAmount;

			//Total SEPA recipients
			Write(_SepaTotalRecipients.ToString(), 8);

			//Total of SEPA transfer records, including header and total records
			_SepaTotalRecords++;
			Write(_SepaTotalRecords.ToString(), 10);

			//free
			WriteBlanks(560);

			//increase global total records
			_GlobalTotalRecords++;
		}


		/// <summary>
		/// Writes global totals record
		/// </summary>
		private void WriteGlobalTotalsRecord()
		{
			//RECORD CODE
			Write("99");

			//OPERATION TYPE
			Write("ORD");

			//Global total
			WriteCurrency(_GlobalTotalAmount, 17);

			//Global total of recipients, code 002, 006 and 008
			Write(_Settings.Recipients.Count.ToString(), 8);

			//Global total of records
			_GlobalTotalRecords++;
			Write(_GlobalTotalRecords.ToString(), 10);

			//free
			WriteBlanks(560);
		}


    }
}
