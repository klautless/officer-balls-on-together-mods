using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using PurrLobby;

namespace StatusMessage.patches
{
    [HarmonyPatch(typeof(TextChannelManager))]
    public static class StatusPatch
    {
        public static string _ifTeleportString = "";
        public static string statusmsg = "";
        public static float awaytimer = 0f;
        public static float nameTimer = 0f;
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
                    if (gradientReady) gradientReady = false;
                    SetName("basic");
                    if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                    if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                    EmitUpdate();
                    
                }
            }
            else
            {
                awaytimer += Time.deltaTime;
            }
            if(awaytimer >= Plugin.configAFKTimer.Value * 60 && statusmsg == "" && !isAFK && Plugin.configUseAFK.Value)
            {
                isAFK = true;
                if (gradientReady) gradientReady = false;
                SetName("afk");
                if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                EmitUpdate();
                
                
            }
            else if(awaytimer >= Plugin.configBRBTimer.Value * 60 && statusmsg == "" && !isBRB && Plugin.configUseBRB.Value)
            {
                isBRB = true;
                if (gradientReady) gradientReady = false;
                SetName("brb");

                if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                EmitUpdate();
                
            }

            if(Plugin.configUseGradient.Value && Plugin.configScrollingGradient.Value && gradientReady)
            {
                nameTimer += Time.deltaTime;
                if (nameTimer > tickRate)
                {
                    nameTimer = 0;
                    rollingOffset++;
                    if (rollingOffset >= nameLen) rollingOffset = 0;
                    if (statusmsg != "")
                    {
                        SetName("status");
                        EmitUpdate();
                    }
                    else if (isAFK)
                    {
                        SetName("afk");
                        EmitUpdate();
                    }
                    else if (isBRB)
                    {
                        SetName("brb");
                        EmitUpdate();
                    }
                    else
                    {
                        SetName("basic");
                        EmitUpdate();
                    }
                    CheckChangeTickrate();
                }
            }
        }
        public static int nameLen;
        public static int rollingOffset = 0;
        public static float tickRate = 0.0625f;
        public static bool gradientReady = false;
        public static List<string> colorlist = [];
        public static void MakeGradient(string name)
        {
            nameLen = name.Length;
            nameLen += Plugin.configGradientBuffer.Value;
            int r_one = Convert.ToInt32(Plugin.configGradientColor1.Value.Substring(0,2), 16);
            int r_two = Convert.ToInt32(Plugin.configGradientColor2.Value.Substring(0,2), 16);
            int r_three = Convert.ToInt32(Plugin.configGradientColor3.Value.Substring(0,2), 16);

            int g_one = Convert.ToInt32(Plugin.configGradientColor1.Value.Substring(2,2), 16);
            int g_two = Convert.ToInt32(Plugin.configGradientColor2.Value.Substring(2,2), 16);
            int g_three = Convert.ToInt32(Plugin.configGradientColor3.Value.Substring(2,2), 16);

            int b_one = Convert.ToInt32(Plugin.configGradientColor1.Value.Substring(4,2), 16);
            int b_two = Convert.ToInt32(Plugin.configGradientColor2.Value.Substring(4,2), 16);
            int b_three = Convert.ToInt32(Plugin.configGradientColor3.Value.Substring(4,2), 16);

            colorlist.Clear();
            if(!Plugin.configScrollingGradient.Value)
            {
                if(!Plugin.configUseThirdColor.Value)
                {
                    for (var i = 0; i < nameLen; i++)
                    {
                        string r = ColorSnap(r_one + (int)((r_two - r_one) * i / nameLen)).ToString("x2");
                        string g = ColorSnap(g_one + (int)((g_two - g_one) * i / nameLen)).ToString("x2");
                        string b = ColorSnap(b_one + (int)((b_two - b_one) * i / nameLen)).ToString("x2");
                        string final = r + g + b;
                        colorlist.Add(final);
                    }
                }
                else
                {
                    int halfway = (int)Math.Ceiling((double)nameLen / 2);
                    if( halfway < 1) halfway = 1;
                    for (var i = 0; i < halfway; i++)
                    {
                        string r = ColorSnap(r_one + (int)((r_two - r_one) * i / halfway)).ToString("x2");
                        string g = ColorSnap(g_one + (int)((g_two - g_one) * i / halfway)).ToString("x2");
                        string b = ColorSnap(b_one + (int)((b_two - b_one) * i / halfway)).ToString("x2");
                        string final = r + g + b;
                        colorlist.Add(final);
                    }
                    int secondhalf = nameLen - halfway > 0 ? nameLen - halfway : 1;
                    for (var i = 0; i < secondhalf; i++)
                    {
                        string r = ColorSnap(r_two + (int)((r_three - r_two) * i / halfway)).ToString("x2");
                        string g = ColorSnap(g_two + (int)((g_three - g_two) * i / halfway)).ToString("x2");
                        string b = ColorSnap(b_two + (int)((b_three - b_two) * i / halfway)).ToString("x2");
                        string final = r + g + b;
                        colorlist.Add(final);
                    }
                }
            }
            else
            {
                int halfway = (int)Math.Ceiling((double)nameLen / 2);
                if( halfway < 1) halfway = 1;
                for (var i = 0; i < halfway; i++)
                {
                    string r = ColorSnap(r_one + (int)((r_two - r_one) * i / halfway)).ToString("x2");
                    string g = ColorSnap(g_one + (int)((g_two - g_one) * i / halfway)).ToString("x2");
                    string b = ColorSnap(b_one + (int)((b_two - b_one) * i / halfway)).ToString("x2");
                    string final = r + g + b;
                    colorlist.Add(final);

                }
                int secondhalf = nameLen - halfway > 0 ? nameLen - halfway : 1;
                for (var i = 0; i < secondhalf; i++)
                {
                    string r = ColorSnap(r_two + (int)((r_one - r_two) * i / halfway)).ToString("x2");
                    string g = ColorSnap(g_two + (int)((g_one - g_two) * i / halfway)).ToString("x2");
                    string b = ColorSnap(b_two + (int)((b_one - b_two) * i / halfway)).ToString("x2");
                    string final = r + g + b;
                    colorlist.Add(final);
                }
            }
            if(colorlist.Count > 0)
            {
                gradientReady = true;
                if (statusmsg != "") SetName("status");
                else if (isAFK) SetName("afk");
                else if (isBRB) SetName("brb");
                else SetName("basic");
            }
        }
        public static void CheckChangeTickrate()
        {
            try
            {
            LobbyManager lobbyManager = MonoSingleton<MultiplayerManager>.I._lobbyManager;
            int cur = lobbyManager.CurrentLobby.Members.Count;
            int roundup = (int)Math.Ceiling((double)cur / 16);
            tickRate = 0.0625f * roundup;
            }
            catch (NullReferenceException) {}
        }
        public static string ApplyGradient(string name)
        {
            if (!gradientReady) return "";
            name = name.Trim();
            string nametotal = "";
            char[] chars = name.ToCharArray();
            for (int c = 0; c < chars.Count(); c++)
            {
                int colorIndex = c + rollingOffset;
                if (colorIndex >= colorlist.Count()) colorIndex -= colorlist.Count();
                if (colorIndex < 0) colorIndex = 0;
                //Debug.Log("Debug:" + colorIndex.ToString() + ", " + colorlist.Count().ToString() + "; " + c.ToString() + ", " + chars.Count().ToString() + ".");

                string a = "<color=#" + colorlist[colorIndex] + ">" + chars[c].ToString() + "</color>";
                nametotal += a;
            }
            return nametotal;
        }
        public static int ColorSnap(int input)
        {
            if (input < 0) input = 0;
            if (input > 255) input = 255;
            return input;
        }

        [HarmonyPatch("OnEnterPressed")]
        [HarmonyPrefix]
        public static bool TextChecker()
        {    
            string text = MonoSingleton<UIManager>.I.MessageInput.text;
            if (text == "/help")
            {
                ResetPostMessage(true);
                Notify("/help status for StatusManager commands");
                Notify("/help gradient for commands to control gradients");
                return true;
            }
            if (text.Length >= 15) // /changebrackets @ 15
            {
                if (text.Substring(0,15) == "/changebrackets")
                {
                    if (text.Length >= 18)
                    {
                        Plugin.configBracketType.Value = text.Substring(16,2);
                        if (statusmsg != "")
                        {
                            if (gradientReady) gradientReady = false;
                            SetName("status");
                            if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                            if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                            EmitUpdate();
                        }
                        Notify("Brackets changed to " + Plugin.configBracketType.Value);
                    }
                    else
                    {
                        Notify("Brackets currently set to " + Plugin.configBracketType.Value);
                    }

                    ResetPostMessage();
                    return false;

                    
                }
            }
            if (text == "/gradientscroll")
            {
                gradientReady = false;
                Plugin.configScrollingGradient.Value = !Plugin.configScrollingGradient.Value;
                if(Plugin.configScrollingGradient.Value) Plugin.configUseThirdColor.Value = !Plugin.configScrollingGradient.Value;
                string scrollOnOff = Plugin.configScrollingGradient.Value ? "on" : "off";
                rollingOffset = 0;
                if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                Notify("Scrolling gradient turned " + scrollOnOff);
                EmitUpdate();
                ResetPostMessage();
                return false;
            }
            if (text.Length >= 16) // /gradientstretch
            {
                if (text.Substring(0,16) == "/gradientstretch")
                {
                    if(int.TryParse(text.Substring(17), out int index))
                    {
                        if (index < 0) index = 0;
                        else if (index > 64) index = 64;
                        Plugin.configGradientBuffer.Value = index;
                        Notify("Gradient buffer size set to " + index.ToString() + ".");
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                    }
                    else
                    {
                        Notify("Gradient buffer size currently set to " + Plugin.configGradientBuffer.Value.ToString() + ".");
                    }
                    EmitUpdate();
                    ResetPostMessage();
                    return false;
                }
            }
            if (text.Length >= 14) // /gradientcolor, /gradientthree
            {
                string commandcheck = text.Substring(0,14);
                switch (commandcheck)
                {
                    case "/gradientcolor":
                        if (text.Length >= 16)
                        {
                            if(int.TryParse(text.Substring(15,1), out int index))
                            {
                                if(index==1)
                                {
                                    if (text.Length >= 23)
                                    {
                                        gradientReady = false;
                                        Plugin.configGradientColor1.Value = text.Substring(17,6);
                                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                                        Notify("Gradient color 1 set to <color=#" + Plugin.configGradientColor1.Value + ">" + Plugin.configGradientColor1.Value + "</color>.");
                                    }
                                    else
                                    {
                                        Notify("Gradient color 1 currently set to <color=#" + Plugin.configGradientColor1.Value + ">" + Plugin.configGradientColor1.Value + "</color>.");
                                    }
                                }
                                else if(index==2)
                                {
                                    if (text.Length >= 23)
                                    {
                                        gradientReady = false;
                                        Plugin.configGradientColor2.Value = text.Substring(17,6);
                                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                                        Notify("Gradient color 2 set to <color=#" + Plugin.configGradientColor2.Value + ">" + Plugin.configGradientColor2.Value + "</color>.");
                                    }
                                    else
                                    {
                                        Notify("Gradient color 2 currently set to <color=#" + Plugin.configGradientColor2.Value + ">" + Plugin.configGradientColor2.Value + "</color>.");
                                    }
                                }
                                else if(index==3)
                                {
                                    if (text.Length >= 23)
                                    {
                                        gradientReady = false;
                                        Plugin.configGradientColor3.Value = text.Substring(17,6);
                                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                                        Notify("Gradient color 3 set to <color=#" + Plugin.configGradientColor3.Value + ">" + Plugin.configGradientColor3.Value + "</color>.");
                                    }
                                    else
                                    {
                                        Notify("Gradient color 3 currently set to <color=#" + Plugin.configGradientColor3.Value + ">" + Plugin.configGradientColor3.Value + "</color>.");
                                    }
                                }
                                else
                                {
                                    Notify("Must specify a number 1 - 3 and a color.");
                                    Notify("example: /gradientcolor 1 123456");
                                }

                            }
                            else
                            {
                                Notify("Must specify a number 1 - 3 and a color.");
                                Notify("example: /gradientcolor 1 123456");
                            }
                        }
                        else
                        {
                            Notify("Must specify a number 1 - 3 and a color.");
                            Notify("example: /gradientcolor 1 123456");
                        }
                        EmitUpdate();
                        ResetPostMessage();
                        return false;
                    case "/gradientthree":
                        gradientReady = false;
                        Plugin.configUseThirdColor.Value = !Plugin.configUseThirdColor.Value;
                        string threeonoff = Plugin.configUseThirdColor.Value ? "on" : "off";
                        if (Plugin.configUseThirdColor.Value) Plugin.configScrollingGradient.Value = !Plugin.configUseThirdColor.Value;
                        rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                        Notify("Gradient 3 turned " + threeonoff);
                        EmitUpdate();
                        ResetPostMessage();
                        return false;
                }
            }
            if (text.Length >= 12) // /clearstatus, /statuscolor, /usegradient
            {
                string commandcheck = text.Substring(0,12);
                switch (commandcheck)
                {
                    case "/usegradient":
                        if (Plugin.configUseGradient.Value)
                        {
                            switch(CheckName(Plugin.configNameBase.Value))
                            {
                                case true:
                                    break;
                                case false:
                                    Notify("Gradient names can't use tags.");
                                    Notify("/setname to a tagless name first.");
                                    ResetPostMessage();
                                    return false;
                            }
                        }
                        Plugin.configUseGradient.Value = !Plugin.configUseGradient.Value;
                        string usegrad = Plugin.configUseGradient.Value ? "on" : "off";
                        Notify("Gradients turned " + usegrad);
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;                
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                        else
                        {
                            gradientReady = false;
                            SetName("basic");
                        }

                        EmitUpdate();
                        ResetPostMessage();

                        return false;
                    case "/clearstatus":
                        statusmsg = "";
                        if (gradientReady) gradientReady = false;
                        SetName("basic");
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                        EmitUpdate();
                        ResetPostMessage();
                        Notify("Status message cleared.");
                    
                        return false;
                    case "/statuscolor":
                        if (text.Length >= 19)
                        {
                            string color = text.Substring(13,6);
                            Plugin.configCustomColor.Value = color;
                            if (statusmsg != "")
                            {
                                if (gradientReady) gradientReady = false;
                                SetName("status");
                                if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                                if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                                EmitUpdate();
                                Notify("Status color changed to <color=#" + Plugin.configCustomColor.Value + ">" + Plugin.configCustomColor.Value + "</color>.");
                            }
                            
                        }
                        else
                        {
                            Notify("Status color currently set to <color=#" + Plugin.configCustomColor.Value + ">" + Plugin.configCustomColor.Value + "</color>.");  
                        }
                        ResetPostMessage();
                        return false;
                }
            }
            if (text.Length >= 9) // /brbcolor /afkcolor
            {
                string commandcheck = text.Substring(0,9);
                string color = "";
                switch (commandcheck)
                {
                    case "/brbcolor":
                    if (text.Length >= 16)
                    {
                        color = text.Substring(10,6);
                        Plugin.configBRBColor.Value = color;

                        ResetPostMessage();
                        Notify("BRB color changed to <color=#" + Plugin.configBRBColor.Value + ">" + Plugin.configBRBColor.Value + "</color>.");
                        return false;
                    }
                    else
                    {
                        ResetPostMessage();
                        Notify("BRB color currently set to <color=#" + Plugin.configBRBColor.Value + ">" + Plugin.configBRBColor.Value + "</color>.");
                        return false;
                    }

                    case "/afkcolor":
                    if (text.Length >= 16)
                    {
                        color = text.Substring(10,6);
                        Plugin.configAFKColor.Value = color;
                        ResetPostMessage();
                        Notify("AFK color changed to <color=#" + Plugin.configAFKColor.Value + ">" + Plugin.configAFKColor.Value + "</color>.");
                        return false;
                    }
                    else
                    {
                        ResetPostMessage();
                        Notify("AFK color currently set to <color=#" + Plugin.configAFKColor.Value + ">" + Plugin.configAFKColor.Value + "</color>.");
                        return false;
                    }
                        
                }
            }
            if (text.Length >= 9) // /afktimer, /brbtimer
            {
                string commandcheck = text.Substring(0,9);
                switch (commandcheck)
                {
                    case "/brbtimer":
                    if(text.Length>10)
                    {
                        if(int.TryParse(text.Substring(10), out int brbnum))
                        {
                            Plugin.configBRBTimer.Value = brbnum;
                            Notify("BRB timer changed to " + Plugin.configBRBTimer.Value.ToString() + " minutes.");
                        }
                    }
                    else
                    {
                        Notify("BRB timer currently set to " + Plugin.configBRBTimer.Value.ToString() + " minutes.");
                    }
                    ResetPostMessage();
                    return false;

                    case "/afktimer":
                    if(text.Length>10)
                    {
                        if(int.TryParse(text.Substring(10), out int afknum))
                        {
                            Plugin.configAFKTimer.Value = afknum;
                            Notify("AFK timer changed to " + Plugin.configAFKTimer.Value.ToString() + " minutes.");
                        }
                    }
                    else
                    {
                        Notify("AFK timer currently set to " + Plugin.configAFKTimer.Value.ToString() + " minutes.");
                    }
                    ResetPostMessage();
                    return false;
                }
            }
            if (text.Length >= 8) // /setname
            {
                if (text.Substring(0,8) == "/setname")
                {
                    if (text.Length > 9)
                    {
                        
                        string newname = text.Substring(9); //gradientReady? ApplyGradient(text.Substring(9)) : text.Substring(9);
                        if (Plugin.configUseGradient.Value)
                        {
                            switch(CheckName(newname))
                            {
                                case true:
                                    break;
                                case false:
                                    Notify("Gradient names can't use tags.");
                                    ResetPostMessage();
                                    return false;
                            }
                        }
                        if (gradientReady) gradientReady = false;
                        Plugin.configNameBase.Value = newname;
                        Notify("Name changed to " + newname + ".");
                        ResetPostMessage();
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(newname);
                        EmitUpdate();
                    
                    }
                    else
                    {
                        string name = gradientReady ? ApplyGradient(Plugin.configNameBase.Value) : Plugin.configNameBase.Value;
                        Notify("Name currently registered as " + name + ".");
                        ResetPostMessage();
                    }
                    return false;
                }
            }
            if (text.Length >= 7) // /status, /afkmsg, /brbmsg
            {
                string commandcheck = text.Substring(0,7);
                switch (commandcheck)
                {
                    case "/brbmsg":
                    if (text.Length > 8)
                    {
                        Plugin.configBRBMessage.Value = text.Substring(8);
                        ResetPostMessage();
                        Notify("BRB message changed to " + Plugin.configBRBMessage.Value + ".");
                        return false;
                        
                    }
                    else
                    {
                        ResetPostMessage();
                        Notify("BRB message currently set to " + Plugin.configBRBMessage.Value + ".");
                        return false;
                    }

                    case "/afkmsg":
                    if (text.Length > 8)
                    {
                        Plugin.configAFKMessage.Value = text.Substring(8);
                        ResetPostMessage();
                        Notify("AFK message changed to " + Plugin.configAFKMessage.Value + ".");
                        return false;   
                    }
                    else
                    {
                        ResetPostMessage();
                        Notify("AFK message currently set to " + Plugin.configAFKMessage.Value + ".");
                        return false;
                    }

                    case "/status":
                    if (text.Length > 8)
                    {
                        statusmsg = text.Substring(8);
                        if (gradientReady) gradientReady = false;
                        SetName("status");
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                        EmitUpdate();
                        ResetPostMessage();
                        Notify("Status changed to " + statusmsg + ".");
                        return false;
                    }
                    else
                    {
                        statusmsg = "";
                        if (gradientReady) gradientReady = false;
                        SetName("basic");
                        if (Plugin.configScrollingGradient.Value) rollingOffset = 0;
                        if (Plugin.configUseGradient.Value) MakeGradient(Plugin.configNameBase.Value);
                        EmitUpdate();
                        ResetPostMessage();
                        Notify("Status message cleared.");

                        return false;
                    }
                }
            }
            if(text.Length == 7) // /useafk, /usebrb
            {
                switch(text)
                {
                    case "/useafk":
                    Plugin.configUseAFK.Value = !Plugin.configUseAFK.Value;
                    string a_onoff = Plugin.configUseAFK.Value ? "on" : "off";

                    ResetPostMessage();
                    Notify("AFK system turned " + a_onoff + ".");
                    return false;

                    case "/usebrb":
                    Plugin.configUseBRB.Value = !Plugin.configUseBRB.Value;
                    string b_onoff = Plugin.configUseBRB.Value ? "on" : "off";

                    ResetPostMessage();
                    Notify("BRB system turned " + b_onoff + ".");
                    return false;
                }
            }
            if (text.ToLower() == "/help statusmanager" || text.ToLower() == "/help status")
            {
                Notify("<b>Available commands:</b>");
                Notify("/setname <color=#4394b0>name</color>");
                Notify("/status <color=#4394b0>anything</color>");
                Notify("/statuscolor <color=#9db143>123456</color>");
                Notify("/clearstatus");
                Notify("/changebrackets <color=#4394b0>()</color>");
                Notify("/useafk, /usebrb (same syntax applies for all below)");
                Notify("/afkmsg <color=#4394b0>AFK</color>");
                Notify("/afktimer <color=#a83131>x</color> (in minutes)");
                Notify("/afkcolor <color=#9db143>123456</color>");
                
                ResetPostMessage();
                return false;
            }
            if (text.ToLower() == "/help gradient")
            {
                Notify("<b>Available commands:</b>");
                Notify("/usegradient");
                Notify("<b><i>Make sure to /setname to ONLY your name, no tags</b></i>");
                Notify("/gradientcolor <color=#a83131>x</color> <color=#9db143>123456</color>");
                Notify("/gradientstretch <color=#a83131>xx</color> (0-64)");
                Notify("/gradientthree - enable third gradient color");
                Notify("/gradientscroll - gradient autoscrolling");
                Notify("turning on third gradient will disable gradient scrolling and vice versa.");
                
                ResetPostMessage();
                return false;
            }
            return true;
        }
        public static void SetName(string type)
        {
            switch(type)
            {
                case "basic":
                    string basic_namebase = gradientReady ? ApplyGradient(Plugin.configNameBase.Value) : Plugin.configNameBase.Value;
                    MonoSingleton<DataManager>.I.PlayerData.Name = basic_namebase;
                    break;
                case "status":
                    string status_namebase = gradientReady ? ApplyGradient(Plugin.configNameBase.Value) : Plugin.configNameBase.Value;
                    string status_pre = Plugin.configBracketType.Value.Substring(0,1);
                    string status_post = Plugin.configBracketType.Value.Substring(1);
                    string status_color = Plugin.configCustomColor.Value;
                    MonoSingleton<DataManager>.I.PlayerData.Name = status_namebase + " <color=#" + status_color + ">" + status_pre + statusmsg + status_post + "</color>";
                    break;
                case "brb":
                    string brb_namebase = gradientReady ? ApplyGradient(Plugin.configNameBase.Value) : Plugin.configNameBase.Value;
                    string brb_pre = Plugin.configBracketType.Value.Substring(0,1);
                    string brb_post = Plugin.configBracketType.Value.Substring(1);
                    string brbmsg = Plugin.configBRBMessage.Value;
                    string brb_color = Plugin.configBRBColor.Value;
                    
                    MonoSingleton<DataManager>.I.PlayerData.Name = brb_namebase + " <color=#" + brb_color + ">" + brb_pre + brbmsg + brb_post + "</color>";
                    break;
                case "afk":
                    string afk_namebase = gradientReady ? ApplyGradient(Plugin.configNameBase.Value) : Plugin.configNameBase.Value;
                    string afk_pre = Plugin.configBracketType.Value.Substring(0,1);
                    string afk_post = Plugin.configBracketType.Value.Substring(1);
                    string afkmsg = Plugin.configAFKMessage.Value;
                    string afk_color = Plugin.configAFKColor.Value;
                    MonoSingleton<DataManager>.I.PlayerData.Name = afk_namebase + " <color=#" + afk_color + ">" + afk_pre + afkmsg + afk_post + "</color>";
                    break;
            }
        }
        public static void Notify(string notification)
        {
            NetworkSingleton<TextChannelManager>.I.AddNotification(notification);
        }
        public static void ResetPostMessage(bool skiptext = false)
        {
            MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
            EventSystem.current.SetSelectedGameObject(null);
            if(!skiptext) MonoSingleton<UIManager>.I.MessageInput.text = "";
        }
        public static void EmitUpdate()
        {
            if (MonoSingleton<MainSceneManager>.I != null)
            {
                try
                {
                    NetworkSingleton<TextChannelManager>.I.MainCustomizationController.UpdatePlayerInfo(MonoSingleton<DataManager>.I.PlayerData.GetPlayerIdInfo());
                    string text3 = (MonoSingleton<UIManager>.I.PlayerText.text = (NetworkSingleton<TextChannelManager>.I.UserName = MonoSingleton<DataManager>.I.PlayerData.Name));
                    if (_ifTeleportString != "")
                    {
                        var namefieldRef = AccessTools.FieldRefAccess<UIManager, TextMeshProUGUI>("_playerText");
                        var nameinstance = MonoSingleton<UIManager>.I;
                        var playname = MonoSingleton<DataManager>.I.PlayerData.Name;
                        namefieldRef(nameinstance).text = playname + _ifTeleportString;
                    }
                }
                catch (NullReferenceException) {}
            }
        }
        public static bool CheckName( string name )
        {
            char[] chars = name.ToCharArray();
            bool prefound = false;
            bool postfound = false;
            foreach (char c in chars)
            {
                if (c == '<') prefound = true;
                if (c == '>') postfound = true;
            }
            if (prefound && postfound) return false;
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
    }
}
