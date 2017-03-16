using System;
using System.Collections.Generic;


namespace FlexiTools
{

    public class Fields
    {
        public int Id { get; set; }
        public string VATID { get; set; }
        public List<string> Country { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public List<string> Vendors { get; set; }
        public List<string> CountryCode { get; set; }
    }

    public class Record
    {
        public string id { get; set; }
        public Fields fields { get; set; }
        public string createdTime { get; set; }
    }

    public class AirtableBU
    {
        public List<Record> records { get; set; }
        public string offset { get; set; }
    }

}
