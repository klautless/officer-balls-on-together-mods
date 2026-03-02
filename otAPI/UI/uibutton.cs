using System;
using System .Collections;

using UnityEngine;
using UnityEngine .EventSystems;

namespace _otAPI {
    public class UIButton : MonoBehaviour {
        public EventTrigger ev { get; private set; }
        public UIPackage UIP { get; set; }
        internal IEnumerator Initialize (
            UIPackage Package,
            UIPanel Panel
        ) {
            if ( Package .Action == null ) {
                Debug .Log ( "otAPI: UIButton Action can't be null!" );
                yield break;
            }
            if ( Package .StorePackage ) {
                UIP = Package with { };
                Package .StorePackage = false;
            }
            Panel .CreateHoverBehavior (
                Package .Theme,
                Package .Channel1,
                Package .Channel2
            );
            ev = gameObject .GetComponent < EventTrigger > ( );
            EventTrigger .Entry click = new ( );
            click .eventID = EventTriggerType .PointerDown;
            click .callback .AddListener ( ( data ) => {
                    Click (
                        gameObject, Package .UseClick,
                        Package .Action, Panel
                    );
                }
            );
            ev .triggers .Add ( click );
            gameObject .name =
                Package .ObjectName != "UI Object"
                ? Package .ObjectName
                : "UI Button";
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
        private void Click (
            GameObject buttonObj,
            bool useClick,
            Action Action,
            UIPanel panel
        ) {
            if ( buttonObj == null ) return;
            otAPI .RunDelayed ( panel .Reset, 0.01f );
            if ( useClick ) otAPI .Click ( );
            if ( Action != null ) {
                try { Action. Invoke ( ); }
                catch ( Exception ex ) { Debug .Log ( $"otAPI: exception during button invoke: { ex }" ); }
            }
        }
    }
}