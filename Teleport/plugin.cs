using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Teleport.patches;

namespace Teleport
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("officerballs.StatusManager", "1.1.0.0")]   
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.Teleport";
        public const string modName = "Teleport";
        public const string modVersion = "1.1.1.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {
            harmony.PatchAll(typeof(AddModdedTag));
            harmony.PatchAll(typeof(TeleportPatch));
            harmony.PatchAll(typeof(Teleporter));

            mls.LogInfo("Teleportation system loaded.");
            
        }

    }
}
