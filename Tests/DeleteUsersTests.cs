using Allure.NUnit;
using Allure.NUnit.Attributes;
using RestApiCSharp.ConstantsTestingGeneral;
using System.Net;

namespace RestApiCSharp.Tests
{
    [AllureNUnit]
    public class DeleteUsersTests : BaseApiTest
    {
        [SetUp]
        public async Task Setup()
        {
            var zipCodes = new List<string> 
            { "oz", "oz1", "oz2", "oz3" };

            await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
        }

        [Test]
        [AllureIssue("BUG_DeleteUser_1")]
        [AllureStep("Delete user and validate response")]
        public async Task DeleteUser_AllFields_Return204_UserDeleted_ZipCodeReturned_Test()
        {
            var user = new User
            {
                Age = 0,
                Name = "u",
                Sex = "FEMALE",
                ZipCode = "oz"
            };

            await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            var deleteUsersResponse = await ApiClientInstance.DeleteUser(ConstantsTesting.WriteScope, user);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);
            var getZipCodesResponse = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(deleteUsersResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent),
                            $"Status code 204 not returned." +
                            $"Expected status code 204 (No Content), but got {deleteUsersResponse.StatusCode}. " +
                            $"Response content: {await deleteUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("u"),
                            $"The user 'u' was not deleted. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getZipCodesResponse.Content.ReadAsStringAsync(), Does.Contain("oz"),
                            $"Not all available zip codes found. Response content: {await getZipCodesResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_DeleteUser_2")]
        [AllureStep("Delete user with required fields and validate response")]
        public async Task DeleteUser_RequiredFields_Return204_UserDeleted_ZipCodeReturned_Test()
        {
            var user = new User
            {
                Age = 0,
                Name = "u1",
                Sex = "FEMALE",
                ZipCode = "oz1"
            };

            var userRequiredFields = new User
            {
                Name = "u1",
                Sex = "FEMALE"
            };

            await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            var deleteUsersResponse = await ApiClientInstance.DeleteUser(ConstantsTesting.WriteScope, userRequiredFields);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);
            var getZipCodesResponse = await ApiClientInstance.GetZipCodes(ConstantsTesting.ReadScope);

            Assert.That(deleteUsersResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent),
                            $"Status code 204 not returned." +
                            $"Expected status code 204 (No Content), but got {deleteUsersResponse.StatusCode}. " +
                            $"Response content: {deleteUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("u1"),
                            $"The user 'u1' was not deleted. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getZipCodesResponse.Content.ReadAsStringAsync(), Does.Contain("oz1"),
                            $"Not all available zip codes found. Response content: {await getZipCodesResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_DeleteUser_3")]
        [AllureStep("Delete user and validate response")]
        public async Task DeleteUser_RequiredFieldsBoth_Return204_UserDeleted_Test()
        {
            var user = new User
            {
                Name = "u2",
                Sex = "FEMALE"
            };

            await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            var deleteUsersResponse = await ApiClientInstance.DeleteUser(ConstantsTesting.WriteScope, user);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(deleteUsersResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent),
                            $"Status code 204 not returned." +
                            $"Expected status code 204 (No Content), but got {deleteUsersResponse.StatusCode}. " +
                            $"Response content: {await deleteUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("u2"),
                            $"The user 'u2' was not deleted. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Delete user with missing required fields and check for conflict")]
        public async Task DeleteUser_NotAllReqFields_Return409_UserNotDeleted_Test()
        {
            var user = new User
            {
                Age = 0,
                Name = "u3",
                Sex = "FEMALE",
                ZipCode = "oz3"
            };

            var userNotAllReqFields = new User
            {
                Age = 0,
                Name = "u3",
                ZipCode = "oz3"
            };

            await ApiClientInstance.CreateUsers(ConstantsTesting.WriteScope, user);

            var deleteUsersResponse = await ApiClientInstance.DeleteUser(ConstantsTesting.WriteScope, userNotAllReqFields);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            var deleteUsersResponseContent = await deleteUsersResponse.Content.ReadAsStringAsync();
            var getUsersResponseContent = await getUsersResponse.Content.ReadAsStringAsync();

            Assert.That(deleteUsersResponseContent, Does.Contain("Conflict"),
                 $"Response content does not indicate FailedDependency. Content: {deleteUsersResponseContent}");
            Assert.That(getUsersResponseContent, Does.Contain("u3"),
                            $"The user 'u1' was not deleted. Response content: {getUsersResponseContent}");
        }
    }
}
