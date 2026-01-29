using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoveTweaks.patches;

namespace MoveTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.MoveTweaks";
        public const string modName = "MoveTweaks";
        public const string modVersion = "1.0.0.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {
            mls.LogInfo("Movement tweaks loaded.");
            
            harmony.PatchAll(typeof(SprintX));
            harmony.PatchAll(typeof(SprintY));
            harmony.PatchAll(typeof(MovementPatch));
            harmony.PatchAll(typeof(BubblePatch));
            harmony.PatchAll(typeof(InfiniteBubbles));
        }

    }
}