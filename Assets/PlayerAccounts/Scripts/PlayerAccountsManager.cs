using System;
using LightJson;
using Unisave.Facades;
using Unisave.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerAccounts
{
    public class PlayerAccountsManager : MonoBehaviour
    {
        #region "Anonymous Authentication"

        public bool authenticateAnonymouslyOnStart = false;

        /// <summary>
        /// Call this to perform anonymous authentication.<br/>
        /// Anonymous authentication is ideal for mobile and web games,
        /// where you don't want to add friction to the user experience.
        /// It works by generating a random token for each player that
        /// is stored in PlayerPrefs and acts as both the username
        /// and the password.
        /// </summary>
        public void AuthenticateAnonymously()
        {
            string token = AnonymousToken.Get();

            OnFacet<PlayerAccountsFacet>
                .Call<JsonObject>(
                    nameof(PlayerAccountsFacet.AuthenticateAnonymously),
                    token
                )
                .Then(player => {
                    // TODO
                    Debug.Log("Received login response!");
                    if (!this.enabled)
                        Debug.Log("We are disabled!");
                    playerDocument = player;
                    onPlayerChange?.Invoke();
                })
                .Catch(exception => {
                    // TODO
                    Debug.LogException(exception);
                });
        }
        
        #endregion
        
        #region "Email Authentication"
        
        // links to forms and submit button
        
        // with rememberMeToken
        
        #endregion

        private JsonObject playerDocument = null;

        private UnityEvent onPlayerChange = new UnityEvent();

        void Start()
        {
            if (authenticateAnonymouslyOnStart)
                AuthenticateAnonymously();
        }
        
        public T Me<T>()
        {
            return (T) Serializer.FromJson(playerDocument, typeof(T));
        }

        public void WatchMe(UnityAction callback)
        {
            onPlayerChange.AddListener(callback);
        }

        public void UpdateMe<T>(T data)
        {
            JsonObject jsonData = Serializer.ToJson<T>(data);
            
            OnFacet<PlayerAccountsFacet>
                .Call<JsonObject>(
                    nameof(PlayerAccountsFacet.UpdateMe),
                    jsonData
                )
                .Then(player => {
                    // TODO
                    Debug.Log("Received update response!");
                    if (!this.enabled)
                        Debug.Log("We are disabled!");
                    playerDocument = player;
                    onPlayerChange?.Invoke();
                })
                .Catch(exception => {
                    // TODO
                    Debug.LogException(exception);
                });
        }

        public T Player<T>(string playerId)
        {
            return default(T);
        }
    }
}