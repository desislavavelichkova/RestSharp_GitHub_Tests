using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp_GitHub_Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHub_Issues_Tests
{
    public class ApiTestsGitHub
    {
        private RestClient client;
        private RestRequest request;

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient("https://api.github.com");
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues");
            this.client.Authenticator = new HttpBasicAuthenticator("desislavavelichkova", "token");
        }
            public async Task<Issue> CreateIssue(string title, string body)
            {
                var request = new RestRequest("repos/desislavavelichkova/postman/issues");
                request.AddBody(new { body, title });

                var respons = await this.client.ExecuteAsync(request, Method.Post);
                var issue = JsonSerializer.Deserialize<Issue>(respons.Content);
                return issue;

            }

        [Test]
        public async Task Test_GitHub_APIRequest()
        {           
            var respons = await this.client.ExecuteAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, respons.StatusCode);
        }
        [Test]
        public async Task Test_GitHub_GetRequest_AllIssues()
        {
            var respons = await this.client.ExecuteAsync(request);
            var issues = JsonSerializer.Deserialize<List<Issue>>(respons.Content);
            
            foreach (var issue in issues)
            {
                Assert.Greater(issue.id, 0);
                Assert.IsNotEmpty(issue.body);
                Assert.IsNotEmpty(issue.title);
                Assert.IsNotEmpty(issue.html_url);
                Assert.Greater(issue.number, 0);
            }
            Assert.That(issues.Count > 0);
        }
        [Test]
        public async Task Test_GitHub_GetRequest_IssueByNumber()
        {
            var respons = await this.client.ExecuteAsync(request);
            var issues = JsonSerializer.Deserialize<List<Issue>>(respons.Content);

            var currentIssue = issues.FirstOrDefault(x => x.number == 14);       
            
            Assert.AreEqual(14, currentIssue.number);
        }

        [Test]
        public async Task Test_GitHub_GetRequest_IssueByNumber_StatusCode()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/14");
            var respons = await this.client.ExecuteAsync(request);

            var issue = JsonSerializer.Deserialize<Issue>(respons.Content);

            Assert.AreEqual(HttpStatusCode.OK, respons.StatusCode);
            Assert.AreEqual(1251670916, issue.id);
            Assert.AreEqual(14, issue.number);
        }

        [Test]
        public async Task Test_GitHub_GetRequest_WithValid_Issue()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/14");
            var respons = await this.client.ExecuteAsync(request);

            var issue = JsonSerializer.Deserialize<Issue>(respons.Content);

            Assert.AreEqual(HttpStatusCode.OK, respons.StatusCode);
            Assert.AreEqual(1251670916, issue.id);
            Assert.AreEqual(14, issue.number);
        }
        [Test]
        public async Task Test_GitHub_GetRequest_WithInvalid_Issue()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/143");
            var respons = await this.client.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.NotFound, respons.StatusCode);
        }

       [Test]
       public async Task Test_Create_GitHubIssue()
        {
            string title = "New issue from RestSharp";
            string body = "Some body here";
            
            var issue = await CreateIssue(title, body);

            Assert.Greater(issue.id, 0);
            Assert.Greater(issue.number, 0);
            Assert.IsNotEmpty(issue.body);
            Assert.IsNotEmpty(issue.title);

        }
        [Test]
        public async Task Test_GitHub_EditTitle_ValidIssue()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/14");
            request.AddBody(new { title = "New title from RestSharp" });
            var respons = await this.client.ExecuteAsync(request, Method.Patch);
            
            var issue = JsonSerializer.Deserialize<Issue>(respons.Content);

            Assert.AreEqual("New title from RestSharp", issue.title);
            Assert.AreEqual(HttpStatusCode.OK, respons.StatusCode);
            Assert.AreEqual(1251670916, issue.id);
            Assert.AreEqual(14, issue.number);
        }

        [Test]
        public async Task Test_GitHub_EditTitle_InvalidIssue()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/143");
            var respons = await this.client.ExecuteAsync(request, Method.Patch);

            Assert.AreEqual(HttpStatusCode.NotFound, respons.StatusCode);
        }

        [Test]
        public async Task Test_GitHub_EditTitle_ValidIssue_InvalidToken()
        {
            this.request = new RestRequest("repos/desislavavelichkova/postman/issues/14");
            this.client.Authenticator = new HttpBasicAuthenticator("desislavavelichkova", "token");

            var respons = await this.client.ExecuteAsync(request, Method.Patch);

            Assert.AreEqual(HttpStatusCode.Unauthorized, respons.StatusCode);
        }
    }
}