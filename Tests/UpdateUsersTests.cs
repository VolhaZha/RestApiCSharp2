using Allure.NUnit;
using Allure.NUnit.Attributes;
using RestApiCSharp.ConstantsTestingGeneral;
using RestApiCSharp.Models;
using System.Net;

namespace RestApiCSharp.Tests
{
    [AllureNUnit]
    public class UpdateUsersTests : BaseApiTest
    {
        [SetUp]
        public async Task Setup()
        {
            var zipCodes = new List<string> 
            { "oz", "oz1", "oz2", "oz3", "oz4", "oz5", "oz6", "oz7", "oz02", "oz12", "oz22", "oz32", "oz42" };

            await ApiClientInstance.ExpandZipCodes(ConstantsTesting.WriteScope, zipCodes);
        }

        [Test]
        [AllureStep("Update user via PATCH and verify user is updated successfully")]
        public async Task UpdateUserPatch_Return200_UserUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u", Sex = "FEMALE", ZipCode = "oz"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New", Sex = "MALE", ZipCode = "oz02" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPatch(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                            $"Status code 200 not returned." +
                            $"Expected status code 200 (OK), but got {updateUserResponse.StatusCode}. " +
                            $"Response content: {await updateUserResponse.Content.ReadAsStringAsync()}");
            Assert.That(getUsersResponse.Content, Does.Contain("New"),
                            $"The user 'New' was not added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Update user via PUT and verify user is updated successfully")]
        public async Task UpdateUserPut_Return200_UserUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u1", Sex = "FEMALE", ZipCode = "oz1"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New1", Sex = "MALE", ZipCode = "oz12" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPut(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                            $"Status code 200 not returned." +
                            $"Expected status code 200 (OK), but got {updateUserResponse.StatusCode}. " +
                            $"Response content: {await updateUserResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Contain("New"),
                            $"The user 'New' was not added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Update user via PATCH with incorrect zip code and verify update is rejected")]
        public async Task UpdateUserIncorrectZipCodePatch_Return424_UserNotUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u2", Sex = "FEMALE", ZipCode = "oz2"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New2", Sex = "MALE", ZipCode = "oz222" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPatch(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await updateUserResponse.Content.ReadAsStringAsync(), Does.Contain("FailedDependency"),
                 $"Response content does not indicate FailedDependency. Content: {await updateUserResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("New2"),
                $"The user 'New2' was added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Update user via PUT with incorrect zip code and verify update is rejected")]
        public async Task UpdateUserIncorrectZipCodePut_Return424_UserNotUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u3", Sex = "FEMALE", ZipCode = "oz3"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New3", Sex = "MALE", ZipCode = "oz332" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPut(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await updateUserResponse.Content.ReadAsStringAsync(), Does.Contain("FailedDependency"),
                 $"Response content does not indicate FailedDependency. Content: {await updateUserResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("New3"),
                $"The user 'New3' was added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureStep("Update user via PATCH with missing required fields and verify conflict response")]
        public async Task UpdateUserNotAllReqFieldsPatch_Return409_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u4", Sex = "FEMALE", ZipCode = "oz4"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New4", ZipCode = "oz42" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPatch(ConstantsTesting.WriteScope, userUpdate);

            Assert.That(await updateUserResponse.Content.ReadAsStringAsync(), Does.Contain("Conflict"),
                 $"Response content does not indicate Conflict. Content: {await updateUserResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_UpdateUser_1")]
        [AllureStep("Update user via PATCH with missing required fields and verify user is not updated")]
        public async Task UpdateUserNotAllReqFieldsPatch_UserNotUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u5", Sex = "FEMALE", ZipCode = "oz5"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New4", ZipCode = "oz42" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPatch(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("New4"),
                $"The user 'New4' was added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(    getUsersResponse.Content, Does.Contain("u5"),
                $"The user 'u5' was removed. Response content: {    getUsersResponse.Content}");
        }

        [Test]
        [AllureStep("Update user via PUT with missing required fields and verify conflict response")]
        public async Task UpdateUserNotAllReqFieldsPut_Return409_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u6", Sex = "FEMALE", ZipCode = "oz6"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New4", ZipCode = "oz42" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPut(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await updateUserResponse.Content.ReadAsStringAsync(), Does.Contain("Conflict"),
                 $"Response content does not indicate Conflict. Content: {await updateUserResponse.Content.ReadAsStringAsync()}");
        }

        [Test]
        [AllureIssue("BUG_UpdateUser_2")]
        [AllureStep("Update user via PUT with missing required fields and verify user is not updated")]
        public async Task UpdateUserNotAllReqFieldsPut_UserNotUpdated_Test()
        {
            var usersInitialCreation = new List<User>
            {
                new User { Age = 1, Name = "u7", Sex = "FEMALE", ZipCode = "oz7"}
            };

            await ApiClientInstance.CreateUsersList(ConstantsTesting.WriteScope, usersInitialCreation);

            var userUpdate = new UserUpdate
            {
                UserNewValues = new User { Age = 20, Name = "New4", ZipCode = "oz42" },
                UserToChange = usersInitialCreation[0]
            };

            var updateUserResponse = await ApiClientInstance.UpdateUsersPut(ConstantsTesting.WriteScope, userUpdate);
            var getUsersResponse = await ApiClientInstance.GetUsers(ConstantsTesting.ReadScope);

            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Not.Contain("New4"),
                $"The user 'New4' was added. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
            Assert.That(await getUsersResponse.Content.ReadAsStringAsync(), Does.Contain("u7"),
                $"The user 'u7' was removed. Response content: {await getUsersResponse.Content.ReadAsStringAsync()}");
        }
    }
}
