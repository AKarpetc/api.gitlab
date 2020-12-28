using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITLab.AP.Adapter.DTO
{
    public class AddRelease
    {
        public string name { get; set; }
        public string tag_name { get; set; }
        public string description { get; set; }
        public string[] milestones { get; set; }
        public Assets assets { get; set; }
    }

}
