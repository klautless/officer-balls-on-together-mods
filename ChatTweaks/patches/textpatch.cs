using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using PurrNet;
using System.Collections.Generic;

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
            string textdupe = text + "";
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
            tMP_Text.text = "<size=" + size + "><color=#" + color + "> " + stamp + userName + "</color>:" + cleaner[0] + "<color=#" + Plugin.configColorWrap.Value + "> " + textdupe;
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
            CommandAPI.Utilities.Notify("Available commands:");
            CommandAPI.Utilities.Notify("/mutelocal");
            CommandAPI.Utilities.Notify("/muteglobal");
            CommandAPI.Utilities.Notify("/muteduringtimer");
            CommandAPI.Utilities.Notify("/mutejoinleave");
            CommandAPI.Utilities.Notify("/usetimestamps");
            CommandAPI.Utilities.Notify("/disablechattags");

            CommandAPI.Utilities.Notify("/textsize <color=#a83131>x</color>");
            CommandAPI.Utilities.Notify("/textcolor <color=#9db143>123456</color>");
            CommandAPI.Utilities.Notify("/systemcolor <color=#9db143>123456</color>");
            CommandAPI.Utilities.Notify("/outlinecolor <color=#9db143>123456</color>");
            CommandAPI.Utilities.Notify("/outlinewidth <color=#a83131>x.xx</color>");
            CommandAPI.Utilities.Notify("/outlineopacity <color=#a83131>0-255</color>");
        }
        public static void ToggleChatTags( string[] args )
        {   Plugin.configCleanUpChat.Value = !Plugin.configCleanUpChat.Value;
            CommandAPI.Utilities.Notify("Chat tags turned " + OnOff(Plugin.configCleanUpChat.Value)); }
        public static void MuteLocal( string[] args)
        {   Plugin.configLocalNoises.Value = !Plugin.configLocalNoises.Value;
            CommandAPI.Utilities.Notify("Local message noises turned " + OnOff(Plugin.configLocalNoises.Value)); }
        public static void MuteGlobal( string[] args)
        {   Plugin.configGlobalNoises.Value = !Plugin.configGlobalNoises.Value;
            CommandAPI.Utilities.Notify("Global message noises turned " + OnOff(Plugin.configGlobalNoises.Value)); }
        public static void MuteDuringTimer( string[] args)
        {   Plugin.configMuteDuringFocus.Value = !Plugin.configMuteDuringFocus.Value;
            CommandAPI.Utilities.Notify("Mute during timer turned " + OnOff(Plugin.configMuteDuringFocus.Value)); }
        public static void MuteJoinLeave( string[] args)
        {   Plugin.configJoinLeaveNoises.Value = !Plugin.configJoinLeaveNoises.Value;
            CommandAPI.Utilities.Notify("Join/leave noises turned " + OnOff(Plugin.configJoinLeaveNoises.Value)); }
        public static void UseTimestamps( string[] args)
        {   Plugin.configUseTimeStamps.Value = !Plugin.configUseTimeStamps.Value;
            CommandAPI.Utilities.Notify("Timestamps turned " + OnOff(Plugin.configUseTimeStamps.Value)); }
        
        public static void TextColor( string[] args )
        {
            if( args.Length == 0 )
            {
                CommandAPI.Utilities.Notify("Text color currently set to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
                return;
            }
            if (args[0][0] == '#') args[0] = args[0].Substring(1);
            if ( ValidateHex(args[0]) && args[0].Length >= 6 )
            {
                Plugin.configColorWrap.Value = args[0].Substring(0, 6);
                CommandAPI.Utilities.Notify("Text color changed to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
            }
            else CommandAPI.Utilities.Notify("Invalid color code. Use hexadecimal (HTML) only.");
        }
        public static void SystemColor( string[] args )
        {
            if( args.Length == 0 )
            {
                CommandAPI.Utilities.Notify("System text color currently set to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
                return;
            }
            if (args[0][0] == '#') args[0] = args[0].Substring(1);
            if ( ValidateHex(args[0]) && args[0].Length >= 6 )
            {
                Plugin.configSystemColorWrap.Value = args[0].Substring(0, 6);
                CommandAPI.Utilities.Notify("System text color changed to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
            }
            else CommandAPI.Utilities.Notify("Invalid color code. Use hexadecimal (HTML) only.");
        }
        public static void OutlineColor( string[] args )
        {
            if( args.Length == 0 )
            {
                CommandAPI.Utilities.Notify("Outline color currently set to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
                return;
            }
            if (args[0][0] == '#') args[0] = args[0].Substring(1);
            if ( ValidateHex(args[0]) && args[0].Length >= 6 )
            {
                Plugin.configOutlineColor.Value = args[0].Substring(0, 6);
                CommandAPI.Utilities.Notify("Outline color changed to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
            }
            else CommandAPI.Utilities.Notify("Invalid color code. Use hexadecimal (HTML) only.");
        }
        public static void OutlineWidth( string[] args )
        {
            if( args.Length == 0)
            {
                CommandAPI.Utilities.Notify("Outline width currently set to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                return;
            }
            if (float.TryParse(args[0], out float output))
            {
                Plugin.configOutlineWidth.Value = output;
                CommandAPI.Utilities.Notify("Outline width changed to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                outlineChanger();
            }
            else CommandAPI.Utilities.Notify("Invalid input. Please try a float (example: 0.15)");
        }
        public static void OutlineOpacity( string[] args )
        {
            if( args.Length == 0 )
            {
                CommandAPI.Utilities.Notify("Outline opacity currently set to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                return;
            }
            if (int.TryParse(args[0], out int output))
            {
                if (output < 0 || output > 255)
                {
                    CommandAPI.Utilities.Notify("Invalid input. Please use an integer from 0 to 255.");
                    return;
                }
                Plugin.configOutlineOpacity.Value = output;
                CommandAPI.Utilities.Notify("Outline opacity changed to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                outlineChanger();
            }
            else CommandAPI.Utilities.Notify("Invalid input. Please use an integer from 0 to 255.");
        }
        public static void TextSize( string[] args )
        {
            if( args.Length == 0 )
            {
                CommandAPI.Utilities.Notify("Text size currently set to " + Plugin.configTextSize.Value.ToString() + ".");
                return;
            }
            if (int.TryParse(args[0], out int output))
            {
                if (output < 0 || output > 255)
                {
                    CommandAPI.Utilities.Notify("Invalid input. Please use an integer from 0 to 255.");
                    return;
                }
                Plugin.configTextSize.Value = output;
                CommandAPI.Utilities.Notify("Text size changed to " + Plugin.configTextSize.Value.ToString() + ".");
            }
            else CommandAPI.Utilities.Notify("Invalid input. Please use an integer from 0 to 255.");
        }

        public static void outlineChanger()
        {

            var fieldref = HarmonyLib.AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_messageTextForFont");
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
                var msgRef = HarmonyLib.AccessTools.FieldRefAccess<TextChannelManager, List<GameObject>>("_messageObjectsLocal");
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
                var msgRef = HarmonyLib.AccessTools.FieldRefAccess<TextChannelManager, List<GameObject>>("_messageObjectsGlobal");
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
        public static string OnOff(bool value) { return value ? "on." : "off."; }
        public static bool ValidateHex( string input)
        {   string pattern = @"^[0-9a-fA-F]+$";
            return Regex.IsMatch(input, pattern); }
    }
}
