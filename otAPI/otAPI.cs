using System;
using System .Collections;
using System .Collections .Generic;
using System .IO;
using System .Linq;
using System .Reflection;
using System .Text .RegularExpressions;

using BepInEx;
using BepInEx .Configuration;
using HarmonyLib;

using UnityEngine;
using UnityEngine .EventSystems;

using PurrNet;

namespace _otAPI {
    public enum Channel { Local, Global, Null }
    public enum AuxTiming { Before, During, After }
    public enum ArgType { String, HexColor, Int, Bool, Float, Null }
    public enum UIType {
        Panel, Text, Image,
        Button, Input, Slider,
        Scrollable,
        NotificationTray,
        Null
    }
    public enum AnchorType {
        TopLeft, TopCenter, TopRight,
        Left, Center, Right,
        BottomLeft, BottomCenter, BottomRight
    }
    public enum SliderType { Vertical, Horizontal }
    public enum ThemeChannel {
        Border, Body, Header, Text,
        Button, Hover, System, SystemHover,
        Clear
    }
    public enum Flags {
        GrabberTarget, MarkAsGrabber
    }
    public enum VerificationError {
        IntExpected, BoolExpected, FloatExpected,
        OutsideRange, BadTypeComparison, BadStringSize, BadHexColor,
        NonOptionalOmitted, None
    }
    [ BepInPlugin (
        modGUID,
        modName,
        modVersion
    ) ]
    public partial class otAPI : BaseUnityPlugin {
        public const string modGUID = "ob.otAPI";
        public const string modName = "otAPI";
        public const string modVersion = "1.0.0.0";
        private const string errPrefix = "otAPI error: ";
        private Harmony harmony = new Harmony( modGUID );

        public static Canvas Canvas { get; internal set; }
        public static float ScaleFactor { get; internal set; }
        public static List < UITheme > themes { get; internal set; }
        public static UITheme Theme { get; internal set; }
        public static Dictionary < string, AppStack > AppList { get; internal set; } = new ( );
        public static GameObject rootHUD { get; internal set; }
        public static UINotificationTray mainTray = null;
        public static event Action < UITheme > ThemeChange;
        public static AudioSource aus { get; internal set; }
        public static Dictionary < string, AudioClip > ClipPool { get ; internal set ; } = [ ];
        public static bool isDeleting { get; private set; } = false;
        public static Vector2 V2Center { get; private set; } =
            new Vector2 ( 0.5f, 0.5f );
        /*public static Dictionary < string, string > colors { get; private set; } =
            new Dictionary < string, string > {
            { "true","<mark=#179c43>" }, { "false","<mark=#c40c2e>" },
            { "int", "<mark=#25acf5>" }, { "float","<mark=#f53141>" }, 
            { "string","<mark=#f58122>" }, { "color","<mark=#ca7ef2>" }
        };*/

        public static ConfigEntry < bool > phone_militaryTime { get; internal set; }
        public static ConfigEntry < string > phone_lastTheme { get; internal set; }
        public static ConfigEntry < Vector2 > phone_lastPosition { get; internal set; }
        public static ConfigEntry < float > phone_Scale { get; internal set; }
        public static ConfigEntry < Vector2 > notificationtray_lastPosition { get; internal set; }
        public static ConfigEntry < string > desiredApps { get; internal set; }
        public static ConfigEntry < bool > dev_suppressWarnings { get; internal set; }

        internal static bool initialized = false;
        internal static Dictionary <  string , Dictionary < string, Action > > updateCycles = new ( );
         public static Cache < string, Sprite > spriteCache { get; internal set; } = new Cache < string, Sprite > ( 250 );
         public static List < Depot > depots { get; internal set; } = new ( );
         public static List < UIPackage > appIcons { get; internal set; } = new ( );
        internal static List < string > passthroughs = new ( );
        internal static string loadHelper = "";
        internal static bool sortedYet = false;

        internal static Coroutine CoreRoutine;
        internal static IEnumerator ConstructionRoutine = null;
        internal static bool ConstructionFree = true;
        internal static Queue < KeyValuePair < string, UIPackage > > ConstructionQueue = new ( );
        internal static RoutineRunner RoutineRunner = null;

        internal static UIPackage mainTrayPackage = new ( ) {
            ObjectName = "Notification Tray",
            DepotFolder = appID,
            Type = UIType .NotificationTray,
            Position = new Vector2 ( -0.25f, 1 ),
            Size = new Vector2 ( 0.33f, 0.125f ),
            SubPosition = new Vector2 ( 0, 1f ),
            SubSize = new Vector2 ( 0.95f, 0.25f ),
            SubRadius = 0.9f,
            Spacing = 0.6f,
            Channel1 = ThemeChannel .Clear,
            Channel2 = ThemeChannel .Button,
            Children = new ( ) {
                new ( ) {
                    Radius = 1f,
                    Unclamped = true,
                    Size = new Vector2 ( 0.2f, 0.2f ),
                    Position = new Vector2 ( 1.2f, 0.5f ),
                    Channel1 = ThemeChannel .Header
                }
            },
            PostBuild = ( locker ) => {
                mainTray .transform .localPosition = notificationtray_lastPosition .Value;
                MakeGrabber (
                    AppList [ appID ] .Buffer .Get,
                    mainTray .gameObject,
                    true,
                    ( ) => { notificationtray_lastPosition .Value = mainTray .transform .localPosition; }
                );
                locker .SetResult ( true );
            }
        };
        
        internal void Awake ( ) {
            harmony.PatchAll ( typeof ( HarmonyHooks ) );
            SelfCFGs ( );
            Debug .Log ( "otAPI core loaded." );
        }
        private void SelfCFGs ( ) {
            phone_militaryTime = Config .Bind (
                "Phone", "UseMilitaryTime", false,
                "Whether modPhone time is displayed in 24hr or 12hr format."
            );
            phone_lastTheme = Config .Bind (
                "Phone", "LastTheme", "ob:meteorite",
                "Last theme used on your modPhone."
            );
            phone_lastPosition = Config .Bind (
                "Phone", "LastPosition", Vector2 .zero,
                "Memory of last screen position for the phone for future reboots."
            );
            phone_Scale = Config .Bind (
                "Phone", "Scale", 1f,
                "Scale phone is generated with. Rejoin lobby to resolve stretching."
            );
            notificationtray_lastPosition = Config .Bind (
                "Notifications", "LastPosition", Vector2 .zero,
                "Memory of the last screen position for the notification tray."
            );
            desiredApps = Config .Bind (
                "Apps", "DesiredApps", "Button_Pomodoro,Button_TodoList,Button_modPhone,Button_TaskManager,Button_Journal",
                "Attempts to hide and sort apps."
            );
            dev_suppressWarnings = Config .Bind (
                "Dev", "SurpressWarnings", true,
                "Hides warnings (setting to false while developing is encouraged)"
            );
        }
        internal void Update ( ) {
            if ( !initialized ) return;
            if ( initialized & EventSystem .current .currentSelectedGameObject == null ) {
                if ( Input .GetKey ( KeyCode .LeftControl ) &
                Input .GetKey ( KeyCode .LeftShift ) & 
                Input .GetKeyDown ( KeyCode .Equals ) ) {
                    ResUp ( );
                }
                if ( Input .GetKey ( KeyCode .LeftControl ) & 
                Input .GetKey ( KeyCode .LeftShift ) & 
                Input .GetKeyDown ( KeyCode .Minus ) ) {
                    ResDown ( );
                }
            }
            if ( Canvas != null ) {
                if ( Time .frameCount % 3 == 0 )
                if ( ScaleFactor != Canvas .scaleFactor ) ScaleFactor = Canvas .scaleFactor;
            }
            if ( updateCycles.Count > 0 ) {
                foreach ( Dictionary < string, Action > DepotFolder in updateCycles .Values ) {
                    foreach ( Action act in DepotFolder .Values ) {
                        act .Invoke ( );
                    }
                }
            }
        }
        internal static IEnumerator Initializer ( ) {
            Canvas = GameObject .Find ( "Canvas" ) .GetComponent < Canvas > ( );
            ScaleFactor = Canvas .scaleFactor;

            
            AppList .Clear ( );
            AppList .Add ( appID, new AppStack ( ) );
            
            Dictionary < string, UIPackage > modphonePrefabs = new ( );
            foreach ( KeyValuePair < string, UIPackage > kv in modPhone .Prefabs ) {
                UIPackage _prefab = kv .Value with {
                    DepotFolder = appID
                };
                modphonePrefabs .Add ( kv .Key, _prefab );
            }
            modPhone .Prefabs = modphonePrefabs;
            if ( modPhone .Bools != null )
                AppList [ appID ] .Bools = new ( modPhone .Bools );
            if ( modPhone .Floats != null )
                AppList [ appID ] .Floats = new ( modPhone .Floats );
            if ( modPhone .Ints != null )
                AppList [ appID ] .Ints = new ( modPhone .Ints );
            if ( modPhone .Strings != null )
                AppList [ appID ] .Strings = new ( modPhone .Strings );
            if ( modPhone .Vectors != null )
                AppList [ appID ] .Vectors = new ( modPhone .Vectors );
            if ( modPhone .Prefabs != null )
                AppList [ appID ] .Prefabs = new ( modphonePrefabs );
            if ( modPhone .PersistentUI != null )
                AppList [ appID ] .PersistentUI = new ( modPhone .PersistentUI );
            if ( modPhone .PersistentUpdates != null )
                AppList [ appID ] .PersistentUpdates = new ( modPhone .PersistentUpdates );
            ConstructionQueue .Enqueue (
                new KeyValuePair < string, UIPackage >
                ( appID, modPhone )
            );
            foreach ( UITheme T in default_themes ) {
                themes .Add ( T );
            }
            foreach ( Depot depot in depots ) {
                if ( depot .app != null ) {
                    AppList .Add ( depot .name, new AppStack ( ) );
                    if ( depot .themelist != null ) {
                        foreach ( UITheme T in depot .themelist ) {
                            themes .Add ( T );
                        }
                    }
                    if ( depot .app != null) {
                        UIPackage import = ( UIPackage ) depot .app;
                        Dictionary < string, UIPackage > prefabs = new ( );
                        if ( depot .UsesApp && depot .UsesIcon ) {
                            appIcons .Add ( ( UIPackage ) depot .icon );
                        }
                        if ( import .Prefabs == null ) { } else {
                            foreach ( KeyValuePair < string, UIPackage > kv in import .Prefabs ) {
                                UIPackage _prefab = kv .Value with {
                                    DepotFolder = depot .name
                                };
                                prefabs .Add ( kv .Key, _prefab );
                            }
                        }
                        if ( import .Prefabs != null )
                            AppList [ depot .name ] .Prefabs = new ( prefabs );
                        if ( import .Bools != null )
                            AppList [ depot .name ] .Bools = new ( import .Bools );
                        if ( import .Floats != null )
                            AppList [ depot .name ] .Floats = new ( import .Floats );
                        if ( import .Ints != null )
                            AppList [ depot .name ] .Ints = new ( import .Ints );
                        if ( import .Strings != null )
                            AppList [ depot .name ] .Strings = new ( import .Strings );
                        if ( import .Vectors != null )
                            AppList [ depot .name ] .Vectors = new ( import .Vectors );
                        if ( import .Actions != null )
                            AppList [ depot .name ] .Actions = new ( import .Actions );
                        if ( import .PersistentUI != null )
                            AppList [ depot .name ] .PersistentUI = new ( import .PersistentUI );
                        if ( import .PersistentUpdates != null )
                            AppList [ depot .name ] .PersistentUpdates = new ( import .PersistentUpdates );
                    }
                    ConstructionQueue .Enqueue (
                        new KeyValuePair < string, UIPackage >
                        ( depot .name, ( UIPackage ) depot .app )
                    );
                }
            }

            if ( Theme == null ) {
                foreach ( UITheme t in themes ) {
                    if ( $"{ t .author }:{ t .name }" == phone_lastTheme .Value ) {
                        Theme = t;
                        break;
                    }
                }
            }
            if ( Theme == null ) Theme = themes .FirstOrDefault ( );
            //if ( !sortedYet ) {
            List < Depot > sortedDepots = depots
                .OrderBy ( p => p .author )
                .ThenBy ( p => p .shortName )
                .ToList ( )
            ;
            depots = sortedDepots;
                //sortedYet = true;
            //}
            ConstructionRoutine = RunCoroutine ( Construction ( ), true );
            if ( ConstructionRoutine != null ) {
                while ( ConstructionRoutine .MoveNext ( ) ) {
                    yield return ConstructionRoutine .Current;
                }
            }
            /*IEnumerator Icon = RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, modPhoneIconPackage ) ), true );
            if ( Icon == null ) { yield break; } else { while ( Icon .MoveNext ( ) ) {
                    yield return Icon .Current;
                }
            }*/
            IEnumerator tray = RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, mainTrayPackage ) ), true );
            if ( tray == null) { yield break; } else { while ( tray .MoveNext ( ) ) {
                    yield return tray .Current;
                }
            }
            RunCoroutine ( CreateAppIcons ( async ( ) => {
                        if ( !AppList [ appID ] .UI .ContainsKey ( homePage ) ) {
                            UIPackage HomePage = AppList [ appID ] .Prefabs [ homePage ];
                            RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, HomePage ) ) );
                        }
                        AttemptIconOrder ( );
                        AppList [ appID ] .UI [ appID ] .SetActive ( false );
                        AppList [ appID ] .UI [ appID ] .transform .localPosition = phone_lastPosition .Value;
                        initialized = true;
                        Debug .Log ( "otAPI initialization completed." );
                        CoreRoutine = null;

                    }
                )
            );
        }
        internal static void SetIconOrder ( ) {
            GameObject pool = GameObject .Find ( "/Canvas/GameUI/MainUI/Panel_ProductivityButtons" );
            if ( pool == null ) { Debug .Log ( "no pool!" ); return; }
            string total = "";
            int sum = pool .transform .childCount;
            for ( int c = 0; c < sum; c++ ) {
                int C = c;
                GameObject child = pool .transform .GetChild ( C ) .gameObject;
                if ( child == null ) continue;
                if ( child .activeSelf ) {
                    if ( total == "" ) { total += child .name; }
                    else total += $",{ child .name }";
                }
            }
            desiredApps .Value = total;
        }
        internal static void AttemptIconOrder ( ) {
            GameObject normals = GameObject .Find ( "/Canvas/GameUI/MainUI/Panel_ProductivityButtons" );
            GameObject horiButtons = Canvas .transform .Find ( "Mask_Horizontal/Panel_ProductivityButtons" ) .gameObject;
            GameObject vertButtons = Canvas .transform .Find ( "Mask_Vertical/Panel_ProductivityButtons" ) .gameObject;
            if ( normals == null || horiButtons == null || vertButtons == null ) return;
            string [ ] _items = desiredApps .Value .Split ( ',', StringSplitOptions .RemoveEmptyEntries );
            List < string > items = new ( );
            for ( int x = 0; x < _items .Length; x++ ) {
                int X = x;
                Transform exister = normals .transform .Find ( _items [ X ] );
                if ( exister == null ) { } else {
                    items .Add ( _items [ X ] );
                }
            }

            Transform [ ] T1s = new Transform [ normals .transform .childCount ];
            Transform [ ] T2s = new Transform [ normals .transform .childCount ];
            Transform [ ] T3s = new Transform [ normals .transform .childCount ];
            for ( int c = 0; c < normals .transform .childCount; c++ ) {
                int C = c;
                Transform T1 = normals .transform .GetChild ( C );
                Transform T2 = horiButtons .transform .GetChild ( C );
                Transform T3 = vertButtons .transform .GetChild ( C );
                if ( T1 == null || T2 == null || T3 == null ) continue;
                GameObject G1 = T1 .gameObject;
                GameObject G2 = T2 .gameObject;
                GameObject G3 = T3 .gameObject;
                if ( G1 == null || G2 == null || G3 == null ) continue;
                if ( !items .Contains ( T1 .name ) ) {
                    if ( T1 .name != "Button_modPhone" ) {
                        G1 .SetActive ( false ); 
                        G2 .SetActive ( false ); 
                        G3 .SetActive ( false ); 
                    }
                } else {
                    int pos = items .IndexOf ( T1 .name );
                    T1s [ pos ] = T1;
                    T2s [ pos ] = T2;
                    T3s [ pos ] = T3;
                }
            }
            for ( int t = 0; t < T1s .Length; t++ ) {
                int T = t;
                if ( T1s [ T ] == null || T2s [ T ] == null || T3s [ T ] == null ) continue;
                T1s [ T ] .SetSiblingIndex ( T );
                T2s [ T ] .SetSiblingIndex ( T );
                T3s [ T ] .SetSiblingIndex ( T );
            }
        }
        internal static IEnumerator Despawner ( ) {
            initialized = false;
            Canvas = null;
            appIcons .Clear ( );
            AppList .Clear ( );
            ConstructionQueue .Clear ( );

            yield return CoreRoutine = null;
        }
        public static IEnumerator ClearChildren (
            GameObject Target,
            string DepotFolder,
            List < string > UI_To_Keep = null,
            List < string > Update_Cycles_To_keep = null,
            Action Post_clear_action = null
        ) {
            if ( updateCycles .ContainsKey ( DepotFolder ) ) {
                List < string > del = new ( );
                foreach (  string d in updateCycles [ DepotFolder ] .Keys ) {
                    if ( Update_Cycles_To_keep != null ) {
                        if ( Update_Cycles_To_keep .Contains ( d ) ) { continue; }
                    }
                    del .Add ( d );
                }
                foreach ( string d in del ) { 
                    updateCycles [ DepotFolder ] .Remove ( d );
                }
            }
            if ( AppList .ContainsKey ( DepotFolder ) ) {
                List < string > del = new ( );
                foreach ( string s in AppList [ DepotFolder ] .UI .Keys ) {
                    if ( UI_To_Keep != null ) {
                        if ( UI_To_Keep .Contains ( s ) ) { continue; }
                    }
                    del .Add ( s );
                }
                foreach ( string d in del ) {
                    AppList [ DepotFolder ] .UI .Remove ( d );
                }
            }
            yield return null;
            IEnumerator next = RunCoroutine (
                CleanChildrenAndAct ( Target, UI_To_Keep, 0, Post_clear_action ), true );
            while ( next .MoveNext ( ) ) {
                yield return next . Current;
            }
        }
        
        public static void MakeGrabber (
            GameObject Handle,
            GameObject Target,
            bool LimitRangeByHandle = true,
            Action ReleaseAction = null
        ) {
            DragController DC = Handle .AddComponent < DragController > ( );
            DC .canvasRect = Canvas .GetComponent < RectTransform > ( );
            DC .grabber = Handle .GetComponent < RectTransform > ( );
            DC .primaryRect = Target .GetComponent < RectTransform > ( );
            DC .onRelease = ReleaseAction;
            DC .limitRangeByHandle = LimitRangeByHandle;
        }
        public static void Notify (
            string notification,
            UINotificationTray tray = null
        ) {
            tray =
                tray != null
                ? tray
                : mainTray
            ;
            RunCoroutine ( tray .Notify ( notification ) );
        }
        public static void Click ( ) { MonoSingleton < SFXManager > .I .PlayUIClick ( ); }
        public static void Edit < type, value > (
            type Instance,
            string FieldName,
            value Value
        ) {
            if ( Instance == null || Value == null ) return;
            try {    
                var t = typeof ( type );
                var field = t .GetField ( FieldName, BindingFlags .NonPublic | BindingFlags .Instance );
                field .SetValue ( Instance, Value );
            } catch ( Exception ex ) { Debug .Log ( $"otAPI: error accessing field { FieldName } on { Instance }. { ex .Message }" ); }            
        }
        public static void Call < type > (
            type Instance,
            string MethodName,
            object [ ] Parameters = null
        ) {
            var t = typeof ( type );
            var method = t .GetMethod (
                MethodName,
                BindingFlags .NonPublic |
                BindingFlags .Instance |
                BindingFlags .Public
            );
            if ( method != null ) {
                method .Invoke ( Instance, Parameters );
            }
        }

        public static void ScrollCheck ( GameObject obj, UIPackage k ) {
            if ( k .ScrollRect != null ) {
                if ( !obj .TryGetComponent ( typeof ( ScrollForwarder ), out Component _ ) ) {
                    ScrollForwarder sf = obj .AddComponent < ScrollForwarder > ( );
                    sf.scrollRect = k .ScrollRect;
                }
            }
        }
        public static Sprite LoadSprite (
            string target,
            Vector2Int size,
            Assembly externalAssembly = null,
            bool overrideFilter = false,
            FilterMode filter = FilterMode .Point
        ) {
            try
            {    
                Texture2D texture = new Texture2D ( size.x, size.y );
                MemoryStream mem = new ( );
                Assembly assembly =
                    externalAssembly != null
                    ? externalAssembly
                    : Assembly .GetExecutingAssembly ( );
                ;
                assembly .GetManifestResourceStream ( target )
                    .CopyTo ( mem );
                byte [ ] imageData = mem .ToArray ( );
                texture .LoadImage ( imageData );
                if ( overrideFilter ) texture .filterMode = filter;
                texture .Apply ( );

                Sprite output = Sprite .Create (
                    texture,
                    new Rect ( 0, 0, texture.width, texture.height ),
                    V2Center
                );
                string key = $"UIImage_{ target }";
                if ( !spriteCache .TryGetValue ( key, out Sprite _ ) ) {
                    spriteCache .Add ( key, output );
                }
                return output;
            }
            catch ( Exception ex ) { 
                Debug .Log ( $"Couldn't load sprite { target }: { ex }" );
                return null;
            }
        }
        public static void AddUpdateCycle (
            string depotFolder,
            string name,
            Action action
        ) {
            if ( !updateCycles .ContainsKey ( depotFolder ) ) {
                updateCycles .Add ( depotFolder, new ( ) );
                updateCycles [ depotFolder ] [ name ] = action;
            } else {
                updateCycles [ depotFolder ] [ name ] = action;
            }
        }
        public static void AddClickAction (
            GameObject Object,
            Action Action,
            bool UseClick = true
        ) {
            EventTrigger ev = AddOrGet < EventTrigger > ( Object );
            EventTrigger .Entry click = new ( );
            click .eventID = EventTriggerType .PointerDown;
            if ( UseClick ) {
                click .callback .AddListener ( ( data ) => {
                        if ( Object != null ) Action .Invoke ( );
                        Click ( );
                    }
                );
            } else {
                click .callback .AddListener ( ( data ) => {
                        if ( Object != null ) Action .Invoke ( );
                    }
                );
            }
            ev .triggers .Add ( click );
        }
        public static type AddOrGet < type > ( GameObject obj )
            where type : Component {
            if ( obj .TryGetComponent ( typeof ( type ), out Component Found ) ) {
                return Found as type;
            }
            return obj .AddComponent < type > ( );
        }
        
        public static IEnumerator SafishDeleter (
            GameObject obj
        ) {
            isDeleting = true;
            yield return null;
            UnityProxy .Destroy ( obj );
            yield return null;
            isDeleting = false;
        }
        internal static IEnumerator CleanChildrenAndAct (
            GameObject Object,
            List < string > UI_to_keep,
            float delay = 0f,
            Action action = null
        ) {
            Transform transform = Object . transform;
            EventSystem .current .SetSelectedGameObject ( null );
            for ( int i = transform .childCount - 1; i >= 0; i-- ) {
                if ( UI_to_keep .Contains ( transform .GetChild ( i ) .name ) ) continue;
                Destroy ( transform .GetChild ( i ) .gameObject );
                yield return null;
            }
            if ( delay != 0f ) yield return new WaitForSeconds ( delay );
            if ( action != null ) action .Invoke ( );
            else yield return null;
        }
        internal static void ResUp ( ) { if ( Canvas == null ) return; Canvas .scaleFactor += 0.025f; }
        internal static void ResDown ( ) { if ( Canvas == null ) return; Canvas .scaleFactor -= 0.025f; }
        internal static IEnumerator CreateAppIcons (
            Action postjob = null
        ) {
            for ( int p = 0; p < appIcons .Count; p++ ) {
                int P = p;
                UIPackage IconPackage = appIcons [ P ];
                float timeoutLimit = 10f;
                float timeout = 0;
                while ( AppList [ IconPackage .DepotFolder ] == null ) {
                    if ( timeout > timeoutLimit ) {
                        Debug .Log ( $"otAPI: CreateAppIcon couldn't find the app for { IconPackage .DepotFolder } in time. Aborted!" );
                        yield break;
                    } else {
                        timeout += Time .deltaTime;
                    }
                    yield return null;
                }
                while ( AppList [ IconPackage .DepotFolder ] .UI [ IconPackage .DepotFolder ] == null ) {
                    if ( timeout > timeoutLimit ) {
                        Debug .Log ( $"otAPI: CreateAppIcon couldn't find the app for { IconPackage .DepotFolder } in time. Aborted!" );
                        yield break;
                    } else {
                        timeout += Time .deltaTime;
                    }
                    yield return null;
                }
                UIPackage Icon = IconPackage with {
                    ObjectName = $"Button_{ IconPackage .ObjectName }",
                    ImgScale = 70f, // "Canvas/Mask_Horizontal/Panel_ProductivityButtons"
                    Parent = GameObject .Find ( "/Canvas/GameUI/MainUI/Panel_ProductivityButtons" ),
                    PostBuild = ( locker ) => {
                        GameObject buffer = AppList [ IconPackage .DepotFolder ] .Buffer .Get;
                        if ( buffer != null ) {
                            UIImage img = buffer .GetComponent < UIImage > ( );
                            img .CreateHoverBehavior ( 1.428f );
                            AddClickAction ( buffer, ( ) => {
                                    GameObject menu = AppList [ IconPackage .DepotFolder ] .UI [ IconPackage .DepotFolder ];
                                    if ( menu != null ) menu .SetActive ( !menu .activeSelf );
                                }
                            );
                        }
                        locker .SetResult ( true );
                    }
                };
                UIPackage Hori = Icon with {
                    Parent = Canvas .transform .Find ( "Mask_Horizontal/Panel_ProductivityButtons" ) .gameObject,
                    ImgScale = 55f
                };
                UIPackage Vert = Icon with {
                    Parent = Canvas .transform .Find ( "Mask_Vertical/Panel_ProductivityButtons" ) .gameObject,
                    ImgScale = 55f
                };
                IEnumerator jobs = QueueJobs (
                    KeyValuePair .Create < string, List < UIPackage> > (
                        IconPackage .DepotFolder, [ Icon, Hori, Vert ]
                    )
                );
                while ( jobs .MoveNext ( ) ) {
                    yield return jobs .Current;
                }
            }
            if ( postjob != null ) { postjob .Invoke ( ); }
        }
        
        public static bool ValidateHex (
            string input
        ) {
            string pattern = @"^[0-9a-fA-F]+$";
            if ( input == "" ) return true;
            return Regex .IsMatch ( input, pattern );
        }

        internal static void ErrorMsg (
            Dictionary < int, VerificationError > errors,
            string clarifier = ""
        ) {
            foreach ( var err in errors ) {
                switch ( err.Value ) {
                    case VerificationError .None:
                    break;
                    case VerificationError .IntExpected:
                        Notify ( $"call failed; int expected for argument { err .Key + 1 }. Examples: 1, 5, 200." );
                    break;
                    case VerificationError .FloatExpected:
                        Notify ( $"call failed; float expected for argument { err.Key + 1 }. Examples: 0.3, 2, 5.3." );
                    break;
                    case VerificationError .BoolExpected:
                        Notify ( $"call failed; bool expected for argument { err .Key + 1 }. Accepted args: true or false." );
                    break;
                    case VerificationError .BadTypeComparison:
                        Notify ( $"call failed; bad type comparison at argument { err .Key + 1 }. { clarifier }" );
                    break;
                    case VerificationError .OutsideRange:
                        Notify ( $"call failed; input was outside range for argument { err .Key + 1 }. { clarifier }" );
                    break;
                    case VerificationError .BadStringSize:
                        Notify ( $"call failed; bad string size at argument { err .Key + 1 }. { clarifier }" );
                    break;
                    case VerificationError .BadHexColor:
                        Notify ( $"call failed; bad hex color at argument { err .Key + 1 }. { clarifier }" );
                    break;
                    case VerificationError .NonOptionalOmitted:
                        Notify ( $"call failed; non-optional argument omitted at argument { err .Key + 1 }. { clarifier }" );
                    break;
                }
            }
        }
    }
}
