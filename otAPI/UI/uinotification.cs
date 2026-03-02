using System .Collections;

using UnityEngine;
using UnityEngine .UI;

using DG .Tweening;
using TMPro;
using PurrNet;

namespace _otAPI {
    public class UINotificationTray : MonoBehaviour {
        public UIPackage NotificationPackage { get; internal set; }
        public UIPackage UIP { get; set; }
        public UIPanel trayPanel;
        
        internal float fadeSpeed = 0.3f;
        internal float moveRate = 0.1f;
        internal Vector2 Direction;
        internal float Spacing;
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
                : "UI Notification Tray"
            ;
            UIPackage traypkg = Package with {
                Children = null,
                Type = UIType .Panel
            };

            trayPanel = gameObject .AddComponent < UIPanel > ( );

            IEnumerator tr = trayPanel .Initialize ( traypkg );
            if ( tr == null ) { yield break; } else {
                while ( tr .MoveNext ( ) ) {
                    yield return tr .Current;
                }
            }
            //tray = otAPI .AppList [ Package .DepotFolder ] .Buffer .Get;
            //tray .transform .SetParent ( gameObject .transform, false );
            NotificationPackage = traypkg with {
                ObjectName = "UI Notification",
                AnchorType = AnchorType .Center,
                Channel1 = Package .Channel2,
                Position = Package .SubPosition,
                Size = Package .SubSize,
                SkipsRethemes = true,
                Radius = Package .SubRadius,
                Parent = gameObject
            };
            Spacing = Package .Spacing;
            Direction = Package .Direction;
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
            if ( otAPI .mainTray == null ) { otAPI .mainTray = this; }
            otAPI .AppList [ Package .DepotFolder ] .Buffer .Set ( gameObject );
            IEnumerator recursor = otAPI .RecursiveCreation ( Package, gameObject );
            if ( recursor == null ) { yield break; } else {
                while ( recursor .MoveNext ( ) ) {
                    yield return recursor .Current;
                }
            }
        }
        public IEnumerator Notify ( string notification ) {
            while ( otAPI .isDeleting ) yield return null;
            if ( transform .childCount > 0 ) {
                for ( int n = transform .childCount - 1; n >= 0; n-- ) {
                    Transform child = transform .GetChild ( n );
                    if ( child == null ) continue;
                    GameObject _game = transform .GetChild ( n ) .gameObject;
                    if ( _game == null ) continue;
                    if ( _game .TryGetComponent ( typeof ( UINotification ), out Component noti ) ) {
                        Vector3 _vec3 = child .localPosition;
                        Vector2 _dir = Direction;
                        Vector2 _ns = GetComponent < RectTransform > ( ) .rect .size *
                            NotificationPackage .Size;
                        float _s = Spacing;
                        _vec3 .x += _dir .x * _ns .x / otAPI .ScaleFactor * _s;
                        _vec3 .y += _dir .y * _ns .y / otAPI .ScaleFactor * _s;
                        child .DOLocalMove ( _vec3, moveRate );
                    }
                }
            }
            GameObject UINGO = new GameObject ( "UINotification" );
            UINGO .transform .SetParent ( transform, false );
            UINotification UIN = UINGO .AddComponent < UINotification > ( );
            IEnumerator uini = UIN .Initialize ( notification, this );
            if ( uini == null ) { yield break; } else {
                while ( uini .MoveNext ( ) ) {
                    yield return uini .Current;
                }
            }
        }
    }
    public class UINotification : MonoBehaviour {
        public UINotificationTray Tray = null;
        public GameObject np;
        internal Image image;
        internal TMP_Text tmp;
        internal float duration;
        internal bool running = false;
        internal bool initFailed = false;
        internal IEnumerator Initialize (
            string text,
            UINotificationTray tray
        ) {
            Tray = tray;
            UIPackage _k = tray .NotificationPackage; // with { Parent = this .gameObject };
            duration = _k .Duration;
            if ( this == null ) { initFailed = true; yield break; }
            else {
                UIPanel p = gameObject .AddComponent < UIPanel > ( );

                IEnumerator nk = p .Initialize ( _k );
                if ( nk == null ) { initFailed = true; yield break; } else {
                    while ( nk .MoveNext ( ) ) {
                        yield return nk .Current;
                    }
                }
            }
            np = gameObject;
            if ( np == null ) { initFailed = true; yield break; }
            //np = otAPI .CreateUI < UIPanel > ( _k ) .gameObject;
            UIPackage _t = _k with {
                ObjectName = "UI Notification Text",
                AnchorType = AnchorType .Left,
                Position = new Vector2 ( 0.05f, 0 ),
                Size = new Vector2 ( 0.975f, 1f ),
                Unclamped = true,
                Parent = np,
                Type = UIType .Text
            };
            
            Vector3 vec3 = transform .localPosition;
            Vector2 _dir = _k .Direction;
            RectTransform Rect = GetComponent < RectTransform > ( );
            if ( Rect == null ) { initFailed = true; yield break; }
            Vector2 _size = Rect .rect .size;
            vec3 .x += _dir .x * _size .x;
            vec3 .y += _dir .y * _size .y;
            transform .DOLocalMove ( vec3, tray .moveRate );
            image = otAPI .AddOrGet < Image > ( gameObject );
            if ( image == null ) { initFailed = true; yield break; }
            IEnumerator tk = otAPI .CreateUI ( _t );
            if ( tk == null ) { initFailed = true; yield break; } else {
                while ( tk .MoveNext ( ) ) {
                    yield return tk .Current;
                }
            }
            UIText Text = otAPI .AppList [ tray .NotificationPackage .DepotFolder ]
                .Buffer .Get .GetComponent < UIText > ( );
            if ( Text == null ) { initFailed = true; yield break; }
            Text .SetString ( text );
            tmp = Text .Text;
            tmp .overflowMode = TextOverflowModes .Ellipsis;
            running = true;
        }
        void Update ( ) {
            if ( initFailed ) {
                initFailed = false;
                if ( np == null ) { }
                else UnityProxy .Destroy ( np );
                UnityProxy .Destroy ( this );
            }
            if ( running ) {
                if ( duration > 0) {
                    duration -= Time .deltaTime;
                }
                else {
                    running = false;
                    DOTween .To (
                        ( ) => tmp .alpha,
                        alpha => tmp .alpha = alpha,
                        0f,
                        Tray .fadeSpeed / 2
                    );
                    DOTween .To (
                        ( ) => image .color,
                        color => image .color = color,
                        Color .clear,
                        Tray .fadeSpeed
                    ) .OnComplete ( ( ) => {
                            UnityProxy .Destroy ( np );
                            UnityProxy .Destroy ( this );
                        }
                    );
                }
            }
        }
    }
}