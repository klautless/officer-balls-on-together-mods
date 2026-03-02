using UnityEngine;

namespace _otAPI {
    public partial class otAPI {
        public static Vector2 Vec2 ( float value ){ return new Vector2 ( value, value ); }
        public static Vector2 Vec3to2 ( Vector3 vec3 ) { return new Vector2 ( vec3 .x, vec3 .y ); }
        public static Vector3 Vec2to3 ( Vector2 vec2 ) { return new Vector3 ( vec2 .x, vec2 .y, 0 ); }
        public static Vector2 CenterOf ( Vector2 vec2 ) { return new Vector2 ( vec2 .x / 2, vec2 .y / 2 ); }
        
        public static void PivotAndAnchor (
            RectTransform Rect,
            RectTransform ParentRect,
            AnchorType Anchor,
            Vector2 PivotPos
        ) {
            switch ( Anchor ) {
                case AnchorType .TopLeft:
                    Rect .pivot  = new Vector2 ( 0, 1 );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .TopCenter:
                    Rect .pivot = new Vector2 ( 0.5f, 1 );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .TopRight:
                    Rect .pivot = new Vector2 ( 1, 1 );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .Left:
                    Rect .pivot = new Vector2 ( 0, 0.5f );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .Center:
                    Rect .pivot = V2Center;
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .pivot;
                break;
                case AnchorType .Right:
                    Rect .pivot = new Vector2 ( 1, 0.5f );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .BottomLeft:
                    Rect .pivot = Vector2 .zero;
                    Rect .anchorMin = Vector2 .zero;
                    Rect .anchorMax = Vector2 .zero;
                break;
                case AnchorType .BottomCenter:
                    Rect .pivot = new Vector2 ( 0.5f, 0 );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
                case AnchorType .BottomRight:
                    Rect .pivot = new Vector2 ( 1, 0 );
                    Rect .anchorMin = Rect .pivot;
                    Rect .anchorMax = Rect .anchorMin;
                break;
            }
        }
        public static void SetAnchoredPos ( // yeah, i know
            RectTransform Rect,             // hope glues and prayers
            UIPackage Package
        ) {
            RectTransform ParentRect =
                Package .Parent == rootHUD
                ? null
                : Package .Parent .GetComponent < RectTransform > ( )
            ;
            UIPanel UP =
                ParentRect == null
                ? null
                : ParentRect .GetComponent < UIPanel > ( )
            ;
            if ( UP == null ) { } else if ( UP .LE == null ) { }
            else if ( !UP . fixedSize ) { } else {
                Rect .sizeDelta = new Vector2 (
                    Package .Size .x * UP .LE .preferredWidth,
                    Package .Size .y * UP .LE .preferredHeight
                );
                Rect .anchoredPosition = new Vector2 (
                    Package .Position .x * UP .LE .preferredWidth / 2,
                    Package .Position .y * UP .LE .preferredHeight / 2
                );
                return;
            }

            PivotAndAnchor (
                Rect,
                ParentRect,
                Package .AnchorType,
                Package .Position
            );
            Vector2 Pos = Package .Position;
            Vector2 Size = Package .Size;
            bool Unclamped = Package .Unclamped;
            Vector2 screenCenter = new Vector2 ( Screen .width * 0.5f, Screen .height * 0.5f );
            Vector2 screenOffset = new Vector2 ( Pos .x * Screen .width * 0.5f, Pos.y * Screen .height * 0.5f );
            Vector2 screenPoint = screenCenter + screenOffset;

            RectTransformUtility .ScreenPointToLocalPointInRectangle (
                ParentRect ?? Canvas .GetComponent < RectTransform > ( ),
                screenPoint, null,
                out Vector2 localPoint
            );
            if ( Package .overrideSize ) {
                //Rect.anchoredPosition = localPoint;

                Rect .anchoredPosition =
                    ParentRect != null
                    ? ParentRect .rect .size *
                        Pos * 0.5f * ScaleFactor
                    : localPoint;
                ;
                if ( ParentRect != null) {
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Horizontal,
                        Screen .width * Size .x / ScaleFactor
                    );
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Vertical,
                        Screen .height * Size .y / ScaleFactor
                    );
                }
                if ( ParentRect != null ) {
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Horizontal,
                        Mathf .Abs ( ParentRect .rect .width * Size .x )
                    );
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Vertical,
                        Mathf .Abs ( ParentRect .rect .height * Size .y )
                    );   
                } else {
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Horizontal,
                        Screen .width * Size .x / ScaleFactor
                    );
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Vertical,
                        Screen .height * Size .y / ScaleFactor
                    );
                }
            } else if ( Unclamped ) {
                Rect .anchoredPosition =
                    ParentRect != null
                    ? ParentRect .rect .size *
                        Pos * ScaleFactor
                    : localPoint
                ;
                if ( Package .ImgScale != 1f ) {
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Horizontal,
                        Package .ImgSize .x / ScaleFactor * Package .ImgScale
                    );
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Vertical,
                        Package .ImgSize .y / ScaleFactor * Package .ImgScale
                    );
                } else {
                    if ( ParentRect != null ) {
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Horizontal,
                            Mathf .Abs ( ParentRect .rect .width * Size .x )
                        );
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Vertical,
                            Mathf .Abs ( ParentRect .rect .height * Size .y )
                        );   
                    } else {
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Horizontal,
                            Screen .width * Size .x / ScaleFactor
                        );
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Vertical,
                            Screen .height * Size .y / ScaleFactor
                        );
                    }
                }
            } else {
                Vector2 clampedSize = new Vector2 (
                    Mathf .Clamp01 ( Size .x ),
                    Mathf .Clamp01 ( Size .y )
                );
                if ( Package .ImgScale != 1f ) {
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Horizontal,
                        Package .ImgSize .x / ScaleFactor * Package .ImgScale
                    );
                    Rect .SetSizeWithCurrentAnchors (
                        RectTransform .Axis .Vertical,
                        Package .ImgSize .y / ScaleFactor * Package .ImgScale
                    );
                } else {
                    if ( ParentRect != null ) {
                        float minWidth = Mathf .Max ( 1, ParentRect .rect .width * clampedSize .x );
                        float minHeight = Mathf .Max ( 1, ParentRect .rect .height * clampedSize .y);
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Horizontal,
                            Mathf .Abs ( ParentRect .rect .width * clampedSize .x )
                        );
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Vertical,
                            Mathf .Abs ( ParentRect .rect .height * clampedSize .y )
                        );   
                    } else {
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Horizontal,
                            Screen .width * Size .x / ScaleFactor
                        );
                        Rect .SetSizeWithCurrentAnchors (
                            RectTransform .Axis .Vertical,
                            Screen .height * Size .y / ScaleFactor
                        );
                    }
                }
            
            Vector2 pos = ParentRect != null
                ? ParentRect .rect .size * Pos * 0.5f
                : localPoint
            ;

            Vector2 halfSize = Rect .rect .size * 0.5f;
            Vector2 parentSize = ParentRect != null
                ? ParentRect .rect .size
                : new Vector2 ( Screen .width / ScaleFactor, Screen .height / ScaleFactor )
            ;
            Vector2 max = parentSize * 0.5f;
            Vector2 min = -max;

            pos.x = Mathf .Clamp ( pos .x, min .x + halfSize .x, max .x - halfSize .x );
            pos.y = Mathf .Clamp ( pos .y, min .y + halfSize .y, max .y - halfSize .y );

            Rect.anchoredPosition = Package .BuildOffscreen ? new Vector2 ( 0, 100000f ) : pos;
            }
        }
    }
}