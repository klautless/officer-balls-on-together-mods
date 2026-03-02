using System;
using System .Collections;

using UnityEngine;
using UnityEngine .UI;

using TMPro;

namespace _otAPI {
    public class UIText : MonoBehaviour {
        public TMP_Text Text;
        public string String { get; internal set; }
        public string textShade { get; internal set; }
        public UIPackage UIP { get; set; }
        private ThemeChannel channel;
        private int textSize;
        private bool skipsRethemes;
        internal IEnumerator Initialize (
            UIPackage Package,
            TMP_Text TMP
        ) {
            String = Package .String;
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Text";
            skipsRethemes = Package .SkipsRethemes;
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            channel = Package .TextChannel;
            textShade = Package .Theme .GetChannelAsTextTag ( channel );
            textSize = Package .TextSize;
            TextMeshProUGUI tmP = GetComponent < TextMeshProUGUI > ( );
            if ( tmP == null ) yield break; else {
                tmP .maskable = true;
                TMP .text = $"<size={ textSize }><#{ textShade }>{ String }";
                if ( TMP .transform == null || Package .Parent == null ) yield break;
                if ( Package .Parent .transform == null ) yield break;
                TMP .transform .SetParent ( Package .Parent .transform, false );
            }
            RectTransform rt = TMP .GetComponent < RectTransform > ( );
            if ( rt == null ) yield break;
            RectTransform prect = Package .Parent .GetComponent < RectTransform > ( );
            if ( prect == null ) { otAPI .SetAnchoredPos ( rt, Package ); } else {
                UIPanel UP = prect .GetComponent < UIPanel > ( );
                if ( UP == null ) { otAPI .SetAnchoredPos ( rt, Package ); } else {
                    if ( UP == null ) { otAPI .SetAnchoredPos ( rt, Package ); }
                    else if ( !UP . fixedSize ) { otAPI .SetAnchoredPos ( rt, Package ); } else {
                        Vector2 Size =
                            Package .Unclamped
                            ? Package .Size
                            : new Vector2 (
                                Mathf .Clamp01 ( Package .Size .x ),
                                Mathf .Clamp01 ( Package .Size .y )
                            )
                        ;
                        Vector2 NextSize = new Vector2 (
                            Size .x * UP .LE .preferredWidth,
                            Size .y * UP .LE .preferredHeight
                        );
                        Vector2 minPos = new Vector2 (
                            -UP .LE .preferredWidth / 2 + NextSize .x / 2,
                            -UP .LE .preferredHeight / 2 + NextSize .y / 2
                        );
                        Vector2 maxPos = -minPos;
                        Vector2 Range = maxPos - minPos;
                        Vector2 desired = new Vector2 (
                            Package .Position .x * Range .x / 2,
                            Package .Position .y * Range .y / 2
                        );
                        rt .anchorMax = otAPI .V2Center; rt .anchorMin = otAPI .V2Center;
                        LayoutRebuilder .ForceRebuildLayoutImmediate ( prect );
                        rt .localPosition =
                            Package .Unclamped
                            ? desired
                            : new Vector2 (
                                Mathf .Clamp ( desired .x, minPos .x, maxPos .x ),
                                Mathf .Clamp ( desired .y, minPos .y, maxPos .y )
                            )
                        ;
                        rt .sizeDelta = NextSize;
                        LayoutRebuilder .ForceRebuildLayoutImmediate ( prect );
                    }
                }
            }
            yield return null;
            if ( TMP == null ) yield break;
            else Text = TMP;
            otAPI .ScrollCheck ( gameObject, Package );
            if ( Package .Mark & otAPI .AppList .ContainsKey ( Package .DepotFolder ) ) {
                if ( !otAPI .AppList [ Package .DepotFolder ] .UI .ContainsKey ( Package .ObjectName )
                    | Package .ObjectName == "UI Object"
                ) {
                    otAPI .AppList [ Package .DepotFolder ] .UI .Add (
                        Package .ObjectName != "UI Object"
                            ? Package .ObjectName
                            : $"{ Package .ObjectName }_{ otAPI .AppList [ Package .DepotFolder ] .UI .Count + 1 }",
                        gameObject
                    );
                }
                else {
                    Debug .Log ( $"otAPI construction error during { Package .DepotFolder }." );
                    Debug .Log ( "construction cancelled." );
                    yield break;
                }
            }
            //.raycastTarget = false
            otAPI .AppList [ Package .DepotFolder ] .Buffer .Set ( gameObject );
            IEnumerator recursor = otAPI .RecursiveCreation ( Package, gameObject );
            if ( recursor == null ) { yield break; } else {
                while ( recursor .MoveNext ( ) ) {
                    yield return recursor .Current;
                }
            }
            yield return null;

        }
        public void SetString ( string input ) { String = input; Text .text = $"<size={ textSize }><#{ textShade }>{ String }"; }
        
        public void Retheme (
            UITheme theme,
            bool bypass = false
        ) {
            if ( skipsRethemes & !bypass ) return;

            textShade = theme .GetChannelAsTextTag ( channel );
            SetString ( String );
        }
        public void SetSize ( int input ) { textSize = Math .Max ( input, 1 ); }
    }
}