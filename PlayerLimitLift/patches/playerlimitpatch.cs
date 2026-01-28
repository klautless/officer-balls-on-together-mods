using HarmonyLib;
using UnityEngine;
using TMPro;

namespace PlayerLimitLift.patches
{
    [HarmonyPatch(typeof(MainMenuUIController))]
    
    public class PlayerLimitPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch( ref TextMeshProUGUI ____filterPlayerText )
        {
            MonoSingleton<MultiplayerManager>.I.FilterPlayerValue = Plugin.configDefaultLobbySize.Value;
            ____filterPlayerText.text = Plugin.configDefaultLobbySize.Value.ToString();
        }

        [HarmonyPatch("ButtonChangeFilterPlayer")]
        [HarmonyPrefix]
        public static bool Button1Patch( int value, MainMenuUIController __instance, ref TextMeshProUGUI ____filterPlayerText )
        {
            var amount = ( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ) 
                            ? Plugin.configShiftSkipRate.Value : 1;
            var val = MonoSingleton<MultiplayerManager>.I.FilterPlayerValue;
            if (value == 1)
            {
                val = (val - 1 + 1) % 128 + amount;
            }
            else if (val == 1)
            {
                val = 128;
            }
            else
            {
                val-=amount;
            }
            MonoSingleton<MultiplayerManager>.I.FilterPlayerValue = val;
            ____filterPlayerText.text = val.ToString();
            return false;
        }
        [HarmonyPatch("ButtonChangeFilterListPlayer")]
        [HarmonyPrefix]
        public static bool Button2Patch( int value, MainMenuUIController __instance, ref TextMeshProUGUI ____filterListPlayerText )
        {
            var amount = ( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ) 
                            ? Plugin.configShiftSkipRate.Value : 1;
            var val = MonoSingleton<MultiplayerManager>.I.FilterListPlayerValue;
            if (value == 1)
            {
                val = (val + amount) % 128;
            }
            else if (val == 0)
            {
                val = 128;
            }
            else
            {
                val-=amount;
            }
            MonoSingleton<MultiplayerManager>.I.FilterListPlayerValue = val;
            ____filterListPlayerText.text = ((val == 0) ? MonoSingleton<SettingsController>.I.AllString.String : val.ToString());
            return false;
        }

    }
}
