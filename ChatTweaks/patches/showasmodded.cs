
using HarmonyLib;
using System.Collections.Generic;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    public class AddModdedTag
    {
        [HarmonyPatch("CreateLobby")]
        [HarmonyPrefix]
        public static void NameChanger( ref List<bool> ___FilterSocialTags )
        {
            var val = MonoSingleton<MultiplayerManager>.I.FilterPlayerValue;
            var text = MonoSingleton<MainMenuUIController>.I.CreateSessionNameInputField.text;
            ___FilterSocialTags[4] = true;
        }
    }
}
