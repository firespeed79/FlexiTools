using System;
using System.Collections.Generic;


namespace FlexiTools
{

    public class FieldsVendor
    {
        public int Id { get; set; }
        public string VATID { get; set; }
        public List<string> Country { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public List<string> BusinessUnit { get; set; }
        public string NationalVATID { get; set; }
        public string IBAN { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string GLCode { get; set; }
        public List<string> CountryCode { get; set; }
        public List<int> BusinessUnitID { get; set; }
    }

    public class RecordVendor
    {
        public string id { get; set; }
        public FieldsVendor fields { get; set; }
        public string createdTime { get; set; }
    }

    public class AirtableVendor

{
    public List<RecordVendor> records { get; set; }
        public string offset { get; set; }
    }
}
