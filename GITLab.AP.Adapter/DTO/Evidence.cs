using System;

namespace GITLab.AP.Adapter.DTO
{
    public class Evidence
    {
        public string sha { get; set; }
        public string filepath { get; set; }
        public DateTime collected_at { get; set; }
    }

}
