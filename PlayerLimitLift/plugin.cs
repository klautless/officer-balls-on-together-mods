using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerLimitLift.patches;

namespace PlayerLimitLift
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.PlayerLimitLift";
        public const string modName = "Player Limit Lift";
        public const string modVersion = "1.0.2.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            mls.LogInfo("Player Limit Lifted to 24.");
            harmony.PatchAll(typeof(PlayerLimitPatch));
            harmony.PatchAll(typeof(ServerNameModder));
        }

    }
}
