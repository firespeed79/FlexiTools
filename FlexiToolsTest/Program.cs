using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexiTools;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FlexiToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //Webservices web = new Webservices("https://api.airtable.com/v0/appqPCzJBa8Pgha6L", "key3Rc7LgggCdWPYM");
            //web.getAirtableDataVendor();

            /*Processor proc = new Processor("ABBYYBeNeLux", "KjsHmfZdDrBu9ZxGPV/XOKyu", null);
            ProcessingSettings settings = new ProcessingSettings();

            settings.SetCountry("Netherlands");
            settings.TreatAsPhoto = false;
            proc.Process(null, null);*/


            string path = @"C:\ImportFolders\Markaryds Buss[690704 - 3555]\Markaryds buss.pdf";

            string pattern = @"\[(.*?)\]";
            Match m = Regex.Match(path, pattern, RegexOptions.IgnoreCase);

            if (m.Success)
            {

                string BUid = m.Groups[1].Value;
                //doc.Properties.Set("InvoicePredefinedVendorId", BUid);

            }
        }
    }
}
