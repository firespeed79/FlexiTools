
extern alias flexitools;
using System;
using System.Collections.Generic;
using System.Text;
using flexitools.ABBYY.FlexiCapture;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace FlexiTools
{
    public class Webservices
    {
        public string AirtableURL { get; private set; }
        public string AirtableAPIKey { get; private set; }

        public string GenericURLBU { get; private set; }
        public string GenericURLVendor { get; private set; }
        public string GenericAPIKey { get; private set; }

        public enum Services { AIRTABLE, GENERIC}

        public Services service { get; set; }

        public Dictionary<string,string> MappingBU { get; private set; }

        public Dictionary<string, string> MappingVendor { get; private set; }

        public enum DataTypes { JSON, XML}

        public DataTypes DataType { get; private set; }

        public IDataSet dataset { get; set; }

        //private Logger logger;

        //This method assumes Airtable is the selected service.
        public Webservices (string url, string key)
        {

            /*LoggingConfiguration config = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = "${message}";

            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            //logger = LogManager.GetLogger("ABBYY FlexiCapture Tools");*/

            this.AirtableURL = url;
            this.AirtableAPIKey = key;

            //logger.Info("Succesfully created a Webservice object with the following Airatble: " + this.AirtableURL);

        }

        //This method allows you to select the desired webservice.
        public Webservices(Services Serv, string UrlBU, string UrlVendor, string Key, DataTypes type, Dictionary<string,string> Vendor, Dictionary<string, string> BU)
        {

        }

        public void FillBusinessUnitDataSet(IDataSet set)
        {
            //logger.Info("Starting filling process of the Business Unit Data Set!");
            this.dataset = set;
            this.getAirtableDataBU();
            //logger.Info("BU upload completed!");

        }

        public void FillVendorDataSet(IDataSet set)
        {
            //logger.Info("Starting filling process of the Vendor Data Set!");
            this.dataset = set;
            this.getAirtableDataVendor();
            //logger.Info("Vendor upload completed!");

            DateTime date = DateTime.Parse("");
        }

        public void AirtableMappingGenerator()
        {
            Dictionary<string, string> BU = new Dictionary<string, string>();
            BU.Add("Id", "fieds.Id");
            BU.Add("VATID", "fieds.VATID");
            BU.Add("CountryCode", "fields.CountryCode[0]");
            BU.Add("Name", "fields.Name");
            BU.Add("Street", "fields.Street");
            BU.Add("ZIP", "fields.ZIP");
            BU.Add("City", "fields.City");

            Dictionary<string, string> Vendors = new Dictionary<string, string>();
            Vendors.Add("BusinessUnitId", "fields.BusinessUnitID[0]");
            Vendors.Add("Id", "fields.Id");
            Vendors.Add("VATID", "fieds.VATID");
            Vendors.Add("NationalVATID", "fieds.NationalVATID");
            Vendors.Add("IBAN", "fieds.IBAN");
            Vendors.Add("CountryCode", "fields.CountryCode[0]");
            Vendors.Add("Name", "fields.Name");
            Vendors.Add("Street", "fields.Street");
            Vendors.Add("ZIP", "fields.ZIP");
            Vendors.Add("City", "fields.City");
            Vendors.Add("BankAccount", "fields.BankAccount");
            Vendors.Add("BankCode", "fields.BankCode");
            Vendors.Add("GLCode", "fields.GLCode");

        }

        public string GET (string url, string header, string parameters)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}?{1}",url,parameters));
            request.Headers.Add(header);

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    //logger.Error(errorText);
                }
                throw;
            }
        }

        public void getAirtableDataBU()
        {

            string dataBU = this.GET(this.AirtableURL+ "/BussinessUnits", String.Format("Authorization: Bearer {0}", this.AirtableAPIKey), "");
            //string dataVendors = this.GET(this.AirtableURL + "/Vendors", String.Format("Authorization: Bearer {0}", this.AirtableAPIKey), "");
            //logger.Debug(dataBU);

            AirtableBU BU = JsonConvert.DeserializeObject<AirtableBU>(dataBU);
            //AirtableVendor Vendor = JsonConvert.DeserializeObject<AirtableVendor>(dataVendors);

            foreach(Record rec in BU.records)
            {

                IDataSetRecord tempRecordBU = dataset.CreateRecord();

                tempRecordBU.AddValue("Id", rec.fields.Id);
                tempRecordBU.AddValue("Name", rec.fields.Name);
                tempRecordBU.AddValue("VATID", rec.fields.VATID);
                tempRecordBU.AddValue("CountryCode", rec.fields.CountryCode[0]);
                tempRecordBU.AddValue("ZIP", rec.fields.ZIP);
                tempRecordBU.AddValue("Street", rec.fields.Street);
                tempRecordBU.AddValue("City", rec.fields.City);

                this.dataset.AddRecord(tempRecordBU);
                tempRecordBU = null;

            }

        }

        public void getAirtableDataVendor()
        {

            
            string dataVendors = this.GET(this.AirtableURL + "/Vendors", String.Format("Authorization: Bearer {0}", this.AirtableAPIKey), "");
            //logger.Debug(dataVendors);
            Console.WriteLine(dataVendors);

            //AirtableBU BU = JsonConvert.DeserializeObject<AirtableBU>(dataBU);
            AirtableVendor Vendor = JsonConvert.DeserializeObject<AirtableVendor>(dataVendors);
            Console.WriteLine(Vendor.ToString());

            //logger.Info("Offsett" + Vendor.offset);

            foreach (RecordVendor rec in Vendor.records)
            {

                IDataSetRecord tempRecordBU = dataset.CreateRecord();

                tempRecordBU.AddValue("Id", rec.fields.Id);
                if(rec.fields.BusinessUnitID != null)
                {
                    tempRecordBU.AddValue("BusinessUnitId", rec.fields.BusinessUnitID[0]);
                }
                
                tempRecordBU.AddValue("Name", rec.fields.Name);
                tempRecordBU.AddValue("VATID", rec.fields.VATID);
                tempRecordBU.AddValue("NationalVATID", rec.fields.NationalVATID);

                if(rec.fields.CountryCode != null)
                {
                    tempRecordBU.AddValue("CountryCode", rec.fields.CountryCode[0]);
                }

                tempRecordBU.AddValue("ZIP", rec.fields.ZIP);
                tempRecordBU.AddValue("IBAN", rec.fields.IBAN);
                tempRecordBU.AddValue("Street", rec.fields.Street);
                tempRecordBU.AddValue("City", rec.fields.City);
                tempRecordBU.AddValue("BankAccount", rec.fields.BankAccount);
                tempRecordBU.AddValue("BankCode", rec.fields.BankCode);
                tempRecordBU.AddValue("GLCode", rec.fields.GLCode);

                this.dataset.AddRecord(tempRecordBU);
                tempRecordBU = null;

            }

            while(Vendor.offset != null && Vendor.offset != "")
            {
                string dataVendorsOffset = this.GET(this.AirtableURL + "/Vendors", String.Format("Authorization: Bearer {0}", this.AirtableAPIKey), "");
                AirtableVendor VendorOffset = JsonConvert.DeserializeObject<AirtableVendor>(dataVendorsOffset);

                foreach (RecordVendor rec in VendorOffset.records)
                {

                    IDataSetRecord tempRecordBU = dataset.CreateRecord();

                    tempRecordBU.AddValue("Id", rec.fields.Id);
                    tempRecordBU.AddValue("BusinessUnitID", rec.fields.BusinessUnitID);
                    tempRecordBU.AddValue("Name", rec.fields.Name);
                    tempRecordBU.AddValue("VATID", rec.fields.VATID);
                    tempRecordBU.AddValue("NationalVATID", rec.fields.VATID);
                    tempRecordBU.AddValue("CountryCode", rec.fields.CountryCode[0]);
                    tempRecordBU.AddValue("ZIP", rec.fields.ZIP);
                    tempRecordBU.AddValue("Street", rec.fields.Street);
                    tempRecordBU.AddValue("City", rec.fields.City);
                    tempRecordBU.AddValue("BankAccount", rec.fields.BankAccount);
                    tempRecordBU.AddValue("BankCode", rec.fields.BankCode);
                    tempRecordBU.AddValue("GLCode", rec.fields.GLCode);

                    this.dataset.AddRecord(tempRecordBU);
                    tempRecordBU = null;

                }

            }

        }

        public void test(IDocument doc, IProcessingCallback Processing) {

            Processing.ReportMessage("Setting fc_Predefined:InvoicePredefinedVendorId from import folder...");

            IPage page = doc.Pages[0];
            string path = page.ImageSource;

            string pattern = @"\[(.*?)\]";
            Match m = Regex.Match(path, pattern, RegexOptions.IgnoreCase);

            Processing.ReportMessage("Testing RegEx...");

            if (m.Success)
            {

                string BUid = m.Groups[1].Value;

                Processing.ReportMessage("Regex succesfull, setting " + BUid + " as the BU ID!");

                doc.Properties.Set("fc_Predefined:InvoicePredefinedVendorId", BUid);

            } else
            {
                Processing.ReportWarning("Could not detect BU ID :-(");

            }


        }

    }
}
