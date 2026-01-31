using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using PurrNet;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace ChatTweaks.patches
{
    
    [HarmonyPatch(typeof(TextChannelManager))]
    
    public class TextPatcher
    {
        [HarmonyPatch("AddMessageUI")]
        [HarmonyPrefix]
        public static bool MsgUIFix( string userName, string text, bool isLocal, int senderIndex, ref List<GameObject> ____messageObjectsLocal, ref List<GameObject> ____messageObjectsGlobal, ref TMP_Text ____textPrefab )
        {
            TMP_Text tMP_Text = UnityProxy.Instantiate( ____textPrefab, isLocal ? MonoSingleton<UIManager>.I.TextContentLocalTransform : MonoSingleton<UIManager>.I.TextContentGlobalTransform);
            var stamp = Plugin.configUseTimeStamps.Value ? "[" + DateTime.Now.ToString("h:mm tt") + "]" : "";
            if (Plugin.configMsgNoises.Value )
            {
		        MonoSingleton<SFXManager>.I.PlayRodAppear();
            }
            string size = Plugin.configTextSize.Value.ToString();
            tMP_Text.text = "<size=" + size + ">" + stamp + " <color=#" + ScriptableSingleton<GameSettings>.I.MessageOthersColors[senderIndex] + "ff>" + userName + ":</color><color=#" + Plugin.configColorWrap.Value + "> " + text;
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
                MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                EventSystem.current.SetSelectedGameObject(null);
                NetworkSingleton<TextChannelManager>.I.AddNotification("/help chat for ChatTweaks commands");
                return true;
            }
            if (text.ToLower() == "/help chat" || text.ToLower() == "/help chattweaks")
            {
                NetworkSingleton<TextChannelManager>.I.AddNotification("Available commands:");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/mutetext");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/mutejoinleave");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/usetimestamps");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/textsize <color=#a83131>x</color>");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/textcolor <color=#9db143>123456</color>");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/systemcolor <color=#9db143>123456</color>");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/outlinecolor <color=#9db143>123456</color>");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/outlinewidth <color=#a83131>x.xx</color>");
                NetworkSingleton<TextChannelManager>.I.AddNotification("/outlineopacity <color=#a83131>0-255</color>");

                MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                EventSystem.current.SetSelectedGameObject(null);
                MonoSingleton<UIManager>.I.MessageInput.text = "";
                return false;
            }
            if (text.ToLower() == "/mutetext")
            {
                Plugin.configMsgNoises.Value = !Plugin.configMsgNoises.Value;
                string isMuted = Plugin.configMsgNoises.Value ? "on" : "off";
                NetworkSingleton<TextChannelManager>.I.AddNotification("Message noises turned " + isMuted);
                MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                EventSystem.current.SetSelectedGameObject(null);
                MonoSingleton<UIManager>.I.MessageInput.text = "";
                return false;
            }
            if (text.ToLower() == "/mutejoinleave")
            {
                Plugin.configJoinLeaveNoises.Value = !Plugin.configJoinLeaveNoises.Value;
                string isMuted = Plugin.configJoinLeaveNoises.Value ? "on" : "off";
                NetworkSingleton<TextChannelManager>.I.AddNotification("Join/leave noises turned " + isMuted);
                MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                EventSystem.current.SetSelectedGameObject(null);
                MonoSingleton<UIManager>.I.MessageInput.text = "";
                return false;
            }
            if (text.ToLower() == "/usetimestamps")
            {
                Plugin.configUseTimeStamps.Value = !Plugin.configUseTimeStamps.Value;
                string isMuted = Plugin.configUseTimeStamps.Value ? "on" : "off";
                NetworkSingleton<TextChannelManager>.I.AddNotification("Timestamps turned " + isMuted);
                MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                EventSystem.current.SetSelectedGameObject(null);
                MonoSingleton<UIManager>.I.MessageInput.text = "";
                return false;
            }
            if (text.Length >= 10) // /textcolor
            {
                if (text.Substring(0,10) == "/textcolor")
                {
                    if (text.Length >= 17)
                    {
                        Plugin.configColorWrap.Value = text.Substring(11, 6);
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Text color changed to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Text color currently set to <color=#" + Plugin.configColorWrap.Value + ">" + Plugin.configColorWrap.Value + "</color>.");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
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
                        NetworkSingleton<TextChannelManager>.I.AddNotification("System text color changed to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("System text color currently set to <color=#" + Plugin.configSystemColorWrap.Value + ">" + Plugin.configSystemColorWrap.Value + "</color>.");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
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
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Outline opacity changed to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                            outlineChanger();
                        }
                        else
                        {
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Outline opacity currently set to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                        }
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Outline opacity currently set to " + Plugin.configOutlineOpacity.Value.ToString() + ".");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
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
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Outline color changed to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
                        outlineChanger();
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Outline color currently set to <color=#" + Plugin.configOutlineColor.Value + ">" + Plugin.configOutlineColor.Value + "</color>.");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
                    return false;
                }
                else if (text.Substring(0,13) == "/outlinewidth")
                {
                    if (text.Length >= 15)
                    {
                        if (float.TryParse(text.Substring(14), out float output))
                        {
                            Plugin.configOutlineWidth.Value = output;
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Outline width changed to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                            outlineChanger();
                        }
                        else
                        {
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Outline width currently set to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                        }
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Outline width currently set to " + Plugin.configOutlineWidth.Value.ToString() + ".");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
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
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Text size changed to " + Plugin.configTextSize.Value.ToString() + ".");
                
                        }
                        else
                        {
                            NetworkSingleton<TextChannelManager>.I.AddNotification("Text size currently set to " + Plugin.configTextSize.Value.ToString() + ".");
                        }
                    }
                    else
                    {
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Text size currently set to " + Plugin.configTextSize.Value.ToString() + ".");
                    }
                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch("SendMessageAsync")]
        [HarmonyPrefix]
        public static bool HelpCloser(byte[] textBytes)
        {
            string text = Encoding.Unicode.GetString(textBytes);
            if(text == "/help")
            {
                return false;
            }
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
