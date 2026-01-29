
using HarmonyLib;

namespace MoveTweaks.patches
{
    [HarmonyPatch(typeof(InventoryManager))]
    public static class InfiniteBubbles
    {

        [HarmonyPatch("DecreaseItemCount")]
        [HarmonyPrefix]
        public static bool ConsumePatch( ref ShopItemType itemType)
        {
            if (itemType == ShopItemType.CottonCandyPink ||
                itemType == ShopItemType.LemonZestYellow ||
                itemType == ShopItemType.BlueBerrySplashBlue ||
                itemType == ShopItemType.GrapePopPurple ||
                itemType == ShopItemType.SourAppleGreen )
            {
                return false;
            }
            return true;


        }

    }
}