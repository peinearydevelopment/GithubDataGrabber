namespace ConsoleApplication.Github
{
    public class Commit
    {
        public string sha { get; set; }
        public Git.Commit commit { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string comments_url { get; set; }
        public Owner author { get; set; }
        public Owner committer { get; set; }
    }
}