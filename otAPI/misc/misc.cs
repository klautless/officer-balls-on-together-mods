using System;
using System .Collections .Generic;
using System .Threading .Tasks;

using UnityEngine;
using UnityEngine .EventSystems;
using UnityEngine .UI;

namespace _otAPI {
    public class AppStack {
        public Buffer < GameObject > Buffer = new ( );
        public Dictionary < string, GameObject > UI = new ( );
        public List < string > PersistentUI = new ( );
        public List < string > PersistentUpdates = new ( );
        public Dictionary < string, UIPackage > Prefabs = new ( );
        public Dictionary < string, Action > Actions = new ( );
        public Dictionary < string, bool > Bools = new ( );
        public Dictionary < string, float > Floats = new ( );
        public Dictionary < string, int > Ints = new ( );
        public Dictionary < string, string > Strings = new ( );
        public Dictionary < string, Vector2 > Vectors = new ( );
        public Dictionary < string, AsyncAction > Tasks = new ( );
        public Coroutine routine = null; public Action cancel = null;
        public bool isRunning { get; internal set; } = false;
        public List < TaskCompletionSource < bool > > runners = new ( );
    }
    public class Cache < Token, Contents > {
        private readonly int capacity;
        private readonly Dictionary < Token, LinkedListNode < Contents > > cache;
        private readonly LinkedList < Contents > indexer;
        public Cache ( int Capacity ) {
            capacity = Capacity;
            cache = new Dictionary < Token, LinkedListNode < Contents > > ( );
            indexer = new LinkedList < Contents > ( );
        }
        public bool TryGetValue ( Token key, out Contents value ) {
            if ( cache .TryGetValue ( key, out var node ) ) {
                value = node.Value;
                indexer .Remove ( node );
                indexer .AddLast ( node );
                return true;
            }
            value = default!;
            return false;
        }
        public void Add ( Token key, Contents value ) {
            if ( cache.Count >= capacity && ! cache .ContainsKey ( key ) ) {
                var first = indexer .First;
                indexer .RemoveFirst();
                indexer .Remove ( first .Value );
            }
            var newNode = new LinkedListNode < Contents > ( value );
            indexer .AddLast ( newNode );
            cache [ key ] = newNode;
        }
    }
    public class ScrollTunnel : MonoBehaviour { public ScrollRect ScrollRect; }
    public class Buffer < Value > {
        private readonly List < Value > buffer = new ( );
        public Value Get {
            get => buffer [ 0 ];
        }
        public void Set ( Value value ) {
            buffer .Clear ( );
            buffer .Add ( value );
        }
    }
    public struct AsyncAction {
        public Func < Task > Action { get; set; }
        public async Task Run ( ) { if ( Action == null ) return; else await Action ( ); }
    }
    public struct AspectGroup {
        public enum Modes { None, WidthControlsHeight, HeightControlsWidth, FitInParent, EnvelopeParent };
        public Modes Mode;
        public float Ratio;
    }
    internal class RoutineRunner : MonoBehaviour { }
    public class ScrollForwarder : MonoBehaviour, IScrollHandler {
        public ScrollRect scrollRect;
        public void OnScroll ( PointerEventData data ) {
            scrollRect .OnScroll ( data );
        }
    }
    public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        public RectTransform grabber;
        public RectTransform primaryRect;
        public RectTransform canvasRect;
        public Action onRelease = null;
        public bool limitRangeByHandle = false;
        private Vector2 dragOffset;
        private Vector2 vmin; private Vector2 vmax;
        public void OnBeginDrag ( PointerEventData data ) {
            Vector2 localPoint;
            Vector2 v1;
            Vector2 v2;
            RectTransformUtility .ScreenPointToLocalPointInRectangle (
                canvasRect, data .position, data .pressEventCamera, out localPoint
            );
            RectTransformUtility .ScreenPointToLocalPointInRectangle (
                canvasRect, Vector2 .zero, data .pressEventCamera, out v1
            );
            RectTransformUtility .ScreenPointToLocalPointInRectangle (
                canvasRect, new Vector2 ( Screen .width, Screen .height ), data .pressEventCamera, out v2
            );
            vmin = v1; vmax = v2;
            dragOffset =
                limitRangeByHandle
                ? localPoint - primaryRect .anchoredPosition
                : localPoint - grabber .anchoredPosition
            ;
        }
        public void OnDrag ( PointerEventData data ) {
            Vector2 targetPos;
            RectTransformUtility .ScreenPointToLocalPointInRectangle (
                canvasRect, data .position, data .pressEventCamera, out targetPos
            );
            targetPos -= dragOffset;
            if ( limitRangeByHandle ) {
                Vector2 clampedPos = ClampToRect ( targetPos, canvasRect, grabber );
                Vector2 delta = primaryRect .anchoredPosition - clampedPos;
                //grabber .anchoredPosition = clampedGrabberPos;
                primaryRect .anchoredPosition -= delta; // * otAPI .ScaleFactor;
                
            }
            else {
                primaryRect .anchoredPosition = ClampToRect ( targetPos, canvasRect, primaryRect );
            }
        }
        public void OnEndDrag ( PointerEventData data ) {
            if ( onRelease  != null ) onRelease .Invoke ( );
        }
        private Vector2 ClampToRect (
            Vector2 targetPos,
            RectTransform canvas,
            RectTransform element
        ) {
            if ( !limitRangeByHandle ) {
                Vector2 min = new Vector2 (
                    canvas .rect .xMin + element .rect .width * 0.5f,
                    canvas .rect .yMin + element .rect .height * 0.5f
                );
                Vector2 max = new Vector2 (
                    canvas .rect .xMax - element .rect .width * 0.5f,
                    canvas .rect .yMax - element .rect .height * 0.5f
                );
                return new Vector2 (
                    Mathf .Clamp ( targetPos .x, min .x, max .x ),
                    Mathf .Clamp ( targetPos .y, min .y, max .y )
                );
            } else {
                Vector2 min = new Vector2 (
                    vmin .x - element .rect .width - element .anchoredPosition .x * 0.5f,
                    vmin .y - element .rect .height + element .anchoredPosition .y * 0.5f
                );
                Vector2 max = new Vector2 (
                    vmax .x - element .rect .width - element .anchoredPosition .x,
                    vmax .y - element .rect .height - element .anchoredPosition .y
                );
                return new Vector2 (
                    Mathf .Clamp ( targetPos .x, min .x, max .x ),
                    Mathf .Clamp ( targetPos .y, min .y, max .y )
                );
            }
        }
    }
}