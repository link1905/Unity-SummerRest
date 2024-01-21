namespace RestSourceGenerator.Metadata
{
    public struct Configuration
    {
        public string Assembly { get; set; }
        public string DataSerializer { get; set; }
        public string AuthDataRepository { get; set; }
        public Request[] Domains { get; set; }
    }
}