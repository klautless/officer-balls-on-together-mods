using BepInEx;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Zoomies.patches
{
    [HarmonyPatch(typeof(PlayerMovementController))]
    internal class ZoomPatch
    {
        [HarmonyPatch("OnSpawned")]
        [HarmonyPostfix]
        public static void Zoomies()
        {
            var fieldRef = AccessTools.FieldRefAccess<GameSettings, float>("_zoomMax");
            var instance = ScriptableSingleton<GameSettings>.I;
            fieldRef(instance) = 10f;

        }
    }
}
