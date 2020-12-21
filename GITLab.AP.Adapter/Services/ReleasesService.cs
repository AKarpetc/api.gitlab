using GITLab.AP.Adapter.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.Services
{
    public class ReleasesService
    {
        private readonly string _url;
        private readonly string _privateToken;

        public ReleasesService(string url, string privateToken)
        {
            _url = url;
            _privateToken = privateToken;
        }

        public IEnumerable<Release> GetAllReleases()
        {
            WebRequest request = WebRequest.Create($"{_url}releases");
            request.Headers.Add($"Private-Token:{_privateToken}");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var collection = JsonConvert.DeserializeObject<List<Release>>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return collection;
        }

    }
}
