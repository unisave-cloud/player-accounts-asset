using LightJson;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerAccounts.Examples.AnonymousAuthentication
{
    public class PlayerInfoDisplay : MonoBehaviour
    {
        public PlayerAccountsManager playerAccounts;

        public Text playerInfoText;

        void Start()
        {
            playerInfoText.text = "Loading...";
            
            playerAccounts.WatchMe(() => {
                JsonObject player = playerAccounts.Me<JsonObject>();
                playerInfoText.text = player.ToString(pretty: true);
            });
        }
    }
}