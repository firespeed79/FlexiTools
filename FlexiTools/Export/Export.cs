extern alias flexitools;

using flexitools::ABBYY.FlexiCapture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlexiTools.Export
{
    public class Export
    {

        IDocument doc;
        IProcessingCallback log;

        public Export(IDocument document, IProcessingCallback callback)
        {

            if(callback != null)
            {

                log = callback;

            } else
            {

                throw new ArgumentNullException("callback", "The IProcessingCallback can not be empty.");

            }

            if(document != null)
            {

                doc = document;

            } else
            {

                log.ReportError("The IDocument can not be empty.");
                throw new ArgumentNullException("document", "The IDocument can not be empty.");

            }

        }

        public string ToJSON()
        {

            string json = "";


            JObject obj = new JObject();

            if(doc.Children.Count > 0)
            {

                foreach(IField field in doc.Children)
                {

                    if(field.Children.Count > 0)
                    {
                        obj.Add(field.Name, this.getChildren(field.Children));
                    } else
                    {
                        obj.Add(field.Name, field.Text);
                    }

                }

            }


            return json;

        }

        private JObject getChildren(IFields fields)
        {

            JObject children = new JObject();

            foreach(IField field in fields)
            {
                if(field.Children.Count > 0)
                {
                    children.Add(field.Name, this.getChildren(field.Children));
                } else
                {

                    children.Add(field.Name, field.Text);

                }


            }

            return children;

        }

    }
}
