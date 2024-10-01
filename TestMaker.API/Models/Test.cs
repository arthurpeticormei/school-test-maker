namespace TestMaker.API.Models
{
    public class Test
    {
        public TestHeader Header { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
