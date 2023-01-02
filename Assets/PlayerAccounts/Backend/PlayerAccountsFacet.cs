using System;
using System.Collections.Generic;
using System.Linq;
using LightJson;
using Unisave.Arango;
using Unisave.Authentication;
using Unisave.Contracts;
using Unisave.Facades;
using Unisave.Facets;

namespace PlayerAccounts
{
    public class PlayerAccountsFacet : Facet
    {
        private PlayerAccountsConfig config =
            PlayerAccountsConfig.GetConfigInstance();
        
        #region "Anonymous Authentication"
        
        public JsonObject AuthenticateAnonymously(string anonymousToken)
        {
            ValidateAnonymousToken(anonymousToken);
            
            JsonObject player = RetrieveAnonymousPlayer(anonymousToken);

            if (player == null)
                player = CreateAnonymousPlayer(anonymousToken);

            // TODO: Auth facade should work with documents, not entities
            //Auth.Login(player["_id"]);
            Session.Set(AuthenticationManager.SessionKey, player["_id"]);

            // TODO: prune the returned fields based on security rules
            return player;
        }

        private void ValidateAnonymousToken(string anonymousToken)
        {
            if (string.IsNullOrEmpty(anonymousToken) || anonymousToken.Length < 8)
                throw new InvalidOperationException("Anonymous token too short.");
            
            if (anonymousToken.Length > 64)
                throw new InvalidOperationException("Anonymous token too long.");
        }

        private JsonObject RetrieveAnonymousPlayer(string anonymousToken)
        {
            JsonObject player = null;
            
            AssertPlayersCollection(() => {
                player = DB.Query(@"
                    FOR p IN @@collection
                        FILTER p.anonymousToken == @token
                        RETURN p
                ")
                    .Bind("@collection", config.PlayersCollection)
                    .Bind("token", anonymousToken)
                    .FirstAs<JsonObject>();
            });

            return player;
        }

        private JsonObject CreateAnonymousPlayer(string anonymousToken)
        {
            var player = new JsonObject() {
                ["anonymousToken"] = anonymousToken
            };
            
            AssertPlayersCollection(() => {
                player = DB.Query(@"
                    INSERT @player INTO @@collection
                    RETURN NEW
                ")
                    .Bind("@collection", config.PlayersCollection)
                    .Bind("player", player)
                    .FirstAs<JsonObject>();
            });

            return player;
        }

        #endregion

        public JsonObject UpdateMe(JsonObject data)
        {
            data = RemoveFields(data, config.NonWritableFieldsByMyself);
            
            // TODO: something like Auth.Id() facade
            string playerId = Session.Get<string>(AuthenticationManager.SessionKey);

            if (playerId == null)
                throw new InvalidOperationException("No player is authenticated.");
            
            AssertPlayersCollection(() => {
                data = DB.Query(@"
                    UPDATE PARSE_IDENTIFIER(@id).key WITH @data IN @@collection
                    RETURN NEW
                ")
                    .Bind("@collection", config.PlayersCollection)
                    .Bind("id", playerId)
                    .Bind("data", data)
                    .FirstAs<JsonObject>();
            });
            
            // TODO: remove fields not to be seen
            return data;
        }

        private static JsonObject RemoveFields(JsonObject document, string[] fields)
        {
            JsonObject result = new JsonObject();

            foreach (KeyValuePair<string, JsonValue> pair in document)
            {
                if (fields.Contains(pair.Key))
                    continue;

                result[pair.Key] = pair.Value;
            }

            return result;
        }
        
        private static JsonObject PreserveFields(JsonObject document, string[] fields)
        {
            JsonObject result = new JsonObject();

            foreach (string field in fields)
            {
                if (document.ContainsKey(field))
                    result[field] = document[field];
            }

            return result;
        }
        
        private void AssertPlayersCollection(Action body)
        {
            try
            {
                body.Invoke();
            }
            catch (ArangoException e) when (e.ErrorNumber == 1203)
            {
                CreatePlayersCollection();
                
                body.Invoke();
            }
        }

        private void CreatePlayersCollection()
        {
            IArango arango = Facade.App.Resolve<IArango>();
            
            arango.CreateCollection(
                config.PlayersCollection,
                CollectionType.Document
            );
        }
    }
}