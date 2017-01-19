namespace ConsoleApplication.Git
{
    public class Commit
    {
        public Person author { get; set; }
        public Person committer { get; set; }
        public string message { get; set; }
        public Tree tree { get; set; }
        public string url { get; set; }
        public int comment_count { get; set; }
    }
}