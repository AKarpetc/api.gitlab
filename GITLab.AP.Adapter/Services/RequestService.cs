using GITLab.AP.Adapter.Interfaces;
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
    internal class RequestService : IRequestService
    {
        private readonly string _privateToken;

        public RequestService(string privateTocken)
        {
            _privateToken = privateTocken;
        }

        public T Get<T>()
        {
            return default(T);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add($"Private-Token:{_privateToken}");

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = await reader.ReadToEndAsync();
            var collection = JsonConvert.DeserializeObject<List<T>>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return collection;
        }

        public async Task<W> PostAsync<T, W>(T model, string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add($"Private-Token:{_privateToken}");

            request.ContentType = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                string json = JsonConvert.SerializeObject(model);

                streamWriter.Write(json);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var responce = JsonConvert.DeserializeObject<W>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return responce;
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add($"Private-Token:{_privateToken}");

            request.ContentType = "application/json";
            request.Method = "DELETE";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = await reader.ReadToEndAsync();
            var release = JsonConvert.DeserializeObject<T>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            return release;
        }
    }
}
