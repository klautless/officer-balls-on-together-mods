/*internal class otAPIMenu : MonoBehaviour {

        public bool overrideNotificationTray { get; private set; } = false;
        public static float scaleFactor { get; private set; }
        public static UITheme theme { get; set; }
        public Coroutine UIRunner { get; private set; }
        internal static GameObject APIPageSpace;

        private static bool sortedYet = false;
        private static GameObject APIObj;
        private static GameObject DepotListFrame;
        private static UIScrollable DepotLabelPool;
        internal static bool drawerHidden = false;
        internal static float lastY = -1694f;
        private static float depotCloseDistance = 206f;
        private static float drawerMoveRate = 0.17f;
        private static int [ ] depotHeaderTextSizes = [ 60, 36, 44 ];
        private static List < DepotLabel > activeLabels = new ( );
        private static GameObject APIMenuIcon;
        private static Vector2 iconOffset = new Vector2 ( -1, 0);
        private static GameObject APITooltip;
        private static Vector2Int tooltipSize = new Vector2Int ( 296, 80 );
        internal Dictionary < string, UIPackage > Packages =
            new Dictionary < string, UIPackage > { {
                "mainMenu", new UIPackage ( ) with {
                    Position = new Vector2 ( 562f, 216f ),
                    Size = new Vector2Int ( 540, 610 ),
                    SubSize = new Vector2Int ( 530, 600 ),
                    Channel1 = ThemeChannel .Border,
                    Channel2 = ThemeChannel .Body,
                    Radius = 80,
                    StartInactive = true
                }
            }, {
                "mainGrabber", new UIPackage ( ) with {
                    Position = new Vector2 ( 0f, 258f ),
                    Size = new Vector2Int ( 510, 60 ),
                    Radius = 55,
                    Channel1 = ThemeChannel .Header
                }
            }, { "exitButton", new UIPackage ( ) with {
                    Position = new Vector2 ( -220f, 0f ),
                    Size = new Vector2Int ( 36, 36 ),
                    Radius = 70,
                    Action = MenuSwap,
                    UseClick = true,
                    Channel1 = ThemeChannel .System,
                    Channel2 = ThemeChannel .SystemHover
                }
            }, {
                "depotList", new UIPackage ( ) {
                    Position = new Vector2 ( -375f, -12f ),
                    SubPosition = new Vector2 ( 24f, 0f ),
                    Size = new Vector2Int ( 240, 530 ),
                    SubSize = new Vector2Int ( 240, 518 ),
                    Channel1 = ThemeChannel .Border,
                    Channel2 = ThemeChannel .Body,
                    Radius = 55,
                }
            }, {
                "depotHider", new UIPackage ( ) with {
                    Position = new Vector2 ( -108f, 0f ),
                    Size = new Vector2Int ( 12, 420 ),
                    Radius = 12,
                    Action = DepotDrawer,
                    UseClick = true,
                    Channel1 = ThemeChannel .Body,
                    Channel2 = ThemeChannel .Body
                }
            }, {
                "tray", new UIPackage ( ) with {
                    Position = new Vector2 ( -260, 418 ),
                    Size = new Vector2Int ( 580, 240 ),
                    SubPosition = new Vector2 ( 0, 115 ),
                    SubSize = new Vector2Int ( 560, 40 ),
                    Radius = 20, SubRadius = 40,
                    Direction = Vector2 .down,
                    Spacing = 1.2f,
                    Theme = theme,
                    Channel1 = ThemeChannel .Clear,
                    Channel2 = ThemeChannel .Header
                }
            }, {
                "trayHandle", new UIPackage ( ) with {
                    Position = new Vector2 ( -315, 98 ),
                    Size = new Vector2Int ( 48, 48 ),
                    Radius = 48,
                    Theme = theme,
                    Channel1 = ThemeChannel .Button
                }
            }, {
                "iconPanel", new UIPackage ( ) with {
                    Position = new Vector2 ( 936, -467 ),
                    Size = new Vector2Int ( 50, 48 ),
                    SubPosition = Vector2 .zero,
                    SubSize = new Vector2Int ( 68, 68 ),
                    Radius = 40,
                    Channel1 = ThemeChannel .Border,
                    Path = "otAPI/images/frames/button.png"
                }
            }, {
                "tooltipPanel", new UIPackage ( ) with {
                    Position = new Vector2 ( 834.5f, -467f ),
                    Size = new Vector2Int ( 148, 40 ),
                    SubPosition = Vector2 .zero,
                    SubSize = new Vector2Int ( 296, 80 ),
                    Radius = 28,
                    TextSize = 24,
                    StartInactive = true,
                    Channel1 = ThemeChannel .Border,
                    String = "Modcyclopedia",
                    Path = "otAPI/images/frames/tooltip.png"
                }
            }
        };
        public Canvas c;
        internal void Initialize ( ) {
            if ( theme == null )
            { theme = otAPI .themes .FirstOrDefault ( ); }
            if ( !sortedYet )
            {
                List < Depot > sortedDepots = otAPI .depots
                    .OrderBy ( p => p .author )
                    .ThenBy ( p => p .shortName )
                    .ToList ( )
                ;
                otAPI .depots = sortedDepots;
                sortedYet = true;
            }
            GameObject Canvas = GameObject .Find ( "Canvas" );
            c = Canvas .GetComponent < Canvas > ( );
            scaleFactor = c .scaleFactor;
            otAPI .RunCoroutine ( otAPI .menu .ImmediateUI ( ) );
        }
        void Update ( ) {
            if ( c != null ) {
                if ( Time .frameCount % 3 != 0 ) return;
                if ( scaleFactor != c .scaleFactor ) scaleFactor = c .scaleFactor;
            }
            if ( otAPI .initialized & EventSystem .current .currentSelectedGameObject == null ) {
                if ( Input .GetKey ( KeyCode .LeftControl ) &  Input .GetKeyDown ( KeyCode .Plus ) ) {
                    ResUp ( );
                }
                if ( Input .GetKey ( KeyCode .LeftControl ) &  Input .GetKeyDown ( KeyCode .Minus ) ) {
                    ResDown ( );
                }
            }
        }
        private IEnumerator ImmediateUI ( ) {
            yield return MakeTray ( );
            yield return MakeTooltip ( );
            yield return MakeMenuIcon ( );
            yield return MakeMenu ( );
            yield return BuildDepotLabels ( );
        }
        private IEnumerator MakeTray ( ) {
            UIPackage _tray = Packages [ "tray" ] with { Parent = otAPI .rootHUD };
            Packages [ "tray " ] = _tray;
            UINotificationTray tray = otAPI .CreateUI < UINotificationTray > (
                Packages [ "tray" ]
            );
            yield return null;
            UIPackage _tH = Packages [ "trayHandle" ] with {
                Parent = tray .tray,
                //Channel1 = Packages [ "tray" ] .Channel2
            };
            GameObject Handle = otAPI .CreateUI < UIPanel > (
                _tH
            ) .gameObject;
            yield return null;
            otAPI .MakeGrabber ( Handle, tray .tray .gameObject, true );
            otAPI .mainTray = tray;
        }
        private IEnumerator MakeMenu ( ) {
            APIObj = otAPI .CreateUI < UIPanel > (
                Packages [ "mainMenu" ]
            ) .gameObject;
            yield return null;
            UIPackage _dL = Packages [ "depotList" ] with {
                Parent = APIObj
            };
            DepotListFrame = otAPI .CreateUI < UIPanel > (
                _dL
            ) .gameObject;
            yield return null;
            UIPackage _dI = Packages [ "depotList" ] with {
                Parent = DepotListFrame,
                Position = Packages [ "depotList" ] .SubPosition,
                Size = Packages [ "depotList" ] .SubSize,
                Channel1 = Packages [ "depotList" ] .Channel2
            };
            otAPI .CreateUI < UIPanel > (
                _dI
            );
            yield return null;
            UIPackage _dLK = Packages [ "depotList" ] with {
                Parent = DepotListFrame,
                Position = new Vector2 ( 10f, 0f ),
                Channel1 = ThemeChannel .Clear,
                Channel2 = ThemeChannel .Clear
            };
            DepotLabelPool = otAPI .CreateUI < UIScrollable > (
                _dLK
            );
            yield return null;
            UIPackage _dU = Packages [ "depotHider" ] with {
                Parent = DepotListFrame
            };
            otAPI .CreateUI < UIButton > (
                _dU
            );
            yield return null;
            UIPackage _sL = Packages [ "mainMenu" ] with {
                Position = Vector2 .zero,
                Size = Packages [ "mainMenu" ] .SubSize,
                Channel1 = Packages [ "mainMenu" ] .Channel2,
                Parent = APIObj,
                StartInactive = false
            };
            GameObject SettingsLiner = otAPI .CreateUI < UIPanel > (
                _sL
            ) .gameObject;
            yield return null;
            yield return APIPageSpace = new GameObject ( "Page", typeof ( RectTransform ) );
            APIPageSpace .transform .SetParent ( SettingsLiner .transform, false );
            APIPageSpace .transform .localPosition = Vector2 .zero;

            UIPackage _gb = Packages [ "mainGrabber" ] with {
                Parent = SettingsLiner
            };
            GameObject GripBanner = otAPI .CreateUI < UIPanel > (
                _gb
            ) .gameObject;
            yield return null;
            otAPI .MakeGrabber (
                GripBanner,
                APIObj,
                true
            );
            UIPackage _eb = Packages [ "exitButton" ] with {
                Parent = GripBanner
            };
            otAPI .CreateUI < UIButton > (
                _eb
            );
            yield return null;
            //DepotDrawer ( );
            MenuSwap ( );
            
        }
        internal static void MenuSwap ( ) {
            if ( otAPI .menu .pageRunner == null ) {
                otAPI .menu .pageRunner = otAPI .RunCoroutine ( otAPI .menu .UILock ( ) );
                if ( !drawerHidden ) DepotDrawer ( );
                float moveDur = 0.2f;
                float swapY = APIObj .transform .localPosition .y;
                APIObj .transform .DOLocalMoveY ( lastY, moveDur ) .OnComplete ( ( ) => {
                        APIObj .SetActive ( true );
                        otAPI .menu .pageRunner = null;
                    }
                );
                lastY = swapY;
            }
        }
        internal static void DepotDrawer ( ) {
            float curX = DepotListFrame .transform .localPosition .x;
            float distance = depotCloseDistance;
            if ( !drawerHidden ) {
                DepotListFrame .transform .DOLocalMoveX (
                    curX + ( distance / scaleFactor ), drawerMoveRate )
                    .OnComplete ( ( ) => {
                        foreach ( DepotLabel dL in activeLabels ) { dL .gameObject .SetActive ( false ); }
                        otAPI .RunCoroutine ( otAPI .menu .RecheckLabels ( ) );
                    }
                );
            }
            else {
                foreach ( DepotLabel dL in activeLabels ) {
                    dL .gameObject .SetActive ( true );
                }
                DepotListFrame .transform .DOLocalMoveX (
                    curX - ( distance / scaleFactor ), drawerMoveRate )
                    .OnComplete ( ( ) => {
                        otAPI .RunCoroutine ( otAPI .menu .RecheckLabels ( ) );
                    }
                );
            }
            drawerHidden = !drawerHidden;
        }
        internal IEnumerator RecheckLabels ( ) {
            
            //float delay 
            yield return new WaitForSeconds ( 0.016f );

            foreach ( DepotLabel dL in activeLabels ) {
                dL .panel .HideIfCovered ( DepotLabelPool .MaskRect );
            }
        }
        private IEnumerator MakeMenuIcon ( ) {
            GameObject obj = otAPI .CreateUI < UIPanel > (
                Packages [ "iconPanel" ]
            ) .gameObject;
            yield return null;
            GameObject parent = obj;
            GameObject icon = new GameObject (
                "Menu Button",
                typeof ( RectTransform ),
                typeof ( EventTrigger )
            );
            icon .transform .SetParent ( parent .transform, false );
            icon .transform .localPosition = iconOffset;
            
            EventTrigger ev = icon .GetComponent < EventTrigger > ( );
            EventTrigger .Entry entry = new ( );
            entry .eventID = EventTriggerType .PointerEnter;
            entry .callback .AddListener ( ( data ) => {
                    if ( APITooltip != null ) {
                        APITooltip.transform.parent.gameObject.SetActive ( true );
                    }
                }
            );
            ev .triggers .Add ( entry );
            EventTrigger .Entry exit = new ( );
            exit .eventID = EventTriggerType .PointerExit;
            exit .callback .AddListener ( ( data ) => {
                    if ( APITooltip != null ) {
                        APITooltip .transform .parent .gameObject .SetActive ( false );
                    }
                }
            );
            ev .triggers .Add ( exit );
            EventTrigger .Entry click = new ( );
            click .eventID = EventTriggerType .PointerDown;
            click .callback .AddListener ( ( data ) => {
                    if ( otAPI .menu .pageRunner == null ) {
                        otAPI .Click ( );
                        MenuSwap ( );
                    }
                }
            );
            ev .triggers .Add ( click );
            UIPackage _ik = Packages [ "iconPanel" ] with {
                Position = Packages [ "iconPanel" ] . SubPosition,
                Size = Packages [ "iconPanel" ] .SubSize,
                Parent = icon
            };
            GameObject image = otAPI .CreateUI < UIImage > (
                _ik
            ) .gameObject;
            RectTransform irect = image .GetComponent < RectTransform > ( );
            irect .sizeDelta = new Vector2Int ( 100, 100 ) ; // atypical correction for a lazy guy who doesn't wanna remake sprite
            APIMenuIcon = icon;
            
        }
        private IEnumerator MakeTooltip ( ) {
            GameObject TooltipBase = otAPI .CreateUI < UIPanel > (
                Packages [ "tooltipPanel" ]
            ) .gameObject;
            yield return null;
            GameObject tooltip = new GameObject ( "otAPI_tooltip", typeof ( RectTransform ) );
            yield return null;
            tooltip .transform .SetParent ( TooltipBase .transform, false );
            tooltip .transform .localPosition = Vector3 .zero;

            RectTransform rectTransform = tooltip .GetComponent < RectTransform > ( );
            rectTransform .sizeDelta = new Vector2 ( tooltipSize .x, tooltipSize .y );
            UIPackage _ik = Packages [ "tooltipPanel" ] with {
                Position = Packages [ "tooltipPanel" ] .SubPosition,
                Size = Packages [ "tooltipPanel" ] .SubSize,
                StartInactive = false,
                Parent = tooltip
            };
            otAPI .CreateUI < UIImage > (
                _ik
            );
            yield return null;
            UIPackage _tk = _ik with {
                Position = new Vector2 ( 17f, 0f ),
            };
            otAPI .CreateUI < UIText > (
                _tk
            );
            yield return null;
            APITooltip = tooltip;
        }
        internal IEnumerator BuildDepotLabels ( ) {
            activeLabels .Clear ( );

            int bufferspace = 24;
            float top = 115f / scaleFactor;
            int index = 0;
            float hor_offset = 3.75f / scaleFactor;
            UIPackage dltags = new UIPackage ( ) with {
                Size = new Vector2Int ( 190, 40 ),
                SubPosition = Vector2 .zero,
                TextSize = 44,
                Radius = 40,
                TextChannel = ThemeChannel .Body,
                ScrollRect = DepotLabelPool .ScrollRect,
                Channel1 = ThemeChannel .Button,
                Parent = DepotLabelPool .Content
            };
            for ( int d = 0; d < otAPI .depots .Count; d++ ) {
                Depot depot = otAPI .depots [ d ];
                Vector2 pos = new Vector2 (
                    hor_offset,
                    top - index * ( bufferspace / scaleFactor )
                );
                UIPackage _dlk = dltags with {
                    Position = pos
                };
                DepotLabel dL = otAPI .CreateUnsafe < DepotLabel> ( "DepotLabel" );
                yield return null;
                dL .Initialize ( _dlk, depot );
                yield return null;
                activeLabels .Add ( dL );
                index++;
            }
        }
        internal IEnumerator PrintDepotPage ( Depot depot ) {
            yield return null;
            Debug .Log ("PDP called");
            Vector3 vec3 = Vector3 .zero; vec3 .y -= 10 / scaleFactor;
            UIPackage _name = new UIPackage {
                Position = new Vector2 ( -200, 180 ),
                String = depot .name,
                TextSize = depotHeaderTextSizes [ 0 ],
                Parent = APIPageSpace
            };
            GameObject name = otAPI .CreateUI < UIText > ( _name ) .gameObject;
            yield return null;
            otAPI .menu .pageRunner = null;
            UIPackage _author = _name with {
                String = depot .author,
                Position = vec3,
                TextSize = depotHeaderTextSizes [ 1 ],
                Parent = name
            };
            GameObject author = otAPI .CreateUI < UIText > ( _author  ) .gameObject;
            yield return null;
            UIPackage _description = _author with {
                String = depot .description,
                TextSize = depotHeaderTextSizes [ 2 ],
                Parent = author
            };
            GameObject description = otAPI .CreateUI < UIText > ( _description, "Depot Description" ) .gameObject;
            yield return null;
            UIPackage _rd = new UIPackage ( ) with {
                Parent = description,
                Position = new Vector2 ( 0, -30 ),
                Size = new Vector2Int ( 80, 40 ),
                Channel1 = ThemeChannel .Button,
                Channel2 = ThemeChannel .Hover,
                Action = ResDown
            };
            UIButton _ResDown = otAPI .CreateUI < UIButton > ( _rd );
            yield return null;
            UIPackage _ru = _rd with {
                Position = new Vector2 ( 120, 0 ),
                Parent = _ResDown .gameObject,
                Action = ResUp
            };
            GameObject _ResUp = otAPI .CreateUI < UIButton > ( _ru ) .gameObject;
            yield return null;

            otAPI .menu .pageRunner = null;


            /*
            );
            yield return null;
            descObj .verticalAlignment = VerticalAlignmentOptions .Top;
            yield return depotPageInsert = new UIScrollable (
            );
            yield return new WaitForSeconds ( 0.015f );
            while ( depotInsert == null ) {
                yield return null;
            }
            foreach ( KeyValuePair < string, Alias > keyval in selectedDepot .aliases ) {
                if ( keyval .Value .cfgLink == null ) continue;
                yield return null;
                yield return new DepotSettingsBar (
                    keyval .Value, theme, depotInsert, depotPageInsert );
            }
        }
        internal static void ResUp ( ) { GameObject .Find ( "Canvas" ) .GetComponent < Canvas > ( ) .scaleFactor += 0.05f; }
        internal static void ResDown ( ) { GameObject .Find ( "Canvas" ) .GetComponent < Canvas > ( ) .scaleFactor -= 0.05f; }
        internal IEnumerator UILock ( ) { yield return null; }
        internal IEnumerator ClearUI ( ) {
            yield return null;
            yield return otAPI .menu .StartCoroutine (
                otAPI .ClearChildrenSafely ( otAPI. rootHUD. transform ) );
        }
    }*/



    /*internal class DepotLabel : MonoBehaviour {
        internal UIPackage Package;
        public UIText label { get; private set; }
        internal UIPanel panel;
        private Depot depot;
        internal IEnumerator Initialize (
            UIPackage _Package,
            Depot Depot
        ) {
            depot = Depot;
            Package = _Package;
            transform .SetParent ( Package .Parent .transform, false );
            panel = otAPI .CreateUI < UIPanel > ( Package );
            panel .CreateHoverBehavior (
                Package .Theme,
                Package .Channel1,
                Package .Channel2
            );
            
            EventTrigger ev = otAPI .AddOrGet < EventTrigger > ( panel .gameObject );
            EventTrigger .Entry select = new ( );
            select .eventID = EventTriggerType .PointerDown;
            select .callback .AddListener ( ( data ) => {
                    if ( gameObject  == null ) return;
                    if ( otAPI .menu .pageRunner == null ) {
                        otAPI .Click ( );
                        otAPI .RunDelayed (
                            panel .Recolor,
                            0.01f
                        );
                        otAPI .menu .pageRunner = otAPI .RunCoroutine (
                            otAPI .CleanChildrenAndAct (
                                otAPIMenu .APIPageSpace .transform,
                                otAPI .menu .PrintDepotPage ( depot )
                            )
                        );
                    }
                }
            );
            UIPackage _tk = Package with {
                Position = Package .SubPosition,
                Parent = panel .gameObject
            };
            ev .triggers .Add ( select );
            UIText label = otAPI .CreateUI < UIText > ( _tk );
            label .SetString ( $"<align=center>{ depot .shortName }" );
            otAPI .ScrollCheck ( gameObject, Package );
        }
    }*/
    /*internal class DepotSettingsBar : MonoBehaviour {
        public Alias alias { get; private set; }
        internal Vector2Int size { get; private set; } = new Vector2Int ( 480, 40 );
        internal Vector2 textOffset { get; private set; } = new Vector2 ( 180f, 0f );
        internal int radius = 28;
        internal int textSize = 38;

        internal DepotSettingsBar (
            Alias _alias,
            UIPackage _Package
        ) {
            UIPackage Package = _Package;
            alias = _alias;
            GameObject par = otAPI .CreateUI < UIPanel > ( Package ) .gameObject;
            UIPackage Package2 = Package with {
                Position = Package .SubPosition,
                Parent = par
            };
            UIText UIT = otAPI .CreateUI < UIText > ( Package2 );
            UIT .SetString ( $"<align=right>{ alias .name }" );
            if ( alias .cfgLink != null ) {
                UIPackage sk = Package with {
                    Length = 100,
                    SliderType = SliderType .Horizontal,
                    Position = new Vector2 ( -210, 0 ),
                    Channel1 = ThemeChannel .Border,
                    Channel2 = ThemeChannel .Body,
                    Parent = par
                };
                UIPackage ik = sk with {
                    Width = 115
                };
                bool spawnInp = true;
                bool spawnSlider = false;
                int inputXOffset = -400;
                switch ( alias .cfgLink .valueType ) {
                    case ArgType .Bool:
                        spawnInp = false;
                    break;
                    case ArgType .Float:
                        ik .Placeholder = "float";
                        sk .ArgType = ArgType .Float;
                        spawnSlider = true;
                    break;
                    case ArgType .HexColor:
                        ik .Placeholder = "color";
                        ik .Width = 330;
                        inputXOffset = -287;
                    break;
                    case ArgType .Int:
                        ik .Placeholder = "int";
                        sk .ArgType = ArgType .Int;
                        spawnSlider = true;
                    break;
                    case ArgType .String:
                        ik .Placeholder = "string";
                        ik .Width = 330;
                        inputXOffset = -287;
                    break;
                }
                ik .Position = new Vector2 ( inputXOffset, 0 );
                if ( spawnInp ) {
                    UIInput UII = otAPI .CreateUI < UIInput > ( ik );
                }
                if ( spawnSlider ) {
                    UISlider UIS = otAPI .CreateUI < UISlider > ( sk ); 
                }
            }
        }
        private string [ ] CurrentAndPlaceholder (
            CfgLink _cfgLink,
            Arg [ ] _args
        ) {
            string [ ] results = [ "", "" ];
            // bool useBounds = false; use Package .Unclamped
            // string rangeStyle = "<size=24>"; use Package .TextSize
            switch ( _cfgLink .valueType ) {
                case ArgType .Bool:
                    results [ 0 ] =
                        _cfgLink .boolLink .Value
                        ? "true" : "false";
                break;
                case ArgType .Int:
                    results [ 0 ] =
                        _cfgLink .intLink .Value .ToString ( );
                    if (
                        _args [ 0 ] .minIn != null &&
                        _args [ 0 ] .maxIn != null
                    ) {
                        results [ 1 ] =
                            $"{ _args [ 0 ] .minIn }";
                    }
                break;
                case ArgType .Float:
                break;
                case ArgType .String:
                break;
                case ArgType .HexColor:
                break;
            }
            return results;
        }
    }*/


    /*
    public class BufferDict < Key, Value > {
        private readonly Dictionary < Key, Value > dict = new ( );
        public Value Last { get; private set; }
        public int Count => dict .Count;
        public void Add ( Key key, Value value ) {
            dict .Add ( key, value );
            Last = value;
        }
        public Value this [ Key key ] {
            get => dict [ key ];
            set {
                dict [ key ] = value;
                Last = value;
            }
        }
        public bool ContainsKey ( Key key ) => dict .ContainsKey ( key );
        public bool TryGetValue ( Key key, out Value value ) =>
            dict .TryGetValue ( key, out value );
    }*/

    /*public static IEnumerator QueueWavLoad (
            List < string > LoadTargets,
            Assembly _asm = null
        ) {
            if ( _asm == null ) _asm = Assembly .GetExecutingAssembly ( );
            while ( loadHelper != "" ) {
                yield return null;
            }
            yield return RunCoroutine ( LoadWav ( LoadTargets, _asm ), true );
            loadHelper = "";
        }
        
        private static IEnumerator LoadWav (
            List < string > LoadTargets,
            Assembly _asm = null
        ) {
            foreach ( string target in LoadTargets ) {
                Debug .Log ( target );
                yield return loadHelper = target;
                MemoryStream mem = new ( );
                yield return null;
                _asm .GetManifestResourceStream ( target )
                    .CopyTo ( mem );
                byte [ ] audioData = mem .ToArray ( );
                yield return null;
                float [ ] floatData = Convert16BitByteArrayToFloat ( audioData );
                yield return null;
                AudioClip clip = AudioClip .Create ( "Clip", floatData .Length, 2, 44100, false);
                clip .SetData ( floatData, 0 );
                ClipPool .Add ( target, clip );
                Debug .Log ( $"Clip loaded: { target }" );
                yield return null;
            }
            yield return loadHelper = "";
        }
        public static float [ ] Convert16BitByteArrayToFloat ( byte [ ] byteArray ) {
            float [ ] floatArray = new float [ byteArray .Length / 2 ];
            for ( int i = 0; i < floatArray .Length; i++ ) {
                floatArray [ i ] = ( short ) ( byteArray [ i * 2 ] | byteArray [ i * 2 + 1 ] << 8 ) / 32768f;
            }
            return floatArray;
        }*/

        /*public static bool IsInBounds (
            RectTransform Inside,
            RectTransform Bounds
        ) {
            Vector2 boundsTop = new Vector2 (
                Bounds .position .x - Bounds .rect .size .x / ScaleFactor / 2,
                Bounds .position .y - Bounds .rect .size .y / ScaleFactor / 2
            );
            Vector2 boundsBot = new Vector2 (
                Bounds .position .x + Bounds .rect .size .x / ScaleFactor / 2,
                Bounds .position .y + Bounds .rect .size .y / ScaleFactor / 2
            );
            Vector2 insideTop = new Vector2 (
                Inside .position .x - Inside .rect .size .x / ScaleFactor / 2,
                Inside .position .y - Inside .rect .size .y / ScaleFactor / 2
            );
            Vector2 insideBot = new Vector2 (
                Inside .position .x + Inside .rect .size .x / ScaleFactor / 2,
                Inside .position .y + Inside .rect .size .y / ScaleFactor / 2
            );
            return boundsBot .x >= insideTop .x && insideTop .x >= boundsTop .x
                && boundsBot .y >= insideTop .y && insideTop .y >= boundsTop .y
                && boundsBot .x >= insideBot .x && insideBot .x >= boundsTop .x
                && boundsBot .y >= insideBot .y && insideBot .y >= boundsTop .y;
        }*/

        /*

        internal static type CreateUnsafe < type > (
            string name
        ) where type : Component {
            return new GameObject ( name, typeof ( type ) ) .GetComponent < type > ( );
        }
        public static void PlayClip (
            AudioClip clip
        ) {
            if ( clip == null ) return;
            aus . PlayOneShot ( clip );
        }
        */

        /*
    public struct AudioData {
        public AudioClip Clip;
        public string Path;
        public string LogicalName;
        public AudioData (
            AudioClip _Clip,
            string _Path,
            string _LogicalName
        ) {
            Clip = _Clip;
            Path = _Path;
            LogicalName = _LogicalName;
        }
    }*/
    /*EventTrigger trigger = AddOrGet < EventTrigger > ( Handle );
            EventTrigger .Entry drag = new ( );
            drag .eventID = EventTriggerType .Drag;
            drag .callback .AddListener ( ( BaseEventData _data ) => {
                    PointerEventData data = ( PointerEventData ) _data;
                    RectTransform rect = Target .GetComponent < RectTransform > ( );
                    RectTransform grabberRect = Handle .GetComponent < RectTransform > ( );
                    Vector2 anchorPos = rect .anchoredPosition;
                    Vector2 grabberPos = grabberRect .anchoredPosition;
                    GameObject Canvas = otAPI .Canvas .gameObject;
                    RectTransform canvasRect = Canvas .GetComponent < RectTransform > ( );
                    anchorPos += data .delta / ScaleFactor;
                    Vector2 minPos = new ( );
                    Vector2 maxPos = new ( );
                    if ( LimitRangeByHandle ) {
                        grabberPos += data .delta / ScaleFactor;
                        minPos = new Vector2 (
                            canvasRect .rect .xMin + grabberRect .rect .width * 0.5f,
                            canvasRect .rect .yMin + grabberRect .rect .height * 0.5f
                        );
                        maxPos = new Vector2 (
                            canvasRect .rect .xMax - grabberRect .rect .width * 0.5f,
                            canvasRect .rect .yMax - grabberRect .rect .height * 0.5f
                        );
                        grabberPos .x = Mathf .Clamp ( grabberPos .x, minPos .x, maxPos .x );
                        grabberPos .y = Mathf .Clamp ( grabberPos .y, minPos .y, maxPos .y );
                        Vector2 diff = grabberRect .anchoredPosition - grabberPos;
                        rect .anchoredPosition -= diff;
                    } else {
                        minPos = new Vector2 (
                            canvasRect .rect .xMin + rect .rect .width * 0.5f,
                            canvasRect .rect .yMin + rect .rect .height * 0.5f
                        );
                        maxPos = new Vector2 (
                            canvasRect .rect .xMax - rect .rect .width * 0.5f,
                            canvasRect .rect .yMax - rect .rect .height * 0.5f
                        );
                        anchorPos .x = Mathf .Clamp ( anchorPos .x, minPos .x, maxPos .x );
                        anchorPos .y = Mathf .Clamp ( anchorPos .y, minPos .y, maxPos .y );
                        rect .anchoredPosition = anchorPos;
                    }
                }
            );
            trigger .triggers .Add ( drag );
            if ( ReleaseAction != null ) {
                EventTrigger .Entry release = new ( );
                release .eventID = EventTriggerType .EndDrag;
                release .callback .AddListener ( ( data ) => {
                        ReleaseAction .Invoke ( );
                    }
                );
                trigger .triggers .Add ( release );
            }*/

            /*if ( otAPI .IsInBounds ( rect, BoundaryRect ) ) {
                for ( int c = 0; c < gameObject .transform .childCount; c++ ) {
                    GameObject child = gameObject .transform .GetChild ( c ) .gameObject;
                    if ( !child .activeSelf ) child .SetActive ( true );
                }
                if ( renderer .cull ) renderer .cull = false;
            }
            else {
                for ( int c = 0; c < gameObject .transform .childCount; c++ ) {
                    GameObject child = gameObject .transform .GetChild ( c ) .gameObject;
                    if ( child .activeSelf ) child .SetActive ( false );
                }
                if ( !renderer .cull ) renderer .cull = true;
            }*/

            /*Package .Children = null;
            Vector2 TrackSize = new Vector2 (
                Package .SliderType == SliderType .Horizontal ? Package .Length : trackWidth,
                Package .SliderType == SliderType .Vertical ? Package .Length : trackWidth
            );
            string tempName = Package .ObjectName;
            int num = otAPI .AppList [ Package .DepotFolder ] .UI .Count + 1;
            UIPackage SubPack = Package with {
                ObjectName = $"UI Object { num }",
                Type = UIType .Panel,
                Size = TrackSize,
                Radius = trackRadius,
                overrideSize = true
            };
            IEnumerator subNum = otAPI .CreateUI ( SubPack );
            while ( subNum .MoveNext ( ) ) {
                yield return subNum .Current;
            }
            GameObject Slider = otAPI .AppList [ Package .DepotFolder ] .UI [ $"UI Object { num }" ];
            num += 1;
            SubPack .ObjectName = $"UI Object { num }";
            SubPack .Position = Vector2 .zero;
            SubPack .Size = knobSize;
            SubPack .Radius = knobRadius;
            SubPack .Channel1 = Package .Channel2;
            SubPack .Parent = Slider;
            IEnumerator knobpa = otAPI .CreateUI ( SubPack );
            while ( knobpa .MoveNext ( ) ) {
                yield return knobpa .Current;
            }
            Slider .name =
                tempName != "UI Object"
                ? tempName
                : "UI Slider"
            ;
            GameObject Knob = otAPI .AppList [ Package .DepotFolder ] .UI [ $"UI Object { num }" ];
            EventTrigger ev = Knob .AddComponent < EventTrigger > ( );
            EventTrigger .Entry clickStart = new ( );
            clickStart .eventID = EventTriggerType .PointerDown;
            clickStart .callback .AddListener ( ( data ) => {
                    Debug .Log ( "Clicked knob" );
                }
            );
            ev .triggers .Add ( clickStart );*/