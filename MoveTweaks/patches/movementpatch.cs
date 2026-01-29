
using HarmonyLib;
using UnityEngine;

namespace MoveTweaks.patches
{
    [HarmonyPatch(typeof(PlayerMovementController))]
    public static class MovementPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void MovePatch( PlayerMovementController __instance)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f) && Input.GetMouseButton(2))
            {
                var playerpos = NetworkSingleton<TextChannelManager>.I.MainPlayer.transform;// = controller.transform.position;
                Vector3 worldPosition = hit.point;
                worldPosition.y = playerpos.position.y;
                playerpos.LookAt(worldPosition);
                __instance.transform.rotation = Quaternion.Slerp(__instance.transform.rotation, playerpos.rotation, 5f * Time.deltaTime);;
                
            }
        }

    }
}