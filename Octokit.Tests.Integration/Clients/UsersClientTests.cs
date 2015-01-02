﻿using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;
using Octokit.Tests.Helpers;
using Octokit.Tests.Integration;
using Xunit;

public class UsersClientTests
{
    public class TheGetMethod
    {
        [IntegrationTest]
        public async Task ReturnsSpecifiedUser()
        {
            var github = Helper.GetAuthenticatedClient();

            // Get a user by username
            var user = await github.User.Get("tclem");

            Assert.Equal("GitHub", user.Company);
        }

        [IntegrationTest]
        public async Task ReturnsSpecifiedUserUsingAwaitableCredentialProvider()
        {
            var github = new GitHubClient(new ProductHeaderValue("OctokitTests"),
                new ObservableCredentialProvider());

            // Get a user by username
            var user = await github.User.Get("tclem");

            Assert.Equal("GitHub", user.Company);
        }

        class ObservableCredentialProvider : ICredentialStore
        {
            public async Task<Credentials> GetCredentials()
            {
                return await Observable.Return(Helper.Credentials);
            }
        }
    }

    public class TheCurrentMethod
    {
        [IntegrationTest]
        public async Task ReturnsSpecifiedUser()
        {
            var github = Helper.GetAuthenticatedClient();

            var user = await github.User.Current();

            Assert.Equal(Helper.UserName, user.Login);
        }
    }

    public class TheUpdateMethod
    {
        [IntegrationTest]
        public async Task FailsWhenNotAuthenticated()
        {
            var github = Helper.GetAnonymousClient();

            var userUpdate = new UserUpdate
            {
                Name = Helper.Credentials.Login,
                Bio = "UPDATED BIO"
            };

            var e = await AssertEx.Throws<AuthorizationException>(async 
                () => await github.User.Update(userUpdate));
            Assert.Equal(HttpStatusCode.Unauthorized, e.StatusCode);
        }

        [IntegrationTest]
        public async Task FailsWhenAuthenticatedWithBadCredentials()
        {
            var github = Helper.GetBadCredentialsClient();

            var userUpdate = new UserUpdate
            {
                Name = Helper.Credentials.Login,
                Bio = "UPDATED BIO"
            };

            var e = await AssertEx.Throws<AuthorizationException>(async 
                () => await github.User.Update(userUpdate));
            Assert.Equal(HttpStatusCode.Unauthorized, e.StatusCode);
        }
    }
}
