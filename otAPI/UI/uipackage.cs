using System;
using System .Collections .Generic;
using System .Reflection;

using UnityEngine;
using UnityEngine .UI;

namespace _otAPI {
    #nullable enable
    public struct UIPackage {
        private string ? _objectName; public string ObjectName {
            get => _objectName ?? "UI Object";
            set => _objectName = value;
        }
        private GameObject ? _parent; public GameObject Parent {
            get => _parent ?? otAPI .rootHUD;
            set => _parent = value;
        }
        private List < UIPackage > ? _children; public List < UIPackage > Children {
            get => _children ?? [ ];
            set => _children = value;
        }
        public Action ? PostBuild;
        private bool ? _mark; public bool Mark {
            get => _mark ?? false;
            set => _mark = value;
        }
        public Dictionary < string, UIPackage > ? Prefabs;
        public Dictionary < string, Action > ? Actions;
        public Dictionary < string, AsyncAction > ? Tasks;
        public Dictionary < string, bool > ? Bools;
        public Dictionary < string, float > ? Floats;
        public Dictionary < string, int > ? Ints;
        public Dictionary < string, string > ? Strings;
        public Dictionary < string, Vector2 > ? Vectors;
        public List < string > ? PersistentUI;
        public List < string > ? PersistentUpdates;
        public Action ? Action;
        public ScrollRect ? ScrollRect;
        public AspectGroup ? Aspect;
        private UIType ? _type; public UIType Type {
            get => _type ?? UIType .Panel;
            set => _type = value;
        }
        private string ? _string; public string String {
            get => _string ?? "";
            set => _string = value;
        }
        private string ? _path; public string Path {
            get => _path ?? "";
            set => _path = value;
        }
        private string ? _placeholder; public string Placeholder {
            get => _placeholder ?? "";
            set => _placeholder = value;
        }
        private Vector2 ? _position; public Vector2 Position {
            get => _position ?? Vector2 .zero;
            set => _position = value;
        }
        private Vector2 ? _size; public Vector2 Size {
            get => _size ?? new Vector2 ( 0.1f, 0.1f );
            set => _size = Vector2 .Max ( value, Vector2 .zero );
        }
        private float ? _radius; public float Radius {
            get => _radius ?? 0.33f;
            set => _radius = Mathf .Clamp01 ( value );
        }
        private Vector2 ? _subPosition; public Vector2 SubPosition {
            get => _subPosition ?? Vector2 .zero;
            set => _subPosition = value;
        }
        private Vector2 ? _subSize; public Vector2 SubSize {
            get => _subSize ?? new Vector2 ( 0.1f, 0.1f );
            set => _subSize = Vector2 .Max ( value, Vector2 .zero );
        }
        private float ? _subRadius; public float SubRadius {
            get => _subRadius ?? 0.1f;
            set => _subRadius = Mathf .Clamp01 ( value );
        }
        private Vector2Int ? _imgSize; public Vector2Int ImgSize {
            get => _imgSize ?? new Vector2Int ( 64, 64 );
            set => _imgSize = Vector2Int .Min ( value, Vector2Int .one );
        }
        private float ? _width; public float Width {
            get => _width ?? 0.33f;
            set => _width = Mathf .Clamp01 ( value );
        }
        private float ? _length; public float Length {
            get => _length ?? 0.2f;
            set => _length = Mathf .Clamp01 ( value );
        }
        private float ? _spacing; public float Spacing {
            get => _spacing ?? 0.8f;
            set => _spacing = Mathf .Max ( value, 0f );
        }
        private float ? _shrink; public float Shrink {
            get => _shrink ?? 0.92f;
            set => _shrink = Mathf .Clamp01 ( value );
        }
        private int ? _spacePad; public int SpacePad {
            get => _spacePad ?? 16;
            set => _spacePad = Math .Max ( value, 0 );
        }
        private int ? _textSize; public int TextSize {
            get => _textSize ?? 44;
            set => _textSize = Math .Max ( value, 1 );
        }
        private int ? _characterLimit; public int CharacterLimit {
            get => _characterLimit ?? 250;
            set => _characterLimit = Math .Clamp ( value, 1, 100000 );
        }
        private float ? _duration; public float Duration {
            get => _duration ?? 3f;
            set => _duration = Mathf .Clamp ( value, 0f, 60f );
        }
        private UITheme ? _theme; public UITheme Theme {
            get => _theme ?? otAPI .Theme;
            set => _theme = value;
        }
        private ThemeChannel ? _channel1; public ThemeChannel Channel1 {
            get => _channel1 ?? ThemeChannel .Body;
            set => _channel1 = value;
        }
        private ThemeChannel ? _channel2; public ThemeChannel Channel2 {
            get => _channel2 ?? ThemeChannel .Body;
            set => _channel2 = value;
        }
        private ThemeChannel ? _textChannel; public ThemeChannel TextChannel {
            get => _textChannel ?? ThemeChannel .Text;
            set => _textChannel = value;
        }
        private Vector2 ? _direction; public Vector2 Direction {
            get => _direction ?? Vector2 .down;
            set => _direction = value;
        }
        private Vector2 ? _pivot; public Vector2 Pivot {
            get => _pivot ?? otAPI .V2Center;
            set => _pivot = value;
        }
        private AnchorType ? _anchorType; public AnchorType AnchorType {
            get => _anchorType ?? AnchorType .Center;
            set => _anchorType = value;
        }
        private bool ? _startInactive; public bool StartInactive {
            get => _startInactive ?? false;
            set => _startInactive = value;
        }
        private float ? _imgScale; public float ImgScale {
            get => _imgScale ?? 1f;
            set => _imgScale = value;
        }
        private bool ? _storePackage; public bool StorePackage {
            get => _storePackage ?? false;
            set => _storePackage = value;
        }
        private bool ? _unclamped; public bool Unclamped {
            get => _unclamped ?? false;
            set => _unclamped = value;
        }
        private bool ? _expands; public bool Expands {
            get => _expands ?? true;
            set => _expands = value;
        }
        private bool ? _useClick; public bool UseClick {
            get => _useClick ?? true;
            set => _useClick = value;
        }
        private bool ? _skipsRethemes; public bool SkipsRethemes {
            get => _skipsRethemes ?? false;
            set => _skipsRethemes = value;
        }
        private bool ? _limitRangeByGrabber; public bool LimitRangeByGrabber {
            get => _limitRangeByGrabber ?? false;
            set => _limitRangeByGrabber = value;
        }
        private ArgType ? _argType; public ArgType ArgType {
            get => _argType ?? ArgType .Null;
            set => _argType = value;
        }
        private SliderType ? _sliderType; public SliderType SliderType {
            get => _sliderType ?? SliderType .Horizontal;
            set => _sliderType = value;
        }
        private Assembly ? _assembly; public Assembly Assembly {
            get => _assembly ?? Assembly .GetExecutingAssembly ( );
            set => _assembly = value;
        }
        internal string _depotFolder; public string DepotFolder {
            get => _depotFolder ?? "";
            set => _depotFolder = value;
        }

        
        private bool ? _overrideSize; internal bool overrideSize {
            get => _overrideSize ?? false;
            set => _overrideSize = value;
        }
        private bool ? _laidOut; internal bool LaidOut {
            get => _laidOut ?? false;
            set => _laidOut = value;
        }
    }
    #nullable disable
}