using Unisave.Utils;
using UnityEngine;

namespace PlayerAccounts
{
    /// <summary>
    /// Provides access to the anonymous token representing the user using
    /// the device. It is used for anonymous authentication.
    /// </summary>
    public class AnonymousToken
    {
        private const string TokenKey = "auth.playerToken";

        /// <summary>
        /// Retrieves the anonymous token for this device.
        /// </summary>
        public static string Get()
        {
            if (!PlayerPrefs.HasKey(TokenKey))
            {
                PlayerPrefs.SetString(TokenKey, Str.Random(32));
                PlayerPrefs.Save();
            }

            return PlayerPrefs.GetString(TokenKey);
        }
    }
}