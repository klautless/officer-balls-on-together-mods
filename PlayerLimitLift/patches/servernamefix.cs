
using HarmonyLib;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

using PurrNet;
using System.Collections.Generic;

namespace PlayerLimitLift.patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    
    public class ServerNameModder
    {
        [HarmonyPatch("CreateLobby")]
        [HarmonyPrefix]
        public static void NameChanger()
        {
            //Debug.Log("branch reached");
            var val = MonoSingleton<MultiplayerManager>.I.FilterPlayerValue;
            var text = MonoSingleton<MainMenuUIController>.I.CreateSessionNameInputField.text;
            if (val > 16)
            {
                MonoSingleton<MainMenuUIController>.I.CreateSessionNameInputField.text = "[MODDED] " + text;
            }

        }

    }
}
