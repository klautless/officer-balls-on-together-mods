using BepInEx;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using PurrNet;
using PurrNet.Packing;
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
using UnityEngine.Audio;
using System.Drawing.Printing;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(PlayerPanelController))]
    internal class PlayerLeftPatch
    {
        [HarmonyPatch("DespawnHandle")]
        [HarmonyPrefix]
        public static void PlayerLeftNoti( NetworkTransform netTransform, PlayerPanelController __instance)
        {

		for (int i = 0; i < __instance.PlayerSteamIDs.Count; i++)
		{
			if (__instance.PlayerTransforms[i] == netTransform)
			{
				var dname = __instance.IDInfos[i].Name;
                NetworkSingleton<TextChannelManager>.I.AddNotification( Encoding.Unicode.GetString(dname) + " has left the server.");
                
                NetworkSingleton<TextChannelManager>.I.MainSFXController.PlayPetSound(PetType.Frog);
                break;
			}
		}
        }

    }
}