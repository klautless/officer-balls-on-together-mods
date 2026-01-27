
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Teleport.patches
{
    [HarmonyPatch(typeof(PlayerPanelController))]
    public static class TeleportPatch
    {
        public static bool setup = false;
        public static int index = 1;
        
        public static PlayerController target = null;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void TelePatch()
        {
            var test1 = AccessTools.FieldRefAccess<CustomizationUIController, GameObject>("_customizationPanel");
            var instance1 = MonoSingleton<CustomizationUIController>.I;
            if (test1(instance1).activeSelf) return;

            var test2 = AccessTools.FieldRefAccess<PlayerPanelController, GameObject>("_reportPanel");
            var instance2 = NetworkSingleton<PlayerPanelController>.I;
            if (test2(instance2).activeSelf) return;

            

            int tempdigit = 0;
            var fieldRef = AccessTools.FieldRefAccess<UIManager, TMP_InputField>("_messageInputField");
            var instance = MonoSingleton<UIManager>.I;
            if (fieldRef(instance).isFocused) return;
            if(Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
            {
                if( NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count == 1 )
                {
                    //Debug.Log("can't teleport without other players!");
                    return;
                }
                index += 1;
                tempdigit = 1;
                goto changed;
            }
            else if(Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift))
            {
                if( NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count == 1 )
                {
                    //Debug.Log("can't teleport without other players!");
                    return;
                }
                index -= 1;
                tempdigit = -1;
                goto changed;
                
            }
            else if(Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.LeftShift))
            {
                if( NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count == 1 )
                {
                    //Debug.Log("can't teleport without other players!");
                    return;
                }
                goto warp;
            }
            return;

            changed:
                for (var i = 0; i < 2; i++)
                {
                    if (index < 0) index = NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count - 1;
                    else if (index >= NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count ) index = 0;
                    target = NetworkSingleton<PlayerPanelController>.I.PlayerTransforms[index].GetComponent<PlayerController>();
                    var taname = target.PlayerNameText.text;
                    var ourname = MonoSingleton<DataManager>.I.PlayerData.Name;
                    if (taname != ourname ) break;
                    if (tempdigit == 1) index+=1;
                    else if (tempdigit == -1) index-=1;
                
                }
                var namefieldRef = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_playerText");
                var nameinstance = MonoSingleton<UIManager>.I;
                var playname = MonoSingleton<DataManager>.I.PlayerData.Name;

                //int num = NetworkSingleton<PlayerPanelController>.I.PlayerIDs.IndexOf(info.sender);
		        target = NetworkSingleton<PlayerPanelController>.I.PlayerTransforms[index].GetComponent<PlayerController>();
                var tname = target.PlayerNameText.text;
                namefieldRef(nameinstance).text = playname + "            -            " + tname + " selected";
                //Debug.Log(tname + " selected.");
                return;
            warp:
                if (target)
                {
                    var controller = target.transform.GetComponentInChildren<PlayerMovementController>() as PlayerMovementController;
                    NetworkSingleton<TextChannelManager>.I.MainPlayer.position = controller.transform.position;
                    //Debug.Log("warped?");

                    //Debug.Log(target.transform.GetComponentInChildren<PlayerMovementController>());
                }
                //else Debug.Log("no valid target!");

                return;
        }

        [HarmonyPatch("DespawnHandle")]
        [HarmonyPostfix]
        static public void DespawnHandlePatch( PlayerPanelController __instance)
        {
            if (index >= __instance.PlayerSteamIDs.Count) index = __instance.PlayerSteamIDs.Count - 1;
            target = NetworkSingleton<PlayerPanelController>.I.PlayerTransforms[index].GetComponent<PlayerController>();
            var namefieldRef = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_playerText");
            var nameinstance = MonoSingleton<UIManager>.I;
            var playname = MonoSingleton<DataManager>.I.PlayerData.Name;

            var tname = target.PlayerNameText.text;
            namefieldRef(nameinstance).text = playname + "            -            " + tname + " selected";
            //Debug.Log("player disconnected. " + tname + " selected.");
                
        }

    }
}
