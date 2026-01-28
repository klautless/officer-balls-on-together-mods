// Credit to 岚风 雷 / Arashi_Lei (https://github.com/gqxastg) for sorting out all the oversized lobby issues!
using HarmonyLib;
using UnityEngine;
using PurrNet.Packing;
using PurrNet;
using PurrNet.Modules;
using PurrNet.Transports;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.UI;

namespace PlayerLimitLift.patches
{

    public static class SSS
    {
        public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom) where T : Delegate
        {
            IntPtr ptr = method.MethodHandle.GetFunctionPointer();
            return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
        }
    }
    [HarmonyPatch(typeof(TextChannelManager))]
    public class TextChannelManagerPatch
    {
        //public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null);
        
        [HarmonyPatch(typeof(TextChannelManager), nameof(TextChannelManager.SendMessageAsync_Original_0))]
        [HarmonyPrefix]
        public static bool SendMessagePatcher(TextChannelManager __instance,
        ref byte[] textBytes, ref byte[] userName, ref bool isLocal,
        ref Vector3 pos, ref string playerID, ref RPCInfo info,
        ref string ____playerId)
        {
            string @string = Encoding.Unicode.GetString(textBytes);
            string string2 = Encoding.Unicode.GetString(userName);
            int num = 0;
            PlayerPanelController i = NetworkSingleton<PlayerPanelController>.I;
            for (int j = 0; j < i.PlayerIDs.Count; j++)
            {
                if (info.sender == i.PlayerIDs[j])
                {
                    num = j;
                    break;
                }
            }
            if (num > 15) num = 15;
            if (!(info.sender == __instance.localPlayer))
            {
                var method = AccessTools.Method(typeof(TextChannelManager),"OnChannelMessageReceived");
                method.Invoke(__instance, new object[] {string2, @string, pos, isLocal, num, playerID});
                return false;
            }
            if (playerID != ____playerId)
            {
                var method = AccessTools.Method(typeof(TextChannelManager),"AddMessageUI");
                method.Invoke(__instance, new object[] {string2, @string, isLocal, num});
                int num2 = -1;
                for (int k = 0; k < i.PlayerSteamIDs.Count; k++)
                {
                    if (i.PlayerSteamIDs[k] == playerID)
                    {
                        num2 = k;
                        break;
                    }
                }
                if (num2 != -1 && num2 < i.PlayerTransforms.Count)
                {
                    NetworkTransform networkTransform = i.PlayerTransforms[num2];
                    if (networkTransform != null)
                    {
                        TextBubbleController componentInChildren = networkTransform.GetComponentInChildren<TextBubbleController>();
                        if (componentInChildren != null)
                        {
                            componentInChildren.ShowTextBubble(textBytes, isLocal, pos, playerID);
                        }
                    }
                }
                return false;
            }
            var methoder = AccessTools.Method(typeof(TextChannelManager),"AddMessageUI");
            methoder.Invoke(__instance, new object[] {string2, @string, isLocal, num});
            __instance.MainTextBubble.ShowTextBubble(textBytes, isLocal, pos, playerID);
                
            return false;
        }


        [HarmonyPatch(typeof(TextChannelManager), nameof(TextChannelManager.OnReceivedRpc))]
        [HarmonyPrefix]
        public static bool RPCInterceptPatcher(ref int id, ref RPCInfo info, ref RPCPacket packet,
        ref BitPacker stream, ref bool asServer, TextChannelManager __instance)
        {
            if (id==0 && asServer)
            {
                int num = -1;
                List<PlayerID> playerIDs = NetworkSingleton<PlayerPanelController>.I.PlayerIDs;
                for (int i = 0; i < playerIDs.Count; i++)
                {
                    if (info.sender == playerIDs[i])
                    {
                        num = i;
                        break;
                    }
                }
                if(num > 15)
                {
                    byte[] array = null;
                    Packer<byte[]>.Read(stream, ref array);
                    byte[] array2 = null;
                    Packer<byte[]>.Read(stream, ref array2);
                    bool flag = false;
                    Packer<bool>.Read(stream, ref flag);
                    Vector3 zero = Vector3.zero;
                    Packer<Vector3>.Read(stream, ref zero);
                    string text = null;
                    Packer<string>.Read(stream, ref text);
                    __instance.SendMessageAsync(array, array2, flag, zero, text, default(RPCInfo));
                    return false;
                }
            }
            if (id == 0)
            {
                __instance.HandleRPCGenerated_0(stream, packet, info, asServer);
                return false;
            }
            MethodInfo Base = AccessTools.DeclaredMethod(typeof(TextChannelManager), "OnReceivedRpc");
            Base.GetMethodWithoutOverrides
                <Action<int, BitPacker, RPCPacket, RPCInfo, bool>>
                (__instance).Invoke(id, stream, packet, info, asServer);
            
            return false;
	    }

    }
}
