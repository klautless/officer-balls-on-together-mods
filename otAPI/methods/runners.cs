using System;
using System .Collections;

using UnityEngine;

namespace _otAPI {
    public partial class otAPI {
        public static Coroutine RunCoroutine (
            IEnumerator ienumerator,
            string DepotFolder = null
        ) {
            if ( RoutineRunner == null ) {
                Debug .Log ( "otAPI CoreRoutine error! Routine cancelled." );
                return null;
            }
            IEnumerator CancelWrapper ( ) {
                bool wasCancelled = false;
                Action cancel = ( ) => {
                    wasCancelled = true;
                };
                if ( AppList .ContainsKey ( DepotFolder ) ) {
                    AppList [ DepotFolder ] .isRunning = true;
                    AppList [ DepotFolder ] .cancel = cancel;
                }
                while ( ienumerator .MoveNext ( ) && !wasCancelled ) {
                    yield return ienumerator .Current;
                }
                if ( AppList .ContainsKey ( DepotFolder ) ) {
                    AppList [ DepotFolder ] .routine = null;
                    AppList [ DepotFolder ] .isRunning = false;
                    AppList [ DepotFolder ] .cancel = null;
                }
            }
            if ( DepotFolder != null ) {
                return AppList [ DepotFolder ] .routine = RoutineRunner
                    .StartCoroutine ( CancelWrapper ( ) );
            } else {
                return RoutineRunner .StartCoroutine ( ienumerator );
            }
        }
        public static IEnumerator RunCoroutine (
            IEnumerator ienumerator,
            bool returns
        ) {
            if ( RoutineRunner == null ) {
                Debug .Log ( "otAPI RoutineRunner error! Routine cancelled." );
                yield break;
            }
            CoreRoutine = RoutineRunner .StartCoroutine ( ienumerator );
            while ( ienumerator .MoveNext ( ) ) {
                yield return ienumerator .Current;
            }
        }

        public static IEnumerator RunDelayed (
            IEnumerator action,
            float delay
        ) {
            if ( RoutineRunner == null ) {
                Debug .Log ( "otAPI RoutineRunner error! Routine cancelled." );
                yield break;
            }
            yield return new WaitForSeconds ( delay );
            IEnumerator _action = RunCoroutine ( action, true );
            while ( _action .MoveNext ( ) ) {
                yield return _action .Current;
            }
            
        }
        public static void RunDelayed (
            Action action,
            float delay
        ) {
            if ( RoutineRunner == null ) {
                Debug .Log ( "otAPI RoutineRunner error! Routine cancelled." );
                return;
            }
            CoreRoutine = RoutineRunner .StartCoroutine ( _runDelayed ( action, delay ) );
        }
        internal static IEnumerator _runDelayed (
            Action action,
            float delay
        ) {
            yield return new WaitForSeconds ( delay );
            action .Invoke ( );
        }
        public static void CancelJobs ( string DepotFolder ) {
            if ( AppList [ DepotFolder ] != null )
                ConstructionQueue .Clear ( );
                AppList [ DepotFolder ] .cancel ? .Invoke ( );
                ConstructionFree = true;
                //AppList [ DepotFolder ] .routine = null;
        }
    }
}