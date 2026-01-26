using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoomies.patches;
namespace Zoomies
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.Zoomies";
        public const string modName = "Zoomies";
        public const string modVersion = "1.0.0.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            mls.LogInfo("Zooming tweaked.");
            
            harmony.PatchAll(typeof(ZoomPatch));
        }

    }
}
