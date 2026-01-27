using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Teleport.patches;

namespace Teleport
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.Teleport";
        public const string modName = "Teleport";
        public const string modVersion = "1.0.2.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake()
        {
            mls.LogInfo("Teleportation system loaded.");
            
            harmony.PatchAll(typeof(TeleportPatch));
        }

    }
}
