/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Spudmash Media Pty Ltd. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdentityServer4.Contrib.AwsDynamoDB.Models.Extensions
{
    /// <summary>
    /// Client dynamo DBE xtensions.
    /// </summary>
    public static class ClientDynamoDBExtensions
    {
        //This claim converter will be used to make it so that Claims can be deserialized
        private static JsonSerializerSettings _settings = new JsonSerializerSettings(){Converters = new List<JsonConverter>(){new ClaimConverter()}};

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns>The client.</returns>
        /// <param name="cd">Cd.</param>
        public static Client GetClient(this ClientDynamoDB cd)
        {
            if (cd == null) return null;

            return JsonConvert.DeserializeObject<Client>(cd.JsonString, _settings);
        }

        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <returns>The clients.</returns>
        /// <param name="cd">Cd.</param>
        public static IEnumerable<Client> GetClients(this IEnumerable<ClientDynamoDB> cd)
        {
            if (cd == null) return null;

            return cd.Select(item => item.GetClient());
        }

        /// <summary>
        /// Gets the client dynamo db.
        /// </summary>
        /// <returns>The client dynamo db.</returns>
        /// <param name="cd">Cd.</param>
        public static ClientDynamoDB GetClientDynamoDB(this Client cd)
        {
            if (cd == null || string.IsNullOrEmpty(cd.ClientId)) return null;

            return new ClientDynamoDB
            {
                ClientId = cd.ClientId,
                JsonString = JsonConvert.SerializeObject(cd)
            };
        }

        /// <summary>
        /// Clients the dynamo DB.
        /// </summary>
        /// <returns>The dynamo DB.</returns>
        /// <param name="cd">Cd.</param>
        public static IEnumerable<ClientDynamoDB> ClientDynamoDBs(this IEnumerable<Client> cd)
        {
            if (cd == null || cd.Count() == 0) return null;

            return cd.Select(item => item.GetClientDynamoDB());
        }
    }

    /// <summary>
    /// Custom converter for claims objects which have no default constructor
    /// </summary>
    public class ClaimConverter : JsonConverter
    {
        /// <summary>
        /// Can Convert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(System.Security.Claims.Claim));
        }

        /// <summary>
        /// Method to manually construct a claim object in a the full constructor
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = (string)jo["Type"];
            string value = (string)jo["Value"];
            string valueType = (string)jo["ValueType"];
            string issuer = (string)jo["Issuer"];
            string originalIssuer = (string)jo["OriginalIssuer"];
            return new Claim(type, value, valueType, issuer, originalIssuer);
        }

        /// <summary>
        /// Can Write
        /// </summary>
        /// <value></value>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Write JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
