
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingTweaks.patches
{
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
                MonoSingleton<DataManager>.I.PlayerDataZip.BaitCounts = ____baitCounts;
                MonoSingleton<DataManager>.I.SavePlayerZipData();
            }
        }
        public static float updateDelayer = 0f;
        public static bool loaded = false;
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void UpdateTweak()
        {
            if (updateDelayer > 0) { updateDelayer -= Time.deltaTime; return; }
            if (MonoSingleton<MainSceneManager>.I != null && !loaded)
            {
                try {
                    var loadtest1 = MonoSingleton<FishingManager>.I.FishingCont;
                    var loadtest2 = NetworkSingleton<TextChannelManager>.I.MainMovementController;
                    var _preload3 = AccessTools.FieldRefAccess<FishingManager, Transform>("_castIndicator");
                    var load3 = _preload3(MonoSingleton<FishingManager>.I);
                    loaded = true;
                     }
                catch (NullReferenceException) { loaded = false; return; }
            }
            else if (!loaded) { updateDelayer = 2.5f; return; }
            try
            {
                if (MonoSingleton<InputManager>.I.IsMouseButton0Down)
                {            
                    NetworkSingleton<TextChannelManager>.I.MainMovementController.GroundState = GroundState.Grounded;

                    NetworkSingleton<TextChannelManager>.I.MainMovementController.MovementState = MovementState.Idle;
                }
                // autocatch

                if (MonoSingleton<FishingManager>.I.FishingCont.GetAlert() && Plugin.configAutoCatch.Value)
                {
                    if ( clickDelayer <= 0 )
                    {
                        Traverse.Create(MonoSingleton<InputManager>.I).Property("IsMouseButton0Down").SetValue(true);
                        clickDelayer = 0.0625f;
                    }
                    else clickDelayer -= Time.deltaTime;
                }
                // auto-recast
                if (recastPos != Vector3.zero &&
                    Plugin.configAutoRecast.Value &&
                    MonoSingleton<FishingManager>.I.CurrentBait != BaitType.None &&
                    MonoSingleton<FishingManager>.I.FishingState == FishingState.None &&
                    MonoSingleton<FishingManager>.I.FishingCont.FishingRod.activeSelf)
                {
                    var _castIndicator = AccessTools.FieldRefAccess<FishingManager, Transform>("_castIndicator");
                    _castIndicator(MonoSingleton<FishingManager>.I).gameObject.SetActive(value: true);
                    _castIndicator(MonoSingleton<FishingManager>.I).transform.position = recastPos;
                    
                    Traverse.Create(MonoSingleton<FishingManager>.I).Property("FishingState").SetValue(FishingState.Casting);
                    var _isWater = AccessTools.FieldRefAccess<FishingManager, bool>("_isWater");
                    _isWater(MonoSingleton<FishingManager>.I) = true;
                    Traverse.Create(MonoSingleton<InputManager>.I).Property("IsMouseButton0Up").SetValue(true);
                }
            }
            catch (NullReferenceException)
            {
                updateDelayer = 2.5f;
                loaded = false;
                Debug.Log("fishtweaks loadin delayed");
            }
        }
        [HarmonyPatch("FishMiniGameUpdate")]
        [HarmonyPostfix]
        public static void autoClickSnapon( FishingManager __instance)
        {
            // autocatch
            if (Plugin.configAutoCatch.Value && checkClick(__instance))
            {
                if ( clickDelayer <= 0 )
                {
                    Traverse.Create(MonoSingleton<InputManager>.I).Property("IsMouseButton0Down").SetValue(true);
                    clickDelayer = 0.03125f;
                }
                else clickDelayer -= Time.deltaTime;
            }
        }
        public static bool checkClick( FishingManager instance)
        {
            var _fishIcon = AccessTools.FieldRefAccess<FishingManager, RectTransform>("_fishIcon");
            var _clickZone = AccessTools.FieldRefAccess<FishingManager, RectTransform>("_clickZone");
            var _clickZoneDegree = AccessTools.FieldRefAccess<FishingManager, float>("_clickZoneDegree");
            float z = _fishIcon(instance).rotation.eulerAngles.z;
            float z2 = _clickZone(instance).rotation.eulerAngles.z;
            float num = z2 + _clickZoneDegree(instance);
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
        /*[HarmonyPatch(typeof(FishingController), nameof(FishingController.IsReleaseButtonClicked), MethodType.Setter)]
        [HarmonyPostfix]
        public static void AddClickIfAuto( FishingController __instance )
            { if (Plugin.configAutoCatch.Value) Traverse.Create(__instance).Property("IsReleaseButtonClicked").SetValue(true); }
        */
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
                                if (caughtFish.Size < fish.MinSize || caughtFish.Size > fish.MaxSize)
                                {
                                    caughtFish.Size = size;
                                    caughtFish.SizeRatio = sizeRatio;
                                }
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
                                if (caughtFish.Size < fish.MinSize || caughtFish.Size > fish.MaxSize)
                                {
                                    caughtFish.Size = size;
                                    caughtFish.SizeRatio = sizeRatio;
                                }
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
                                if (caughtFish.Size < fish.MinSize || caughtFish.Size > fish.MaxSize)
                                {
                                    caughtFish.Size = size;
                                    caughtFish.SizeRatio = sizeRatio;
                                }
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
            if (Plugin.configAutoCatch.Value) FishingPatch.sendClick = true;

            if (Plugin.configInfiniteBait.Value)
            {
                var _baitCountsField = AccessTools.FieldRefAccess<FishingManager, List<int>>("_baitCounts");
                var fishingMgr = MonoSingleton<FishingManager>.I;
                var _baitCounts = _baitCountsField(fishingMgr);
                for (int bait = 0; bait < _baitCounts.Count; bait++)
                {
                    _baitCounts[bait] = 40;
                }
                MonoSingleton<DataManager>.I.PlayerDataZip.BaitCounts = _baitCounts;
                MonoSingleton<DataManager>.I.SavePlayerZipData();
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
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("SetFishingCast")]
        [HarmonyPrefix]
        public static bool SkipFishingCast()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("SetFishingReel")]
        [HarmonyPrefix]
        public static bool SkipFishingReel()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("SetFishingAlert")]
        [HarmonyPrefix]
        public static bool SkipFishingAlert()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("StopFishingReel")]
        [HarmonyPrefix]
        public static bool SkipStopFishingReel( ref AudioSource ____audioSource)
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value )
            {
                ____audioSource.Stop();
                return false;
            }
            else return true;
        }
    }
    [HarmonyPatch(typeof(SFXManager))]
    public static class BlockSounds2
    {
        [HarmonyPatch("PlayFishCatch")]
        [HarmonyPrefix]
        public static bool SkipFishCatch()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("PlayFishClick")]
        [HarmonyPrefix]
        public static bool SkipFishClick()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("PlayFishFail")]
        [HarmonyPrefix]
        public static bool SkipFishFail()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
            else return true;
        }
        [HarmonyPatch("PlayFishSuccess")]
        [HarmonyPrefix]
        public static bool SkipFishSuccess()
        {
            if (( Plugin.configMuteFishingDuringFocus.Value &&
            MonoSingleton<PomodoroController>.I.PomodoroType == PomodoroType.Study &&
            !MonoSingleton<PomodoroController>.I.IsPaused ) || Plugin.configMuteMinigame.Value ) return false;
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
                Notify("/muteminigame");
                Notify("/mutefishduringtimer");
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
            if (text.ToLower() == "/muteminigame")
            {
                Plugin.configMuteMinigame.Value = !Plugin.configMuteMinigame.Value;
                string isMuted = Plugin.configMuteMinigame.Value ? "muted" : "unmuted";
                Notify("Fishing minigame " + isMuted + ".");
                FinishCmds();
                return false;
            }
            if (text.ToLower() == "/mutefishduringtimer")
            {
                Plugin.configMuteFishingDuringFocus.Value = !Plugin.configMuteFishingDuringFocus.Value;
                string isMuted = Plugin.configMuteFishingDuringFocus.Value ? "enabled" : "disabled";
                Notify("Mute fishing during timer " + isMuted + ".");
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
                    MonoSingleton<DataManager>.I.PlayerDataZip.BaitCounts = _baitCounts;
                    MonoSingleton<DataManager>.I.SavePlayerZipData();
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
