﻿using GITLab.AP.Adapter.DTO;
using GITLab.AP.Adapter.DTO.Enums;
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
    public class MergeRequestService
    {
        private readonly string _url;
        private readonly string _privateToken;

        public MergeRequestService(string url, string privateToken)
        {
            _url = url;
            _privateToken = privateToken;
        }

        public IEnumerable<MergeRequestGet> GetAll(DateTime dateStart, MergeStates state = MergeStates.merged)
        {
            var mrCollection = new List<MergeRequestGet>();

            int i = 1;
            int count = 1;
            while (count > 0)
            {
                WebRequest request = WebRequest.Create($"{_url}merge_requests?&sort=desc&state={state}&scope=all&page=" + i);
                request.Headers.Add($"Private-Token:{_privateToken}");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var collection = JsonConvert.DeserializeObject<List<MergeRequestGet>>(responseFromServer);
                count = collection.Count();
                mrCollection.AddRange(collection.Where(x => x.merged_at > dateStart));

                if (!collection.Any() || collection.Min(x => x.merged_at) < dateStart)
                {
                    break;
                }

                reader.Close();
                dataStream.Close();
                response.Close();

                i++;
            }

            return mrCollection;
        }
    }
}
