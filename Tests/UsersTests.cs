using Allure.NUnit;
using Allure.NUnit.Attributes;
using Newtonsoft.Json;
using RestApiCSharp.ConstantsTestingGeneral;
using System.Net;

namespace RestApiCSharp.Tests
{
    [AllureNUnit]
    public class UsersTests : BaseApiTest
    {
        [SetUp]
        public async Task Setup()
        {
            var zipCodes = new List<string> { "oz", "oz1", "oz2", "oz3", "oz4" };

            await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);

            var users = new List<User>
            {
                new User { Age = 0, Name = "u1", Sex = "FEMALE", ZipCode = "oz1" },
                new User { Name = "u3", Sex = "FEMALE" }
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, users);
        }

        [Test]
        [AllureStep("Create user with all fields and verify 201 Created response")]
        public async Task AddUserAllFields_Return201_Test()
        {
            var user = new User
            {
                Age = 0,
                Name = "u",
                Sex = "FEMALE",
                ZipCode = "oz"
            };

            var response = await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Status code 201 not returned." +
                $"Expected status code 201 (Created), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Verify user created with all fields is present in users list")]
        public async Task AddUserAllFields_UserAdded_Test()
        {
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("u1"),
                $"The user 'u1' was not added. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_AddUser_1")]
        [AllureStep("Verify zip code is removed after creating user with all fields")]
        public async Task AddUserAllFields_ZipCodeRemoved_Test()
        {
            var response = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("oz1"),
                $"The zip code 'oz1' was not removed. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Create user with required fields only and verify 201 Created response")]
        public async Task AddUserReqFields_Return201_Test()
        {
            var user = new User
            {
                Name = "u2",
                Sex = "FEMALE"
            };

            var response = await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created),
                $"Status code 201 not returned." +
                $"Expected status code 201 (Created), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Verify user created with required fields only is present in users list")]
        public async Task AddUserReqFields_UserAdded_Test()
        {
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("u3"),
                $"The user 'u3' was not added. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Create user with incorrect zip code and verify FailedDependency response")]
        public async Task AddUserIncorrectZipCode_Return424_Test()
        {
            var user = new User
            {
                Age = 0,
                Name = "u4",
                Sex = "FEMALE",
                ZipCode = "brfrtr"
            };

            HttpResponseMessage response = null;

            response = await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("FailedDependency"),
                 $"Response content does not indicate FailedDependency. Content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Verify user is not created when incorrect zip code is provided")]
        public async Task AddUserIncorrectZipCode_UserNotAdded_Test()
        {
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("u4"),
                $"The user 'u4' was not added. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_AddUser_2")]
        [AllureStep("Create duplicate user and verify 400 BadRequest response")]
        public async Task AddUserDuplicate_Return400_Test()
        {
            var user = new User
            {
                Name = "u1",
                Sex = "FEMALE"
            };

            HttpResponseMessage response = null;

            response = await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Status code 400 not returned. " +
                $"Expected status code 400 (BadRequest), " +
                $"but got {response.StatusCode}. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_AddUser_3")]
        [AllureStep("Verify duplicate user is not added to users list")]
        public async Task AddUserDuplicate_UserNotAdded_Test()
        {
            var initialResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);
            var initialContent = await initialResponse.Content.ReadAsStringAsync(); 
            var initialUsers = JsonConvert.DeserializeObject<List<User>>(initialContent);
            int initialCount = initialUsers!.Count;

            var user = new User
            {
                Name = "u1",
                Sex = "FEMALE"
            }; 
            
            var response = await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            var finalResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);
            var finalContent = await finalResponse.Content.ReadAsStringAsync(); 
            var finalUsers = JsonConvert.DeserializeObject<List<User>>(finalContent);
            int finalCount = finalUsers!.Count;

            Assert.That(finalCount, Is.EqualTo(initialCount),
                $"Expected no duplicate user to be added. Initial count: {initialCount}, Final count: {finalCount}. " +
                $"Response content: {finalContent}");
        }
    }
}
