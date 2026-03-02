
using System;
using System .Linq;
using System .Collections .Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine .UI;

using BepInEx;
using BepInEx.Configuration;

using DG .Tweening;
using TMPro;
using PurrNet;

namespace _otAPI {
    public partial class otAPI {

        const string appID = "modPhone";
        const string contents = "Scrollspace";
        const string homePage = "Home Page";

        internal static UIPackage modPhoneIconPackage = new ( ) {
            ObjectName = appID,
            DepotFolder = appID,
            Type = UIType .Image,
            Path = "otAPI/images/icons/phone.png",
            ImgSize = new Vector2Int ( 270, 270 ),
        };
        internal static UIPackage modPhone = new ( ) {
            DepotFolder = appID,
            ObjectName = appID,
            PersistentUpdates = [ "TimeText Update" ],
            PersistentUI = [
                appID, contents, homePage, "Depot List", "Theme List", "Phone Settings",
                "Depot App", "Retheme App", "TimeText", "Back Button", "Phone Settings App",
            ],
            Bools = new ( ) { 
                { "submenu_resize", false }
            },
            Ints = new ( ) {
                { "setting_setup", 0 }
            },
            Strings = new ( ) {
                { "backbutton_target", "none" },
                { "lastapp", "none" }
            },
            Vectors = new ( ) {
                { "viewport_size", Vector2 .one },
                { "viewport_pos", Vector2 .one }
            },
            Mark = true,
            Size = new Vector2 ( 0.2f, 0.63f ),
            Position = new Vector2 ( 0.5f, 0.2f ),
            Channel1 = ThemeChannel .Border,
            Radius = 0.4f,
            StartInactive = true,
            Aspect = new ( ) {
                Mode = AspectGroup .Modes .HeightControlsWidth,
                Ratio = 0.57f
            },
            PostBuild = ( ) => {
                AppList [ appID ] .UI [ appID ] .transform .localPosition = phone_lastPosition .Value;

                UIPackage HP = AppList [ appID ] .Prefabs [ homePage ] with {
                    Parent = AppList [ appID ] .UI [ contents ],
                    ScrollRect = AppList [ appID ] .UI [ contents ]
                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                }; AppList [ appID ] .Prefabs [ homePage ] = HP;

                UIPackage PS = AppList [ appID ] .Prefabs [ "Phone Settings" ] with {
                    Parent = AppList [ appID ] .UI [ contents ],
                    ScrollRect = AppList [ appID ] .UI [ contents ]
                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                }; AppList [ appID ] .Prefabs [ "Phone Settings" ] = PS;

                UIPackage DP = AppList [ appID ] .Prefabs [ "Depot Page" ] with {
                    Parent = AppList [ appID ] .UI [ contents ],
                    ScrollRect = AppList [ appID ] .UI [ contents ]
                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                }; AppList [ appID ] .Prefabs [ "Depot Page" ] = DP;

                UIPackage DL = AppList [ appID ] .Prefabs [ "Depot List" ] with {
                    Parent = AppList [ appID ] .UI [ contents ],
                    ScrollRect = AppList [ appID ] .UI [ contents ]
                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                }; AppList [ appID ] .Prefabs [ "Depot List" ] = DL;

                UIPackage TL = AppList [ appID ] .Prefabs [ "Theme List" ] with {
                    Parent = AppList [ appID ] .UI [ contents ],
                    ScrollRect = AppList [ appID ] .UI [ contents ]
                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                }; AppList [ appID ] .Prefabs [ "Theme List" ] = TL;
                //phone_Scale .Value = Slider .slider .value;
                AppList [ appID ] .UI [ appID ] .transform .localScale = Vec2to3 ( Vec2 ( phone_Scale .Value ) );

                MakeGrabber (
                    AppList [ appID ] .UI [ appID ],
                    AppList [ appID ] .UI [ appID ],
                    false,
                    ( ) => { phone_lastPosition .Value = AppList [ appID ] .UI [ appID ] .transform .localPosition; }
                );
                AppList [ appID ] .UI [ "Back Button" ]
                    .transform .GetChild ( 0 ) .gameObject
                    .SetActive ( false );
                /*if ( !AppList [ appID ] .UI .ContainsKey ( homePage ) ) {
                        UIPackage HomePage = AppList [ appID ] .Prefabs [ homePage ];
                        RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, HomePage ) ) );
                    }*/
                RunCoroutine ( CreateAppIcon (
                    modPhoneIconPackage, null, ( ) => {
                        if ( !AppList [ appID ] .UI .ContainsKey ( homePage ) ) {
                                UIPackage HomePage = AppList [ appID ] .Prefabs [ homePage ];
                                RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, HomePage ) ) );
                            }
                        } 
                    )
                );
            },
            Children = new ( ) {
                new ( ) {
                    AnchorType = AnchorType .Right,
                    Unclamped = true,
                    Size = new Vector2 ( 0.03f, 0.1f ),
                    Position = new Vector2 ( 0.06f, 0.4f ),
                    Radius = 1f,
                    Channel1 = ThemeChannel .Border,
                    Channel2 = ThemeChannel .Hover,
                    Type = UIType .Button,
                    Action = async ( ) => {
                        await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                        await AppList [ appID ] .Tasks [ "home menu" ] .Run ( );
                    }
                },
                new ( ) {
                    ObjectName = "Panel Insert",
                    Size = new Vector2 ( 0.92f, 0.94f ),
                    Children = new ( ) {
                        new ( ) {
                            ObjectName = "Back Button",
                            Mark = true,
                            Type = UIType .Button,
                            Size = new Vector2 ( 0.975f, 0.25f ),
                            Radius = 0.75f,
                            Position = new Vector2 ( 0f, 0.735f ),
                            Channel1 = ThemeChannel .System,
                            Channel2 = ThemeChannel .SystemHover,
                            Children = new ( ) {
                                new ( ) {
                                    Type = UIType .Text,
                                    String = "<align=center>Loading...",
                                    TextSize = 52,
                                    Size = new Vector2 ( 1f, 0.25f ),
                                    Position = new Vector2 ( 0, 0.55f )
                                }
                            },
                            Action = async ( ) => {
                                await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                                switch ( AppList [ appID ] .Strings [ "backbutton_target" ] ) {
                                    case "none": break;
                                    case "home":
                                        await AppList [ appID ] .Tasks [ "home menu" ] .Run ( );
                                        break;
                                    case "depotlist":
                                        AppList [ appID ] .Strings [ "backbutton_target" ] = "home";
                                        await AppList [ appID ] .Tasks [ "depot menu" ] .Run ( );
                                        break;
                                }
                            }
                        }, new ( ) {
                            ObjectName = contents,
                            Size = new Vector2 ( 1f, 1f ),
                            Shrink = 0.99f,
                            Position = Vector2 .zero,
                            Channel1 = ThemeChannel .Border,
                            Channel2 = ThemeChannel .Body,
                            Type = UIType .Scrollable,
                            Mark = true
                        }
                    }
                }
            },
            Tasks = new ( ) { {
                    "settings menu", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "shrink menu" ] .Run ( );
                            GameObject psApp = AppList [ appID ] .UI [ "Phone Settings App" ];
                            if ( psApp == null ) return;
                            UIImage psImg = psApp .GetComponent < UIImage > ( );
                            if ( psImg == null ) return;
                            RectTransform psRect = psApp .GetComponent < RectTransform > ( );
                            if ( psRect == null ) return;
                            AppList [ appID ] .UI [ homePage ] .SetActive ( false );
                            psRect .sizeDelta = psImg .storedSize;
                            RunCoroutine (
                                ClearChildren (
                                    AppList [ appID ] .UI [ contents ], appID,
                                    AppList [ appID ] .PersistentUI, AppList [ appID ] .PersistentUpdates, async ( ) => {
                                        if ( !AppList [ appID ] .UI .ContainsKey ( "Phone Settings" ) )
                                            RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, AppList [ appID ] .Prefabs [ "Phone Settings"] ) ) );
                                        else {
                                            AppList [ appID ] .UI [ "Phone Settings" ] .SetActive ( true );
                                            await Task .Delay ( 3 );
                                            ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                                .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                                            DOTween .To (
                                                ( ) => SR .verticalNormalizedPosition,
                                                change => SR .verticalNormalizedPosition = change,
                                                1f,
                                                0.1f
                                            );
                                        }
                                        AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( )
                                            .ScrollRect .enabled = true
                                        ;
                                    }
                                ), appID
                            );
                            // "Phone Settings"
                        }
                    }
                },
                {
                    "retheme menu", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "shrink menu" ] .Run ( );
                            GameObject recolorApp = AppList [ appID ] .UI [ "Retheme App" ];
                            if ( recolorApp == null ) return;
                            UIImage recolorImg = recolorApp .GetComponent < UIImage > ( );
                            if ( recolorImg == null ) return;
                            RectTransform recolorRect = recolorApp .GetComponent < RectTransform > ( );
                            if ( recolorRect == null ) return;
                            AppList [ appID ] .UI [ homePage ] .SetActive ( false );
                            recolorRect .sizeDelta = recolorImg .storedSize;
                            RunCoroutine (
                                ClearChildren (
                                    AppList [ appID ] .UI [ contents ], appID,
                                    AppList [ appID ] .PersistentUI, AppList [ appID ] .PersistentUpdates, async ( ) => {
                                        if ( !AppList [ appID ] .UI .ContainsKey ( "Theme List" ) )
                                            RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, AppList [ appID ] .Prefabs [ "Theme List"] ) ) );
                                        else {
                                            AppList [ appID ] .UI [ "Theme List" ] .SetActive ( true );
                                            await Task .Delay ( 3 );
                                            ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                                .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                                            DOTween .To (
                                                ( ) => SR .verticalNormalizedPosition,
                                                change => SR .verticalNormalizedPosition = change,
                                                1f,
                                                0.1f
                                            );
                                        }
                                        AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( )
                                            .ScrollRect .enabled = true
                                        ;
                                    }
                                ), appID
                            );
                        }
                    }
                },
                {
                    "depot menu", new ( ) {
                        Action = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            //CancelJobs ( appID );
                            ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                            DOTween .To (
                                ( ) =>  SR .verticalNormalizedPosition,
                                change => SR .verticalNormalizedPosition = change,
                                1f,
                                0.1f
                            );

                            if ( AppList [ appID ] .Strings [ "lastapp" ] != "none" ) {
                                string toShut = AppList [ appID ] .Strings [ "lastapp" ];
                                if ( AppList [ appID ] .UI .ContainsKey ( toShut ) ) {
                                    GameObject obj = AppList [ appID ] .UI [ toShut ];
                                    if ( obj == null )  { } else {
                                        if ( AppList [ appID ] .Bools .ContainsKey ( $"{ toShut }_setup" ) ) {
                                            if ( AppList [ appID ] .Bools [ $"{ toShut }_setup" ] ) {
                                                obj .SetActive ( false );
                                            } else {
                                                RunCoroutine ( SafishDeleter ( obj ), appID );
                                                if ( AppList [ appID ] .UI .ContainsKey ( toShut ) ) AppList [ appID ] .UI .Remove ( toShut );
                                                if ( AppList [ appID ] .PersistentUI .Contains ( toShut ) ) AppList [ appID ] .PersistentUI .Remove ( toShut );
                                                
                                            }
                                        } 
                                    }
                                }
                                AppList [ appID ] . Strings [ "lastapp" ] = "none";
                            }

                            await AppList [ appID ] .Tasks [ "shrink menu" ] .Run ( );
                            GameObject depotApp = AppList [ appID ] .UI [ "Depot App" ];
                            if ( depotApp == null ) return;
                            UIImage depotImg = depotApp .GetComponent < UIImage > ( );
                            if ( depotImg == null ) return;
                            RectTransform depotRect = depotApp .GetComponent < RectTransform > ( );
                            if ( depotRect == null ) return;
                            AppList [ appID ] .UI [ homePage ] .SetActive ( false );
                            depotRect .sizeDelta = depotImg .storedSize;
                            RunCoroutine (
                                ClearChildren (
                                    AppList [ appID ] .UI [ contents ], appID,
                                    AppList [ appID ] .PersistentUI, AppList [ appID ] .PersistentUpdates, ( ) => {
                                        if ( !AppList [ appID ] .UI .ContainsKey ( "Depot List" ) ) {
                                            AppList [ appID ] .UI [ "Back Button" ]
                                                .transform .GetChild ( 0 ) .gameObject
                                                .SetActive ( true );
                                            RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, AppList [ appID ] .Prefabs [ "Depot List"] ), ( ) => {
                                                        AppList [ appID ] .UI [ "Back Button" ]
                                                            .transform .GetChild ( 0 ) .gameObject
                                                            .SetActive ( false );
                                                    }
                                                )
                                            );
                                        }
                                        else AppList [ appID ] .UI [ "Depot List" ] .SetActive ( true );
                                        SR .enabled = true;
                                    }
                                ), appID
                            );
                        }
                    }
                },
                {
                    "home menu", new ( ) {
                        Action = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            //CancelJobs ( appID );
                            ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                            DOTween .To (
                                ( ) =>  SR .verticalNormalizedPosition,
                                change => SR .verticalNormalizedPosition = change,
                                1f,
                                0.1f
                            );
                            //AppList [ appID ] .UI [ contents ] .transform .DOLocalMoveY ( 0, 0.1f );
                            if ( AppList [ appID ] .Bools .ContainsKey ( "themelist_setup") ) {
                                if ( !AppList [ appID ] .Bools [ "themelist_setup" ] ) {
                                    Transform TLT = AppList [ appID ] .UI [ contents ] .transform .Find ( "Theme List" );
                                    if ( TLT == null ) { } else {
                                        GameObject TLTO = TLT .gameObject; if ( TLTO == null ) { } else {
                                            RunCoroutine ( SafishDeleter ( TLTO ), appID );
                                            if ( AppList [ appID ] .UI .ContainsKey ( "Theme List" ) ) AppList [ appID ] .UI .Remove ( "Theme List" );
                                        }
                                    }
                                    /*RunCoroutine ( SafishDeleter ( AppList [ appID ] .UI [ contents ] .transform .Find ( "Theme List" ) .gameObject ), appID );
                                    if ( AppList [ appID ] .UI .ContainsKey ( "Theme List" ) ) AppList [ appID ] .UI .Remove ( "Theme List" );*/
                                }
                            }
                            if ( AppList [ appID ] .Bools .ContainsKey ( "settings_setup") ) {
                                if ( !AppList [ appID ] .Bools [ "settings_setup" ] ) {
                                    Transform DLT = AppList [ appID ] .UI [ contents ] .transform .Find ( "Phone Settings" );
                                    if ( DLT == null ) { } else {
                                        GameObject DLTO = DLT .gameObject; if ( DLTO == null ) { } else {
                                            RunCoroutine ( SafishDeleter ( DLTO ), appID );
                                            if ( AppList [ appID ] .UI .ContainsKey ( "Phone Settings" ) ) AppList [ appID ] .UI .Remove ( "Depot List" );
                                        }
                                    }
                                }
                            }
                            if ( AppList [ appID ] .Bools .ContainsKey ( "depotlist_setup") ) {
                                if ( !AppList [ appID ] .Bools [ "depotlist_setup" ] ) {
                                    Transform DLT = AppList [ appID ] .UI [ contents ] .transform .Find ( "Depot List" );
                                    if ( DLT == null ) { } else {
                                        GameObject DLTO = DLT .gameObject; if ( DLTO == null ) { } else {
                                            RunCoroutine ( SafishDeleter ( DLTO ), appID );
                                            if ( AppList [ appID ] .UI .ContainsKey ( "Depot List" ) ) AppList [ appID ] .UI .Remove ( "Depot List" );
                                        }
                                    }
                                }
                            }
                            if ( AppList [ appID ] .UI .ContainsKey ( "Phone Settings" ) )
                                AppList [ appID ] .UI [ "Phone Settings" ] .SetActive ( false );
                            if ( AppList [ appID ] .UI .ContainsKey ( "Depot List" ) )
                                AppList [ appID ] .UI [ "Depot List" ] .SetActive ( false );
                            if ( AppList [ appID ] .UI .ContainsKey ( "Theme List" ) )
                                AppList [ appID ] .UI [ "Theme List" ] .SetActive ( false );
                            if ( AppList [ appID ] .Strings [ "lastapp" ] != "none" ) {
                                string toShut = AppList [ appID ] .Strings [ "lastapp" ];
                                if ( AppList [ appID ] .UI .ContainsKey ( toShut ) ) {
                                    GameObject obj = AppList [ appID ] .UI [ toShut ];
                                    if ( obj == null ) return;
                                    else obj .SetActive ( false );
                                }
                                AppList [ appID ] . Strings [ "lastapp" ] = "none";
                            }
                            AppList [ appID ] .Strings [ "backbutton_target" ] = "none";
                            UIPackage HomePage = AppList [ appID ] .Prefabs [ homePage ];
                            RunCoroutine (
                                ClearChildren (
                                    AppList [ appID ] .UI [ contents ],
                                    appID, AppList [ appID ] .PersistentUI, AppList [ appID ] .PersistentUpdates, ( ) => {
                                        if ( !AppList [ appID ] .UI .ContainsKey ( homePage ) )
                                            RunCoroutine ( QueueJob ( KeyValuePair .Create ( appID, HomePage ) ) );
                                        else AppList [ appID ] .UI [ homePage ] .SetActive ( true );
                                        SR .enabled = false;
                                        bool ifFullSize = AppList [ appID ] .Bools [ "submenu_resize" ];
                                        if ( ifFullSize ) {
                                            AppList [ appID ] .Bools [ "submenu_resize" ] = false;
                                            RectTransform viewportRect = AppList [ appID ] .UI [ contents ]
                                                .transform .parent .GetComponent < RectTransform > ( );
                                            RectTransform containerRect = AppList [ appID ] .UI [ contents ]
                                                .transform .parent .parent .GetComponent < RectTransform > ( );
                                            containerRect .DOLocalMoveY (
                                                AppList [ appID ] .Vectors [ "viewport_pos" ] .y,
                                                0.1f
                                            );
                                            DOTween .To (
                                                ( ) => viewportRect .sizeDelta,
                                                change => viewportRect .sizeDelta = change,
                                                AppList [ appID ] .Vectors [ "viewport_size" ],
                                                0.1f
                                            );
                                            DOTween .To (
                                                ( ) => containerRect .sizeDelta,
                                                change => containerRect .sizeDelta = change,
                                                AppList [ appID ] .Vectors [ "viewport_size" ],
                                                0.1f
                                            );
                                        }
                                    }
                                )
                            );
                        }
                    }
                },
                {
                    "setup_boolsetting", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                            GameObject _this = AppList [ appID ] .Buffer .Get;
                            if ( _this == null ) return;
                            UIPanel This = _this .GetComponent < UIPanel > ( );
                            if ( This == null ) return;
                            if ( This .transform .childCount < 4 ) { Debug .Log ( $"otAPI: menu interrputed!" ); return; }
                            UIPanel Toggle = This .transform .GetChild ( 0 ) .GetComponent < UIPanel > ( );
                            UIText Current = This .transform .GetChild ( 1 ) .GetComponent < UIText > ( );
                            UIText Name = This .transform .GetChild ( 2 ) .GetComponent < UIText > ( );
                            UIText Info = This .transform .GetChild ( 3 ) .GetComponent < UIText > ( );
                            if ( Toggle == null || Current == null || Name == null || Info == null ) return;

                            UIPackage UIP = This .UIP;
                            int Index = -1;
                            if ( UIP .Ints == null ) return; else {
                                if ( UIP .Ints .ContainsKey ( "depot_index" ) ) Index = UIP .Ints [ "depot_index" ];
                            }
                            if ( Index == -1 ) return;
                            string Target = $"{ _this .name }";
                            if ( Target == "" ) return;
                            Depot depot = depots [ Index ];
                            if ( depot == null ) return;
                            string name = "";
                            string info = "";
                            Alias alias = null;
                            CfgLink link = null;
                            if ( !depot .aliases .ContainsKey ( Target ) ) return; else {
                                name = $"{ depot .aliases [ Target ] .name }";
                                info = $"{ depot .aliases [ Target ] .description }";
                                alias = depot .aliases [ Target ];
                                link = depot .aliases [ Target ] .cfgLink;
                            }
                            if ( alias == null ) return;
                            Name .Text .overflowMode = TextOverflowModes .Ellipsis;
                            Name .SetString ( name );
                            Info .Text .overflowMode = TextOverflowModes .Masking;
                            Info .Text .textWrappingMode = TextWrappingModes .Normal;
                            Info .SetString ( info );
                            Current .SetString ( link .boolLink .Value ? "<align=center>on" : "<align=center>off" );
                            Toggle .CreateHoverBehavior ( UIP .Theme, Toggle .mainChannel, Toggle .hoverChannel );
                            GameObject ToGo = Toggle .gameObject;
                            if ( ToGo == null ) return; else {    
                                AddClickAction ( Toggle .gameObject, ( ) => {
                                        if ( ToGo == null ) return;
                                        link .boolLink .Value = !link .boolLink .Value;
                                        string boolState =
                                            link .boolLink .Value
                                            ? "true."
                                            : "false."
                                        ;
                                        
                                        if ( Current != null ) Current .SetString ( link .boolLink .Value ? "<align=center>on" : "<align=center>off" );
                                        
                                        string [ ] args = [  ];
                                        if ( alias .action != null ) alias .action .Invoke ( args );
                                        Notify ( $"{ name } { link .changeString } { boolState }" );

                                    }
                                );
                            }
                            _this .SetActive ( true );
                            This .UIP = default;
                        }
                    }
                },
                {
                    "setup_floatsetting", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                            GameObject _this = AppList [ appID ] .Buffer .Get;
                            if ( _this == null ) return;
                            UIPanel This = _this .GetComponent < UIPanel > ( );
                            if ( This == null ) return;
                            if ( This .transform .childCount < 6 ) { Debug .Log ( $"otAPI: menu interrputed!" ); return; }
                            UISlider Slider = This .transform .GetChild ( 0 ) .GetComponent < UISlider > ( );
                            UIInput Input = This .transform .GetChild ( 1 ) .GetComponent < UIInput > ( );
                            UIPanel Cancel = This .transform .GetChild ( 2 ) .GetComponent < UIPanel > ( ); 
                            UIPanel Apply = This .transform .GetChild ( 3 ) .GetComponent < UIPanel > ( ); 
                            UIText Name = This .transform .GetChild ( 4 ) .GetComponent < UIText > ( ); 
                            UIText Info = This .transform .GetChild ( 5 ) .GetComponent < UIText > ( ); 
                            if ( Slider == null || Input == null || Cancel == null || Apply == null || Name == null || Info == null ) return;

                            UIPackage UIP = This .UIP;
                            int Index = -1;
                            if ( UIP .Ints == null ) return; else {
                                if ( UIP .Ints .ContainsKey ( "depot_index" ) ) Index = UIP .Ints [ "depot_index" ];
                            }
                            if ( Index == -1 ) return;
                            string Target = $"{ _this .name }";
                            if ( Target == "" ) return;
                            Depot depot = depots [ Index ];
                            Alias alias = null;
                            if ( depot == null ) return;
                            string name = "";
                            string info = "";
                            CfgLink link = null;
                            if ( !depot .aliases .ContainsKey ( Target ) ) return; else {
                                name = $"{ depot .aliases [ Target ] .name }";
                                info = $"{ depot .aliases [ Target ] .description }";
                                link = depot .aliases [ Target ] .cfgLink;
                                alias = depot .aliases [ Target ];
                            }
                            if ( alias == null ) return;
                            if ( Name .Text == null ) return; else {
                                Name .Text .overflowMode = TextOverflowModes .Ellipsis;
                                Name .SetString ( name );
                            }
                            if ( Info .Text == null ) return; else {
                                Info .Text .overflowMode = TextOverflowModes .Masking;
                                Info .Text .textWrappingMode = TextWrappingModes .Normal;
                                Info .SetString ( info );
                            }
                            Slider slider = Slider .slider;
                            if ( slider == null ) return;
                            if ( alias .args .Length > 0 ) {
                                if ( alias .args [ 0 ] .maxIn != null && alias .args [ 0 ] . minIn != null ) {
                                    slider .minValue = ( float ) alias .args [ 0 ] .minIn;
                                    slider .maxValue = ( float ) alias .args [ 0 ] .maxIn;
                                }
                            }
                            slider .onValueChanged .AddListener ( ( float value ) => {
                                    if ( Input == null ) return;
                                    Input .input .text = value .ToString ( "F2" );
                                }
                            );
                            if ( Input .input == null ) return; else {
                                Input .input .characterLimit = 7;
                                Input .input .characterValidation = TMP_InputField.CharacterValidation .Decimal;
                                Input .input .onSubmit .AddListener ( ( string value ) => {
                                        if ( slider == null || Input == null || Input .input == null ) return;
                                        float .TryParse ( value, out float falue );
                                        slider .value = falue;
                                    }
                                );
                            }
                            if ( Cancel == null ) return; else {
                                Cancel .CreateHoverBehavior ( UIP .Theme, Cancel .mainChannel, Cancel .hoverChannel );
                                GameObject CaGo = Cancel . gameObject;
                                if ( CaGo == null ) return; else {
                                    AddClickAction ( CaGo, ( ) => {
                                            if ( slider == null ) return;
                                            slider .value = link .floatLink .Value;
                                        }
                                    );
                                }
                            }
                            if ( Apply == null ) return; else {
                                Apply .CreateHoverBehavior ( UIP .Theme, Apply .mainChannel, Apply .hoverChannel );
                                GameObject ApOj = Apply .gameObject;
                                if ( ApOj == null ) return; else {
                                    AddClickAction ( ApOj, ( ) => {
                                            if ( slider == null ) return;
                                            link .floatLink .Value = slider .value;
                                            string [ ] args = [ slider .value .ToString ( ) ];
                                            if ( alias .action != null ) alias .action .Invoke ( args );
                                            Notify ( $"{ name } { link .changeString } { link .floatLink .Value }." ); 
                                        }
                                    );
                                }
                            }
                            if ( slider == null ) return; else {
                                slider .value = link .floatLink .Value;
                            }
                            _this .SetActive ( true );
                            This .UIP = default;
                        }
                    }
                },
                {
                    "setup_hexsetting", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                            GameObject _this = AppList [ appID ] .Buffer .Get;
                            if ( _this == null ) return;
                            UIPanel This = _this .GetComponent < UIPanel > ( );
                            if ( This == null ) return;
                            if ( This .transform .childCount < 12 ) { Debug .Log ( $"otAPI: menu interrputed!" ); return; }
                            UISlider R_slide = This .transform .GetChild ( 0 ) .GetComponent < UISlider > ( );
                            if ( R_slide == null ) return;
                            Slider _RS = R_slide .slider;
                            UISlider G_slide = This .transform .GetChild ( 1 ) .GetComponent < UISlider > ( );
                            if ( G_slide == null ) return;
                            Slider _GS = G_slide .slider;
                            UISlider B_slide = This .transform .GetChild ( 2 ) .GetComponent < UISlider > ( );
                            if ( B_slide == null ) return;
                            Slider _BS = B_slide .slider;
                            GameObject R_InpObj = This .transform .GetChild ( 3 ) .gameObject;
                            if ( R_InpObj == null ) return;
                            GameObject G_InpObj = This .transform .GetChild ( 4 ) .gameObject;
                            if ( G_InpObj == null ) return;
                            GameObject B_InpObj = This .transform .GetChild ( 5 ) .gameObject;
                            if ( B_InpObj == null ) return;
                            GameObject hexObj = This .transform .GetChild ( 6 ) .gameObject;
                            if ( hexObj == null ) return;
                            UIInput HexInpt = hexObj .GetComponent < UIInput > ( );
                            if ( HexInpt == null ) return;
                            UIPanel FutureColor = This .transform .GetChild ( 7 ) .GetComponent < UIPanel > ( );
                            if ( FutureColor == null ) return;
                            UIPanel CurrentColor = This .transform .GetChild ( 8 ) .GetComponent < UIPanel > ( );
                            if ( CurrentColor == null ) return;
                            GameObject CancelButton = This .transform .GetChild ( 9 ) .gameObject;
                            if ( CancelButton == null ) return;
                            GameObject ApplyButton = This .transform .GetChild ( 10 ) .gameObject;
                            if ( ApplyButton == null ) return;
                            GameObject nameText = This .transform .GetChild ( 11 ) .gameObject;
                            if ( nameText == null ) return;
                            GameObject infoText = This .transform .GetChild ( 12 ) .gameObject;
                            if ( infoText == null ) return;

                            UIPackage UIP = This .UIP;
                            int Index = -1;
                            if ( UIP .Ints == null ) return; else {
                                if ( UIP .Ints .ContainsKey ( "depot_index" ) ) Index = UIP .Ints [ "depot_index" ];
                            }
                            if ( Index == -1 ) return;
                            string Target = $"{ _this .name }";
                            if ( Target == "" ) return;
                            Depot depot = depots [ Index ];
                            if ( depot == null ) return;
                            string name = "";
                            string info = "";
                            CfgLink link = null;
                            Alias alias = null;
                            if ( !depot .aliases .ContainsKey ( Target ) ) return; else {
                                name = $"{ depot .aliases [ Target ] .name }";
                                info = $"{ depot .aliases [ Target ] .description }";
                                link = depot .aliases [ Target ] .cfgLink;
                                alias = depot .aliases [ Target ];
                            }
                            if ( alias == null ) return;
                            UIText NameText = nameText .GetComponent < UIText > ( );
                            if ( NameText == null ) return; else {
                                if ( NameText .Text == null ) return; else {
                                    NameText .Text .overflowMode = TextOverflowModes .Ellipsis;
                                    NameText .SetString ( name );
                                }
                            }
                            UIInput R_Inp = R_InpObj .GetComponent < UIInput > ( );
                            if ( R_Inp == null ) return;
                            UIInput G_Inp = G_InpObj .GetComponent < UIInput > ( );
                            if ( G_Inp == null ) return;
                            UIInput B_Inp = B_InpObj .GetComponent < UIInput > ( );
                            if ( B_Inp == null ) return;
                            UIText InfoText = infoText .GetComponent < UIText > ( );
                            if ( InfoText == null ) return; else {
                                if ( InfoText .Text == null ) return; else {
                                    InfoText .Text .overflowMode = TextOverflowModes .Masking;
                                    InfoText .Text .textWrappingMode = TextWrappingModes .Normal;
                                    InfoText .SetString ( info );
                                }
                            }
                            void ColorMix (
                                float r, float g, float b,
                                out Color color,
                                out string hexed
                            ) {
                                color = new Color ( r, g, b );
                                hexed = $"{ ( ( byte ) Math .Clamp ( r * 255f, 0, 255 ) ) .ToString ( "X2" ) }";
                                hexed += $"{ ( ( byte ) Math .Clamp ( g * 255f, 0, 255 ) ) .ToString ( "X2" ) }";
                                hexed += $"{ ( ( byte ) Math .Clamp ( b * 255f, 0, 255 ) ) .ToString ( "X2" ) }";
                            }
                            if ( _RS == null || _GS == null || _BS == null ) return; else {
                                _RS .onValueChanged .AddListener ( ( float value ) => {
                                        if ( _RS == null || _GS == null || _BS == null ) return;
                                        int R = ( int ) ( value * 255f );
                                        float G = _GS .value;
                                        float B = _BS .value;
                                        if ( R_Inp == null ) return;
                                        if ( R_Inp .input == null ) return;
                                        string text = R .ToString ( );
                                        R_Inp .input .text = text != "0" ? text : "";
                                        ColorMix (
                                            value, G, B,
                                            out Color c, out string h
                                        );
                                        if ( HexInpt == null ) return; else {
                                            if ( HexInpt .input == null ) return; else { HexInpt .input .text = h; }
                                        }
                                        if ( FutureColor == null ) return; else { FutureColor .Recolor ( c ); }
                                    }
                                );
                                _GS .onValueChanged .AddListener ( ( float value ) => {
                                        if ( _RS == null || _GS == null || _BS == null ) return;
                                        float R = _RS .value;
                                        int G = ( int ) ( value * 255f );
                                        float B = _BS .value;
                                        if ( G_Inp == null ) return;
                                        if ( G_Inp .input == null ) return;
                                        string text = G .ToString ( );
                                        G_Inp .input .text = text != "0" ? text : "";
                                        ColorMix (
                                            R, value, B,
                                            out Color c, out string h
                                        );
                                        if ( HexInpt == null ) return; else {
                                            if ( HexInpt .input == null ) return; else { HexInpt .input .text = h; }
                                        }
                                        if ( FutureColor == null ) return; else { FutureColor .Recolor ( c ); }
                                    }
                                );
                                _BS .onValueChanged .AddListener ( ( float value ) => {
                                        if ( _RS == null || _GS == null || _BS == null ) return;
                                        float R = _RS .value;
                                        float G = _GS .value;
                                        int B = ( int ) ( value * 255f );
                                        if ( B_Inp == null ) return;
                                        if ( B_Inp .input == null ) return;
                                        string text = B .ToString ( );
                                        B_Inp .input .text = text != "0" ? text : "";
                                        ColorMix (
                                            R, G, value,
                                            out Color c, out string h
                                        );
                                        if ( HexInpt == null ) return; else {
                                            if ( HexInpt .input == null ) return; else { HexInpt .input .text = h; }
                                        }
                                        if ( FutureColor == null ) return; else { FutureColor .Recolor ( c ); }
                                    }
                                );
                            }
                            if ( R_Inp .input == null ) { return; } else { 
                                R_Inp .input .characterValidation = TMP_InputField .CharacterValidation .Digit;
                                R_Inp .input .characterLimit = 3;
                                R_Inp .input .onValueChanged .AddListener ( ( string _value ) => {
                                        if ( R_slide == null ) return;
                                        int .TryParse ( _value, out int value );
                                        if ( _value == "" ) { value = 0; }
                                        if ( value > 255 ) {
                                            value = 255;
                                            R_Inp .input .text = value .ToString ( );
                                        }
                                        _RS .value = ( float ) ( value / 255f );
                                    }
                                );
                            }
                            if ( G_Inp .input == null ) { return; } else {
                                G_Inp .input .characterValidation = TMP_InputField .CharacterValidation .Digit;
                                G_Inp .input .characterLimit = 3;
                                G_Inp .input .onValueChanged .AddListener ( ( string _value ) => {
                                        if ( G_slide == null ) return;
                                        int .TryParse ( _value, out int value );
                                        if ( _value == "" ) { value = 0; }
                                        if ( value > 255 ) {
                                            value = 255;
                                            G_Inp .input .text = value .ToString ( );
                                        }
                                        _GS .value = ( float ) ( value / 255f );
                                    }
                                );
                            }
                            if ( B_Inp .input == null ) { return; } else {
                                B_Inp .input .characterValidation = TMP_InputField .CharacterValidation .Digit;
                                B_Inp .input .characterLimit = 3;
                                B_Inp .input .onValueChanged .AddListener ( ( string _value ) => {
                                        if ( B_slide == null ) return;
                                        int .TryParse ( _value, out int value );
                                        if ( _value == "" ) { value = 0; }
                                        if ( value > 255 ) {
                                            value = 255;
                                            B_Inp .input .text = value .ToString ( );
                                        }
                                        _BS .value = ( float ) ( value / 255f );
                                    }
                                );
                            }
                            if ( HexInpt .input == null ) { return; } else {
                                HexInpt .input .characterValidation = TMP_InputField .CharacterValidation .Regex;
                                HexInpt .input .characterLimit = 6;
                                Edit ( HexInpt .input, "m_RegexValue", "^[0-9a-fA-F]+$" );
                                HexInpt .input .onSubmit .AddListener ( ( string _value ) => {
                                        ColorUtility .TryParseHtmlString ( $"#{ _value }", out Color color );
                                        if ( _RS == null || _GS == null || _BS == null ) return; else {
                                            _RS .value = color .r;
                                            _GS .value = color .g;
                                            _BS .value = color .b;
                                        }
                                    }
                                );
                                string cur = link .stringLink .Value;
                                HexInpt .input .text = cur;
                                ColorUtility .TryParseHtmlString ( $"#{ cur }", out Color color );
                                if ( CurrentColor == null ) return; else { CurrentColor .Recolor ( color ); }
                                Call ( HexInpt .input, "SendOnSubmit" );
                            }
                            if ( ApplyButton == null ) { return; } else {
                                UIPanel ApPa = ApplyButton .GetComponent < UIPanel > ( );
                                if ( ApPa == null ) return;
                                ApPa .CreateHoverBehavior ( UIP .Theme, ApPa .mainChannel, ApPa .hoverChannel );
                                AddClickAction ( ApplyButton, ( ) => {
                                        if ( HexInpt == null || ApplyButton == null ) return;
                                        if ( HexInpt .input == null ) return;
                                        if ( HexInpt .input .text == "" ) return;
                                        if ( _RS == null || _GS == null || _BS == null ) return; else {
                                            ColorMix (
                                                _RS .value, _GS .value, _BS .value,
                                                out Color co, out string hexed
                                            );
                                            link .stringLink .Value = hexed;
                                            if ( CurrentColor == null ) return; else { CurrentColor .Recolor ( co ); }
                                            string [ ] args = [ hexed ];
                                            if ( alias .action != null ) alias .action .Invoke ( args );
                                            Notify ( $"{ name } { link .changeString } \"{ link .stringLink .Value }\".");
                                        }
                                    }
                                );
                            }
                            if ( CancelButton == null ) { return; } else {
                                UIPanel CaPa = CancelButton .GetComponent < UIPanel > ( );
                                if ( CaPa == null ) return;
                                CaPa .CreateHoverBehavior ( UIP .Theme, CaPa .mainChannel, CaPa .hoverChannel );
                                AddClickAction ( CancelButton, ( ) => {
                                    if ( HexInpt == null || CancelButton == null ) return;
                                    if ( HexInpt .input == null ) return;
                                    string cur = depot .aliases [ Target ] .cfgLink .stringLink .Value;
                                    HexInpt .input .text = cur;
                                    ColorUtility .TryParseHtmlString ( $"#{ cur }", out Color color );
                                    Call ( HexInpt .input, "SendOnSubmit" );

                                    }
                                );
                            }
                            _this .SetActive ( true );
                            This .UIP = default;
                        }
                    }
                },
                {
                    "setup_intsetting", new ( ) {
                        Action = async ( ) => {
                            await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                            GameObject _this = AppList [ appID ] .Buffer .Get;
                            if ( _this == null ) return;
                            UIPanel This = _this .GetComponent < UIPanel > ( );
                            if ( This == null ) return;
                            if ( This .transform .childCount < 6 ) { Debug .Log ( $"otAPI: menu interrputed!" ); return; }
                            UISlider Slider = This .transform .GetChild ( 0 ) .GetComponent < UISlider > ( );
                            UIInput Input = This .transform .GetChild ( 1 ) .GetComponent < UIInput > ( );
                            UIPanel Cancel = This .transform .GetChild ( 2 ) .GetComponent < UIPanel > ( ); 
                            UIPanel Apply = This .transform .GetChild ( 3 ) .GetComponent < UIPanel > ( ); 
                            UIText Name = This .transform .GetChild ( 4 ) .GetComponent < UIText > ( ); 
                            UIText Info = This .transform .GetChild ( 5 ) .GetComponent < UIText > ( ); 
                            if ( Slider == null || Input == null || Cancel == null || Apply == null || Name == null || Info == null ) return;

                            UIPackage UIP = This .UIP;
                            int Index = -1;
                            if ( UIP .Ints == null ) return; else {
                                if ( UIP .Ints .ContainsKey ( "depot_index" ) ) Index = UIP .Ints [ "depot_index" ];
                            }
                            if ( Index == -1 ) return;
                            string Target = $"{ _this .name }";
                            if ( Target == "" ) return;
                            Depot depot = depots [ Index ];
                            Alias alias = null;
                            if ( depot == null ) return;
                            string name = "";
                            string info = "";
                            CfgLink link = null;
                            if ( !depot .aliases .ContainsKey ( Target ) ) return; else {
                                name = $"{ depot .aliases [ Target ] .name }";
                                info = $"{ depot .aliases [ Target ] .description }";
                                link = depot .aliases [ Target ] .cfgLink;
                                alias = depot .aliases [ Target ];
                            }
                            if ( alias == null ) return;
                            if ( Name .Text == null ) return; else {
                                Name .Text .overflowMode = TextOverflowModes .Ellipsis;
                                Name .SetString ( name );
                            }
                            if ( Info .Text == null ) return; else {
                                Info .Text .overflowMode = TextOverflowModes .Masking;
                                Info .Text .textWrappingMode = TextWrappingModes .Normal;
                                Info .SetString ( info );
                            }
                            Slider slider = Slider .slider;
                            if ( slider == null ) return;
                            if ( alias .args .Length > 0 ) {
                                if ( alias .args [ 0 ] .maxIn != null && alias .args [ 0 ] . minIn != null ) {
                                    slider .wholeNumbers = true;
                                    slider .minValue = ( int ) alias .args [ 0 ] .minIn;
                                    slider .maxValue = ( int ) alias .args [ 0 ] .maxIn;
                                }
                            }
                            slider .onValueChanged .AddListener ( ( float value ) => {
                                    value = ( int ) value;
                                    if ( Input == null ) return;
                                    Input .input .text = value .ToString ( );
                                }
                            );
                            if ( Input .input == null ) return; else {
                                Input .input .characterLimit = 7;
                                Input .input .characterValidation = TMP_InputField.CharacterValidation .Integer;
                                Input .input .onSubmit .AddListener ( ( string value ) => {
                                        if ( slider == null || Input == null || Input .input == null ) return;
                                        float .TryParse ( value, out float falue );
                                        slider .value = falue;
                                    }
                                );
                            }
                            if ( Cancel == null ) return; else {
                                Cancel .CreateHoverBehavior ( UIP .Theme, Cancel .mainChannel, Cancel .hoverChannel );
                                GameObject CaGo = Cancel . gameObject;
                                if ( CaGo == null ) return; else {
                                    AddClickAction ( CaGo, ( ) => {
                                            if ( slider == null ) return;
                                            slider .value = link .intLink .Value;
                                        }
                                    );
                                }
                            }
                            if ( Apply == null ) return; else {
                                Apply .CreateHoverBehavior ( UIP .Theme, Apply .mainChannel, Apply .hoverChannel );
                                GameObject ApOj = Apply .gameObject;
                                if ( ApOj == null ) return; else {
                                    AddClickAction ( ApOj, ( ) => {
                                            if ( slider == null ) return;
                                            link .intLink .Value = ( int ) slider .value;
                                            string [ ] args = [ slider .value .ToString ( ) ];
                                            if ( alias .action != null ) alias .action .Invoke ( args );
                                            Notify ( $"{ name } { link .changeString } { link .intLink .Value }." ); 
                                        }
                                    );
                                }
                            }
                            if ( slider == null ) return; else {
                                slider .value = link .intLink .Value;
                            }
                            _this .SetActive ( true );
                            This .UIP = default;
                        }
                    }
                },
                {
                    "shrink menu", new ( ) {
                        Action = async ( ) => {
                            bool ifFullSize = !AppList [ appID ] .Bools [ "submenu_resize" ];
                            if ( ifFullSize ) {
                                AppList [ appID ] .Bools [ "submenu_resize" ] = true;
                                RectTransform viewportRect = AppList [ appID ] .UI [ contents ]
                                    .transform .parent .GetComponent < RectTransform > ( );
                                RectTransform containerRect = AppList [ appID ] .UI [ contents ]
                                    .transform .parent .parent .GetComponent < RectTransform > ( );
                                Vector2 baseSize = viewportRect .sizeDelta;
                                Vector2 basePos = containerRect .localPosition;
                                AppList [ appID ] .Vectors [ "viewport_size" ] = baseSize;
                                AppList [ appID ] .Vectors [ "viewport_pos" ] = basePos;
                                Vector2 futureSize = new Vector2 (
                                    baseSize .x,
                                    baseSize .y * 0.9f
                                );
                                containerRect .DOLocalMoveY (
                                    basePos .y - ( baseSize .y - futureSize .y ) / 2,
                                    0.1f
                                );
                                DOTween .To (
                                    ( ) => viewportRect .sizeDelta,
                                    change => viewportRect .sizeDelta = change,
                                    futureSize,
                                    0.1f
                                );
                                DOTween .To (
                                    ( ) => containerRect .sizeDelta,
                                    change => containerRect .sizeDelta = change,
                                    futureSize,
                                    0.1f
                                );
                            }
                        }
                    }
                },
                {
                    "check construction", new ( ) {
                        Action = async ( ) => {
                            while ( !ConstructionFree ) {
                                await Task .Delay ( 10 );
                            }
                        }
                    }
                }
            },
            Prefabs = new ( ) {
                {
                    homePage, new ( ) {
                        ObjectName = homePage,
                        Mark = true,
                        Size = Vector2 .one,
                        Channel1 = ThemeChannel .Clear,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "TimeText",
                                Radius = 0.77f,
                                Size = new Vector2 ( 0.99f, 0.16f ),
                                Position = new Vector2 ( 0, 0.7f ),
                                Channel1 = ThemeChannel .Header,
                                Children = new ( ) {
                                    new ( ) {
                                        ObjectName = "TimeText",
                                        Type = UIType .Text,
                                        Mark = true,
                                        String = $"<align=center><size=50>"
                                    }
                                }
                            }, new ( ) {
                                ObjectName = "Depot App",
                                Mark = true,
                                Type = UIType .Image,
                                Path = "otAPI/images/icons/depot_App.png",
                                ImgSize = new Vector2Int ( 288, 256 ),
                                Position = new Vector2 ( 0f, 0.2f ),
                                ImgScale = 96f
                            }, new ( ) {
                                ObjectName = "Retheme App",
                                Mark = true,
                                Type = UIType .Image,
                                Path = "otAPI/images/icons/recolor_App.png",
                                ImgSize = new Vector2Int ( 256, 256 ),
                                Position = new Vector2 ( -0.5f, -0.4f ),
                                ImgScale = 96f
                            }, new ( ) {
                                ObjectName = "Phone Settings App",
                                Mark = true,
                                Type = UIType .Image,
                                Path = "otAPI/images/icons/settings_App.png",
                                ImgSize = new Vector2Int ( 256, 256 ),
                                Position = new Vector2 ( 0.5f, -0.4f ),
                                ImgScale = 96f
                            }
                        },
                        PostBuild = ( ) => {
                            GameObject depotApp = AppList [ appID ] .UI [ "Depot App" ];
                            GameObject recolorApp = AppList [ appID ] .UI [ "Retheme App" ];
                            GameObject settingsApp = AppList [ appID ] .UI [ "Phone Settings App" ];
                            UIImage depotImg = depotApp .GetComponent < UIImage > ( );
                            UIImage recolorImg = recolorApp .GetComponent < UIImage > ( );
                            UIImage settingsImg = settingsApp .GetComponent < UIImage > ( );
                            RectTransform depotRect = depotApp .GetComponent < RectTransform > ( );
                            RectTransform recolorRect = recolorApp .GetComponent < RectTransform > ( );
                            RectTransform settingsRect = settingsApp .GetComponent < RectTransform > ( );
                            depotImg .CreateHoverBehavior ( 1.428f );
                            recolorImg .CreateHoverBehavior ( 1.428f );
                            settingsImg .CreateHoverBehavior ( 1.428f );
                            AddClickAction ( depotApp, async ( ) => {
                                    AppList [ appID ] .Strings [ "backbutton_target" ] = "home";
                                    await AppList [ appID ] .Tasks [ "depot menu" ] .Run ( );
                                }
                            );
                            AddClickAction ( recolorApp, async ( ) => {
                                    AppList [ appID ] .Strings [ "backbutton_target" ] = "home";
                                    await AppList [ appID ] .Tasks [ "retheme menu" ] .Run ( );
                                }
                            );
                            AddClickAction ( settingsApp, async ( ) => {
                                    AppList [ appID ] .Strings [ "backbutton_target" ] = "home";
                                    await AppList [ appID ] .Tasks [ "settings menu" ] .Run ( );
                                }
                            );

                            AppList [ appID] .UI [ contents ]
                                .GetComponent < ScrollTunnel > ( )
                                .ScrollRect .enabled = false
                            ;
                            AddUpdateCycle ( appID, "TimeText Update", ( ) => {
                                if ( AppList [ appID ] .UI [ "TimeText" ] != null ) {
                                    if ( Time .frameCount % 6 == 0) {
                                        AppList [ appID ] .UI [ "TimeText" ]
                                            .GetComponent < UIText > ( )
                                            .SetString ( $"<align=center><size=128>{ DateTime .Now .ToString ( "h:mm tt" ) }" )
                                            ;
                                        }
                                    }
                                }
                            );
                        }
                    }
                },
                {
                    "Depot List", new ( ) {
                        ObjectName = "Depot List",
                        Mark = true,
                        Size = Vector2 .one,
                        Channel1 = ThemeChannel .Clear,
                        PostBuild = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( true );
                            TaskCompletionSource < bool > waiter = new ( );
                            if ( !AppList [ appID ] .Bools .ContainsKey ( "depotlist_setup" ) ) {
                                AppList [ appID ] .Bools .Add ( "depotlist_setup", false );
                            }
                            GameObject This = AppList [ appID ] .Buffer .Get;
                            if ( This == null ) return;
                            VerticalLayoutGroup VLG = This .AddComponent < VerticalLayoutGroup > ( );
                            if ( VLG == null ) return; else {
                                VLG .childForceExpandWidth = false;
                                VLG .childForceExpandHeight = false;
                                VLG .childAlignment = TextAnchor .MiddleCenter;
                            }
                            UIPackage DL = AppList [ appID ] .Prefabs [ "Depot Menu Label" ] with {
                                Parent = AppList [ appID ] .UI [ "Depot List" ],
                                ScrollRect = AppList [ appID ] .UI [ contents ]
                                .GetComponent < ScrollTunnel > ( ) .ScrollRect
                            }; AppList [ appID ] .Prefabs [ "Depot Menu Label" ] = DL;
                            List < UIPackage > _build = new ( );
                            for ( int index = 0; index < depots .Count; index++ ) {
                                int _index = index;
                                Depot depot = depots [ _index ];
                                string dname = depot .name;
                                UIPackage newie = DL with { };
                                _build .Add ( newie );
                                UIPackage _DL = _build [ _build .Count - 1 ] with {
                                    ObjectName = $"menulabel_{ _index }",
                                    Mark = true,
                                    Action = async ( ) => {
                                        if ( AppList [ appID ] .UI .ContainsKey ( "Depot List" ) )
                                            AppList [ appID ] .UI [ "Depot List" ] .SetActive ( false );
                                        AppList [ appID ] .Strings [ "backbutton_target" ] = "depotlist";
                                        //CancelJobs ( appID );
                                        RunCoroutine (
                                            ClearChildren (
                                                AppList [ appID ] .UI [ contents ], appID,
                                                AppList [ appID ] .PersistentUI, AppList [ appID ] .PersistentUpdates, async ( ) => {

                                                        AppList [ appID ] .Strings [ "lastapp" ] = $"depotpage_{ dname }";
                                                        if ( AppList [ appID ] .PersistentUI .Contains ( $"depotpage_{ dname }" ) ) {
                                                            if ( AppList [ appID ] .UI .ContainsKey ( $"depotpage_{ dname }" ) ) {
                                                                AppList [ appID ] .UI [ $"depotpage_{ dname }" ] .SetActive ( true );
                                                                await Task .Delay ( 3 );
                                                                ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                                                    .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                                                                DOTween .To (
                                                                    ( ) => SR .verticalNormalizedPosition,
                                                                    change => SR .verticalNormalizedPosition = change,
                                                                    1f,
                                                                    0.1f
                                                                );
                                                            }
                                                        } else {
                                                        UIPackage job = AppList [ appID ] .Prefabs [ "Depot Page" ] with {
                                                            ObjectName = $"depotpage_{ dname }",
                                                            String = $"{ _index }"
                                                        };
                                                        RunCoroutine ( QueueJob ( KeyValuePair .Create (  appID, job ), async ( ) => {
                                                                }
                                                            )
                                                        );
                                                    }
                                                }
                                            )
                                        );
                                    }
                                };
                                _build [ _build .Count - 1 ] = _DL;
                                UIPackage __DL = _build [ _build .Count - 1 ] .Children [ 0 ] with {
                                    String = $"<align=center>{ depot .name }",
                                };
                                _build [ _build .Count - 1 ] .Children [ 0 ] = __DL; 
                            }
                            RunCoroutine ( QueueJobs ( KeyValuePair .Create (  appID, _build ), ( ) => {
                                        waiter .SetResult ( true );
                                    } 
                                ), appID
                            );
                            await waiter .Task;
                            AppList [ appID ] .Bools [ "depotlist_setup" ] = true;
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            await Task .Delay ( 25 );
                        }
                    }
                },
                {
                    "Depot Menu Label", new ( ) {
                        Type = UIType .Button,
                        ObjectName = "Depot Menu Label",
                        Size = new Vector2 ( 0.9f, 0.1f ),
                        Radius = 0.85f,
                        Channel1 = ThemeChannel .Button,
                        Channel2 = ThemeChannel .Hover,
                        Children = new ( ) {
                            new ( ) {
                                Type = UIType .Text,
                                TextSize = 60
                            }
                        }
                    }
                },
                {
                    "Depot Page", new ( ) {
                        StorePackage = true,
                        Size = Vector2 .one,
                        Channel1 = ThemeChannel .Clear,
                        PostBuild = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( true );
                            TaskCompletionSource < bool > waiter = new ( );
                            GameObject DepotInfo = AppList [ appID ] .Buffer .Get;
                            if ( DepotInfo == null ) return;
                            GameObject This = DepotInfo .transform .parent .parent .gameObject;
                            if ( This == null ) return;
                            UIPanel ThisPanel = This .GetComponent < UIPanel > ( );
                            if ( ThisPanel == null ) return;
                            UIPackage source = ThisPanel .UIP;
                            int .TryParse ( source .String, out int value );
                            Depot depot = depots [ value ];
                            string name = depot .name;
                            if ( !AppList [ appID ] .Bools .ContainsKey ( $"depotpage_{ name }_setup" ) )
                                AppList [ appID ] .Bools .Add ( $"depotpage_{ name }_setup", false );
                            // add loading visual?
                            VerticalLayoutGroup VLG = This .AddComponent < VerticalLayoutGroup > ( );
                            if ( VLG == null ) return; else {
                                VLG .childForceExpandWidth = false;
                                VLG .childForceExpandHeight = false;
                                VLG .childAlignment = TextAnchor .MiddleCenter;
                                VLG .spacing = 16;
                            }

                            string author = depot .author;
                            string desc = depot .description;
                            UIText Name = This .transform .GetChild ( 0 ) .GetChild ( 0 ) .GetComponent < UIText > ( );
                            UIText Author = This .transform .GetChild ( 0 ) .GetChild ( 1 ) .GetComponent < UIText > ( );
                            UIText Info = This .transform .GetChild ( 0 ) .GetChild ( 2 ) .GetComponent < UIText > ( );
                            if ( Name == null || Author == null || Info == null ) return;
                            AppList [ appID ] .PersistentUI .Add ( $"depotpage_{ name }" );
                            AppList [ appID ] .UI .Add ( $"depotpage_{ name }", This );
                            Name .SetString ( name );
                            Author .SetString ( author );
                            Info .SetString ( desc );
                            Name .Text .overflowMode = TextOverflowModes .Ellipsis;
                            Author .Text .overflowMode = TextOverflowModes .Ellipsis;
                            Info .Text .overflowMode = TextOverflowModes .Masking;
                            Info .Text .textWrappingMode = TextWrappingModes .Normal;
                            List < UIPackage > jobs = new ( );
                            foreach ( KeyValuePair < string, Alias > a in depot .aliases ) {
                                KeyValuePair < string, Alias > A = a;
                                if ( A .Value .cfgLink == null ) continue;
                                CfgLink Link = A .Value .cfgLink;
                                switch ( Link .valueType ) {
                                    case ArgType .Bool:
                                        UIPackage BoolPkg = AppList [ appID ] .Prefabs [ "Boolean Setting" ] with {
                                            Parent = AppList [ appID ] .UI [ $"depotpage_{ name }" ],
                                            ObjectName = $"{ A .Key }",
                                            ScrollRect = AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( ) .ScrollRect,
                                            Ints = new ( ) { { "depot_index", value } }
                                        };
                                        jobs .Add ( BoolPkg );
                                        continue;
                                    case ArgType .Float:
                                        UIPackage FloatPkg = AppList [ appID ] .Prefabs [ "Float Setting" ] with {
                                            Parent = AppList [ appID ] .UI [ $"depotpage_{ name }" ],
                                            ObjectName = $"{ A .Key }",
                                            ScrollRect = AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( ) .ScrollRect,
                                            Ints = new ( ) { { "depot_index", value } }
                                        };
                                        jobs .Add ( FloatPkg );
                                        continue;
                                    case ArgType .HexColor:
                                        UIPackage HexPkg = AppList [ appID ] .Prefabs [ "Color Setting" ] with {
                                            Parent = AppList [ appID ] .UI [ $"depotpage_{ name }" ],
                                            ObjectName = $"{ A .Key }",
                                            ScrollRect = AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( ) .ScrollRect,
                                            Ints = new ( ) { { "depot_index", value } }
                                        };
                                        jobs .Add ( HexPkg );
                                        continue;
                                    case ArgType .Int:
                                        UIPackage IntPkg = AppList [ appID ] .Prefabs [ "Int Setting" ] with {
                                            Parent = AppList [ appID ] .UI [ $"depotpage_{ name }" ],
                                            ObjectName = $"{ A .Key }",
                                            ScrollRect = AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( ) .ScrollRect,
                                            Ints = new ( ) { { "depot_index", value } }
                                        };
                                        jobs .Add ( IntPkg );
                                        continue;
                                    
                                }
                                //Debug .Log ( $"{ a .Key }, { a .Value .name }: { a .Value .description }" );
                            }
                            RunCoroutine ( QueueJobs ( KeyValuePair .Create ( appID, jobs ), async ( ) => {
                                        for ( int d = 0; d < This .transform .childCount; d ++ ) {
                                            int D = d;
                                            GameObject child = This .transform .GetChild ( D ) .gameObject;
                                            if ( child == null ) continue;
                                            UIPanel panel = child .GetComponent < UIPanel > ( );
                                            if ( panel == null ) continue;
                                            if ( depot .aliases .ContainsKey ( child .name ) ) {
                                                AppList [ appID ] .Buffer .Set ( child );
                                                if ( depot .aliases [ child .name ] .cfgLink == null ) {
                                                    
                                                } else {
                                                    CfgLink Link = depot .aliases [ child .name ] .cfgLink;
                                                    switch ( Link .valueType ) {
                                                        case ArgType .Bool:
                                                            await AppList [ appID ] .Tasks [ "setup_boolsetting" ] .Run ( );
                                                            break;
                                                        case ArgType .Float:
                                                            await AppList [ appID ] .Tasks [ "setup_floatsetting" ] .Run ( );
                                                            break;
                                                        case ArgType .HexColor:
                                                            await AppList [ appID ] .Tasks [ "setup_hexsetting" ] .Run ( );
                                                            break;
                                                        case ArgType .Int:
                                                            await AppList [ appID ] .Tasks [ "setup_intsetting" ] .Run ( );
                                                            break;
                                                    } 
                                                }
                                            }
                                        }
                                        if ( ThisPanel == null ) return; else {
                                            ThisPanel .UIP = default;
                                        }
                                        
                                        DOTween .To (
                                            ( ) => source .ScrollRect .verticalNormalizedPosition,
                                            change => source .ScrollRect .verticalNormalizedPosition = change,
                                            1f,
                                            0.1f
                                        );
                                        waiter .SetResult ( true );
                                    }
                                )
                            );
                            await waiter .Task;
                            AppList [ appID ] .Bools [ $"depotpage_{ name }_setup" ] = true;
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            await Task .Delay ( 25 );
                        },
                        Children = new ( )
                        {
                            new ( ) {
                                ObjectName = "Depot Label",
                                Radius = 0.33f,
                                Expands = false,
                                Size = new Vector2 ( 0.95f, 0.25f ),
                                Channel1 = ThemeChannel .Header,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        ObjectName = "Depot Name",
                                        TextSize = 66,
                                        Position = new Vector2 ( 0, 0.6f ),
                                        Size = new Vector2 ( 0.85f, 0.5f )
                                    }, new ( ) {
                                        Type = UIType .Text,
                                        ObjectName = "Depot Author",
                                        TextSize = 36,
                                        Position = new Vector2 ( 0, 0.1f ),
                                        Size = new Vector2 ( 0.85f, 0.3f )
                                    }, new ( ) {
                                        Type = UIType .Text,
                                        ObjectName = "Depot Info",
                                        TextSize = 42,
                                        Position = new Vector2 ( 0, -0.4f ),
                                        Size = new Vector2 ( 0.85f, 0.3f )
                                    }
                                }
                            }
                        }
                    }
                },
                {
                    "Boolean Setting", new ( ) {
                        ObjectName = "Boolean Setting",
                        Size = new Vector2 ( 0.85f, 0.166f ),
                        Radius = 0.36f,
                        Expands = false,
                        StartInactive = true,
                        StorePackage = true,
                        Channel1 = ThemeChannel .Header,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Toggle",
                                Size = new Vector2 ( 0.2f, 0.27f ),
                                Position = new Vector2 ( 0.67f, -0.35f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Toggle",
                                        TextSize = 24
                                    }
                                }
                            }, new ( ) {
                                ObjectName = "Setting Current",
                                TextSize = 32,
                                String = "<align=center>Off",
                                Type = UIType .Text,
                                Size = new Vector2 ( 0.15f, 0.3f ),
                                Position = new Vector2 ( 0.77f, 0.5f )
                            }, new ( ) {
                                ObjectName = "Setting Name",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 42,
                                Size = new Vector2 ( 0.62f, 0.38f ),
                                Position = new Vector2 ( -0.75f, 0.7f )
                            }, new ( ) {
                                ObjectName = "Setting Description",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 32,
                                Size = new Vector2 ( 0.62f, 0.4f ),
                                Position = new Vector2 ( -0.75f, -0.5f )
                            }
                        }
                    }
                },
                {
                    "Float Setting", new ( ) {
                        ObjectName = "Float Setting",
                        Size = new Vector2 ( 0.85f, 0.24f ),
                        Radius = 0.36f,
                        Expands = false,
                        StartInactive = true,
                        StorePackage = true,
                        Channel1 = ThemeChannel .Header,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Slider",
                                Type = UIType .Slider,
                                Width = 0.6f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Channel1 = ThemeChannel .Text,
                                Position = new Vector2 ( -0.35f, -0.7f )
                            }, new ( ) {
                                ObjectName = "Input",
                                Type = UIType .Input,
                                Placeholder = "0",
                                Unclamped = true,
                                Width = 0.25f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Position = new Vector2 ( 0.6f, -0.7f )
                            }, new ( ) {
                                ObjectName = "Cancel",
                                Position = new Vector2 ( 0.67f, 0.55f ),
                                Size = new Vector2 ( 0.2f, 0.18f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Cancel",
                                        TextSize = 24
                                    }
                                }
                            },new ( ) {
                                ObjectName = "Apply",
                                Position = new Vector2 ( 0.67f, 0.07f ),
                                Size = new Vector2 ( 0.2f, 0.18f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Apply",
                                        TextSize = 24
                                    }
                                }
                            }, new ( ) {
                                ObjectName = "Setting Name",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 42,
                                Size = new Vector2 ( 0.62f, 0.38f ),
                                Position = new Vector2 ( -0.75f, 0.8f )
                            }, new ( ) {
                                ObjectName = "Setting Description",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 32,
                                Size = new Vector2 ( 0.62f, 0.4f ),
                                Position = new Vector2 ( -0.75f, -0.05f )
                            }
                        }
                    }
                },
                {
                    "Int Setting", new ( ) {
                        ObjectName = "Int Setting",
                        Size = new Vector2 ( 0.85f, 0.24f ),
                        Radius = 0.36f,
                        Expands = false,
                        StartInactive = true,
                        StorePackage = true,
                        Channel1 = ThemeChannel .Header,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Slider",
                                Type = UIType .Slider,
                                Width = 0.6f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Channel1 = ThemeChannel .Text,
                                Position = new Vector2 ( -0.35f, -0.7f )
                            }, new ( ) {
                                ObjectName = "Input",
                                Type = UIType .Input,
                                Placeholder = "0",
                                Unclamped = true,
                                Width = 0.25f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Position = new Vector2 ( 0.6f, -0.7f )
                            }, new ( ) {
                                ObjectName = "Cancel",
                                Position = new Vector2 ( 0.67f, 0.55f ),
                                Size = new Vector2 ( 0.2f, 0.18f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Cancel",
                                        TextSize = 24
                                    }
                                }
                            },new ( ) {
                                ObjectName = "Apply",
                                Position = new Vector2 ( 0.67f, 0.07f ),
                                Size = new Vector2 ( 0.2f, 0.18f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Apply",
                                        TextSize = 24
                                    }
                                }
                            }, new ( ) {
                                ObjectName = "Setting Name",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 42,
                                Size = new Vector2 ( 0.62f, 0.38f ),
                                Position = new Vector2 ( -0.75f, 0.8f )
                            }, new ( ) {
                                ObjectName = "Setting Description",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 32,
                                Size = new Vector2 ( 0.62f, 0.4f ),
                                Position = new Vector2 ( -0.75f, -0.05f )
                            }
                        }
                    }
                },
                {
                    "Color Setting", new ( ) {
                        ObjectName = "Color Setting",
                        Size = new Vector2 ( 0.85f, 0.33f ),
                        Position = new Vector2 ( -0.33f, 0f ),
                        Radius = 0.23f,
                        Expands = false,
                        StorePackage = true,
                        Channel1 = ThemeChannel .Header,
                        StartInactive = true,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Slider R",
                                Type = UIType .Slider,
                                Width = 0.6f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Channel1 = ThemeChannel .Text,
                                Position = new Vector2 ( -0.35f, -0.2f )
                            }, new ( ) {
                                ObjectName = "Slider G",
                                Type = UIType .Slider,
                                Width = 0.6f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Channel1 = ThemeChannel .Text,
                                Position = new Vector2 ( -0.35f, -0.51f )
                            }, new ( ) {
                                ObjectName = "Slider B",
                                Type = UIType .Slider,
                                Width = 0.6f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Channel1 = ThemeChannel .Text,
                                Position = new Vector2 ( -0.35f, -0.82f )
                            }, new ( ) {
                                ObjectName = "Input R",
                                Type = UIType .Input,
                                Placeholder = "0",
                                Unclamped = true,
                                Width = 0.25f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Position = new Vector2 ( 0.57f, -0.2f )
                            }, new ( ) {
                                ObjectName = "Input G",
                                Type = UIType .Input,
                                Placeholder = "0",
                                Unclamped = true,
                                Width = 0.25f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Position = new Vector2 ( 0.57f, -0.51f )
                            }, new ( ) {
                                ObjectName = "Input B",
                                Type = UIType .Input,
                                Placeholder = "0",
                                Unclamped = true,
                                Width = 0.25f,
                                Size = new Vector2 ( 0.2f, 0.2f ),
                                Position = new Vector2 ( 0.57f, -0.82f )
                            }, new ( ) {
                                ObjectName = "Hex Input",
                                Type = UIType .Input,
                                Placeholder = "123456",
                                Width = 0.25f,
                                Position = new Vector2 ( 0.84f, 0.1f )
                            }, new ( ) {
                                ObjectName = "Current Color",
                                SkipsRethemes = true,
                                Position = new Vector2 ( 0.82f, -0.25f ),
                                Size = new Vector2 ( 0.1f, 0.16f ),
                                Radius = 0.6f
                            }, new ( ) {
                                ObjectName = "New Color",
                                SkipsRethemes = true,
                                Position = new Vector2 ( 0.82f, -0.65f ),
                                Size = new Vector2 ( 0.1f, 0.16f ),
                                Radius = 0.6f
                            }, new ( ) {
                                ObjectName = "Cancel",
                                Position = new Vector2 ( 0.67f, 0.75f ),
                                Size = new Vector2 ( 0.2f, 0.13f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Cancel",
                                        TextSize = 24
                                    }
                                }
                            },new ( ) {
                                ObjectName = "Apply",
                                Position = new Vector2 ( 0.67f, 0.42f ),
                                Size = new Vector2 ( 0.2f, 0.13f ),
                                Radius = 0.8f,
                                Channel1 = ThemeChannel .Button,
                                Channel2 = ThemeChannel .Hover,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Apply",
                                        TextSize = 24
                                    }
                                }
                            }, new ( ) {
                                ObjectName = "Setting Name",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 42,
                                Unclamped = true,
                                Size = new Vector2 ( 0.62f, 0.2f ),
                                Position = new Vector2 ( -0.75f, 0.8f )
                            }, new ( ) {
                                ObjectName = "Setting Description",
                                Type = UIType .Text,
                                String = "Loading...",
                                TextSize = 32,
                                Unclamped = true,
                                Size = new Vector2 ( 0.62f, 0.25f ),
                                Position = new Vector2 ( -0.75f, 0.35f )
                            }
                        }
                    }
                },
                {
                    "Theme List", new ( ) {
                        ObjectName = "Theme List",
                        Mark = true,
                        Size = Vector2 .one,
                        Channel1 = ThemeChannel .Clear,
                        PostBuild = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( true );
                            TaskCompletionSource < bool > waiter = new ( );
                            if ( !AppList [ appID ] .Bools .ContainsKey ( "themelist_setup" ) ) {
                                AppList [ appID ] .Bools .Add ( "themelist_setup", false );
                            }
                            GameObject This = AppList [ appID ] .Buffer .Get;
                            if ( This == null ) return;
                            VerticalLayoutGroup VLG = This .AddComponent < VerticalLayoutGroup > ( );
                            if ( VLG == null ) return; else {
                                VLG .childForceExpandWidth = false;
                                VLG .childForceExpandHeight = false;
                                VLG .childAlignment = TextAnchor .MiddleCenter;
                            }
                            UIPackage _TT = AppList [ appID ] .Prefabs [ "Theme Tag" ] with {
                                Parent = AppList [ appID ] .UI [ "Theme List" ],
                                ScrollRect = AppList [ appID ] .UI [ contents ]
                                .GetComponent < ScrollTunnel > ( ) .ScrollRect
                            }; AppList [ appID ] .Prefabs [ "Theme Tag" ] = _TT;
                            List < UIPackage > _build = new ( );
                            for ( int t = 0; t < themes .Count; t++ ) {
                                int T = t;
                                UIPackage newie = AppList [ appID ] .Prefabs [ "Theme Tag" ] with { };
                                _build .Add ( newie );
                                UITheme theme = themes [ T ];
                                UIPackage __TT = _build [ T ] with {
                                    ObjectName = $"theme_{ T }",
                                    Theme = theme
                                };
                                _build [ T ] = __TT;
                            }
                            RunCoroutine ( QueueJobs ( KeyValuePair .Create (  appID, _build ), async ( ) => {
                                        await AppList [ appID ] .Tasks [ "check construction" ] .Run ( );
                                        for ( int f = 0; f < themes .Count; f++ ) {
                                            int F = f;
                                            if ( !AppList [ appID ] .UI .Keys .Contains ( $"theme_{ F }" ) ) return;
                                            GameObject tag = AppList [ appID ] .UI [ $"theme_{ F }" ];
                                            if ( tag == null ) return;
                                            UIPanel tagpanel = tag .GetComponent < UIPanel > ( );
                                            if ( tagpanel == null ) return;
                                            UIText name = tag .transform .GetChild ( 0 ) .GetComponent < UIText > ( );
                                            if ( name == null ) return;
                                            UIText author = tag .transform .GetChild ( 1 ) .GetComponent < UIText > ( );
                                            if ( author == null ) return;
                                            UITheme theme = tagpanel .UIP .Theme;
                                            if ( theme == null ) return;
                                            tagpanel .CreateHoverBehavior ( theme, ThemeChannel .Button, ThemeChannel .Hover );
                                            name .Text .overflowMode = TextOverflowModes .Ellipsis;
                                            author .Text .overflowMode = TextOverflowModes .Ellipsis;
                                            name .Retheme ( theme, true );
                                            name .SetString ( $"<align=center>{ theme .name }" );
                                            author .Retheme ( theme, true );
                                            author .SetString ( $"<align=center>{ theme .author }" );
                                            AddClickAction ( tag, ( ) => {
                                                    if ( AppList [ appID ] . UI [ appID ] != null ) {
                                                        AppList [ appID ] . UI [ appID ]
                                                            .GetComponent < UIPanel > ( )
                                                            .Retheme ( theme, true, appID )
                                                        ;
                                                        Theme = theme;
                                                        phone_lastTheme .Value = $"{ theme .author }:{ theme .name }";
                                                    }
                                                    if ( mainTray != null ) {
                                                        mainTray .trayPanel .Retheme ( theme );
                                                    }
                                                    Notify ( $"System theme set to { theme .name }." );
                                                }
                                            );
                                        }
                                        AppList [ appID ] .Bools [ "themelist_setup" ] = true;
                                        waiter .SetResult ( true );
                                    }
                                ),appID
                            );
                            await waiter .Task;
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            await Task .Delay ( 25 );
                        }
                        
                    }
                },
                {
                    "Theme Tag", new ( ) {
                        ObjectName = "Theme Tag",
                        StorePackage = true,
                        Mark = true,
                        Size = new Vector2 ( 0.8f, 0.17f ),
                        Radius = 0.93f,
                        SkipsRethemes = true,
                        Channel1 = ThemeChannel .Button,
                        Channel2 = ThemeChannel .Hover,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Name Field",
                                Type = UIType .Text,
                                SkipsRethemes = true,
                                TextSize = 54,
                                Position = new Vector2 ( 0, 0.33f ),
                                Size = new Vector2 ( 0.95f, 1f ),
                                Unclamped = true
                            }, new ( ) {
                                ObjectName = "Author Field",
                                Type = UIType .Text,
                                SkipsRethemes = true,
                                TextSize = 38,
                                Position = new Vector2 ( 0, -0.33f ),
                                Size = new Vector2 ( 0.95f, 1f ),
                                Unclamped = true
                            }
                        }
                    }  
                },
                {
                    "Phone Settings", new ( ) {
                        ObjectName = "Phone Settings",
                        Mark = true,
                        Size = Vector2 .one,
                        Radius = 0.6f,
                        StorePackage = true,
                        Channel1 = ThemeChannel .Clear,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Aspect Control",
                                Size = new Vector2 ( 0.85f, 0.18f ),
                                Expands = false,
                                Channel1 = ThemeChannel .Header,
                                Children = new ( ) {
                                    new ( ) {
                                        ObjectName = "Text",
                                        Type = UIType .Text,
                                        String = "Adjust Scale",
                                        Size = new Vector2 ( 0.6f, 0.3f ),
                                        Position = new Vector2 ( -0.3f, 0.4f ),
                                        TextSize = 36
                                    }, new ( ) {
                                        ObjectName = "Scale Slider",
                                        Type = UIType .Slider,
                                        Width = 0.85f,
                                        Channel1 = ThemeChannel .Text,
                                        Channel2 = ThemeChannel .Button,
                                        Size = new Vector2 ( 0.3f, 0.2f ),
                                        Position = new Vector2 ( 0f, -0.7f )
                                    }, new ( ) {
                                        ObjectName = "Scale Input",
                                        Type = UIType .Input,
                                        Width = 0.85f,
                                        Size = new Vector2 ( 0.85f, 0.2f ),
                                        Position = new Vector2 ( 0f, -0.2f )
                                    }, new ( ) {
                                        ObjectName = "Apply",
                                        Radius = 0.6f,
                                        Size = new Vector2 ( 0.18f, 0.25f ),
                                        Position = new Vector2 ( 0.66f, 0.4f ),
                                        Channel1 = ThemeChannel .Button,
                                        Children = new ( ) {
                                            new ( ) {
                                                Type = UIType .Text,
                                                String = "<align=center>Apply",
                                                TextSize = 24
                                            }
                                        }
                                    }, new ( ) {
                                        ObjectName = "Cancel",
                                        Radius = 0.6f,
                                        Size = new Vector2 ( 0.18f, 0.25f ),
                                        Position = new Vector2 ( 0.21f, 0.4f ),
                                        Channel1 = ThemeChannel .Button,
                                        Children = new ( ) {
                                            new ( ) {
                                                Type = UIType .Text,
                                                String = "<align=center>Cancel",
                                                TextSize = 24
                                            }
                                        }
                                    }
                                }
                                
                            }
                        },
                        PostBuild = async ( ) => {
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( true );
                            TaskCompletionSource < bool > waiter = new ( );
                            if ( !AppList [ appID ] .Bools .ContainsKey ( "settings_setup" ) ) {
                                AppList [ appID ] .Bools .Add ( "settings_setup", false );
                            }
                            GameObject This = AppList [ appID ] .Buffer .Get
                                .transform .parent .parent .parent .gameObject;
                            if ( This == null ) return;
                            VerticalLayoutGroup VLG = This .AddComponent < VerticalLayoutGroup > ( );
                            if ( VLG == null ) return; else {
                                VLG .childForceExpandWidth = false;
                                VLG .childForceExpandHeight = false;
                                VLG .childAlignment = TextAnchor .MiddleCenter;
                                VLG .spacing = 16;
                            }
                            bool ready = false;
                            UISlider Slider = This .transform .GetChild ( 0 )
                                .GetChild ( 1 ) .GetComponent < UISlider > ( );
                            UIInput Input = This .transform .GetChild ( 0 )
                                .GetChild ( 2 ) .GetComponent < UIInput > ( );
                            UIPanel Apply = This .transform .GetChild ( 0 )
                                .GetChild ( 3 ) .GetComponent < UIPanel > ( );
                            UIPanel Cancel = This .transform .GetChild ( 0 )
                                .GetChild ( 4 ) .GetComponent < UIPanel > ( );
                            if ( Slider == null || Input == null || Apply == null || Cancel == null ) return;
                            Slider .slider .minValue = 0.5f; Slider .slider .maxValue = 4f;
                            Input .input .characterValidation = TMP_InputField .CharacterValidation .Decimal;
                            Slider .slider .onValueChanged .AddListener ( ( float value ) => {
                                    if ( Slider == null || Input == null ) return; else {
                                        Input .input .text = value .ToString ( "F2" );
                                        if ( ready ) {
                                            Vector3 scale = Vec2to3 ( Vec2 ( value ) );
                                            AppList [ appID ] .UI [ appID ] .transform .localScale = scale;
                                        }
                                    }
                                }
                            );
                            Input .input .onSubmit .AddListener ( ( string value ) => {
                                    if ( Slider == null || Input == null ) return; else {
                                        float .TryParse ( value, out float falue );
                                        if ( falue < 0.5f ) { falue = 0.5f; Input .input .text = "0.5"; }
                                        if ( falue > 4f ) { falue = 4f; Input .input .text = "4"; }
                                        Slider .slider .value = falue;
                                    }
                                }
                            );
                            UIPackage UIP = This .GetComponent < UIPanel > ( ) .UIP;
                            Apply .CreateHoverBehavior ( UIP .Theme, Apply .mainChannel, Apply .hoverChannel );
                            AddClickAction ( Apply .gameObject, async ( ) => {
                                    if ( Slider == null || Input == null ) return; else {
                                        phone_Scale .Value = Slider .slider .value;
                                        Notify ( $"Phone Scale changed to { Input .input .text }." ); 
                                    }
                                }
                            );
                            Cancel .CreateHoverBehavior ( UIP .Theme, Cancel .mainChannel, Cancel .hoverChannel );
                            AddClickAction ( Cancel .gameObject, async ( ) => {
                                    if ( Slider == null || Input == null ) return; else {
                                        Slider .slider .value = phone_Scale .Value;
                                    }
                                }
                            );
                            Slider .slider .value = phone_Scale .Value;
                            GameObject pButtons = GameObject .Find ( "/Canvas/GameUI/MainUI/Panel_ProductivityButtons" );
                            if ( pButtons == null ) return;
                            List < UIPackage > jobs = new ( );
                            for ( int x = 0; x < pButtons .transform .childCount; x ++
                            ) {
                                GameObject child = pButtons .transform .GetChild ( x ) .gameObject;
                                //Debug .Log ( child .name );
                                UIPackage AppPkg = AppList [ appID ] .Prefabs [ "Applet Bar" ] with {
                                    Parent = AppList [ appID ] .UI [ "Phone Settings" ],
                                    ObjectName = $"{ child .name }",
                                    ScrollRect = AppList [ appID ] .UI [ contents ]
                                    .GetComponent < ScrollTunnel > ( ) .ScrollRect
                                };
                                jobs .Add ( AppPkg );
                            }
                            GameObject horiButtons = Canvas .transform .Find ( "Mask_Horizontal/Panel_ProductivityButtons" ) .gameObject;
                            GameObject vertButtons = Canvas .transform .Find ( "Mask_Vertical/Panel_ProductivityButtons" ) .gameObject;
                            RunCoroutine ( QueueJobs ( KeyValuePair .Create ( appID, jobs ), async ( ) => {
                                        string Format ( string In ) {
                                            System .Text .StringBuilder oot = new ( );
                                            oot .Append ( In [ 0 ] );
                                            for ( int i = 1; i < In .Length; i++ ) {
                                                if ( char .IsUpper ( In [ i ] ) ) {
                                                    oot .Append ( ' ' );
                                                }
                                                oot .Append ( In [ i ] );
                                            }
                                            return oot .ToString ( );
                                        }
                                        for ( int d = 1; d < This .transform .childCount; d ++ ) {
                                            int D = d;
                                            GameObject child = This .transform .GetChild ( D ) .gameObject;
                                            if ( child == null ) continue;
                                            UIPanel MoveUp = child .transform .GetChild ( 0 ) .GetComponent < UIPanel > ( );
                                            UIPanel MoveDown = child .transform .GetChild ( 1 ) .GetComponent < UIPanel > ( );
                                            UIPanel Hide = child .transform .GetChild ( 2 ) .GetComponent < UIPanel > ( );
                                            UIText AppName = child .transform .GetChild ( 3 ) .GetComponent < UIText > ( );
                                            if ( AppName == null ) continue;
                                            if ( AppName == null || MoveUp == null || MoveDown == null || Hide == null ) continue;
                                            
                                            string processedName = Format ( child .name .Substring ( 7 ) );
                                            //showInList = Config .Bind ( "Apps", $"Show { processedName }", true, "" ); 
                                            AppName .SetString ( processedName );
                                            
                                            MoveUp .CreateHoverBehavior ( UIP .Theme, MoveUp .mainChannel, MoveUp .hoverChannel );
                                            MoveDown .CreateHoverBehavior ( UIP .Theme, MoveDown .mainChannel, MoveDown .hoverChannel );
                                            //Hide .CreateHoverBehavior ( UIP .Theme, Hide .mainChannel, Hide .hoverChannel );
                                            AddClickAction ( MoveUp .gameObject, ( ) => {
                                                    if ( MoveUp .gameObject == null || pButtons == null ||
                                                        horiButtons == null || vertButtons == null
                                                    ) return; else {
                                                        Transform main = pButtons .transform .Find ( child .name );
                                                        Transform hori = horiButtons .transform .Find ( child .name );
                                                        Transform vert = vertButtons .transform .Find ( child .name );
                                                        int mainInd = main .GetSiblingIndex ( );
                                                        if ( mainInd > 0 ) {
                                                            main .SetSiblingIndex ( mainInd - 1 );
                                                            hori .SetSiblingIndex ( mainInd - 1 );
                                                            vert .SetSiblingIndex ( mainInd - 1 );
                                                            child .transform .SetSiblingIndex ( child .transform .GetSiblingIndex ( ) - 1 );
                                                        }
                                                    }
                                                }
                                            );
                                            AddClickAction ( MoveDown .gameObject, ( ) => {
                                                    if ( MoveDown .gameObject == null || pButtons == null ||
                                                        horiButtons == null || vertButtons == null
                                                    ) return; else {
                                                        Transform main = pButtons .transform .Find ( child .name );
                                                        Transform hori = horiButtons .transform .Find ( child .name );
                                                        Transform vert = vertButtons .transform .Find ( child .name );
                                                        int mainInd = main .GetSiblingIndex ( );
                                                        int sibCount = main .parent .childCount;
                                                        if ( mainInd < sibCount - 1 ) {
                                                            main .SetSiblingIndex ( mainInd + 1 );
                                                            hori .SetSiblingIndex ( mainInd + 1 );
                                                            vert .SetSiblingIndex ( mainInd + 1 );
                                                            child .transform .SetSiblingIndex ( child .transform .GetSiblingIndex ( ) + 1 );
                                                        }
                                                    }
                                                }
                                            );
                                            AddClickAction ( Hide .gameObject, ( ) => {
                                                    if ( Hide .gameObject == null || pButtons == null ||
                                                        horiButtons == null || vertButtons == null ||
                                                        child .name .Substring ( 7 ) == "modPhone"
                                                    ) return; else {
                                                        GameObject main = pButtons .transform .Find ( child .name ) .gameObject;
                                                        GameObject hori = horiButtons .transform .Find ( child .name ) .gameObject;
                                                        GameObject vert = vertButtons .transform .Find ( child .name ) .gameObject;
                                                        if ( main == null || hori == null || vert == null ) return; else {
                                                            if ( !AppList [ appID ] .Bools .ContainsKey ( $"showapp_{ processedName }" ) ) {
                                                                AppList [ appID ] .Bools .Add ( $"showapp_{ processedName }", true );
                                                            }
                                                            bool isVis = AppList [ appID ] .Bools [ $"showapp_{ processedName }" ];
                                                            isVis = !isVis; AppList [ appID ] .Bools [ $"showapp_{ processedName }" ] = isVis;
                                                            Hide .mainChannel = isVis ? ThemeChannel .Text : ThemeChannel .System;
                                                            Hide .Retheme ( Theme );
                                                            //Hide .Recolor ( isVis ? UIP .Theme .textColor : UIP .Theme .headerColor );
                                                            main .SetActive ( isVis );
                                                            hori .SetActive ( isVis );
                                                            vert .SetActive ( isVis );
                                                        }
                                                    }
                                                }
                                            );
                                            child .SetActive ( true );
                                        }
                                        ScrollRect SR = AppList [ appID ] .UI [ contents ]
                                            .GetComponent < ScrollTunnel > ( ) .ScrollRect;
                                        
                                        DOTween .To (
                                            ( ) => SR .verticalNormalizedPosition,
                                            change => SR .verticalNormalizedPosition = change,
                                            1f,
                                            0.1f
                                        );
                                        waiter .SetResult ( true );
                                    }
                                )
                            );
                            await waiter .Task;

                            ready = true;

                            AppList [ appID ] .Bools [ "settings_setup" ] = true;
                            //await waiter .Task;
                            AppList [ appID ] .UI [ "Back Button" ]
                                .transform .GetChild ( 0 ) .gameObject
                                .SetActive ( false );
                            await Task .Delay ( 25 );
                        }
                    }
                },
                {
                    "Applet Bar", new ( ) {
                        Expands = false,
                        Size = new Vector2 ( 0.85f, 0.09f ),
                        Channel1 = ThemeChannel .Header,
                        StartInactive = true,
                        Children = new ( ) {
                            new ( ) {
                                ObjectName = "Move Up",
                                Position = new Vector2 ( 0.17f, 0 ),
                                Size = new Vector2 ( 0.12f, 0.6f ),
                                Channel1 = ThemeChannel .Button,
                                Radius = 0.6f,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Up",
                                        TextSize = 24
                                    }
                                }
                            },
                            new ( ) {
                                ObjectName = "Move Down",
                                Position = new Vector2 ( 0.5f, 0 ),
                                Size = new Vector2 ( 0.16f, 0.6f ),
                                Channel1 = ThemeChannel .Button,
                                Radius = 0.6f,
                                Children = new ( ) {
                                    new ( ) {
                                        Type = UIType .Text,
                                        String = "<align=center>Down",
                                        TextSize = 24
                                    }
                                }
                            },
                            new ( ) {
                                ObjectName = "Hide",
                                Position = new Vector2 ( 0.8f, 0 ),
                                Size = new Vector2 ( 0.1f, 0.6f ),
                                Radius = 0.6f,
                                Channel1 = ThemeChannel .Text
                            },
                            new ( ) {
                                ObjectName = "App Name",
                                Type = UIType .Text,
                                TextSize = 32,
                                Position = new Vector2 ( -0.7f, 0 ),
                                Size = new Vector2 ( 0.6f, 0.5f )
                            }
                        }
                    }
                }
            }
        };
    }
}