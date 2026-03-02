using System .Collections;

using UnityEngine;
using UnityEngine .UI;

namespace _otAPI {
    public class UIScrollable : MonoBehaviour {
        public GameObject ScrollContainer { get; private set; }
        public GameObject Content { get; private set; }
        public ScrollRect ScrollRect { get; private set; }
        public RectTransform ContentRect { get; private set; }
        public UIPackage UIP { get; set; }
        internal IEnumerator Initialize (
            UIPackage Package
        ) {
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            UIPackage subPackage = Package with {
                ObjectName = "UI Scrollable Viewport",
                Type = UIType .Panel,
                Position = Vector2 .zero,
                Size = otAPI .Vec2 ( Package .Shrink ),
                Parent = gameObject,
                Channel1 = Package .Channel2,
                Children = null
            };
            IEnumerator viewnumerator = otAPI .CreateUI ( subPackage );
            if ( viewnumerator == null ) { yield break; } else {
                while ( viewnumerator .MoveNext ( ) ) {
                    yield return viewnumerator .Current;
                }
            }
            ScrollRect = gameObject .AddComponent < ScrollRect > ( );
            ScrollRect .horizontal = false; ScrollRect .vertical = true;
            ScrollRect .scrollSensitivity = 10f;
            GameObject viewport = otAPI .AppList [ Package .DepotFolder ] .Buffer .Get;
            Mask viewportMask = viewport .AddComponent < Mask > ( );
            RectTransform ViewportRect = otAPI .AddOrGet < RectTransform > ( viewport );
            
            GameObject content = new GameObject (
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Scrollable Contents"
            );
            ScrollTunnel ST = content .AddComponent < ScrollTunnel > ( );
            ST .ScrollRect = ScrollRect;
            ContentRect = content .AddComponent < RectTransform > ( );
            ContentRect .transform .SetParent ( viewport .transform, false );
            ContentRect .anchorMin = otAPI .V2Center;
            ContentRect .anchorMax = otAPI .V2Center;
            ContentRect .pivot = Package .Pivot;

            VerticalLayoutGroup VLG = content .AddComponent < VerticalLayoutGroup > ( );
            VLG .padding .top = Package .SpacePad; VLG .padding .bottom = Package .SpacePad;
            VLG .spacing = Package .SpacePad;
            VLG .childAlignment = TextAnchor .UpperCenter;
            if ( Package .AnchorType == AnchorType .BottomCenter ) VLG .childAlignment = TextAnchor .LowerCenter;
            VLG .childForceExpandWidth = false; VLG .childForceExpandHeight = false;

            ContentSizeFitter CSF = content .AddComponent < ContentSizeFitter > ( );
            CSF .verticalFit = ContentSizeFitter .FitMode .PreferredSize;
            CSF .horizontalFit = ContentSizeFitter .FitMode .PreferredSize;

            ScrollRect .content = ContentRect;
            ScrollRect .viewport = ViewportRect;
                
            
            ScrollContainer = gameObject;
            Content = content;
            Package .ScrollRect = ScrollRect;
            //LayoutRebuilder .ForceRebuildLayoutImmediate ( ContentRect );
            if ( Package .Mark & otAPI .AppList .ContainsKey ( Package .DepotFolder ) ) {
                if ( !otAPI .AppList [ Package .DepotFolder ] .UI .ContainsKey ( Package .ObjectName )
                    | Package .ObjectName == "UI Object"
                ) {
                    otAPI .AppList [ Package .DepotFolder ] .UI .Add (
                        Package .ObjectName != "UI Object"
                            ? Package .ObjectName
                            : $"{ Package .ObjectName }_{ otAPI .AppList [ Package .DepotFolder ] .UI .Count + 1 }",
                        content
                    );
                }
                else {
                    Debug .Log ( $"otAPI construction error during { Package .DepotFolder }." );
                    Debug .Log ( "construction cancelled." );
                    yield break;
                }
            }
            otAPI .AppList [ Package .DepotFolder ] .Buffer .Set ( content );
            IEnumerator recursor = otAPI .RecursiveCreation ( Package, content );
            if ( recursor == null ) { yield break; } else {
                while ( recursor .MoveNext ( ) ) {
                    yield return recursor .Current;
                }
            }
        }
    }
}