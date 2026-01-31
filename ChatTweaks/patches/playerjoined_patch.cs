
using HarmonyLib;

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
        }

    }
}
