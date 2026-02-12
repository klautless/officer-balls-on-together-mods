using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ChatTweaks.patches;
using CommandAPI;
using System.Collections.Generic;

namespace ChatTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> configTextSize  { get; private set; }
        public static ConfigEntry<string> configColorWrap { get; private set; }
        public static ConfigEntry<string> configSystemColorWrap { get; private set; }
        public static ConfigEntry<bool> configLocalNoises { get; private set; }
        public static ConfigEntry<bool> configGlobalNoises { get; private set; }
        public static ConfigEntry<bool> configMuteDuringFocus { get; private set; }
        public static ConfigEntry<bool> configJoinLeaveNoises  { get; private set; }
        public static ConfigEntry<bool> configUseTimeStamps { get; private set; }
        public static ConfigEntry<bool> configCleanUpChat { get; private set; }
        public static ConfigEntry<string> configOutlineColor { get; private set; }
        public static ConfigEntry<float> configOutlineWidth { get; private set; }
        public static ConfigEntry<int> configOutlineOpacity { get; private set; }

        public const string modGUID = "officerballs.chatTweaks";
        public const string modName = "Chat Tweaks";
        public const string modVersion = "1.1.0.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        public List<CommandInfo> commands = [
            new CommandInfo(
                "chatcommands", "ChatTweaks", TextPatcher.ShowCommands,
                "Shows all available chat commands.", null),
            new CommandInfo(
                "togglechattags", "ChatTweaks", TextPatcher.ToggleChatTags,
                "Disable all richtext tags in chat.", null),
            new CommandInfo(
                "togglelocalnoise", "ChatTweaks", TextPatcher.MuteLocal,
                "Toggles local chat message noises.", null),
            new CommandInfo(
                "toggleglobalnoise", "ChatTweaks", TextPatcher.MuteGlobal,
                "Toggles global chat message noises.", null),
            new CommandInfo(
                "toggletimermute", "ChatTweaks", TextPatcher.MuteDuringTimer,
                "Mutes all chat while timer is running.", null),
            new CommandInfo(
                "mutejoinleave", "ChatTweaks", TextPatcher.MuteJoinLeave,
                "Toggles join/leave noises.", null),
            new CommandInfo(
                "usetimestamps", "ChatTweaks", TextPatcher.UseTimestamps,
                "Enable or disable chat timestamps.", null),

            new CommandInfo(
                "textcolor", "ChatTweaks", TextPatcher.TextColor,
                "Sets the override color for chat text.",
                new Parameter[] { new Parameter("color", ParameterType.String, true) }
            ),
            new CommandInfo(
                "systemcolor", "ChatTweaks", TextPatcher.SystemColor,
                "Sets the color for system messages.",
                new Parameter[] { new Parameter("color", ParameterType.String, true) }
            ),
            new CommandInfo(
                "outlinecolor", "ChatTweaks", TextPatcher.OutlineColor,
                "Sets the outline color for chat text.",
                new Parameter[] { new Parameter("color", ParameterType.String, true) }
            ),

            new CommandInfo(
                "outlinewidth", "ChatTweaks", TextPatcher.OutlineWidth,
                "Sets the outline width for chat text.",
                new Parameter[] { new Parameter("width", ParameterType.Float, true) }
            ),
            new CommandInfo(
                "outlineopacity", "ChatTweaks", TextPatcher.OutlineOpacity,
                "Sets the outline opacity for chat text.",
                new Parameter[] { new Parameter("opacity", ParameterType.Int, true, 0, 255) }
            ),
            new CommandInfo(
                "textsize", "ChatTweaks", TextPatcher.TextSize,
                "Sets the size for chat text.",
                new Parameter[] { new Parameter("size", ParameterType.Int, true, 0, 255) }
            )];

        void Awake()
        {
            harmony.PatchAll(typeof(AddModdedTag));
            harmony.PatchAll(typeof(UIManagerPatch));
            harmony.PatchAll(typeof(TextPatcher));
            harmony.PatchAll(typeof(PlayerPanelPatch));
            harmony.PatchAll(typeof(joinSoundPatch));

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

            foreach (CommandInfo command in commands)
            {
                CommandAPI.CommandRegistry.Register(
                    command.Name,
                    command.Category,
                    command.Handler,
                    command.Description,
                    command.Parameters
                );
            }
            mls.LogInfo("Chat tweaked");
        }
    }
}
