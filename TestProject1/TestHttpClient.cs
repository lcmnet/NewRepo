using CreateUserAPI.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject1
{
    public class TestHttpClient
    {
        private string _url = string.Empty;

        [SetUp]
        public void Setup()
        {
            _url = "https://createuserapi20210919231705.azurewebsites.net/api/users";

        }

        [Test]
        public async Task PassGetUsersUsingRightAPIKey()
        {
            var httpClient = new HttpClient();
           
            httpClient.DefaultRequestHeaders.Add("ApiKey", "CandidateName-Lee-Choong-Meng");

            var response = await httpClient.GetAsync(_url);
            var statusCode = (int) response.StatusCode;

            Assert.AreEqual(statusCode, 200);
        }

        [Test]
        public async Task FailGetUsersUsingWrongAPIKey()
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("ApiKey", "CandidateName-Wrong");

            var response = await httpClient.GetAsync(_url);
            var statusCode = (int) response.StatusCode;
            Assert.AreEqual(statusCode, 401);
        }
    }
}
