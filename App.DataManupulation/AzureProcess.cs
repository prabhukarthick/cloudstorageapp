using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.DataManupulation
{
    public class DataParser
    {
        public async Task<string> ReadJsonValues(string Url)
        {
            HttpClient client = new HttpClient();
            var data = await client.GetAsync(Url).Result.Content.ReadAsStringAsync();
            return data;
        }

        public void ConvertToType(string s)
        {
            var jSon1 = JObject.Parse(s).SelectToken("result");
            var ss = from p in jSon1
                     select new Result()
                     {
                         Id = p["_id"].ToString(),
                         client = p["client"].ToString(),
                         groupName = p["groupName"].ToString(),
                         groupDisplayName = p["groupDisplayName"].ToString(),
                         page = p["page"].ToString(),
                         displayName = p["displayName"].ToString(),
                         thumbnailUrl = p["thumbnailUrl"].ToString(),
                         reportState = p["reportState"].ToString(),
                     };
            GenerateTabularFormat(ss.ToList());
        }

        private void GenerateTabularFormat(List<Result> list)
        {
            StringBuilder oBuilder = new StringBuilder();
            //reader type headers
            oBuilder.AppendLine("Id,client,groupName,groupDisplayName,page,displayName,thumbnailUrl,reportState");

            //reading the content out of list
            foreach (var item in list)
            {
                oBuilder.AppendLine($"{item.Id},{item.client},{item.groupName},{item.groupDisplayName},{item.page},{item.displayName},{item.thumbnailUrl},{item.reportState}");
            }
            var s = oBuilder.ToString();

            BlobOperation oBlob = new BlobOperation();
            oBlob.UploadContent(s, "ExternalData.csv");
        }
    }

    public class Result
    {
        public string Id { get; set; }
        public string client { get; set; }
        public string groupName { get; set; }
        public string groupDisplayName { get; set; }
        public string page { get; set; }
        public string displayName { get; set; }
        public string thumbnailUrl { get; set; }
        public string reportState { get; set; }
    }
}
