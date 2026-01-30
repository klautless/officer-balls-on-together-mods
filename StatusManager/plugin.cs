using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using StatusMessage.patches;

namespace StatusMessage
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<string> configNameBase  { get; private set; }
        public static ConfigEntry<string> configBracketType { get; private set; }
        public static ConfigEntry<string> configCustomColor { get; private set; }

        public static ConfigEntry<bool> configUseBRB  { get; private set; }
        public static ConfigEntry<int> configBRBTimer  { get; private set; }
        public static ConfigEntry<string> configBRBMessage  { get; private set; }
        public static ConfigEntry<string> configBRBColor { get; private set; }

        public static ConfigEntry<bool> configUseAFK  { get; private set; }
        public static ConfigEntry<int> configAFKTimer  { get; private set; }
        public static ConfigEntry<string> configAFKMessage  { get; private set; }
        public static ConfigEntry<string> configAFKColor { get; private set; }

        

        public const string modGUID = "officerballs.StatusManager";
        public const string modName = "Status Manager";
        public const string modVersion = "1.0.1.0";

        private Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);



        void Awake()
        {
            harmony.PatchAll(typeof(StatusPatch));
            harmony.PatchAll(typeof(RevertNamePatch));
            harmony.PatchAll(typeof(CharLimitPatch));
            
            string playername = "";
            //if (!string.IsNullOrEmpty(MonoSingleton<DataManager>.I.PlayerData.Name))
            //{
            //    playername = MonoSingleton<DataManager>.I.PlayerData.Name;
            //}
            configNameBase = Config.Bind("General", "PlayerName", playername, "Your player's name.");
            configBracketType = Config.Bind("General", "BracketType", "()", "How your status messages are wrapped. Two character limit.");
            configCustomColor = Config.Bind("General", "CustomColor", "5ec1c7", "Colorcode to wrap custom status with.");
            
            configUseBRB = Config.Bind("General.BRB", "UseBRB", true, "Enables auto-BRB system.");
            configBRBTimer = Config.Bind("General.BRB", "BRBTimer", 2, "Time in minutes before BRB applies. Must be lower than AFK timer if that's enabled.");
            configBRBMessage = Config.Bind("General.BRB", "BRBMessage", "BRB", "Text for the BRB status.");
            configBRBColor = Config.Bind("General.BRB", "BRBColor", "ffd45d", "Colorcode to wrap BRB status with.");

            configUseAFK = Config.Bind("General.AFK", "UseAFK", true, "Enables auto-AFK system.");
            configAFKTimer = Config.Bind("General.AFK", "AFKTimer", 5, "Time in minutes before AFK applies. Must be higher than BRB timer if that's enabled.");
            configAFKMessage = Config.Bind("General.AFK", "AFKMessage", "AFK", "Text for the AFK status.");
            configAFKColor = Config.Bind("General.AFK", "AFKColor", "e97d45", "Colorcode to wrap AFK status with.");

            
            
            //configUseBRB configBRBTimer configBRBMessage
            //configUseAFK configBRBTimer configBRBMessage
            mls.LogInfo("Status Management system online.");
        }

    }
}
