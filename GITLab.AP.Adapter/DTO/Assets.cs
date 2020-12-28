namespace GITLab.AP.Adapter.DTO
{
    public class Assets
    {
        public int count { get; set; }
        public Source[] sources { get; set; }
        public Link[] links { get; set; }
        public string evidence_file_path { get; set; }
    }

}
