namespace TestMaker.API.Models
{
    public class Question
    {
        public string Statement { get; set; }
        public float Score { get; set; }
        public int SpacingAfter { get; set; } = 0;
        public List<string>? ImagesBase64 { get; set; }
        public List<string>? Alternatives { get; set; }
    }
}
