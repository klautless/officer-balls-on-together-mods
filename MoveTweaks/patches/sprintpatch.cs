
using HarmonyLib;
using UnityEngine;

namespace MoveTweaks.patches
{
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.Horizontal), MethodType.Getter)]
    public static class SprintX
    {
        [HarmonyPostfix]
        public static void xPost(ref float __result)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                __result *= 2.5f;
                
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                __result *= 0.25f;
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.Vertical), MethodType.Getter)]
    public static class SprintY
    {
        [HarmonyPostfix]
        public static void yPost(ref float __result)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                __result *= 2.5f;
                
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                __result *= 0.25f;
            }
        }
    }
}