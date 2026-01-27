
using HarmonyLib;
using PurrNet;
using System.Text;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(PlayerPanelController))]
    internal class PlayerLeftPatch
    {
        [HarmonyPatch("DespawnHandle")]
        [HarmonyPrefix]
        public static void PlayerLeftNoti( NetworkTransform netTransform, PlayerPanelController __instance)
        {

		for (int i = 0; i < __instance.PlayerSteamIDs.Count; i++)
		{
			if (__instance.PlayerTransforms[i] == netTransform)
			{
				var dname = __instance.IDInfos[i].Name;
                NetworkSingleton<TextChannelManager>.I.AddNotification( Encoding.Unicode.GetString(dname) + " has left the server.");
                
                if (Plugin.configJoinLeaveNoises.Value)
                {
                NetworkSingleton<TextChannelManager>.I.MainSFXController.PlayPetSound(PetType.Frog);
                }
                break;
			}
		}
        }

    }
}
