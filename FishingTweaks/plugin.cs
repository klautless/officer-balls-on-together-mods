using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using FishingTweaks.patches;
using _otAPI;
using System.Collections.Generic;
using System.Reflection;

namespace FishingTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [ BepInDependency ( "ob.otAPI", BepInDependency .DependencyFlags .SoftDependency ) ]
    public class Plugin : BaseUnityPlugin {
        public static ConfigEntry<bool> configHideOversizedCatches  { get; private set; }
        public static ConfigEntry<bool> configAutoCatch { get; private set; }
        public static ConfigEntry<bool> configAutoRecast { get; private set; }
        public static ConfigEntry<bool> configInfiniteBait { get; private set; }
        public static ConfigEntry<bool> configMuteFishing  { get; private set; }
        public static ConfigEntry<bool> configMuteMinigame { get; private set; }
        public static ConfigEntry<bool> configMuteFishingDuringFocus { get; private set; }
        public const string modGUID = "officerballs.FishingTweaks";
        public const string modName = "Fishing Tweaks";
        public const string modVersion = "2.0.0.0";


        internal static Depot FishingDepot = otAPI .CreateDepot (
            modName, modName,
            "officer balls",
            "A set of changes to the fishing system.",
            "/"
        );

        private Harmony harmony = new Harmony(modGUID);

        void Awake ( ) {
            ApplyPatches ( );
            BindBinds ( );
            CreateCFG ( );
            Debug .Log ( "Fishing Tweaks Twoke" );
        }
        void ApplyPatches ( ) {
            harmony.PatchAll(typeof(FishingPatch));
            harmony.PatchAll(typeof(FishingPatch2));
            harmony.PatchAll(typeof(GroundCheckPatch));
            harmony.PatchAll(typeof(KeepSizeInCheck));
            harmony.PatchAll(typeof(BlockSounds));
            harmony.PatchAll(typeof(BlockSounds2));
        }
        void BindBinds ( ) {
            configHideOversizedCatches = Config.Bind (
                "General.Toggles", "HideOversizedCatches", false,
                "Resizes oversized fish from other players."
            );
            configAutoCatch = Config.Bind (
                "General.Toggles", "AutoCatch", false,
                "Enable/disable automatic catching."
            );
            configAutoRecast = Config.Bind (
                "General.Toggles", "AutoRecast", false,
                "Automatically recast to last position when fishing."
            );
            configInfiniteBait = Config.Bind (
                "General.Toggles", "InfiniteBait", false,
                "Infinite bait."
            );
            configMuteMinigame = Config.Bind (
                "General.Sound", "MuteMinigame", false,
                "Mutes SFX from the fishing minigame."
            );
            configMuteFishing = Config.Bind (
                "General.Sound", "MuteFishing", false,
                "Gets rid of the annoying bobber noise."
            );
            configMuteFishingDuringFocus = Config.Bind (
                "General.Sound", "MuteDuringFocus", false,
                "Mute all fishing during Pomodoro focus."
            );
        }
        void InfiniteBait ( string [ ] args ) {
            if ( configInfiniteBait .Value ) {
                var _baitCountsField = AccessTools.FieldRefAccess<FishingManager, List<int>>("_baitCounts");
                var fishingMgr = MonoSingleton<FishingManager>.I;
                var _baitCounts = _baitCountsField(fishingMgr);
                for (int bait = 0; bait < _baitCounts.Count; bait++)
                {
                    _baitCounts[bait] = 40;
                }
                MonoSingleton<DataManager>.I.PlayerDataZip.BaitCounts = _baitCounts;
                MonoSingleton<DataManager>.I.SavePlayerZipData();
            }
        }
        void CreateCFG ( ) {
            List < Alias > cfgs = [
                new Alias ( "Auto-Catch", FishingDepot, "Toggles auto-catching.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configAutoCatch ) ),
                new Alias ( "Auto-Recast", FishingDepot, "Toggles auto-recasting.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configAutoRecast ) ),
                new Alias ( "Infinite Bait", FishingDepot, "Unlimited bait.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configInfiniteBait ),
                    InfiniteBait, AuxTiming .After ),
                new Alias ( "Regulate Size", FishingDepot, "Normalizes fish sizing.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configHideOversizedCatches ) ),
                new Alias ( "Mute Fishing", FishingDepot, "Mutes fishing noise.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configMuteFishing ) ),
                new Alias ( "Mute Minigame", FishingDepot, "Mutes fishing minigame.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configMuteMinigame ) ),
                new Alias ( "Mute in Focus", FishingDepot, "Mutes fishing during focus.",
                    new Arg [ ] { new Arg ( ArgType .Bool, true ) },
                    new CfgLink ( ArgType .Bool, configMuteFishingDuringFocus ) ),

            ];
            foreach ( Alias alias in cfgs ) {
                otAPI .AddCfg (
                    alias .name, alias .description, alias .depot,
                    alias .args, alias .cfgLink, alias .action, alias .auxTiming
                );
            }
            
        }

    }
}
