using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.DTO
{
    public class MergeRequestGet
    {
        public int id { get; set; }

        public int iid { get; set; }

        public int project_id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime merged_at { get; set; }

        public string target_branch { get; set; }

        public string source_branch { get; set; }

        public string web_url { get; set; }

        public User assignee { get; set; }

        public User author { get; set; }

        public string[] Labels { get; set; }
    }

}
