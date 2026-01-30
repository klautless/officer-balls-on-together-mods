
using HarmonyLib;
using System.Collections.Generic;

namespace PlayerLimitLift.patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    
    public class AddModdedTag
    {
        [HarmonyPatch("CreateLobby")]
        [HarmonyPrefix]
        public static void NameChanger( ref List<bool> ___FilterSocialTags )
        {
            //Debug.Log("branch reached");
            var val = MonoSingleton<MultiplayerManager>.I.FilterPlayerValue;
            var text = MonoSingleton<MainMenuUIController>.I.CreateSessionNameInputField.text;
            if (val > 16)
            {
                ___FilterSocialTags[4] = true;

                //MonoSingleton<MainMenuUIController>.I.CreateSessionNameInputField.text = "[MODDED] " + text;
            }

        }

    }
}
