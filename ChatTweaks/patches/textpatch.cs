using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using PurrNet;
using System.Collections.Generic;
using _otAPI;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(TextChannelManager))]
    
    public class TextPatcher
    {
        public static string[] CleanText( string userName, string text )
        {
            string[] output = ["",""];
            char[] user_chars = userName.ToCharArray();
            for (var c = 0; c < user_chars.Length; c++) { if (user_chars[c] == '<') {
                bool closingFound = false; bool isTag = false; int skipto = 0;
                if (user_chars.Length > c) {
                    bool skipAndColorize = false; bool markAsCloser = false;
                    if (user_chars[c+1] == '#') skipAndColorize = true;
                    if (user_chars[c+1] == '/') markAsCloser = true;
                    for (var ci = c+1; ci < user_chars.Length; ci++) {
                        if (closingFound || user_chars[ci] == ' ') break; if (user_chars[ci] == '>') {
                            if (!skipAndColorize && !markAsCloser) {
                                string checkTagBase = userName.Substring(c,(ci-c)+1); string checkTag = "";
                                char[] ctag = checkTagBase.ToCharArray();
                                for (var ch = 0; ch < ctag.Length; ch++)
                                    { if (ctag[ch] == '=') { checkTag+=">"; break; } checkTag+=checkTagBase[ch]; }
                                if(checkTag == "<space>" || checkTag == "<page>") { skipto = ci; continue; }
                                else if(checkTag.Length > 6 ){ if (checkTag.Substring(0,7) == "<sprite") { skipto = ci; continue; } }
                                checkTag = checkTag.Insert(1,"/");
                                if (userName.Substring(ci).Contains(checkTag)) closingFound = true;
                                if (closingFound) break;
                                isTag = true; skipto = ci; output[0]+=checkTag; break;
                                }
                                else if (skipAndColorize)
                                    { isTag = true; skipto = ci; break; }
                                else if (markAsCloser)
                                    { isTag = true; skipto = ci; break; }
                    } } }
                    if (isTag)
                        { c = skipto; continue; }
            } }
            for (var c = 0; c < text.Length; c++) { if (text[c] == '<') {
                bool closingFound = false; bool isTag = false; int skipto = 0;
                if (text.Length > c) { for (var ci = c+1; ci < text.Length; ci++)
                    { if (closingFound || text[ci] == ' ') break; if (text[ci] == '>') { isTag = true; skipto = ci; break; } } }
                    if (isTag)
                        { c = skipto; continue; }
                } output[1]+=text[c]; }
            return output;
        }

        [HarmonyPatch("AddMessageUI")]
        [HarmonyPrefix]
        public static bool MsgUIFix( string userName, string text, bool isLocal, int senderIndex, ref List<GameObject> ____messageObjectsLocal, ref List<GameObject> ____messageObjectsGlobal, ref TMP_Text ____textPrefab )
        {
            string textdupe = text + "";
            TMP_Text tMP_Text = UnityProxy.Instantiate( ____textPrefab, isLocal ? MonoSingleton<UIManager>.I.TextContentLocalTransform : MonoSingleton<UIManager>.I.TextContentGlobalTransform);
            var stamp = Plugin.configUseTimeStamps.Value ? "[" + DateTime.Now.ToString("h:mm tt") + "] " : "";
            if ((Plugin.configLocalNoises.Value && isLocal) || Plugin.configGlobalNoises.Value )
            {
                if ( ( Plugin.configMuteDuringFocus.Value && ( MonoSingleton<PomodoroController>.I.PomodoroType != PomodoroType.Study || MonoSingleton<PomodoroController>.I.IsPaused) ) || !Plugin.configMuteDuringFocus.Value )
		        { MonoSingleton<SFXManager>.I.PlayRodAppear(); }
            }
            string[] cleaner = ["",""];
            if (Plugin.configCleanUpChat.Value)
            {
                cleaner = CleanText( userName, text );
                textdupe = cleaner[1];
            }
            var messageColors = ScriptableSingleton<GameSettings>.I.MessageOthersColors;
            string size = Plugin.configTextSize.Value.ToString();
            string color = "";
            if (senderIndex >= 0 && senderIndex < messageColors.Count)
            {
                if (ColorUtility.TryParseHtmlString(messageColors[senderIndex], out Color newcolor))
                {
                    color = ScriptableSingleton<GameSettings>.I.MessageOthersColors[senderIndex];
                }
                else color = "ffffff";
            }
            tMP_Text.text = "<size=" + size + "><color=#" + color + ">" + stamp + userName + "</color>:" + cleaner[0] + "<color=#" + Plugin.configColorWrap.Value + "> " + textdupe;
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
        [HarmonyPatch("AddNotification")]
        [HarmonyPrefix]
        public static void NotifTextPatch( ref string text )
        {
            string size = Plugin.configTextSize.Value.ToString();
            text = "<size=" + size + "><color=#" + Plugin.configSystemColorWrap.Value + ">" + text;
        }

        public static void ShowCommands( string[] args )
        {
            otAPI.Notify("Available commands:");
            otAPI.Notify("/togglelocalnoise");
            otAPI.Notify("/toggleglobalnoise");
            otAPI.Notify("/toggletimermute");
            otAPI.Notify("/mutejoinleave");
            otAPI.Notify("/usetimestamps");
            otAPI.Notify("/togglechattags");

            /*otAPI.Notify($"/textsize X");
            otAPI.Notify($"/textcolor 123456");
            otAPI.Notify($"/systemcolor " + otAPI.colors["color"] + "123456</color>");
            otAPI.Notify($"/outlinecolor " + otAPI.colors["color"] + "123456</color>");
            otAPI.Notify($"/outlinewidth " + otAPI.colors["float"] + "x.xx</color>");
            otAPI.Notify($"/outlineopacity " + otAPI.colors["int"] + "0</color>-" + otAPI.colors["int"] + "255</color>");
            */
            
        }
    }
}