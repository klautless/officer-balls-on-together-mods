using HarmonyLib;

namespace StatusMessage.patches
{
    [HarmonyPatch(typeof(DataManager))]
    internal class RevertNamePatch
    {

        [HarmonyPatch("LoadPlayerData")]
        [HarmonyPostfix]
        public static void SetUser()
        {
            if (Plugin.configNameBase.Value != "" && MonoSingleton<DataManager>.I.PlayerData.Name != Plugin.configNameBase.Value)
            {
                MonoSingleton<DataManager>.I.PlayerData.Name = Plugin.configNameBase.Value;
            }
            else if (Plugin.configNameBase.Value == "")
            {
                Plugin.configNameBase.Value = MonoSingleton<DataManager>.I.PlayerData.Name;
            }
        }
    }
}