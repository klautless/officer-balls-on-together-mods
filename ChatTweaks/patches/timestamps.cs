using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

using PurrNet;
using System.Collections.Generic;

namespace ChatTweaks.patches
{
    
    [HarmonyPatch(typeof(TextChannelManager))]
    
    public class TimeStampsPatch
    {
        [HarmonyPatch("AddMessageUI")]
        [HarmonyPrefix]
        public static bool MsgUIFix( string userName, string text, bool isLocal, int senderIndex, ref List<GameObject> ____messageObjectsLocal, ref List<GameObject> ____messageObjectsGlobal, ref TMP_Text ____textPrefab )
        {
            TMP_Text tMP_Text = UnityProxy.Instantiate( ____textPrefab, isLocal ? MonoSingleton<UIManager>.I.TextContentLocalTransform : MonoSingleton<UIManager>.I.TextContentGlobalTransform);
            var stamp = DateTime.Now.ToString("h:mm tt");
            if (Plugin.configMsgNoises.Value )
            {
		        MonoSingleton<SFXManager>.I.PlayRodAppear();
            }
            string size = Plugin.configTextSize.Value.ToString();
            tMP_Text.text = "<size=" + size + ">[" + stamp + "] <color=#" + ScriptableSingleton<GameSettings>.I.MessageOthersColors[senderIndex] + "ff>" + userName + ":</color> " + text;
            if (isLocal)
            {
                ____messageObjectsLocal.Add(tMP_Text.gameObject);
                if (____messageObjectsLocal.Count > ScriptableSingleton<GameSettings>.I.LocalMessageLimitCount)
                {
                    GameObject obj = ____messageObjectsLocal[0];
                    ____messageObjectsLocal.RemoveAt(0);
                    UnityProxy.Destroy(obj);
                }
            }
            else
            {
                ____messageObjectsGlobal.Add(tMP_Text.gameObject);
                if (____messageObjectsGlobal.Count > ScriptableSingleton<GameSettings>.I.GlobalMessageLimitCount)
                {
                    GameObject obj2 = ____messageObjectsGlobal[0];
                    ____messageObjectsGlobal.RemoveAt(0);
                    UnityProxy.Destroy(obj2);
                }
            }
            return false;

        }

    }
}
