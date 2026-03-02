
using UnityEngine;
using HarmonyLib;
using _otAPI;
using System.Reflection;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(PlayerCustomizationController))]
    internal class joinSoundPatch
    {
        [HarmonyPatch("JoinServerNotificationRPC_Original_1")]
        [HarmonyPrefix]
        public static void joinSoundPatcher()
        {
            if (Plugin.configJoinLeaveNoises.Value)
            {
                NetworkSingleton<TextChannelManager>.I.MainSFXController.PlayPetSound(PetType.FisherBirdy);
            }
            /*new UIImage( Assembly.GetExecutingAssembly(), "chatTweaks/images/smile.png",
                new Vector2Int( 384,384 ),
                Vector2.zero, " smiley",
                out GameObject image );*/
        }

    }
}