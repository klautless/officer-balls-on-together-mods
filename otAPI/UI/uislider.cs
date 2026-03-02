using System;
using System .Collections;

using UnityEngine;
using UnityEngine .UI;

namespace _otAPI {
    public class UISlider : MonoBehaviour {
        public UIPackage UIP { get; set; }
        public Slider slider { get; internal set; }
        public Image bg { get; internal set; }
        public Image fill { get; internal set; }
        public Image handle { get; internal set; }
        public ThemeChannel Channel1 { get; internal set; }
        public ThemeChannel Channel2 { get; internal set; }
        internal IEnumerator Initialize (
            UIPackage Package
        ) {
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Slider";
            Channel1 = Package .Channel1; Channel2 = Package .Channel2;
            slider = gameObject .GetComponent < Slider > ( );
            if ( slider == null ) yield break;
            slider .onValueChanged .RemoveAllListeners ( );
            RectTransform prect = Package .Parent .GetComponent < RectTransform > ( );
            if ( prect == null ) yield break;
            RectTransform Rect = GetComponent < RectTransform > ( );
            if ( Rect == null ) yield break;
            bg = transform .GetChild ( 0 ) .GetComponent < Image > ( );
            if ( bg == null ) yield break; else bg .color = Package .Theme .GetChannel ( Channel2, Package .Theme );
            fill = transform .GetChild ( 1 ) .GetChild ( 0 ) .GetComponent < Image > ( );
            if ( fill == null ) yield break; else fill .color = Package .Theme .GetChannel ( Channel1, Package .Theme );
            handle = transform .GetChild ( 2 ) .GetChild ( 0 ) .GetComponent < Image > ( );
            RectTransform handleRect = transform .GetChild ( 2 ) .GetChild ( 0 ) .GetComponent < RectTransform > ( );
            if ( handleRect == null ) yield break;
            if ( handle == null ) yield break; else {
                RectTransform HRT = handle .GetComponent < RectTransform > ( );
                if ( HRT == null ) yield break;
                Sprite sprite;
                string key = $"{ Math .Abs ( ( int ) HRT .rect .size .x ) }x{ Math .Abs ( ( int ) HRT .rect .size .y ) }_{ Package .Radius }";
                yield return null;
                if ( otAPI .spriteCache .TryGetValue ( key, out Sprite outsprite ) ) {
                    sprite = outsprite;
                    yield return null;
                } else {
                    sprite = BakeHandle ( Package, HRT );
                    yield return null;
                }
                Vector2 spriteSize = sprite .rect .size;
                while ( spriteSize .x <= 1 || spriteSize .y <= 1 ) {
                    sprite = BakeHandle ( Package, HRT );
                    yield return null;
                    spriteSize = sprite .rect .size;
                } 
                handle .sprite = sprite;
                handle .color = Package .Theme .GetChannel ( Package .Channel1, Package .Theme );
            }
            Vector2 resize = Rect .sizeDelta;

            resize .x = prect .rect .width * Package .Width;
            Vector2 pos = prect .rect .size * Package .Position * 0.5f;
            UIPanel UP = prect .GetComponent < UIPanel > ( );
            if ( UP == null ) {  }
            else if ( !UP . fixedSize ) {  } else {

                prect .localPosition = new Vector2 (
                    Package .Position .x * UP .LE .preferredWidth / 2, // + ( UP .LE .preferredWidth / 2 ),
                    Package .Position .y * UP .LE .preferredHeight / 2
                );
                resize .x = UP .LE .preferredWidth * Package .Width;
                
                pos = prect .localPosition;
                LayoutRebuilder .ForceRebuildLayoutImmediate ( prect );
            }

            Rect .sizeDelta = resize;

            Vector2 halfSize = new Vector2 (
                resize .x * 0.5f,
                handleRect .rect .size .y * 0.5f
            );
            Vector2 parentSize = prect .rect .size;
            if ( UP == null ) { } else { if ( UP .LE == null ) { } else {
                    halfSize .y = UP .LE .preferredHeight * Package .Size .y / 2;
                    parentSize = new Vector2 (
                        UP .LE .preferredWidth,
                        UP .LE .preferredHeight
                    );
                };
            }
            Vector2 max = new Vector2 (
                parentSize .x * 0.5f,
                parentSize .y * 0.5f
            );
            Vector2 min = -max;
            Vector2 Range = max - min;
            Vector2 desired = Package .Position * Range / 2;

            pos.x = Mathf .Clamp ( desired .x, min .x + halfSize .x, max .x - halfSize .x );
            pos.y = Mathf .Clamp ( desired .y, min .y + halfSize .y, max .y - halfSize .y );

            Rect.localPosition = pos;
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
        internal Sprite BakeHandle (
            UIPackage Package,
            RectTransform Handle
        ) {
            // Debug .Log ( $"Incoming texture attempt: { Math .Abs ( ( int ) rect .rect .size .x ) } x { Math .Abs ( ( int ) rect .rect .size .y ) }" );
            int safeX = Math .Max ( Math .Abs ( ( int ) Handle .rect .width ), 1 );
            int safeY = Math .Max ( Math .Abs ( ( int ) Handle .rect .height ), 1 );
            Texture2D texture = new Texture2D (
                safeX, safeY
            );
            Color32 [ ] handlecolors = new Color32 [
                safeX * safeY
            ];
            Color color =
                Package .Channel1 != ThemeChannel .Clear
                ? Color .white
                : Color .clear
            ;
            for ( int i = 0; i < handlecolors .Length; i++ ) handlecolors [ i ] = color;
            if ( color == Color .clear ) { goto cut; }
            int r = ( int ) ( Mathf .Min ( texture .width, texture .height ) * 0.5 * Package .Radius );
            r = Mathf .Max ( r, 1 );
            handlecolors = ClearCorner ( handlecolors, 0, 0, r );
            handlecolors = ClearCorner ( handlecolors, texture .width, 0, r );
            handlecolors = ClearCorner ( handlecolors, 0, texture .height, r );
            handlecolors = ClearCorner ( handlecolors, texture .width, texture .height, r );
            
            texture .SetPixels32 ( handlecolors );
            texture .Apply ( );
            cut:
            Sprite sprite = Sprite .Create (
                texture,
                new Rect ( 0, 0, texture.width, texture.height ),
                otAPI.V2Center
            );
            string key =
                $"{ texture .width }x{ texture .height }_{ Package .Radius }";
            
            if ( !otAPI .spriteCache .TryGetValue ( key, out Sprite _ ) ) {
                otAPI .spriteCache .Add ( key, sprite );
            }
            return sprite;
            Color32 [ ] ClearCorner (
                Color32 [ ] cols,
                int cx, int cy, int r
            ) {
                int _x; int _y;
                if ( cx == 0 ) _x = r;
                else _x = cx - r;
                if ( cy == 0 ) _y = r;
                else _y = cy - r;

                for ( int x = cx - r; x <= cx + r; x++ ) {
                    for ( int y = cy - r; y <= cy + r; y++ ) {
                        if ( x >= 0 && x < texture .width && y >= 0 && y < texture .height ) {
                            float dist = Vector2 .Distance ( new Vector2 ( x, y ), new Vector2 ( _x, _y ) );
                            if ( dist > r ) {
                                cols [ x + y * texture .width ] = Color .clear;
                            }
                        }
                    }
                }
                return cols;
            }
        }
        public void Retheme (
            UITheme theme
        ) {
            bg .color = theme .GetChannel ( Channel2, theme );
            fill .color = theme .GetChannel ( Channel1, theme );
            handle .color = theme .GetChannel ( Channel1, theme );
            /*textmesh .color = theme .GetChannel ( TextChannel, theme );
            placeholder .color = theme .GetChannel ( TextChannel, theme );*/
        }
    }
}