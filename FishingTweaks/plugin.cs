using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using FishingTweaks.patches;

namespace FishingTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> configHideAnnoyingCatches  { get; private set; }
        public static ConfigEntry<bool> configAutoCatch { get; private set; }
        public static ConfigEntry<bool> configAutoRecast { get; private set; }
        public static ConfigEntry<bool> configInfiniteBait { get; private set; }
        public static ConfigEntry<bool> configMuteFishing  { get; private set; }
        public static ConfigEntry<bool> configMuteMinigame { get; private set; }
        public static ConfigEntry<bool> configMuteFishingDuringFocus { get; private set; }
        public const string modGUID = "officerballs.FishingTweaks";
        public const string modName = "Fishing Tweaks";
        public const string modVersion = "1.1.0.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {
            mls.LogInfo("Fishing tweaked.");
            harmony.PatchAll(typeof(AddModdedTag));
            harmony.PatchAll(typeof(FishingPatch));
            harmony.PatchAll(typeof(FishingPatch2));
            harmony.PatchAll(typeof(GroundCheckPatch));
            harmony.PatchAll(typeof(KeepSizeInCheck));
            harmony.PatchAll(typeof(ClickSender));
            harmony.PatchAll(typeof(AddCommands));
            harmony.PatchAll(typeof(BlockSounds));
            harmony.PatchAll(typeof(BlockSounds2));

            configHideAnnoyingCatches = Config.Bind("General.Toggles", "HideAnnoyingCatches", false, "Resizes oversized fish from other players.");
            configAutoCatch = Config.Bind("General.Toggles", "AutoCatch", false, "Enable/disable automatic catching.");
            configAutoRecast = Config.Bind("General.Toggles", "AutoRecast", false, "Automatically recast to last position when fishing.");
            configInfiniteBait = Config.Bind("General.Toggles", "InfiniteBait", false, "Infinite bait.");
            configMuteMinigame = Config.Bind("General.Sound", "MuteMinigame", false, "Mutes SFX from the fishing minigame.");
            configMuteFishing = Config.Bind("General.Sound", "MuteFishing", false, "Gets rid of the annoying bobber noise.");
            configMuteFishingDuringFocus = Config.Bind("General.Sound", "MuteDuringFocus", false, "Mute all fishing during Pomodoro focus.");
        }

    }
}
