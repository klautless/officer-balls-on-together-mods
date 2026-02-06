using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using PurrNet;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Windows;

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
                        if (closingFound) break; if (user_chars[ci] == '>') {
                            if (!skipAndColorize && !markAsCloser) {
                                string checkTagBase = userName.Substring(c,(ci-c)+1); string checkTag = "";
                                char[] ctag = checkTagBase.ToCharArray();
                                for (var ch = 0; ch < ctag.Length; ch++)
                                    { if (ctag[ch] == '=') { checkTag+=">"; break; } checkTag+=checkTagBase[ch]; }
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
                    { if (closingFound) break; if (text[ci] == '>') { isTag = true; skipto = ci; break; } } }
                    if (isTag)
                        { c = skipto; continue; }
                } output[1]+=text[c]; }
            return output;
        }

        [HarmonyPatch("AddMessageUI")]
        [HarmonyPrefix]
        public static bool MsgUIFix( string userName, string text, bool isLocal, int senderIndex, ref List<GameObject> ____messageObjectsLocal, ref List<GameObject> ____messageObjectsGlobal, ref TMP_Text ____textPrefab )
        {
            TMP_Text tMP_Text = UnityProxy.Instantiate( ____textPrefab, isLocal ? MonoSingleton<UIManager>.I.TextContentLocalTransform : MonoSingleton<UIManager>.I.TextContentGlobalTransform);
            var stamp = Plugin.configUseTimeStamps.Value ? "[" + DateTime.Now.ToString("h:mm tt") + "]" : "";
            if ((Plugin.configLocalNoises.Value && isLocal) || Plugin.configGlobalNoises.Value )
            {
                if ( ( Plugin.configMuteDuringFocus.Value && ( MonoSingleton<PomodoroController>.I.PomodoroType != PomodoroType.Study || MonoSingleton<PomodoroController>.I.IsPaused) ) || !Plugin.configMuteDuringFocus.Value )
		        { MonoSingleton<SFXManager>.I.PlayRodAppear(); }
            }
            string[] cleaner = ["",""];
            if (Plugin.configCleanUpChat.Value)
            {
                cleaner = CleanText( userName, text );
                text = cleaner[1];
            }
            string size = Plugin.configTextSize.Value.ToString();
            tMP_Text.text = "<size=" + size + ">" + stamp + " <color=#" + ScriptableSingleton<GameSettings>.I.MessageOthersColors[senderIndex] + "ff>" + userName + "</color>:" + cleaner[0] + "<color=#" + Plugin.configColorWrap.Value + "> " + text;
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

        [HarmonyPatch("OnEnterPressed")]
        [HarmonyPrefix]
        public static bool TextChecker()
        {    
            string text = MonoSingleton<UIManager>.I.MessageInput.text;
            if (text == "/help")
            {
                FinishCmds(false);
                Notify("/help chat for ChatTweaks commands");
                return true;
            }
            if (text.ToLower() == "/help chat" || text.ToLower() == "/help chattweaks")
            {
                Notify("Available commands:");
                Notify("/mutelocal");
                Notify("/muteglobal");
                Notify("/muteduringtimer");
                Notify("/mutejoinleave");
                Notify("/usetimestamps");
                Notify("/disablechattags");

                Notify("/textsize <color=#a83131>x</color>");
                Notify("/textcolor <color=#9db143>123456</color>");
                Notify("/systemcolor <color=#9db143>123456</color>");
                Notify("/outlinecolor <color=#9db143>123456</color>");
                Notify("/outlinewidth <color=#a83131>x.xx</color>");
                Notify("/outlineopacity <color=#a83131>0-255</color>");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/disablechattags")
            {
                Plugin.configCleanUpChat.Value = !Plugin.configCleanUpChat.Value;
                string isMuted = Plugin.configCleanUpChat.Value ? "off" : "on";
                Notify("Chat tags turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/mutelocal")
            {
                Plugin.configLocalNoises.Value = !Plugin.configLocalNoises.Value;
                string isMuted = Plugin.configLocalNoises.Value ? "on" : "off";
                Notify("Local message noises turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/muteduringtimer")
            {
                Plugin.configMuteDuringFocus.Value = !Plugin.configMuteDuringFocus.Value;
                string isMuted = Plugin.configMuteDuringFocus.Value ? "on" : "off";
                Notify("Mute during timer turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/muteglobal")
            {
                Plugin.configGlobalNoises.Value = !Plugin.configGlobalNoises.Value;
                string isMuted = Plugin.configGlobalNoises.Value ? "on" : "off";
                Notify("Global message noises turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/mutejoinleave")
            {
                Plugin.configJoinLeaveNoises.Value = !Plugin.configJoinLeaveNoises.Value;
                string isMuted = Plugin.configJoinLeaveNoises.Value ? "on" : "off";
                Notify("Join/leave noises turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/usetimestamps")
            {
                Plugin.configUseTimeStamps.Value = !Plugin.configUseTimeStamps.Value;
                string isMuted = Plugin.configUseTimeStamps.Value ? "on" : "off";
                Notify("Timestamps turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.Length >= 10) // /textcolor
            {
                if (text.Substring(0,10) == "/textcolor")
                {
                    if (text.Length >= 17)
                    {
                        Plugin.configColorWrap.Value = text.Substring(11, 6);
                        Notify("Text color changed to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
                    }
                    else Notify("Text color currently set to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
                    FinishCmds();
                    return false;
                }
            }
            if (text.Length >= 12) // /systemcolor
            {
                if (text.Substring(0,12) == "/systemcolor")
                {
                    if (text.Length >= 19)
                    {
                        Plugin.configSystemColorWrap.Value = text.Substring(13, 6);
                        Notify("System text color changed to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
                    }
                    else Notify("System text color currently set to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
                    FinishCmds();
                    return false;
                }
            }
            //outlinewidth, outlineopacity
            if (text.Length >= 15) // /outlineopacity
            {
                if (text.Substring(0, 15) == "/outlineopacity")
                {
                    if (text.Length >= 17)
                    {
                        if (int.TryParse(text.Substring(16), out int output))
                        {
                            if (output < 0) output = 0;
                            else if (output > 255) output = 255;
                            Plugin.configOutlineOpacity.Value = output;
                            Notify("Outline opacity changed to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                            outlineChanger();
                        }
                        else Notify("Outline opacity currently set to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                    }
                    else Notify("Outline opacity currently set to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                    FinishCmds();
                    return false;
                }
            }
            if (text.Length >= 13) // /outlinecolor, /outlinewidth
            {
                if (text.Substring(0,13) == "/outlinecolor")
                {
                    if (text.Length >= 20)
                    {
                        Plugin.configOutlineColor.Value = text.Substring(14, 6);
                        Notify("Outline color changed to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
                        outlineChanger();
                    }
                    else Notify("Outline color currently set to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
                    FinishCmds();
                    return false;
                }
                else if (text.Substring(0,13) == "/outlinewidth")
                {
                    if (text.Length >= 15)
                    {
                        if (float.TryParse(text.Substring(14), out float output))
                        {
                            Plugin.configOutlineWidth.Value = output;
                            Notify("Outline width changed to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                            outlineChanger();
                        }
                        else Notify("Outline width currently set to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                    }
                    else Notify("Outline width currently set to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                    FinishCmds();
                    return false;
                }
            }
            if (text.Length >= 9) // /textsize
            {
                if (text.Substring(0,9) == "/textsize")
                {
                    if (text.Length > 10)
                    {
                        if(int.TryParse(text.Substring(10), out int newsize))
                        {
                            Plugin.configTextSize.Value = newsize;
                            Notify("Text size changed to " + Plugin.configTextSize.Value.ToString() + ".");
                        }
                        else Notify("Text size currently set to " + Plugin.configTextSize.Value.ToString() + ".");
                    }
                    else Notify("Text size currently set to " + Plugin.configTextSize.Value.ToString() + ".");
                    FinishCmds();
                    return false;
                }
            }
            return true;
        }

        public static void FinishCmds(bool clearText=true)
        {
            MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
            EventSystem.current.SetSelectedGameObject(null);
            if (clearText) MonoSingleton<UIManager>.I.MessageInput.text = "";
        }
        public static void Notify(string text) { NetworkSingleton<TextChannelManager>.I.AddNotification(text); }

        [HarmonyPatch("SendMessageAsync")]
        [HarmonyPrefix]
        public static bool HelpCloser(byte[] textBytes)
        {
            string text = Encoding.Unicode.GetString(textBytes);
            if(text == "/help") return false;
            return true;
        }

        public static void outlineChanger()
        {

            var fieldref = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_messageTextForFont");
            var instance = MonoSingleton<UIManager>.I;
            
            Material mat = fieldref(instance).fontSharedMaterial;

            string colorbase = Plugin.configOutlineColor.Value;
            byte r = Convert.ToByte(colorbase.Substring(0,2), 16);
            byte g = Convert.ToByte(colorbase.Substring(2,2), 16);
            byte b = Convert.ToByte(colorbase.Substring(4,2), 16);

            mat.EnableKeyword("OUTLINE_ON");
            mat.SetFloat("_OutlineWidth", Plugin.configOutlineWidth.Value);
            mat.SetColor("_OutlineColor", new Color32(r, g, b, Convert.ToByte(Plugin.configOutlineOpacity.Value)));
            fieldref(instance).UpdateMeshPadding();

            var textinstance = NetworkSingleton<TextChannelManager>.I;

            if (NetworkSingleton<TextChannelManager>.I.Islocal)
            {
                var msgRef = AccessTools.FieldRefAccess<TextChannelManager, List<GameObject>>("_messageObjectsLocal");
                foreach (GameObject item in msgRef(textinstance))
                {
                    TMP_Text temp = item.GetComponent<TMP_Text>();
                    Material temp_mat = temp.fontSharedMaterial;
                    temp_mat.EnableKeyword("OUTLINE_ON");
                    temp_mat.SetFloat("_OutlineWidth", Plugin.configOutlineWidth.Value);
                    temp_mat.SetColor("_OutlineColor", new Color32(r, g, b, Convert.ToByte(Plugin.configOutlineOpacity.Value)));
                    temp.UpdateMeshPadding();
                    temp.ForceMeshUpdate();
                    temp.enabled = false;
                    temp.enabled = true;
                }
            }
            else
            {
                var msgRef = AccessTools.FieldRefAccess<TextChannelManager, List<GameObject>>("_messageObjectsGlobal");
                foreach (GameObject item in msgRef(textinstance))
                {
                    TMP_Text temp = item.GetComponent<TMP_Text>();
                    Material temp_mat = temp.fontSharedMaterial;
                    temp_mat.EnableKeyword("OUTLINE_ON");
                    temp_mat.SetFloat("_OutlineWidth", Plugin.configOutlineWidth.Value);
                    temp_mat.SetColor("_OutlineColor", new Color32(r, g, b, Convert.ToByte(Plugin.configOutlineOpacity.Value)));
                    temp.UpdateMeshPadding();
                    temp.ForceMeshUpdate();
                    temp.enabled = false;
                    temp.enabled = true;
                }
            }
        }
    }
}
