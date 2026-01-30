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
        public static ConfigEntry<bool> configMsgNoises  { get; private set; }
        public static ConfigEntry<bool> configJoinLeaveNoises  { get; private set; }
        public const string modGUID = "officerballs.chatTweaks";
        public const string modName = "Chat Tweaks";
        public const string modVersion = "1.0.5.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            harmony.PatchAll(typeof(UIManagerPatch));
            harmony.PatchAll(typeof(TimeStampsPatch));
            harmony.PatchAll(typeof(PlayerPanelPatch));
            harmony.PatchAll(typeof(joinSoundPatch));
            configMsgNoises = Config.Bind("General.Toggles", "PlayMessageNoises",true,"Enable or disable on-message noises (takes effect after restart)");
            configJoinLeaveNoises = Config.Bind("General.Toggles", "PlayJoinLeaveNoises",true,"Enable or disable on-leave/join noises (takes effect after restart)");
            configTextSize = Config.Bind("General", "TextSize",48,"Wraps chat with a size tag (takes effect after restart)");
            mls.LogInfo("Chat tweaked");
        }

    }
}

