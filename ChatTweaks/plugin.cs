using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ChatTweaks.patches;

namespace ChatTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> configTextSize  { get; private set; }
        public static ConfigEntry<string> configColorWrap { get; private set; }
        public static ConfigEntry<string> configSystemColorWrap { get; private set; }
        public static ConfigEntry<bool> configMsgNoises  { get; private set; }
        public static ConfigEntry<bool> configJoinLeaveNoises  { get; private set; }
        public static ConfigEntry<bool> configUseTimeStamps { get; private set; }
        public static ConfigEntry<string> configOutlineColor { get; private set; }
        public static ConfigEntry<float> configOutlineWidth { get; private set; }
        public static ConfigEntry<int> configOutlineOpacity { get; private set; }

        public const string modGUID = "officerballs.chatTweaks";
        public const string modName = "Chat Tweaks";
        public const string modVersion = "1.0.7.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            harmony.PatchAll(typeof(AddModdedTag));
            harmony.PatchAll(typeof(UIManagerPatch));
            harmony.PatchAll(typeof(TextPatcher));
            harmony.PatchAll(typeof(PlayerPanelPatch));
            harmony.PatchAll(typeof(joinSoundPatch));

            configMsgNoises = Config.Bind("General.Toggles", "PlayMessageNoises", true, "Enable or disable on-message noises.");
            configJoinLeaveNoises = Config.Bind("General.Toggles", "PlayJoinLeaveNoises", true, "Enable or disable on-leave/join noises.");
            configTextSize = Config.Bind("General", "TextSize", 48, "Wraps chat with a size tag.");
            configColorWrap = Config.Bind("General", "TextColor", "ffffff", "Wraps chat with a color tag.");
            configSystemColorWrap = Config.Bind("General", "SystemTextColor", "d5d6db", "Wraps system messages with a color tag.");

            configOutlineColor = Config.Bind("General", "OutlineColor", "141414", "Outline's color.");
            configOutlineWidth = Config.Bind("General", "OutlineWidth", 0.25f, "Outline's width.");
            configOutlineOpacity = Config.Bind("General", "OutlineOpacity", 205, "Outline's opacity.");

            configUseTimeStamps = Config.Bind("General","UseTimestamps", true, "Whether to use timestamps in chat.");

            mls.LogInfo("Chat tweaked");
        }

    }
}
