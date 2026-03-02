using System .Collections;

using UnityEngine;
using UnityEngine .EventSystems;
using UnityEngine .UI;

using DG .Tweening;

namespace _otAPI {
    public class UIImage : MonoBehaviour {
        
        public Vector2 storedSize { get; private set; }
        public UIPackage UIP { get; set; }
        internal IEnumerator Initialize (
            UIPackage Package
        ) {
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            if ( Package .StartInactive ) gameObject .SetActive ( false );
            Image image = gameObject .AddComponent < Image > ( );
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Image";
            Sprite sprite;
            string key = $"UIImage_{ Package .Path }";
            if ( otAPI .spriteCache .TryGetValue ( key, out Sprite outsprite ) ) {
                sprite = outsprite;
            } else { sprite =
                otAPI .LoadSprite ( Package .Path, Package .ImgSize, Package .Assembly );
            }
            image .sprite = sprite;

            gameObject .transform .SetParent ( Package .Parent .transform, false );
            RectTransform rect = gameObject .GetComponent < RectTransform > ( );
            otAPI .SetAnchoredPos ( rect, Package );
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
        }
        public void CreateHoverBehavior (
            float Ratio
        ) {
            RectTransform rect = GetComponent < RectTransform > ( );
            storedSize = rect .sizeDelta;
            EventTrigger ev = otAPI .AddOrGet < EventTrigger > ( gameObject );
            EventTrigger .Entry hovering = new ( );
            hovering .eventID = EventTriggerType .PointerEnter;
            hovering .callback .AddListener ( ( data ) => {
                    if ( gameObject == null ) return;
                    //rect .sizeDelta *= Ratio;
                    DOTween .To (
                        ( ) => rect .sizeDelta,
                        size => rect .sizeDelta = size,
                        rect .sizeDelta * Ratio,
                        0.1f
                    );
                }
            );
            EventTrigger .Entry exiting = new ( );
            exiting .eventID = EventTriggerType .PointerExit;
            exiting .callback .AddListener ( ( data ) => {
                    if ( gameObject == null ) return;
                    //rect .sizeDelta = storedSize;
                    DOTween .To (
                        ( ) => rect .sizeDelta,
                        size => rect .sizeDelta = size,
                        storedSize,
                        0.1f
                    );
                }
            );
            ev .triggers .Add ( hovering );
            ev .triggers .Add ( exiting );
        }
    }
}
