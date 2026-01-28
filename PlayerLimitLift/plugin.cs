using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using PlayerLimitLift.patches;

namespace PlayerLimitLift
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> configDefaultLobbySize   { get; private set; }
        public static ConfigEntry<int> configShiftSkipRate      { get; private set; }
        public const string modGUID = "officerballs.PlayerLimitLift";
        public const string modName = "Player Limit Lift";
        public const string modVersion = "1.0.5.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            mls.LogInfo("Player Limit Lifted to 128.");
            harmony.PatchAll(typeof(PanelPatch));
            harmony.PatchAll(typeof(PlayerLimitPatch));
            harmony.PatchAll(typeof(ServerNameModder));
            configDefaultLobbySize = Config.Bind("General", "DefaultLobbySize",16,"Lobby size shown on launch (takes effect after restart)");
            configShiftSkipRate = Config.Bind("General", "ShiftSkipRate",16,"Amount that shift+click changes lobby size by (takes effect after restart)");
            
            // Credit to 岚风 雷 / Arashi_Lei (https://github.com/gqxastg) for sorting out the oversized lobby issues!

            harmony.PatchAll(typeof(TextChannelManagerPatch));
        }

    }
}
