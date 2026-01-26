using BepInEx;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Drawing.Printing;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(PlayerCustomizationController))]
    internal class joinSoundPatch
    {
        [HarmonyPatch("NotifyOtherClients_Original_5")]
        [HarmonyPrefix]
        public static void joinSoundPatcher()
        {
            NetworkSingleton<TextChannelManager>.I.MainSFXController.PlayPetSound(PetType.FisherBirdy);
        }

    }
}