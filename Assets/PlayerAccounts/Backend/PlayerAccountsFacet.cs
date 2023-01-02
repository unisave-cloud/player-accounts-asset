using System;
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
        /// <summary>
        /// Name of the database collection containing player documents
        /// </summary>
        public const string PlayersCollection = "players";
        
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
                    .Bind("@collection", PlayersCollection)
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
                    .Bind("@collection", PlayersCollection)
                    .Bind("player", player)
                    .FirstAs<JsonObject>();
            });

            return player;
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
                PlayersCollection,
                CollectionType.Document
            );
        }
    }
}