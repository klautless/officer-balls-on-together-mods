using System;
using System .Collections;
using System .Collections .Generic;

using HarmonyLib;

using UnityEngine;

using TMPro;
using PurrNet;

namespace _otAPI {
    public partial class otAPI {
        internal static IEnumerator Construction ( ) {
            while ( ConstructionQueue .TryPeek ( out KeyValuePair < string, UIPackage > _ ) ) {
                
                KeyValuePair < string, UIPackage > job = ConstructionQueue .Dequeue ( );
                if ( !AppList .ContainsKey ( job .Key ) ) {
                    Debug .Log ( $"otAPI: AppList missing key: { job .Key }" );
                    continue;
                }
                if ( job .Value .Prefabs != null ) AppList [ job .Key ] .Prefabs = job .Value .Prefabs;
                if ( job .Value .Tasks != null ) AppList [ job .Key ] .Tasks = job .Value .Tasks;
                IEnumerator Runner = CreateUI ( job .Value );
                if ( Runner == null ) {
                    Debug .Log ( "otAPI: CreateUI returned null for job: " + job .Value .ObjectName );
                    yield return null;
                } else {
                    while ( Runner .MoveNext ( ) ) {
                        yield return Runner .Current;
                    }
                }
                if ( job .Value .PostBuild != null ) {
                    yield return null;
                    try {
                        job .Value .PostBuild .Invoke ( );
                    } catch ( Exception ex ) {
                        Debug .Log ( $"otAPI: post build failed for { job .Value .ObjectName } UIPackage / { ex }" );
                    }
                }
                else yield return null;
                yield return null;
            }
            yield return null;
            yield return null;
            ConstructionFree = true;
        }
        public static IEnumerator QueueJob (
            KeyValuePair < string, UIPackage > job,
            Action PostOrders = null
        ) {
            ConstructionQueue .Enqueue ( job );

            while ( !ConstructionFree ) {
                yield return null;
            }
            ConstructionFree = false;
            ConstructionRoutine = RunCoroutine ( Construction ( ), true );
            if ( ConstructionRoutine == null ) yield break; else { 
                while ( ConstructionRoutine .MoveNext ( ) ) {
                    yield return ConstructionRoutine .Current;
                }
            }
            if ( PostOrders != null ) { PostOrders .Invoke ( ); yield return null; }
        }
        public static IEnumerator QueueJobs (
            KeyValuePair < string, List < UIPackage > > jobs,
            Action PostOrders = null
        ) {
            foreach ( UIPackage job in jobs .Value ) {
                ConstructionQueue .Enqueue ( KeyValuePair .Create ( jobs .Key, job ) );
                yield return null;
                yield return null;
            }
            while ( !ConstructionFree ) {
                yield return null;
            }
            ConstructionFree = false;
            ConstructionRoutine = RunCoroutine ( Construction ( ), true );
            if ( ConstructionRoutine == null ) {
                Debug .Log ( $"otAPI: construction routine went null during { jobs .Key }" );
                yield break;
            } else {
                while ( ConstructionRoutine .MoveNext ( ) ) {
                    yield return ConstructionRoutine .Current;
                }
            }
            if ( PostOrders != null ) { PostOrders .Invoke ( ); yield return null; }
        }
        internal static IEnumerator CreateUI ( 
            UIPackage Package,
            Depot depot = null
        ) {
            if ( Package .DepotFolder == " ") {
                Debug .Log ( $"otAPI: Creation issue on { Package .ObjectName }: Missing DepotFolder");
                Debug .Log ( "cancelling construction.");
                yield break;
            }
            if ( Package .Type == UIType .Text ) {
                LoadingScreenManager lsi = MonoSingleton < LoadingScreenManager > .I;
                TMP_Text loadingSample = AccessTools
                    .FieldRefAccess < LoadingScreenManager, TMP_Text >
                    ( "_connectingText" )
                    ( lsi )
                ;
                TMP_Text TMP = UnityProxy .Instantiate ( loadingSample );
                UIText text = TMP .gameObject .AddComponent < UIText > ( );
                IEnumerator init = text .Initialize ( Package, TMP );
                if ( init == null ) {
                    yield break;
                } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Panel ) {
                GameObject o = new GameObject ( );
                UIPanel newPanel = o .AddComponent < UIPanel > ( );
                IEnumerator init = newPanel .Initialize ( Package );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Scrollable ) {
                GameObject o = new GameObject ( );
                UIPanel newPanel = o .AddComponent < UIPanel > ( );
                UIPackage PanelPackage = Package with { Children = null, ObjectName = "UIScrollable Container" };
                IEnumerator init = newPanel .Initialize ( PanelPackage );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
                UIScrollable newScrollable = o .AddComponent < UIScrollable > ( );
                init = newScrollable .Initialize ( Package );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Image ) {
                GameObject o = new GameObject ( );
                UIImage newImg = o .AddComponent < UIImage > ( );
                IEnumerator init = newImg .Initialize ( Package );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Button ) {
                GameObject o = new GameObject ( );
                UIPanel newPanel = o .AddComponent < UIPanel > ( );
                UIPackage PanelPackage = Package with { Children = null, ObjectName = "ButtonPanel" };
                IEnumerator init = newPanel .Initialize ( PanelPackage );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
                UIButton newButton = o .AddComponent < UIButton > ( );
                init = newButton .Initialize ( Package, newPanel );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Slider ) {
                GameObject reference = GameObject .Find ( "/Canvas_Settings/Image_Bg/Panel_Main/Panel_Audio/Viewport/Content/MasterAudio/Slider" );
                GameObject o = Instantiate ( reference, Package .Parent .transform );
                UISlider newSlider = o .AddComponent < UISlider > ( );
                IEnumerator init = newSlider .Initialize ( Package );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Input ) {
                UIManager uimi = MonoSingleton < UIManager > .I;
                TMP_InputField inputSample = AccessTools
                    .FieldRefAccess < UIManager, TMP_InputField >
                    ( "_todoListHeaderTextObject" )
                    ( uimi )
                ;
                TMP_InputField TMPI = UnityProxy .Instantiate ( inputSample );
                UIInput newInput = TMPI .gameObject .AddComponent < UIInput > ( );
                IEnumerator init = newInput .Initialize ( Package, TMPI );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .NotificationTray ) {
                GameObject o = new GameObject ( );
                o .transform .SetParent ( Package .Parent .transform, false );
                o .transform .localPosition = Vector3 .zero;
                UINotificationTray newTray = o .AddComponent < UINotificationTray > ( );
                IEnumerator init = newTray .Initialize ( Package );
                if ( init == null ) { yield break; } else {
                    while ( init .MoveNext ( ) ) {
                        yield return init .Current;
                    }
                }
            } else if ( Package .Type == UIType .Null ) {
                Debug .Log ( $"otAPI: construction cancelled for { Package .ObjectName }. remember to specify a UIType." );
            }
        }
        internal static IEnumerator RecursiveCreation (
            UIPackage Package,
            GameObject _Parent
        ) {
            if ( Package .Children != null ) {
                for ( int i = 0; i < Package .Children .Count; i++ ) {
                    int I = i;
                    UIPackage _pck = Package .Children [ I ] with {
                        Parent = _Parent,
                        ScrollRect = Package .ScrollRect,
                        LaidOut = Package .LaidOut,
                        DepotFolder = Package .DepotFolder,
                        StorePackage = false,
                        Aspect = null
                    };
                    Package .Children [ I ] = _pck;
                    IEnumerator subCreate = CreateUI ( Package .Children [ I ] );
                    if ( subCreate == null ) { yield break; } else {
                        while ( subCreate .MoveNext ( ) ) {
                            yield return subCreate .Current;
                        }
                    }
                }
            }
        }
    }
}