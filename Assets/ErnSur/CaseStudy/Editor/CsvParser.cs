using System.Collections.Generic;
using UnityEngine;

namespace ErnSur.CaseStudy.Editor
{
    static class CsvParser
    {
        public static List<StoreItem> ToStoreItems(TextAsset csvAsset)
        {
            return ToStoreItems(csvAsset.text);
        }
        public static List<StoreItem> ToStoreItems(string csv)
        {
            // Split the file into lines by newline characters
            string[] lines = csv.Split('\n');

            // Get the headers from the first line and split by comma
            string[] headers = lines[0].Split(',');

            // Initialize the data list
            var output = new List<StoreItem>();

            // Loop through the rest of the lines
            for (int i = 1; i < lines.Length; i++)
            {
                // Skip empty lines
                if (lines[i] == "") continue;

                // Split the line by comma
                string[] values = lines[i].Split(',');

                // Create a new StoreItem object and assign its properties from this line
                StoreItem item = new StoreItem();
                item.Id = i-1;
                item.Name = values[0];
                item.Price = int.Parse(values[1]);

                // Add this item to the data list
                output.Add(item);
            }

            return output;
        }
    }
}