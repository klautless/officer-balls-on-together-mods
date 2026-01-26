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

namespace FishingTweaks.patches
{
    [HarmonyPatch(typeof(FishingManager))]
    internal class FishingPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void FishingTweaks(ref float ____catchWait)
        {
            ____catchWait = 60f;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void UpdateTweak()
        {
            if (MonoSingleton<InputManager>.I.IsMouseButton0Down)
            {            
                NetworkSingleton<TextChannelManager>.I.MainMovementController.GroundState = GroundState.Grounded;

                NetworkSingleton<TextChannelManager>.I.MainMovementController.MovementState = MovementState.Idle;
            }
        }

    }
    [HarmonyPatch(typeof(TaskManager))]
    internal class FishingPatch2
    {
        [HarmonyPatch("IsReadyForFishing")]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }

    }
    [HarmonyPatch(typeof(IsGroundedChecker))]
    internal class GroundCheckPatch
    {
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        public static bool GroundPatch( IsGroundedChecker __instance)
        {
            if( MonoSingleton<FishingManager>.I.FishingState == FishingState.Casting || MonoSingleton<FishingManager>.I.FishingState == FishingState.Fishing || MonoSingleton<FishingManager>.I.FishingState == FishingState.MiniGame )
            {
                __instance.IsGrounded = true;
                return false;
                
            }
            return true;
        }
        
    }
    
}
