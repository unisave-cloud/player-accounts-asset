using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerAccounts.Editor
{
    public class PlayerManagerWindow : EditorWindow
    {
        [MenuItem("Window/Unisave/PlayerManager", false, 10)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(
                typeof(PlayerManagerWindow),
                false,
                "Player Manager"
            );
        }
        
        [SerializeField]
        private VisualTreeAsset visualTree;

        private void CreateGUI()
        {
            rootVisualElement.Add(visualTree.Instantiate());
        }
    }
}