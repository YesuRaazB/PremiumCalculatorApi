//using System.Net.Http;
//using System.Threading.Tasks;
//using Xunit;
//using premium.Api;
//using Microsoft.AspNetCore.Mvc.Testing;
//using System.Net;

//namespace premium.Tests
//{
//    public class premiumControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
//    {
//        private readonly HttpClient _client;

//        public premiumControllerIntegrationTests(WebApplicationFactory<Program> factory)
//        {
//            // Create a client for in-memory server
//            _client = factory.CreateClient();
//        }

//        [Fact]
//        public async Task GetOccupations_ReturnsList()
//        {
//            var response = await _client.GetAsync("/api/premium/occupations");
//            response.EnsureSuccessStatusCode();

//            string content = await response.Content.ReadAsStringAsync();
//            var arr = JArray.Parse(content);
//            Assert.NotEmpty(arr);
//        }

//[Fact]
//public async Task GetMembers_ReturnsList()
//{
//    var response = await _client.GetAsync("/api/premium/members");
//    response.EnsureSuccessStatusCode();

//    string content = await response.Content.ReadAsStringAsync();
//    var arr = JArray.Parse(content);
//    Assert.NotNull(arr);
//}

//        [Fact]
//        public async Task CalcEndpoint_ReturnsMonthlyPremium()
//        {
//            // Arrange
//            string occupationCode = "Doctor";
//            int age = 30;
//            decimal death = 100000;

//            string url = $"/api/premium/members/calc?occupationCode={occupationCode}&age={age}&death={death}";

//            // Act
//            var response = await _client.GetAsync(url);

//            // Assert
//            response.EnsureSuccessStatusCode();

//            string content = await response.Content.ReadAsStringAsync();
//            Assert.False(string.IsNullOrEmpty(content));

//            // Optional: parse JSON and check MonthlyPremium
//            var obj = JObject.Parse(content);
//            Assert.True(obj.ContainsKey("MonthlyPremium"));
//        }
//    }
//}

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Insurance.Tests
{
    public class MembersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MembersControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalcEndpoint_ReturnsMonthlyPremium()
        {
            var res = await _client.GetAsync("/api/members/calc?occupationCode=Doctor&death=100000&age=30");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(json.monthlyPremium);
        }
    }
}
