using System .Collections;

using UnityEngine;
using UnityEngine .UI;
using UnityEngine .Localization .Components;

using TMPro;
using PurrNet;

namespace _otAPI {
    public class UIInput : MonoBehaviour {
        public TMP_InputField input;
        public TextMeshProUGUI placeholder;
        public TextMeshProUGUI textmesh;
        private ThemeChannel TextChannel;
        public UIPackage UIP { get; set; }
        internal IEnumerator Initialize (
            UIPackage Package,
            TMP_InputField TMPI
        ) {
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            TextChannel = Package .TextChannel;
            input = TMPI;
            gameObject .transform .SetParent ( Package .Parent .transform, false );
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Input"
            ;
            for ( int c = 0; c < input .transform .GetChild ( 0 ) .childCount; c++ ) {
                if ( input .transform .GetChild ( 0 ) .GetChild ( c ) == null ) continue;
                Transform grandchild = input .transform .GetChild ( 0 ) .GetChild ( c );
                if ( grandchild .name == "Text_Placeholder" ) {
                    LocalizeStringEvent lse = grandchild .gameObject .GetComponent < LocalizeStringEvent > ( );
                    if ( lse != null ) UnityProxy .DestroyImmediate ( lse );
                    placeholder = grandchild .gameObject .GetComponent < TextMeshProUGUI > ( );
                    if ( placeholder == null ) yield break;
                    placeholder .maskable = true;
                    placeholder .text = Package .Placeholder;
                    placeholder .color = Package .Theme .GetChannel ( TextChannel, Package .Theme );
                } else if ( grandchild .name == "Text" ) {
                    textmesh = grandchild .gameObject .GetComponent < TextMeshProUGUI > ( );
                    if ( textmesh == null ) yield break;
                    textmesh .overflowMode = TextOverflowModes .Overflow;
                    textmesh .maskable = true;
                    textmesh .color = Package .Theme .GetChannel ( TextChannel, Package .Theme );
                }
            }
            input .onSubmit .RemoveAllListeners ( );
            input .onValueChanged .RemoveAllListeners ( );
            input .onSelect .RemoveAllListeners ( );
            if ( Package .Action != null ) { input .onSubmit .AddListener ( ( data ) => {
                        if ( this == null ) return;
                        Package .Action .Invoke ( );
                    }
                );
            }
            RectTransform inpRt = input .gameObject .GetComponent < RectTransform > ( );
            
            yield return null;
            RectTransform ParentRect = Package .Parent .GetComponent < RectTransform > ( );
            if ( ParentRect == null ) yield break;
            UIPanel UP = ParentRect .GetComponent < UIPanel > ( );
            if ( inpRt == null ) yield break;
            Vector2 size = inpRt .sizeDelta;
            size .x = Package .Width * ParentRect .rect .size .x ;
            inpRt .sizeDelta = size;
            if ( UP == null ) { otAPI .SetAnchoredPos ( inpRt, Package ); }
            else if ( !UP . fixedSize ) { otAPI .SetAnchoredPos ( inpRt, Package ); } else {
                size .x = Package .Width * UP .LE .preferredWidth;
                //size .y = Package .Size .y * UP .LE .preferredHeight;
                float ySpacing = Package .Size .y * UP .LE .preferredHeight;
                Vector2 minPos = new Vector2 (
                    -UP .LE .preferredWidth / 2 + size .x / 2,
                    -UP .LE .preferredHeight / 2 + ySpacing / 2
                );
                Vector2 maxPos = -minPos;
                Vector2 Range = maxPos - minPos;
                inpRt .anchorMin = otAPI .V2Center; inpRt .anchorMax = otAPI .V2Center;
                Vector2 desired = Package .Position * Range / 2;
                inpRt .localPosition =
                    Package .Unclamped
                    ? new Vector2 (
                        Package .Position .x * UP .LE .preferredWidth / 2,
                        Package .Position .y * UP .LE .preferredHeight / 2
                    )
                    : desired
                ;
                inpRt .sizeDelta = size;
                LayoutRebuilder .ForceRebuildLayoutImmediate ( ParentRect );
                
            }
            input .characterLimit = Package .CharacterLimit;
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
            otAPI .AppList [ Package .DepotFolder ] .Buffer .Set ( gameObject );
        }
        public void Retheme (
            UITheme theme
        ) {
            textmesh .color = theme .GetChannel ( TextChannel, theme );
            placeholder .color = theme .GetChannel ( TextChannel, theme );
        }
    }
}