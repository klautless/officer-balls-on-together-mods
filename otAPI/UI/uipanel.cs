using System;
using System .Collections;
using System .Collections .Generic;

using UnityEngine;
using UnityEngine .EventSystems;
using UnityEngine .UI;

namespace _otAPI {
    public class UIPanel : MonoBehaviour {

        public RectTransform rect { get ; private set; }
        public UIPackage UIP { get; set; }
        public bool SkipRetheme { get; private set; } = false;
        internal LayoutElement LE;
        internal bool fixedSize = false;
        private Image image;
        public ThemeChannel mainChannel;
        public ThemeChannel hoverChannel;
        private Color mainColor;
        private Color hoverColor;
        private CanvasRenderer renderer;
        internal IEnumerator Initialize (
            UIPackage Package
        ) {
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Panel";
            bool allowPassthrough = Package .Channel1 == ThemeChannel .Clear;
            if ( Package .Bools == null ) { } else {
                if ( Package .Bools .ContainsKey ( "allowPassthrough" ) ) {
                    allowPassthrough = Package .Bools [ "allowPassthrough" ];
                }
            }
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            if ( Package .Aspect == null ) { } else {
                AspectRatioFitter Aspect = gameObject .AddComponent < AspectRatioFitter > ( );
                if ( Aspect == null ) { } else {
                    switch ( ( ( AspectGroup) Package .Aspect ) .Mode ) {
                        case AspectGroup.Modes.None: break;
                        case AspectGroup.Modes.WidthControlsHeight:
                            Aspect .aspectMode = AspectRatioFitter .AspectMode .WidthControlsHeight; break;
                        case AspectGroup.Modes.HeightControlsWidth:
                            Aspect .aspectMode = AspectRatioFitter .AspectMode .HeightControlsWidth; break;
                        case AspectGroup.Modes.FitInParent:
                            Aspect .aspectMode = AspectRatioFitter .AspectMode .FitInParent; break;
                        case AspectGroup.Modes.EnvelopeParent:
                            Aspect .aspectMode = AspectRatioFitter .AspectMode .EnvelopeParent; break;
                    }
                    //Aspect .aspectMode = Package .Aspect .aspectMode;
                    Aspect .aspectRatio = ( (AspectGroup ) Package .Aspect ) .Ratio;
                }
                Package .Aspect = null;
            }
            SkipRetheme = Package .SkipsRethemes;
            if ( Package .StartInactive ) gameObject .SetActive ( false );
            if ( gameObject .transform == null || Package .Parent == null ) yield break;
            if ( Package .Parent .transform == null ) yield break;
            gameObject .transform .SetParent ( Package .Parent .transform, false );
            rect = otAPI .AddOrGet < RectTransform > ( gameObject );
            otAPI .SetAnchoredPos ( rect, Package );
            yield return null;
            if ( ( Package .ScrollRect != null
                || transform .parent .TryGetComponent ( typeof ( ContentSizeFitter ), out _ ) )
                && !Package .LaidOut
            ) {
                ScrollRect sr = Package .ScrollRect ?? transform .parent .parent .parent .GetComponent < ScrollRect > ( );
                LE = otAPI .AddOrGet < LayoutElement > ( gameObject );
                float Width =
                    Package .Shrink *
                    Package .Size .x *
                    sr .GetComponent
                    < RectTransform > ( ) .rect .width
                ;
                float Height =
                    Package .Shrink *
                    Package .Size .y *
                    sr .GetComponent
                    < RectTransform > ( ) .rect .height
                ;
                //if ( minWidth > LE .minWidth ) LE .minWidth = minWidth;
                //if ( minHeight > LE .minHeight ) LE .minHeight = minHeight;
                LE .preferredWidth = Width; // LE .flexibleWidth = 0;
                if ( Package .Expands ) LE .minHeight = Height;
                else { 
                    LE .preferredHeight = Height; LE .flexibleHeight = 0;
                    fixedSize = true;
                }
                LayoutRebuilder .ForceRebuildLayoutImmediate ( rect );
                Package .LaidOut = true;
            }
            image = otAPI .AddOrGet < Image > ( gameObject );
            renderer = otAPI .AddOrGet < CanvasRenderer > ( gameObject );
            if ( image == null || renderer == null ) yield break;
            Sprite sprite;
            string key = $"{ Math .Abs ( ( int ) rect .rect .size .x ) }x{ Math .Abs ( ( int ) rect .rect .size .y ) }_{ Package .Radius }";
            renderer .cull = true;
            yield return null;
            if ( otAPI .spriteCache .TryGetValue ( key, out Sprite outsprite ) ) {
                sprite = outsprite;
                yield return null;
            } else {
                sprite = BakeSprite ( Package );
                yield return null;
            }
            Vector2 spriteSize = sprite .rect .size;
            while ( spriteSize .x <= 1 || spriteSize .y <= 1 ) {
                sprite = BakeSprite ( Package );
                yield return null;
                spriteSize = sprite .rect .size;
            }
            if ( image == null || sprite == null ) yield break;
            image .sprite = sprite;
            mainColor = Package .Theme .GetChannel ( Package .Channel1, Package .Theme );
            hoverColor = mainColor;
            image .color = mainColor;
            mainChannel = Package .Channel1;
            hoverChannel = Package .Channel2;
            if ( allowPassthrough ) image .raycastTarget = false;
            renderer .cull = false;
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
            IEnumerator recursor = otAPI .RecursiveCreation ( Package, gameObject );
            if ( recursor == null ) { yield break; } else {
                while ( recursor .MoveNext ( ) ) {
                    yield return recursor .Current;
                }
            }
            yield return null;
        }
        internal Sprite BakeSprite ( UIPackage Package ) {
            // Debug .Log ( $"Incoming texture attempt: { Math .Abs ( ( int ) rect .rect .size .x ) } x { Math .Abs ( ( int ) rect .rect .size .y ) }" );
            int safeX = Math .Max ( Math .Abs ( ( int ) rect .rect .width ), 1 );
            int safeY = Math .Max ( Math .Abs ( ( int ) rect .rect .height ), 1 );
            Texture2D texture = new Texture2D (
                safeX, safeY
            );
            Color32 [ ] panelcolors = new Color32 [
                safeX * safeY
            ];
            Color color =
                Package .Channel1 != ThemeChannel .Clear
                ? Color .white
                : Color .clear
            ;
            for ( int i = 0; i < panelcolors .Length; i++ ) panelcolors [ i ] = color;
            if ( color == Color .clear ) { goto cut; }
            int r = ( int ) ( Mathf .Min ( texture .width, texture .height ) * 0.5 * Package .Radius );
            r = Mathf .Max ( r, 1 );
            panelcolors = ClearCorner ( panelcolors, 0, 0, r );
            panelcolors = ClearCorner ( panelcolors, texture .width, 0, r );
            panelcolors = ClearCorner ( panelcolors, 0, texture .height, r );
            panelcolors = ClearCorner ( panelcolors, texture .width, texture .height, r );
            
            texture .SetPixels32 ( panelcolors );
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
        public void CreateHoverBehavior (
            UITheme Theme,
            ThemeChannel flatChannel,
            ThemeChannel hoverChannel
        ) {
            mainColor = Theme .GetChannel ( flatChannel, Theme );
            hoverColor = Theme .GetChannel ( hoverChannel, Theme );
            Reset ( );
            EventTrigger ev = otAPI .AddOrGet < EventTrigger > ( gameObject );
            EventTrigger .Entry hovering = new ( );
            hovering .eventID = EventTriggerType .PointerEnter;
            hovering .callback .AddListener ( ( data ) => {
                    if ( gameObject == null ) return;
                    Recolor ( hoverColor );
                }
            );
            EventTrigger .Entry exiting = new ( );
            exiting .eventID = EventTriggerType .PointerExit;
            exiting .callback .AddListener ( ( data ) => {
                    if ( gameObject == null ) return;
                    Recolor ( mainColor );
                }
            );
            ev .triggers .Add ( hovering );
            ev .triggers .Add ( exiting );
        }
        public void Retheme (
            UITheme Theme,
            bool Recursive = true
        ) {
            if ( SkipRetheme ) return;
            mainColor = Theme .GetChannel ( mainChannel, Theme );
            hoverColor = Theme .GetChannel ( hoverChannel, Theme );
            Reset ( );

            if ( !Recursive ) return;
            List < UIPanel > subPanels = new ( );
            void FindAllUIPanels ( Transform parent ) {
                for ( int i = 0; i < parent .childCount; i++ ) {
                    Transform child = parent .GetChild ( i );
                    if ( child .TryGetComponent ( typeof ( UIPanel ), out Component component ) ) {
                        subPanels .Add ( ( UIPanel ) component );
                    }
                    FindAllUIPanels ( child );
                }
            }
            FindAllUIPanels ( transform );
            float delay = 0.01f;
            foreach ( UIPanel p in subPanels ) {
                otAPI .RunDelayed ( ( ) => { p .Retheme ( Theme, false ); }, delay );
                delay += 0.01f;
            }
            delay = 0.01f;

            List < UIText > subText = new ( );
            void FindAllUIText ( Transform parent ) {
                for ( int i = 0; i < parent .childCount; i++ ) {
                    Transform child = parent .GetChild ( i );
                    if ( child .TryGetComponent ( typeof ( UIText ), out Component component ) ) {
                        subText .Add ( ( UIText ) component );
                    }
                    FindAllUIText ( child );
                }
            }
            FindAllUIText ( transform );
            foreach ( UIText t in subText ) {
                otAPI .RunDelayed ( ( ) => { t .Retheme ( Theme ); }, delay );
                delay += 0.01f;
            }
            delay = 0.01f;
            List < UIInput > subInput = new ( );
            void FindAllUIInput ( Transform parent ) {
                for ( int i = 0; i < parent .childCount; i++ ) {
                    Transform child = parent .GetChild ( i );
                    if ( child .TryGetComponent ( typeof ( UIInput ), out Component component ) ) {
                        subInput .Add ( ( UIInput ) component );
                    }
                    FindAllUIInput ( child );
                }
            }
            FindAllUIInput ( transform );
            foreach ( UIInput u in subInput ) {
                otAPI .RunDelayed ( ( ) => { u .Retheme ( Theme ); }, delay );
                delay += 0.01f;
            }
            delay = 0.01f;
            List < UISlider > subSlider = new ( );
            void FindAllUISlider ( Transform parent ) {
                for ( int i = 0; i < parent .childCount; i++ ) {
                    Transform child = parent .GetChild ( i );
                    if ( child .TryGetComponent ( typeof ( UISlider ), out Component component ) ) {
                        subSlider .Add ( ( UISlider ) component );
                    }
                    FindAllUISlider ( child );
                }
            }
            FindAllUISlider ( transform );
            foreach ( UISlider u in subSlider ) {
                otAPI .RunDelayed ( ( ) => { u .Retheme ( Theme ); }, delay );
                delay += 0.01f;
            }
        }
        public void Reset ( ) { image .color = mainColor; }
        public void Recolor ( Color newColor ) { image .color = newColor; } // use manually with caution
    }
}