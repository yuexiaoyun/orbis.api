using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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
            var request = new RestRequest("api/auth", Method.POST);
            var token = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes("preslav:dr0lhtis"));
            request.AddBody(token);
            var response = await client.ExecuteTaskAsync(request);

        }
    }
}
