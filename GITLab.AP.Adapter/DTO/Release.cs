using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.DTO
{
    public class Release
    {
        public string tag_name { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string description_html { get; set; }
        public DateTime created_at { get; set; }
        public DateTime released_at { get; set; }
        public User author { get; set; }
        public Commit commit { get; set; }
        public Milestone[] milestones { get; set; }
        public string commit_path { get; set; }
        public string tag_path { get; set; }
        public Assets assets { get; set; }
        public Evidence[] evidences { get; set; }
    }

}
