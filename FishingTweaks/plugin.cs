using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingTweaks.patches;

namespace FishingTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.FishingTweaks";
        public const string modName = "Fishing Tweaks";
        public const string modVersion = "1.0.0.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            mls.LogInfo("Fishing tweaked.");
            
            harmony.PatchAll(typeof(FishingPatch));
            harmony.PatchAll(typeof(FishingPatch2));
            harmony.PatchAll(typeof(GroundCheckPatch));
        }

    }
}