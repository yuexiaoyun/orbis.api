using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Orbis.Api.Controllers;
using RestSharp;

namespace Orbis.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {

            var client = new RestClient("http://localhost:19888/");
            var request = new RestRequest("api/users", Method.POST);
            request.AddObject(new UserT() { EmailAddress = "preslav87@gmail.com", Password = "dr0lhtis", Username = "preslav" });
            //var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("preslav:dr0lhtis"));
            //request.AddParameter("Authorization", "Basic " + token, ParameterType.HttpHeader);
            var response = await client.ExecuteTaskAsync(request);
        }
    }
}
