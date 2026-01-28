
using HarmonyLib;
using TMPro;
using PurrLobby;

namespace PlayerLimitLift.patches
{
    [HarmonyPatch(typeof(PlayerPanelController))]
    
    public class PanelPatch
    {
        [HarmonyPatch("UpdateServerPanel")]
        [HarmonyPostfix]
        public static void SizeMarker()
        {
            LobbyManager lobbyManager = MonoSingleton<MultiplayerManager>.I._lobbyManager;
            string max = (lobbyManager.CurrentLobby.MaxPlayers - 1).ToString();
            string cur = lobbyManager.CurrentLobby.Members.Count.ToString();
            var textRef = AccessTools.FieldRefAccess<PlayerPanelController, TextMeshProUGUI>("_lobbyNameText");
            var instance = NetworkSingleton<PlayerPanelController>.I;
            textRef(instance).text = MonoSingleton<MultiplayerManager>.I.LobbyName + " (" + cur + "/" + max + ")";

        }

    }
}
