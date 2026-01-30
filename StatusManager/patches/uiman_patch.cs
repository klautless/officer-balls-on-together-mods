using HarmonyLib;
using TMPro;

namespace StatusMessage.patches
{
    [HarmonyPatch(typeof(UIManager))]
    internal class CharLimitPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void limitLifter(ref TMP_InputField ____messageInputField)
        {
            ____messageInputField.characterLimit = 250;

        }
    }
}