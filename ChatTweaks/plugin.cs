using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatTweaks.patches;

namespace ChatTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "officerballs.chatTweaks";
        public const string modName = "Chat Tweaks";
        public const string modVersion = "1.0.1.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            mls.LogInfo("Chat tweaked");
            harmony.PatchAll(typeof(UIManagerPatch));
            harmony.PatchAll(typeof(TimeStampsPatch));
            harmony.PatchAll(typeof(PlayerLeftPatch));
            harmony.PatchAll(typeof(joinSoundPatch));
        }

    }
}
