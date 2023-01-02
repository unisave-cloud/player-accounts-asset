using LightJson;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerAccounts.Examples.AnonymousAuthentication
{
    public class PlayerNicknameForm : MonoBehaviour
    {
        public PlayerAccountsManager playerAccounts;

        public InputField nicknameField;

        public Button saveNicknameButton;
        
        void Start()
        {
            saveNicknameButton.onClick.AddListener(SaveNickname);
            
            nicknameField.text = "";
            
            playerAccounts.WatchMe(() => {
                var player = playerAccounts.Me<ExamplePlayer>();
                nicknameField.text = player.nickname;
            });
        }

        void SaveNickname()
        {
            playerAccounts.UpdateMe(new JsonObject {
                ["nickname"] = nicknameField.text
            });
            
            // TODO: fix serializer for anonymous type fields
            // <nickname>i__Field
            //
            // playerAccounts.UpdateMe(new {
            //     nickname = nicknameField.text
            // });
        }
    }
}