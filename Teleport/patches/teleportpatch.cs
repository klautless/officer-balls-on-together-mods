
using HarmonyLib;
using TMPro;
using UnityEngine;
using BepInEx.Bootstrap;
using StatusMessage.patches;
using UnityEngine.InputSystem;
using System;

namespace Teleport.patches
{
    [HarmonyPatch(typeof(PlayerPanelController))]
    public static class TeleportPatch
    {
        public static string targetingString;
        public static string combinedName = "";
        public static bool setup = false;
        public static int index = 1;
        public static float wheelstate = 0f;
        public static PlayerController target = null;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
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
            float temp = Mouse.current.scroll.value.y;
            string command = "";
            if (temp < 0 && wheelstate != temp)
            {
                command = "up";
                wheelstate = temp;
            }
            else if (temp > 0 && wheelstate != temp)
            {
                command = "down";
                wheelstate = temp;
            }
            else if (wheelstate != 0f && temp == 0f)
            {
                wheelstate = 0f;
            }
            if(command == "up" && Input.GetKey(KeyCode.LeftShift))
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
            else if(command == "down" && Input.GetKey(KeyCode.LeftShift))
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
            else if(Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
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
                target = null;
                for (var i = 0; i < 2; i++)
                {
                    if (index < 0) index = NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count - 1;
                    else if (index >= NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count ) index = 0;
                    
                    var tar_id = NetworkSingleton<PlayerPanelController>.I.PlayerIDs[index];
                    var self_id = NetworkSingleton<TextChannelManager>.I.localPlayer;
                    if (tar_id != self_id)
                    {
                        target = NetworkSingleton<PlayerPanelController>.I.PlayerTransforms[index].GetComponent<PlayerController>();
                        break;
                    }
                    if (tempdigit == 1) index+=1;
                    else if (tempdigit == -1) index-=1;
                
                }
                var namefieldRef = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_playerText");
                var nameinstance = MonoSingleton<UIManager>.I;
                var playname = MonoSingleton<DataManager>.I.PlayerData.Name;

                
                if (target!=null)
                {
                    var tname = target.PlayerNameText.text;
                    targetingString = "            -            " + tname + " selected";
                }
                else
                {
                    targetingString = "";
                }
                if (Chainloader.PluginInfos.TryGetValue("officerballs.StatusManager", out var basicInfo))
                {
                    StatusPatch._ifTeleportString = targetingString;
                }
                namefieldRef(nameinstance).text = playname + targetingString;
                //Debug.Log(tname + " selected.");
                return;
            warp:
                if (target)
                {
                    var controller = target.transform.GetComponentInChildren<PlayerMovementController>() as PlayerMovementController;
                    Teleporter.warppos = controller.transform.position;
                    
                    NetworkSingleton<TextChannelManager>.I.MainPlayer.position = controller.transform.position;
                    // left in as a failsafe for sitting still, focusing, etc
                    
                    
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
            target = null;
            if (index >= __instance.PlayerSteamIDs.Count) index = __instance.PlayerSteamIDs.Count - 1;
            for (var i = 0; i < 2; i++)
                {
                    if (index < 0) index = NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count - 1;
                    else if (index >= NetworkSingleton<PlayerPanelController>.I.PlayerIDs.Count ) index = 0;
                    
                    var tar_id = NetworkSingleton<PlayerPanelController>.I.PlayerIDs[index];
                    var self_id = NetworkSingleton<TextChannelManager>.I.localPlayer;
                    if (tar_id != self_id)
                    {
                        target = NetworkSingleton<PlayerPanelController>.I.PlayerTransforms[index].GetComponent<PlayerController>();
                        break;
                    }
                    index-=1;
                
                }
            var namefieldRef = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_playerText");
            var nameinstance = MonoSingleton<UIManager>.I;
            var playname = MonoSingleton<DataManager>.I.PlayerData.Name;

            
            if (target!=null)
            {
                var tname = target.PlayerNameText.text;
                targetingString = "            -            " + tname + " selected";
            }
            else
            {
                targetingString = "";
            }
            if (Chainloader.PluginInfos.TryGetValue("officerballs.StatusManager", out var basicInfo))
            {
                StatusPatch._ifTeleportString = targetingString;
            }
            namefieldRef(nameinstance).text = playname + targetingString;
            //Debug.Log("player disconnected. " + tname + " selected.");
                
        }

    }
    [HarmonyPatch(typeof(PlayerMovementController))]
    public static class Teleporter
    {
        public static Vector3 warppos = Vector3.zero;

        [HarmonyPatch("MovePlayer")]
        [HarmonyPrefix]
        public static bool TeleSkip( ref CharacterController ____characterController, PlayerMovementController __instance)
        {
            if (warppos != Vector3.zero)
            {
                if( __instance != NetworkSingleton<TextChannelManager>.I.MainMovementController) return true;
                ____characterController.transform.position = warppos;
                warppos = Vector3.zero;
                return false;
            }
            return true;
        }

    }
    [HarmonyPatch(typeof(PlayerController))]
    public static class PCPatcher
    {
        [HarmonyPatch("OnDespawned")]
        [HarmonyPrefix]
        public static bool OverrideStationaryCheck( PlayerController __instance)
        {
            if(__instance.IsReturningMenu)
            {
                TeleportPatch.target = null;
                TeleportPatch.targetingString = "";
                if (Chainloader.PluginInfos.TryGetValue("officerballs.StatusManager", out var basicInfo))
                {
                    StatusPatch._ifTeleportString = TeleportPatch.targetingString;
                }
            }

            return true;
        }
    }
}
