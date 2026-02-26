using Allure.NUnit;
using Allure.NUnit.Attributes;
using RestApiCSharp.ConstantsTestingGeneral;
using System.Net;

namespace RestApiCSharp.Tests
{
    [AllureNUnit]
    public class ZipCodeTests : BaseApiTest
    {
        [SetUp]
        public async Task Setup()
        {
            var zipCodes = new List<string> { "oz", "oz1", "oz2", "oz3" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
        }

        [Test]
        [AllureStep("Add a single zip code and verify 201 Created response")]
        public async Task AddOneZipCode_Return201()
        {
            var zipCodes = new List<string> { "ozoz" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Status code 201 not returned." +
                $"Expected status code 201 (Created), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Add a single zip code and verify it is present in zip codes list")]
        public async Task AddOneZipCode_ZipCodeAdded()
        {
            var zipCodes = new List<string> { "ozoz" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
            var response2 = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(await response2.Content.ReadAsStringAsync(), Does.Contain("ozoz"),
                $"The zip code 'ozoz' was not added. Response content: {await response2.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Add several zip codes and verify 201 Created response")]
        public async Task AddSeveralZipCodes_Return201()
        {
            var zipCodes = new List<string> { "ozoz1", "ozoz2", "ozoz3" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                "Status code 201 not returned." +
                $"Expected status code 201 (Created), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Add several zip codes and verify all are present in zip codes list")]
        public async Task AddSeveralZipCodes_ZipCodesAdded()
        {
            var zipCodes = new List<string> { "ozoz1", "ozoz2", "ozoz3" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
            var response2 = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            foreach (var expected in zipCodes)
            {
                Assert.That(await response2.Content.ReadAsStringAsync(), Does.Contain(expected),
                    $"The zip codes '{zipCodes}' were not added." +
                    $"Response content: {await response2.Content.ReadAsStringAsync()}");
            }
        }

        [Test]
        [AllureIssue("BUG_GetZip_1")]
        [AllureStep("Get all zip codes and verify 200 OK response")]
        public async Task GetZipCodes_Return200()
        {
            var response = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Status code 200 not returned." +
                $"Expected status code 200 (OK), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get all available zip codes and verify expected zip codes are returned")]
        public async Task GetZipCodes_GetAllAvailableZipCodes()
        {
            var response = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            var expectedStrings = new[] { "oz", "oz1", "oz2", "oz3" };

            foreach (var expected in expectedStrings)
            {
                Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain(expected),
                $"Not all available zip codes found. Response content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        [Test]
        [AllureStep("Add duplicate zip codes and verify 201 Created response")]
        public async Task AddDuplicateZipCodes_Return201()
        {
            var zipCodes = new List<string> { "oz1", "oz2" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                "Status code 201 not returned." +
                $"Expected status code 201 (Created), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Add duplicate zip code and verify it exists in zip codes list")]
        public async Task AddDuplicateZipCodes_ZipCodesAdded()
        {
            var zipCodes = new List<string> { "oz3" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
            var response2 = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(await response2.Content.ReadAsStringAsync(), Does.Contain("oz3"),
                $"The zip code 'oz3' was not added. Response content: {await response2.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_AddDuplicateZip_1")]
        [AllureStep("Add duplicate zip codes and verify no duplicate entries are created")]
        public async Task AddDuplicateZipCodes_NoDuplicatesCreated()
        {
            var zipCodes = new List<string> { "oz1", "oz2" };

            var response = await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
            var response2 = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);
            var response2Content = await response2.Content.ReadAsStringAsync(); 

            var zipCodes2 = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response2Content);

            Assert.That(zipCodes2.Distinct().Count(), Is.EqualTo(zipCodes2.Count),
                $"Zip code list contains duplicates. Response content: {response2Content}");
        }
    }
}
