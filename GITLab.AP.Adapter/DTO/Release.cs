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


    public class Commit
    {
        public string id { get; set; }
        public string short_id { get; set; }
        public string title { get; set; }
        public DateTime created_at { get; set; }
        public string[] parent_ids { get; set; }
        public string message { get; set; }
        public string author_name { get; set; }
        public string author_email { get; set; }
        public DateTime authored_date { get; set; }
        public string committer_name { get; set; }
        public string committer_email { get; set; }
        public DateTime committed_date { get; set; }
    }

    public class Assets
    {
        public int count { get; set; }
        public Source[] sources { get; set; }
        public Link[] links { get; set; }
        public string evidence_file_path { get; set; }
    }

    public class Source
    {
        public string format { get; set; }
        public string url { get; set; }
    }

    public class Link
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool external { get; set; }
        public string link_type { get; set; }
    }

    public class Milestone
    {
        public int id { get; set; }
        public int iid { get; set; }
        public int project_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime due_date { get; set; }
        public DateTime start_date { get; set; }
        public string web_url { get; set; }
        public IssueStats issue_stats { get; set; }
    }

    public class IssueStats
    {
        public int total { get; set; }
        public int closed { get; set; }
    }

    public class Evidence
    {
        public string sha { get; set; }
        public string filepath { get; set; }
        public DateTime collected_at { get; set; }
    }

}
