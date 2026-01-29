
using HarmonyLib;

namespace MoveTweaks.patches
{
    [HarmonyPatch(typeof(MainSceneManager))]
    public static class BubblePatch
    {

        [HarmonyPatch("Init")]
        [HarmonyPostfix]
        public static void MovePatch( ref bool ___IsBubbleAutoPop)
        {
            ___IsBubbleAutoPop = false;
        }

    }
}
