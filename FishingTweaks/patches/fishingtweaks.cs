
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingTweaks.patches
{
    [HarmonyPatch(typeof(InputManager))]
    public static class ClickSender
    {
        [HarmonyPatch(typeof(InputManager), nameof(InputManager.IsMouseButton0Down), MethodType.Setter)]
        [HarmonyPostfix]
        public static void AddClickIfAuto( InputManager __instance )
        {
            if (FishingPatch.sendClick)
            {
                FishingPatch.sendClick = false;
                Traverse.Create(__instance).Property("IsMouseButton0Down").SetValue(true);
            }
        }

        [HarmonyPatch(typeof(InputManager), nameof(InputManager.IsMouseButton0Up), MethodType.Setter)]
        [HarmonyPostfix]
        public static void AddClickIfRecast( InputManager __instance )
        {
            if (FishingPatch.sendUpClick)
            {
                FishingPatch.sendUpClick = false;
                Traverse.Create(__instance).Property("IsMouseButton0Up").SetValue(true);
            }
        }
    }
    [HarmonyPatch(typeof(FishingManager))]
    public static class FishingPatch
    {
        public static float clickDelayer = 0f;
        public static bool sendClick = false;
        public static bool sendUpClick = false;
        public static Vector3 recastPos = Vector3.zero;


        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void FishingTweaks(ref float ____catchWait, ref List<int> ____baitCounts)
        {
            ____catchWait = 60f;
            if (Plugin.configInfiniteBait.Value)
            {
                for (int bait = 0; bait < ____baitCounts.Count; bait++)
                {
                    ____baitCounts[bait] = 40;
                }
            }
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void UpdateTweak( FishingManager __instance, ref Transform ____castIndicator )
        {
            if (MonoSingleton<InputManager>.I.IsMouseButton0Down)
            {            
                NetworkSingleton<TextChannelManager>.I.MainMovementController.GroundState = GroundState.Grounded;

                NetworkSingleton<TextChannelManager>.I.MainMovementController.MovementState = MovementState.Idle;
            }
            // autocatch
            if (__instance.FishingState == FishingState.MiniGame && Plugin.configAutoCatch.Value)
            {
                if ( clickDelayer <= 0 )
                {
                    if (checkClick(__instance))
                    {
                        sendClick = true;
                        clickDelayer = 0.1f;
                    }
                }
                else clickDelayer -= Time.deltaTime;
            }
            // auto-recast
            if (recastPos != Vector3.zero &&
                Plugin.configAutoRecast.Value &&
                __instance.CurrentBait != BaitType.None &&
                __instance.FishingState == FishingState.None &&
                __instance.FishingCont.FishingRod.activeSelf &&
                (NetworkSingleton<TextChannelManager>.I.MainMovementController.MovementState == MovementState.Idle ||
                NetworkSingleton<TextChannelManager>.I.MainMovementController.MovementState == MovementState.Walk) &&
                NetworkSingleton<TextChannelManager>.I.MainMovementController.GroundState == GroundState.Grounded)
            {
                ____castIndicator.gameObject.SetActive(value: true);
                ____castIndicator.transform.position = recastPos;
                Traverse.Create(__instance).Property("FishingState").SetValue(FishingState.Casting);
                var _isWater = AccessTools.FieldRefAccess<FishingManager, bool>("_isWater");
                _isWater(__instance) = true;
                sendUpClick = true;
            }
        }
        [HarmonyPatch("CastFishingRod")]
        [HarmonyPrefix]
        public static void storePosition( ref Transform ____castIndicator )
        {
            recastPos = ____castIndicator.transform.position;
        }

        [HarmonyPatch("CancelFishing")]
        [HarmonyPrefix]
        public static void clearPosition(  )
        {
            recastPos = Vector3.zero;
        }
        public static bool checkClick( FishingManager __instance)
        {
            var _fishIcon = AccessTools.FieldRefAccess<FishingManager, RectTransform>("_fishIcon");
            var _clickZone = AccessTools.FieldRefAccess<FishingManager, RectTransform>("_clickZone");
            var _clickZoneDegree = AccessTools.FieldRefAccess<FishingManager, float>("_clickZoneDegree");
            float z = _fishIcon(__instance).rotation.eulerAngles.z;
            float z2 = _clickZone(__instance).rotation.eulerAngles.z;
            float num = z2 + _clickZoneDegree(__instance);
            if (num >= 360f)
            {
                if (z >= z2 || z <= num - 360f)
                {
                    return true;
                }
            }
            else if (z >= z2 && z <= num)
            {
                return true;
            }
            return false;
        }

    }
    [HarmonyPatch(typeof(TaskManager))]
    internal class FishingPatch2
    {
        [HarmonyPatch("IsReadyForFishing")]
        public static void Postfix(ref bool __result)
        {
            var test1 = AccessTools.FieldRefAccess<MusicManager, MusicInstrumentController>("_currentInstrument");
            var instance1 = NetworkSingleton<MusicManager>.I;
            if (test1(instance1) == null)
            {
                __result = true;
            }
        }

    }
    [HarmonyPatch(typeof(FishingController))]
    internal class KeepSizeInCheck
    {
        [HarmonyPatch("CatchFishRpc_Original_3")]
        [HarmonyPrefix]
        public static void Resizer( ref CaughtFish caughtFish )
        {
            if (Plugin.configHideAnnoyingCatches.Value)
            {
                switch (caughtFish.Source)
                {
                    case FishSource.FreshWater:
                        List<Fish> FreshFishes = ScriptableSingleton<FishingSettings>.I.FreshFishes;
                        for (int i = 0; i < FreshFishes.Count; i++)
                        {
                            if (caughtFish.FishIndex == i)
                            {
                                Fish fish = FreshFishes[i];
                                float size = UnityEngine.Random.Range(fish.MinSize, fish.MaxSize);
                                float sizeRatio = size / fish.AverageSize;
                                caughtFish.Size = size;
                                caughtFish.SizeRatio = sizeRatio;
                            }
                        }
                        break;
                    case FishSource.SaltWater:
                        List<Fish> Fishes = ScriptableSingleton<FishingSettings>.I.Fishes;
                        for (int i = 0; i < Fishes.Count; i++)
                        {
                            if (caughtFish.FishIndex == i)
                            {
                                Fish fish = Fishes[i];
                                float size = UnityEngine.Random.Range(fish.MinSize, fish.MaxSize);
                                float sizeRatio = size / fish.AverageSize;
                                caughtFish.Size = size;
                                caughtFish.SizeRatio = sizeRatio;
                            }
                        }
                        break;
                    case FishSource.Trash:
                        List<Fish> Garbages = ScriptableSingleton<FishingSettings>.I.Garbages;
                        for (int i = 0; i < Garbages.Count; i++)
                        {
                            if (caughtFish.FishIndex == i)
                            {
                                Fish fish = Garbages[i];
                                float size = UnityEngine.Random.Range(fish.MinSize, fish.MaxSize);
                                float sizeRatio = size / fish.AverageSize;
                                caughtFish.Size = size;
                                caughtFish.SizeRatio = sizeRatio;
                            }
                        }
                        break;
                }
            }
        }
        [HarmonyPatch("SinkHook")]
        [HarmonyPrefix]
        public static void ClickOnMinigameStart()
        {
            if (Plugin.configAutoCatch.Value)
            {
                FishingPatch.sendClick = true;
            }

            if (Plugin.configInfiniteBait.Value)
            {
                var _baitCountsField = AccessTools.FieldRefAccess<FishingManager, List<int>>("_baitCounts");
                var fishingMgr = MonoSingleton<FishingManager>.I;
                var _baitCounts = _baitCountsField(fishingMgr);
                for (int bait = 0; bait < _baitCounts.Count; bait++)
                {
                    _baitCounts[bait] = 40;
                }
            }
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
    [HarmonyPatch(typeof(PlayerSFXController))]
    public static class BlockSounds
    {
        [HarmonyPatch("SetFishingDrop")]
        [HarmonyPrefix]
        public static bool SkipDropSound()
        {
            if (Plugin.configMuteFishing.Value) return false;
            else return true;
        }
    }
    [HarmonyPatch(typeof(TextChannelManager))]
    public static class AddCommands
    {
        [HarmonyPatch("OnEnterPressed")]
        [HarmonyPrefix]
        public static bool TextChecker()
        {    
            string text = MonoSingleton<UIManager>.I.MessageInput.text;
            if (text == "/help")
            {
                FinishCmds(false);
                Notify("/help fish for FishingTweaks commands");
                return true;
            }
            if (text.ToLower() == "/help fish" || text.ToLower() == "/help fishingtweaks")
            {
                Notify("Available commands:");
                Notify("/autocatch");
                Notify("/autorecast");
                Notify("/infinitebait");
                Notify("/mutefishing");
                Notify("/forcenormalsizes");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/forcenormalsizes")
            {
                Plugin.configHideAnnoyingCatches.Value = !Plugin.configHideAnnoyingCatches.Value;
                string isMuted = Plugin.configHideAnnoyingCatches.Value ? "on" : "off";
                Notify("Fish size enforcement turned " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/mutefishing")
            {
                Plugin.configMuteFishing.Value = !Plugin.configMuteFishing.Value;
                string isMuted = Plugin.configMuteFishing.Value ? "muted" : "unmuted";
                Notify("Fishing " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/autocatch")
            {
                Plugin.configAutoCatch.Value = !Plugin.configAutoCatch.Value;
                string isMuted = Plugin.configAutoCatch.Value ? "enabled" : "disabled";
                Notify("Auto-catch " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/autorecast")
            {
                Plugin.configAutoRecast.Value = !Plugin.configAutoRecast.Value;
                string isMuted = Plugin.configAutoRecast.Value ? "enabled" : "disabled";
                Notify("Auto-recast " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/infinitebait")
            {
                Plugin.configInfiniteBait.Value = !Plugin.configInfiniteBait.Value;
                string isMuted = Plugin.configInfiniteBait.Value ? "enabled" : "disabled";
                Notify("Infinite bait " + isMuted + ".");
                FinishCmds();
                if (Plugin.configInfiniteBait.Value)
                {
                    var _baitCountsField = AccessTools.FieldRefAccess<FishingManager, List<int>>("_baitCounts");
                    var fishingMgr = MonoSingleton<FishingManager>.I;
                    var _baitCounts = _baitCountsField(fishingMgr);
                    for (int bait = 0; bait < _baitCounts.Count; bait++)
                    {
                        _baitCounts[bait] = 40;
                    }
                }
                return false;
            }
            return true;
        }

        public static void FinishCmds(bool clearText=true)
        {
            MonoSingleton<TaskManager>.I.SetLockState(NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free);
            EventSystem.current.SetSelectedGameObject(null);
            if (clearText) MonoSingleton<UIManager>.I.MessageInput.text = "";
        }
        public static void Notify(string text) { NetworkSingleton<TextChannelManager>.I.AddNotification(text); }

        [HarmonyPatch("SendMessageAsync")]
        [HarmonyPrefix]
        public static bool HelpCloser(byte[] textBytes)
        {
            string text = Encoding.Unicode.GetString(textBytes);
            if(text == "/help") return false;
            return true;
        }
    }
}
