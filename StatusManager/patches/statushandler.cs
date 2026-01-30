using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StatusMessage.patches
{
    [HarmonyPatch(typeof(TextChannelManager))]
    internal class StatusPatch
    {
        public static string statusmsg = "";
        public static float awaytimer = 0f;
        public static bool isAFK = false;
        public static bool isBRB = false;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void TimeChecker()
        {
            if (Input.anyKey)
            {
                bool changed = false;
                awaytimer = 0f;
                if (isBRB)
                {
                    isBRB = false;
                    changed = true;
                }
                if (isAFK)
                {
                    isAFK = false;
                    changed = true;
                }
                if (changed)
                {
                    string namebase = Plugin.configNameBase.Value;

                    MonoSingleton<DataManager>.I.PlayerData.Name = namebase;
                    if (MonoSingleton<MainSceneManager>.I != null)
                    {
                        NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                        string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                    }
                    
                }
            }
            else
            {
                awaytimer += Time.deltaTime;
            }
            if(awaytimer >= Plugin.configAFKTimer.Value * 60 && statusmsg == "" && !isAFK && Plugin.configUseAFK.Value)
            {
                isAFK = true;
                string namebase = Plugin.configNameBase.Value;
                string pre = Plugin.configBracketType.Value.Substring(0,1);
                string post = Plugin.configBracketType.Value.Substring(1);
                string afkmsg = Plugin.configAFKMessage.Value;
                string color = Plugin.configAFKColor.Value;

                MonoSingleton<DataManager>.I.PlayerData.Name = namebase + " <color=#" + color + ">" + pre + afkmsg + post + "</color>";
                if (MonoSingleton<MainSceneManager>.I != null)
                {
                    NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                    string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                }
                
                
            }
            else if(awaytimer >= Plugin.configBRBTimer.Value * 60 && statusmsg == "" && !isBRB && Plugin.configUseBRB.Value)
            {
                isBRB = true;
                string namebase = Plugin.configNameBase.Value;
                string pre = Plugin.configBracketType.Value.Substring(0,1);
                string post = Plugin.configBracketType.Value.Substring(1);
                string brbmsg = Plugin.configBRBMessage.Value;
                string color = Plugin.configBRBColor.Value;

                MonoSingleton<DataManager>.I.PlayerData.Name = namebase + " <color=#" + color + ">" + pre + brbmsg + post + "</color>";
                if (MonoSingleton<MainSceneManager>.I != null)
                {
                    NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                    string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                }
                
            }
        }
        
        [HarmonyPatch("OnEnterPressed")]
        [HarmonyPrefix]
        public static bool TextChecker()
        {    
            string text = MonoSingleton<UIManager>.I.MessageInput.text;
            if (text.Length >= 18) // /changebrackets @ 15
            {
                if (text.Substring(0,15) == "/changebrackets")
                {
                    Plugin.configBracketType.Value = text.Substring(16,2);
                    if (statusmsg != "")
                    {
                        string namebase = Plugin.configNameBase.Value;
                        string pre = Plugin.configBracketType.Value.Substring(0,1);
                        string post = Plugin.configBracketType.Value.Substring(1);
                        string color = Plugin.configCustomColor.Value;

                        MonoSingleton<DataManager>.I.PlayerData.Name = namebase + " <color=#" + color + ">" + pre + statusmsg + post + "</color>";
                        if (MonoSingleton<MainSceneManager>.I != null)
                        {
                            NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                            string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                        }
                    }

                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    
                    MonoSingleton<UIManager>.I.MessageInput.text = "";

                    NetworkSingleton<TextChannelManager>.I.AddNotification("Brackets changed to " + Plugin.configBracketType.Value);
                    return false;

                    
                }
            }
            if (text.Length >= 12) // /clearstatus, /statuscolor
            {
                string commandcheck = text.Substring(0,12);
                switch (commandcheck)
                {
                    case "/clearstatus":
                        statusmsg = "";
                        MonoSingleton<DataManager>.I.PlayerData.Name = Plugin.configNameBase.Value;
                        if (MonoSingleton<MainSceneManager>.I != null)
                        {
                            NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                            string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                        }

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
		                EventSystem.current.SetSelectedGameObject(null);
                        
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Status message cleared.");
                    
                        return false;
                    case "/statuscolor":
                        if (text.Length >= 19)
                        {
                            string color = text.Substring(13,6);
                            Plugin.configCustomColor.Value = color;
                            if (statusmsg != "")
                            {
                                string namebase = Plugin.configNameBase.Value;
                                string pre = Plugin.configBracketType.Value.Substring(0,1);
                                string post = Plugin.configBracketType.Value.Substring(1);

                                MonoSingleton<DataManager>.I.PlayerData.Name = namebase + " <color=#" + color + ">" + pre + statusmsg + post + "</color>";
                                if (MonoSingleton<MainSceneManager>.I != null)
                                {
                                    NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                                    string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                                }
                            }
                            
                        }
                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
		                EventSystem.current.SetSelectedGameObject(null);
		
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Status color changed to <color=#" + Plugin.configCustomColor.Value + ">" + Plugin.configCustomColor.Value + "</color>.");
                    
                        return false;

                }
            }
            if (text.Length >= 16) // /brbcolor /afkcolor
            {
                string commandcheck = text.Substring(0,9);
                string color = "";
                switch (commandcheck)
                {
                    case "/brbcolor":
                        color = text.Substring(10,6);
                        Plugin.configBRBColor.Value = color;

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
		                EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("BRB color changed to <color=#" + Plugin.configBRBColor.Value + ">" + Plugin.configBRBColor.Value + "</color>.");
                        return false;
                    case "/afkcolor":
                        color = text.Substring(10,6);
                        Plugin.configAFKColor.Value = color;

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
		                EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("AFK color changed to <color=#" + Plugin.configAFKColor.Value + ">" + Plugin.configAFKColor.Value + "</color>.");
                        return false;
                        
                }
            }
            if (text.Length > 9) // /afktimer, /brbtimer
            {
                string commandcheck = text.Substring(0,9);
                switch (commandcheck)
                {
                    case "/brbtimer":
                        if(int.TryParse(text.Substring(10), out int brbnum))
                        {
                            Plugin.configBRBTimer.Value = brbnum;
                            NetworkSingleton<TextChannelManager>.I.AddNotification("BRB timer changed to " + Plugin.configBRBTimer.Value.ToString() + ".");
                        }
                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        return false;
                    case "/afktimer":
                        if(int.TryParse(text.Substring(10), out int afknum))
                        {
                            Plugin.configAFKTimer.Value = afknum;
                            NetworkSingleton<TextChannelManager>.I.AddNotification("AFK timer changed to " + Plugin.configAFKTimer.Value.ToString() + ".");
                        }
                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        return false;
                }
            }
            if (text.Length > 8) // /setname
            {
                if (text.Substring(0,8) == "/setname")
                {
                    string newname = text.Substring(9);
                    Plugin.configNameBase.Value = newname;
                    MonoSingleton<DataManager>.I.PlayerData.Name = newname;
                    if (MonoSingleton<MainSceneManager>.I != null)
                    {
                        NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                        string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                    }

                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
		            MonoSingleton<UIManager>.I.MessageInput.text = "";
                    NetworkSingleton<TextChannelManager>.I.AddNotification("Name changed to " + Plugin.configNameBase.Value + ".");
                    return false;
                }
            }
            if (text.Length > 7) // /status, /afkmsg, /brbmsg
            {
                string commandcheck = text.Substring(0,7);
                switch (commandcheck)
                {
                    case "/brbmsg":
                        Plugin.configBRBMessage.Value = text.Substring(8);

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("BRB message changed to " + Plugin.configBRBMessage.Value + ".");
                        return false;
                    case "/afkmsg":
                        Plugin.configAFKMessage.Value = text.Substring(8);

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("AFK message changed to " + Plugin.configAFKMessage.Value + ".");
                        return false;
                    case "/status":
                        statusmsg = text.Substring(8);
                        string namebase = Plugin.configNameBase.Value;
                        string pre = Plugin.configBracketType.Value.Substring(0,1);
                        string post = Plugin.configBracketType.Value.Substring(1);
                        string color = Plugin.configCustomColor.Value;

                        MonoSingleton<DataManager>.I.PlayerData.Name = namebase + " <color=#" + color + ">" + pre + statusmsg + post + "</color>";
                        if (MonoSingleton<MainSceneManager>.I != null)
                        {
                            NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                            string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                        }

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("Status changed to " + statusmsg + ".");
                        return false;
                }
            }
            if(text.Length == 7) // /useafk, /usebrb
                switch(text)
                {
                    case "/useafk":
                        Plugin.configUseAFK.Value = !Plugin.configUseAFK.Value;
                        string a_onoff = Plugin.configUseAFK.Value ? "on" : "off";

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("AFK system turned " + a_onoff + ".");
                        return false;
                    case "/usebrb":
                        Plugin.configUseBRB.Value = !Plugin.configUseBRB.Value;
                        string b_onoff = Plugin.configUseBRB.Value ? "on" : "off";

                        MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                        EventSystem.current.SetSelectedGameObject(null);
                        MonoSingleton<UIManager>.I.MessageInput.text = "";
                        NetworkSingleton<TextChannelManager>.I.AddNotification("BRB system turned " + b_onoff + ".");
                        return false;

                }
            if(text.Length == 5) // /help
            {
                if (text == "/help")
                {
                    NetworkSingleton<TextChannelManager>.I.AddNotification("Available commands:");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/setname <color=#4394b0>name</color>");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/status <color=#4394b0>anything</color>");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/statuscolor <color=#9db143>123456</color>");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/clearstatus");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/changebrackets <color=#4394b0>()</color>");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/useafk, /usebrb (same syntax applies for all below)");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/afkmsg <color=#4394b0>AFK</color>");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/afktimer <color=#a83131>x</color> (in minutes)");
                    NetworkSingleton<TextChannelManager>.I.AddNotification("/afkcolor <color=#9db143>123456</color>");

                    MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
                    EventSystem.current.SetSelectedGameObject(null);
                    MonoSingleton<UIManager>.I.MessageInput.text = "";
                   return false;
                        
                    
                }
            }

            return true;
        }
    }
}