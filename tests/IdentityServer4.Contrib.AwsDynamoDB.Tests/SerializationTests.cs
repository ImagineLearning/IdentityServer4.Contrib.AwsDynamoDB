using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Contrib.AwsDynamoDB.Models;
using IdentityServer4.Contrib.AwsDynamoDB.Models.Extensions;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Xunit;

namespace IdentityServer4.Contrib.AwsDynamoDB.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void CanDeserializeClaims()
        {
            var clientWithClaims = new IdentityServer4.Models.Client
            {
                ClientId = "testclient",
                ClientSecrets = new List<Secret> { new Secret { Value = "supersecret".Sha256(), Description = "Test Secret", Type = IdentityServerConstants.SecretTypes.SharedSecret} },
                AllowedGrantTypes = new List<string> { GrantType.ResourceOwnerPassword, GrantType.ClientCredentials },
                AllowedScopes = new List<string> { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email },
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                //this claim doesn't seem to be serializable
                Claims = new [] { new Claim(JwtClaimTypes.Role, "aclaim") },
                ClientClaimsPrefix = string.Empty
            };

            var dynamoObject = new ClientDynamoDB()
            {
                ClientId = clientWithClaims.ClientId,
                JsonString = JsonConvert.SerializeObject(clientWithClaims)
            };

            var deserialized = dynamoObject.GetClient();

            //this will just verify that everything is working properly, the clientid isn't important in this case
            //an exception would be thrown if serialization failed
            Assert.True(clientWithClaims.ClientId == deserialized.ClientId);
        }
    }
}
