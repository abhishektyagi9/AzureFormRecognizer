using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace funcformrecognizer
{

    public class Analysisdocs
    {

      
        public Dictionary<string, string> keyvaluepair
        {
            get;
            set;
        }

    }
    public class AnalysedDocuments
    {
        public string content { get; set; }
        public List<KeyValuePair> keyValuePairs { get; set; }
        public List<Enttities> entities{get;set;}

    }
    
    public class KeyValuePair
    {
        public string Key { get; set; }
        //
        // Summary:
        //     Field value of the key-value pair.
        public string Value { get; set; }
    }
    public class Enttities
    {
        public string Category { get; set; }
        //
        // Summary:
        //     Field value of the key-value pair.
        public string SubCategory { get; set; }

        public string Value { get; set; }
    }
}
