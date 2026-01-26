using BepInEx;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Drawing.Printing;
using System.Reflection;
using JetBrains.Annotations;
using System.Reflection.Emit;
using MonoMod.Cil;

namespace PlayerLimitLift.patches
{
    [HarmonyPatch(typeof(MainMenuUIController))]
    
    public class PlayerLimitPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch( ref TextMeshProUGUI ____filterPlayerText )
        {
            MonoSingleton<MultiplayerManager>.I.FilterPlayerValue = 24;
            ____filterPlayerText.text = "24";

        }

        [HarmonyPatch("ButtonChangeFilterPlayer")]
        [HarmonyPrefix]
        public static bool Button1Patch( int value, MainMenuUIController __instance, ref TextMeshProUGUI ____filterPlayerText )
        {
            var val = MonoSingleton<MultiplayerManager>.I.FilterPlayerValue;
            if (value == 1)
            {
                val = (val - 1 + 1) % 24 + 1;
            }
            else if (val == 1)
            {
                val = 24;
            }
            else
            {
                val--;
            }
            MonoSingleton<MultiplayerManager>.I.FilterPlayerValue = val;
            ____filterPlayerText.text = val.ToString();
            return false;
        }
        [HarmonyPatch("ButtonChangeFilterListPlayer")]
        [HarmonyPrefix]
        public static bool Button2Patch( int value, MainMenuUIController __instance, ref TextMeshProUGUI ____filterListPlayerText )
        {
            var val = MonoSingleton<MultiplayerManager>.I.FilterListPlayerValue;
            if (value == 1)
            {
                val = (val + 1) % 24;
            }
            else if (val == 0)
            {
                val = 24;
            }
            else
            {
                val--;
            }
            MonoSingleton<MultiplayerManager>.I.FilterListPlayerValue = val;
            ____filterListPlayerText.text = ((val == 0) ? MonoSingleton<SettingsController>.I.AllString.String : val.ToString());
            return false;
        }

    }
}
