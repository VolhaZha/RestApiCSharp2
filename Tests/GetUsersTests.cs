using Allure.NUnit;
using Allure.NUnit.Attributes;
using RestApiCSharp.ConstantsTestingGeneral;
using System.Net;

namespace RestApiCSharp.Tests
{
    [AllureNUnit]
    public class GetUsersTests : BaseApiTest
    {

        [SetUp]
        public async Task Setup()
        {
            var users = new List<User>
            {
                new User { Age = 5, Name = "uGetUsersTests1", Sex = "FEMALE" },
                new User { Age = 15, Name = "uGetUsersTests2", Sex = "FEMALE" },
                new User { Age = 30, Name = "uGetUsersTests3", Sex = "MALE" },
                new User { Age = 50, Name = "uGetUsersTests4", Sex = "MALE" }
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, users);

        }

        [Test]
        [AllureStep("Get users without filters and verify 200 OK response")]
        public async Task GetUsers_Return200_Test()
        {
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Status code 200 not returned." +
                $"Expected status code 200 (OK), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users without filters and verify all users are returned")]
        public async Task GetUsers_UserReceived_Test()
        {


            var user123 = new User
            {
                Age = 0,
                Name = "trololo",
                Sex = "FEMALE",
                ZipCode = "oz"
            };

            await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user123);

            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            var expectedUsers = new[] { "uGetUsersTests1", "uGetUsersTests2", "uGetUsersTests3", "uGetUsersTests4" };

            foreach (var user in expectedUsers)
            {
                Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain(user),
                    $"The user '{user}' was not received. Response content: {await response.Content.ReadAsStringAsync()}");
            }
        }

        [Test]
        [AllureStep("Get users older than specified age and verify 200 OK response")]
        public async Task GetUsersOlderThan_Return200_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("olderThan", "6"),
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Status code 200 not returned." +
                $"Expected status code 200 (OK), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users older than specified age and verify correct users are returned")]
        public async Task GetUsersOlderThan_UserAdded_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("olderThan", "6"),
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            var expectedUsers = new[] { "uGetUsersTests2", "uGetUsersTests3", "uGetUsersTests4" };

            foreach (var user in expectedUsers)
            {
                Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain(user),
                    $"The user '{user}' was not received. Response content: {await response.Content.ReadAsStringAsync()}");
            }
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("uGetUsersTests1"),
            $"The user '{"uGetUsersTests1"}' was received, though should not be. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users younger than specified age and verify 200 OK response")]
        public async Task GetUsersYoungerThan_Return200_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("youngerThan", "6")
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Status code 200 not returned." +
                $"Expected status code 200 (OK), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users younger than specified age and verify correct users are returned")]
        public async Task GetUsersYoungerThan_UserAdded_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("youngerThan", "6")
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            var expectedUsers = new[] { "uGetUsersTests1" };

            foreach (var user in expectedUsers)
            {
                Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain(user),
                    $"The user '{user}' was not received. Response content: {await response.Content.ReadAsStringAsync()}");
            }
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("uGetUsersTests2"),
            $"The user '{"uGetUsersTests2"}' was received, though should not be. Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users filtered by sex and verify 200 OK response")]
        public async Task GetUsersSex_Return200_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("sex", "MALE")
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                $"Status code 200 not returned." +
                $"Expected status code 200 (OK), but got {response.StatusCode}. " +
                $"Response content: {await response.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Get users filtered by sex and verify only male users are returned")]
        public async Task GetUsersSex_UserAdded_Test()
        {
            var parameters = new List<(string name, string value)>
            {
                ("sex", "MALE")
            };
            var response = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope, parameters);

            var expectedUsers = new[] { "uGetUsersTests3", "uGetUsersTests4" };

            foreach (var user in expectedUsers)
            {
                Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain(user),
                    $"The user '{user}' was not received. Response content: {await response.Content.ReadAsStringAsync()}");
            }
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("uGetUsersTests1"),
            $"The user '{"uGetUsersTests1"}' was received, though should not be. Response content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
