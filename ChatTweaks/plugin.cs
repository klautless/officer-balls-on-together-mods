using BepInEx;
using BepInEx .Configuration;
using BepInEx .Logging;
using UnityEngine;
using HarmonyLib;
using TMPro;
using ChatTweaks .patches;
using _otAPI;
using System .Collections .Generic;
using System;
using System.Reflection;

namespace ChatTweaks {
    [ BepInPlugin ( modGUID, modName, modVersion ) ]
    [ BepInDependency ( "officerballs.otAPI", BepInDependency .DependencyFlags .SoftDependency ) ]
    public class Plugin : BaseUnityPlugin {
        public static ConfigEntry < int > configTextSize  { get; private set; }
        public static ConfigEntry < string > configColorWrap { get; private set; }
        public static ConfigEntry < string > configSystemColorWrap { get; private set; }
        public static ConfigEntry < bool > configLocalNoises { get; private set; }
        public static ConfigEntry < bool > configGlobalNoises { get; private set; }
        public static ConfigEntry < bool > configMuteDuringFocus { get; private set; }
        public static ConfigEntry < bool > configJoinLeaveNoises  { get; private set; }
        public static ConfigEntry < bool > configUseTimeStamps { get; private set; }
        public static ConfigEntry < bool > configCleanUpChat { get; private set; }
        public static ConfigEntry < string > configOutlineColor { get; private set; }
        public static ConfigEntry < float > configOutlineWidth { get; private set; }
        public static ConfigEntry < int > configOutlineOpacity { get; private set; }

        const string appID = "Chat Tweaks";
        /*internal static UIPackage Icon = new ( ) {
            ObjectName = appID,
            DepotFolder = appID,
            Type = UIType .Image,
            Path = "chatTweaks/images/messages_App.png",
            ImgSize = new Vector2Int ( 224, 224 ),
            Assembly = Assembly.GetExecutingAssembly()
        };
        internal static UIPackage App = new ( ) {
            DepotFolder = appID,
            ObjectName = appID,
            Mark = true,
            Position = new Vector2 ( -0.5f, 0.25f ),
            StartInactive = true,
            Size = new Vector2 ( 0.66f, 0.66f ),
            PostBuild = async ( locker ) => {
                //otAPI .AppList [ appID ] .UI [ appID ]
                locker .SetResult ( true );
            },
            Children = new ( ) {
                new ( ) {
                    Type = UIType .Input,
                    Position = new Vector2 ( 0f, -0.7f ),
                    Width = 0.95f
                },
                new ( ) {
                    Type = UIType .Scrollable,
                    Position = new Vector2 ( 0f, 0.5f ),
                    Size = new Vector2 ( 0.9f, 0.7f )
                }
            }
        };
        void OnEnable ( ) {
            otAPI .ThemeChange += ThemeChange;
        }
        void OnDisable ( ) {
            otAPI .ThemeChange -= ThemeChange;
        }
        void ThemeChange ( UITheme theme ) {
            if ( otAPI .AppList .ContainsKey ( appID ) ) {
                if ( otAPI .AppList [ appID ] .UI .ContainsKey ( appID ) ) {
                    UIPanel P = otAPI .AppList [ appID ] .UI [ appID ] .GetComponent < UIPanel > ( );
                    P .Retheme ( theme );
                }
            }
        }*/



        public const string modGUID = "officerballs.chatTweaks";
        public const string modName = "Chat Tweaks";
        public const string modVersion = "2.0.0.0";

        private Harmony harmony = new Harmony ( modGUID );

        internal ManualLogSource mls = BepInEx .Logging .Logger .CreateLogSource ( modGUID );
        internal static Depot chatTweaksDepot = otAPI .CreateDepot (
            "Chat Tweaks",
            "Chat Tweaks",
            "officer balls",
            "A group of tools to modify the chatbox.",
            "/"
        );
        internal static void OutlineChanger(string[] args)
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

        private void RunPatches()
        {
            harmony.PatchAll(typeof(UIManagerPatch));
            harmony.PatchAll(typeof(TextPatcher));
            harmony.PatchAll(typeof(PlayerPanelPatch));
            harmony.PatchAll(typeof(joinSoundPatch));
        }
        private void RunBinds()
        {
            configLocalNoises = Config.Bind("General.Toggles", "PlayLocalNoises", true, "Enable or disable on-message noises in local chat.");
            configGlobalNoises = Config.Bind("General.Toggles", "PlayGlobalNoises", true, "Enable or disable on-message noises in global chat.");
            configMuteDuringFocus = Config.Bind("General.Toggles", "MuteDuringTimer", false, "Mutes on top of local/global settings during pomodoro timer.");
            configJoinLeaveNoises = Config.Bind("General.Toggles", "PlayJoinLeaveNoises", true, "Enable or disable on-leave/join noises.");
            configTextSize = Config.Bind("General", "TextSize", 48, "Wraps chat with a size tag.");
            configColorWrap = Config.Bind("General", "TextColor", "ffffff", "Wraps chat with a color tag.");
            configSystemColorWrap = Config.Bind("General", "SystemTextColor", "d5d6db", "Wraps system messages with a color tag.");

            configOutlineColor = Config.Bind("General", "OutlineColor", "141414", "Outline's color.");
            configOutlineWidth = Config.Bind("General", "OutlineWidth", 0.25f, "Outline's width.");
            configOutlineOpacity = Config.Bind("General", "OutlineOpacity", 205, "Outline's opacity.");

            configUseTimeStamps = Config.Bind("General","UseTimestamps", true, "Whether to use timestamps in chat.");
            configCleanUpChat = Config.Bind("General", "DisableChatTags", true, "Removes all effects from text messages.");
        }
        internal void Awake()
        {
            RunPatches();
            RunBinds();

            //List<Alias> basicAliases = [
            // Uses the alias-routing style of constructor: name, depot, description, action ( a string[] args method ),
            //      frontEnd (bool to determine if there's a chat alias),
            //      passThrough (bool to determine if other commands can use this, i.e. a shared /help alias) 
            //      Arg[] args to feed the method, null if none required
            //      Alias to an existing method in your plugins
            //new Alias( "chatcommands", chatTweaksDepot, "Shows all available chat commands.",
            //    TextPatcher.ShowCommands, true, false, null)];
            

            List<Alias> cfgAliases = [
            // Use the cfg-routing style of constructor: name, depot, description as before,
            //  Arg[] args as before,
            //  CfgLink maps to BepInEx ConfigEntry within this plugin (ArgType, ConfigEntry, and optional string changers)
            //  Creates a settings configurator method
            new Alias( "Block Richtext", chatTweaksDepot, "Disable all richtext tags in chat.",
                new Arg[] { new Arg(ArgType.Bool, true) },
                new CfgLink(ArgType.Bool, configCleanUpChat)),
            new Alias( "Local Noise", chatTweaksDepot, "Toggles local chat message noises.",
                new Arg[] { new Arg(ArgType.Bool, true) },
                new CfgLink(ArgType.Bool, configLocalNoises)),
            new Alias( "Global Noise", chatTweaksDepot, "Toggles global chat message noises.",
                new Arg[] { new Arg(ArgType.Bool, true) },
                new CfgLink(ArgType.Bool, configGlobalNoises)),
            new Alias( "Timer Mute", chatTweaksDepot, "Mutes all chat while timer is running.",
                new Arg[] { new Arg(ArgType.Bool, true) },
                new CfgLink(ArgType.Bool, configMuteDuringFocus)),
            new Alias( "Join/Leave Noise", chatTweaksDepot, "Toggles join/leave noises.",
                new Arg[] { new Arg(ArgType.Bool, true) },
                new CfgLink(ArgType.Bool, configJoinLeaveNoises)),
            new Alias( "Timestamps", chatTweaksDepot, "Enable or disable chat timestamps.",
                null, new CfgLink(ArgType.Bool, configUseTimeStamps)),
            new Alias( "Text Color", chatTweaksDepot, "Sets the override color for chat text.",
                new Arg[] { new Arg( ArgType.HexColor, true ) },
                new CfgLink(ArgType.HexColor, configColorWrap) ),

            // Use the cfg-routing constructor, with auxilary action call (when extra steps are needed)
            new Alias( "Outline Color", chatTweaksDepot, "Sets the outline color for chat text.",
                new Arg[] { new Arg( ArgType.HexColor, true ) },
                new CfgLink(ArgType.HexColor, configOutlineColor),
                OutlineChanger, AuxTiming.During ), //final param set to false runs the aux method after settings change method
            new Alias( "Outline Width", chatTweaksDepot, "Sets the outline width for chat text.",
                new Arg[] { new Arg(ArgType.Float, true, 0f, 0.5f)},
                new CfgLink(ArgType.Float, configOutlineWidth),
                OutlineChanger, AuxTiming.During ),
            new Alias( "Outline Opacity", chatTweaksDepot, "Sets the outline opacity for chat text.",
                new Arg[] { new Arg(ArgType.Int, true, 0, 255)},
                new CfgLink(ArgType.Int, configOutlineOpacity),
                OutlineChanger, AuxTiming.During ),
            new Alias( "System Color", chatTweaksDepot, "Sets the color for system messages.",
                new Arg[] { new Arg( ArgType.HexColor, true ) },
                new CfgLink(ArgType.HexColor, configSystemColorWrap ) ),
            new Alias( "Text Size", chatTweaksDepot, "Sets the size for chat text.",
                new Arg[] { new Arg(ArgType.Int, true, 0, 255)},
                new CfgLink(ArgType.Int, configTextSize ) )
                
            ];
            
            // Register regular aliases
            /*foreach (Alias alias in basicAliases)
            {
                otAPI.Register(
                    alias.name, alias.description, alias.depot,
                    alias.action, alias.frontEnd, alias.passThrough,
                    alias.args );
            }*/

            // Register cfg style aliases (aux defaults to null so one CfgAlias method covers all cases)
            foreach (Alias alias in cfgAliases)
            {
                otAPI.AddCfg(
                    alias.name, alias.description, alias.depot,
                    alias.args, alias.cfgLink, alias.action, alias.auxTiming
                );
            }
            mls.LogInfo("Chat tweaked");
        }
    }
}
