
using RestSharp;
using RestSharp.Authenticators;
using RestSharp_GitHub_Tests;
using System;
using System.Collections.Generic;
using System.Text.Json;

var client = new RestClient("https://api.github.com");
client.Authenticator = new HttpBasicAuthenticator("desislavavelichkova", "token");

string url = "/repos/desislavavelichkova/postman/issues";

var request = new RestRequest(url);

request.AddBody(new { title = "New Issue from RestSharp" });

var response = await client.ExecuteAsync(request);

var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);

foreach (var issue in issues)
{    
    Console.WriteLine("Repo Body: " + issue.body);
    Console.WriteLine("Repo URL: " + issue.html_url);
    Console.WriteLine("Repo Id: " + issue.id);
    Console.WriteLine("Repo Number: " + issue.number );
    Console.WriteLine("Issue title: " + issue.title);
    Console.WriteLine(" ");
}

Console.WriteLine("Status code: " + response.StatusCode);
Console.WriteLine($"Body: "+response.Content);

