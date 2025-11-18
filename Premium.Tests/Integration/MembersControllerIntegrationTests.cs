using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Premium.Tests.Integration
{ 
    public class MembersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MembersControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalcEndpoint_ReturnsExpectedMonthly_ForDoctor()
        {
            // Arrange
            var url = "/api/members/calc?occupationCode=Doctor&death=100000&age=30";

            // Act
            var res = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var json = await res.Content.ReadFromJsonAsync<dynamic>();
            // using common formula -> monthly should be 375.00 for 100k, factor 1.5, age 30
            Assert.NotNull(json);
            Assert.NotNull(json.monthlyPremium);
            decimal monthly = (decimal)json.monthlyPremium;
            Assert.Equal(375.00m, decimal.Round(monthly, 2));
        }

        [Fact]
        public async Task CreateMember_PersistsAndReturnsMonthly()
        {
            // Arrange
            var payload = new
            {
                name = "Integration Test",
                ageNextBirthday = 30,
                dateOfBirthMMYYYY = "01/1995",
                occupationCode = "Doctor",
                deathSumInsured = 100000
            };

            // Act
            var post = await _client.PostAsJsonAsync("/api/members", payload);
            Assert.Equal(HttpStatusCode.Created, post.StatusCode);
            var created = await post.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(created.monthlyPremium);
            Assert.Equal(375.00m, decimal.Round((decimal)created.monthlyPremium, 2));
        }
    }

}
