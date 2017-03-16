extern alias flexitools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using flexitools.ABBYY.FlexiCapture;

namespace FlexiTools
{
    public class Receipt
    {

        public string Total { get; set; }
        public string Currency { get; set; }
        public string Payment { get; set; }
        public string TotalTax { get; set; }
        public string  Date { get; set; }
        public string Vendor { get; set; }

        private IProcessingCallback callback;

        public Receipt(string data, IProcessingCallback pc)
        {

            callback = pc;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            if (doc.DocumentElement.ChildNodes.Count > 1)
            {
                callback.ReportError("Only one receipt per image is supported");
                throw new Exception("Only one receipt per image is supported");
            }
            foreach (XmlNode node in doc.DocumentElement.ChildNodes[0].ChildNodes)
            {
                if (node.Name == "vendor")
                {
                    foreach (XmlNode vendorNode in node.ChildNodes)
                    {
                        if (vendorNode.Name == "name")
                        {
                            // classifiedValue has more priority
                            if (String.IsNullOrEmpty(vendorNode["classifiedValue"].InnerText))
                            {
                                XmlNode vendorNameNode = vendorNode["recognizedValue"];
                                this.Vendor = vendorNameNode["text"].InnerText;
                            }
                            else
                            {
                                this.Vendor = vendorNode["classifiedValue"].InnerText;
                            }
                        }
                        else if (vendorNode.Name == "address")
                        {
                            //addressLabel.Text = vendorNode["text"].InnerText.Replace("\n", " ");
                        }
                        else if (vendorNode.Name == "phone")
                        {
                            //phoneFaxLabel.Text = vendorNode["normalizedValue"].InnerText;
                        }
                        else if (vendorNode.Name == "purchaseType")
                        {
                            //purchaseTypeLabel.Text = vendorNode.InnerText;
                        }
                    }
                }
                else if (node.Name == "date")
                {
                    this.Date = node["normalizedValue"].InnerText;
                }
                else if (node.Name == "time")
                {
                    this.Date += " / " + node["normalizedValue"].InnerText;
                }
                else if (node.Name == "subTotal")
                {
                    //subtotalLabel.Text = node["normalizedValue"].InnerText;
                }
                else if (node.Name == "total")
                {
                    this.Total = node["normalizedValue"].InnerText;
                }
                else if (node.Name == "tax")
                {
                    if (node.Attributes["total"].Value == "true")
                    {
                        this.TotalTax = node["normalizedValue"].InnerText;
                    }

                }
                else if (node.Name == "payment")
                {
                    this.Payment = node.Attributes["type"].InnerText;
                }
            }
        }
    }
}
