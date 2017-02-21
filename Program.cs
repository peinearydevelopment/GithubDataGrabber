namespace ConsoleApplication
{
    using Github;

    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System;

    public class Program
    {
        private const string OrganizationName = "aurelia";
        private const string DataDirectory = "data";
        private const string Username = "USER_NAME";
        private static int _numberOfRequests = 0;
        private const string Token = "PERSONAL_API_TOKEN";

        public static void Main(string[] args)
        {
            var repositories = GetGithubObject<Repository>($"https://api.github.com/orgs/{OrganizationName}/repos", $"{OrganizationName}.repos.json").Result;
            foreach (var repositry in repositories)
            {
                if (_numberOfRequests < 60)
                {
                    var commits = GetGithubObject<Commit>($"https://api.github.com/repos/{OrganizationName}/{repositry.name}/commits", $"{OrganizationName}.{repositry.name}.commits.json").Result;
                }
            }
        }

        private static async Task<IEnumerable<TGithub>> GetGithubObject<TGithub>(string url, string filename)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, DataDirectory, filename);
            var githubObjects = new List<TGithub>();

            if (!File.Exists(path))
            {
                IEnumerable<TGithub> returnedGithubObjects = null;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", Username);
                    client.DefaultRequestHeaders.Add("Authorization", $"token {Token}");
                    var page = 1;
                    while (_numberOfRequests < 5000 && (returnedGithubObjects == null || returnedGithubObjects.Any()))
                    {
                        using (var response = await client.GetAsync($"{url}?page={page}&per_page=100"))
                        {
                            _numberOfRequests++;
                            var content = await response.Content.ReadAsStringAsync();
                            returnedGithubObjects = GetItems<TGithub>(content);
                            if (returnedGithubObjects.Any())
                            {
                                githubObjects.AddRange(returnedGithubObjects);
                            }
                        }

                        page++;
                    }
                }

                if (!returnedGithubObjects.Any())
                {
                    using(var file = File.Create(path))
                    {
                        var content = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(githubObjects, Formatting.Indented));
                        file.Write(content, 0, content.Length);
                    }
                }
                else
                {
                    return new TGithub[0];
                }
            }

            return GetItems<TGithub>(File.ReadAllText(path));
        }

        private static IEnumerable<TGithub> GetItems<TGithub>(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<TGithub>>(content);
            }
            catch(Exception)
            {
                Console.Write(content);
                throw;
            }
        }
    }
}
