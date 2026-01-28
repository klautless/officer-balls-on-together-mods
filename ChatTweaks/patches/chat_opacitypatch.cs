using DG.Tweening;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChatTweaks.patches
{
    [HarmonyPatch(typeof(UIManager))]
    internal class UIManagerPatch
    {
        //internal ManualLogSource mls;
        [HarmonyPatch("Update")]
        //[HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void patchReset(ref float ____messageTimer)
        {
            ____messageTimer = 0f;
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void outlineChanger( ref TextMeshProUGUI ____messageTextForFont)
        {
            Material mat = ____messageTextForFont.fontSharedMaterial;

            mat.EnableKeyword("OUTLINE_ON");
            mat.SetFloat("_OutlineWidth", 0.25f);
            mat.SetColor("_OutlineColor", new Color32(20, 20, 20, 205));
            ____messageTextForFont.UpdateMeshPadding();
            
        }
        [HarmonyPatch("SetMessagePanelActiveness")]
        [HarmonyPrefix]
        public static bool ActivenessTweaker( ref TextMeshProUGUI ____messagePlaceholderText,
        ref Color ____messagePlaceholderActiveColor,
        ref float ____uiFadeDuration,
        ref GameObject ____messagePanel,
        ref List<Image> ____messagePanelImages,
        ref List<TextMeshProUGUI> ____messagePanelTexts,
        ref Image ____globalScrollImage, ref Image ____localScrollImage,
        UIManager __instance)
        {
            var test1 = AccessTools.FieldRefAccess<CustomizationUIController, GameObject>("_customizationPanel");
            var instance1 = MonoSingleton<CustomizationUIController>.I;
            if (test1(instance1).activeSelf) return true;

            var test2 = AccessTools.FieldRefAccess<PlayerPanelController, GameObject>("_reportPanel");
            var instance2 = NetworkSingleton<PlayerPanelController>.I;
            if (test2(instance2).activeSelf) return true;
            
            ____globalScrollImage.raycastTarget = true;
            ____localScrollImage.raycastTarget = true;
            __instance.IsMessagePanelActivated = true;
            ____messagePlaceholderText.DOColor(____messagePlaceholderActiveColor, ____uiFadeDuration);
            
            ____messagePanel.gameObject.SetActive(value: true);
            //StartCoroutine(SetMessageFontOutline(!isActive && MonoSingleton<OverlayManager>.I.ResolutionMode != ResolutionMode.Normal));
            float a = 1f;
            for (int i = 0; i < ____messagePanelImages.Count; i++)
            {
                Color color = ____messagePanelImages[i].color;
                color.a = a;
                ____messagePanelImages[i].DOColor(color, ____uiFadeDuration);
            }
            for (int j = 0; j < ____messagePanelTexts.Count; j++)
            {
                Color color2 = ____messagePanelTexts[j].color;
                color2.a = a;
                ____messagePanelTexts[j].DOColor(color2, ____uiFadeDuration);
            }
            return false;
        }

    }
}
