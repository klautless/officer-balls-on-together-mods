using System;
using System .Collections .Generic;
using System .Linq;
using System .Text;

using HarmonyLib;

using UnityEngine;
using UnityEngine .EventSystems;

using TMPro;

namespace _otAPI {
    [ HarmonyPatch ]
    internal class HarmonyHooks : MonoBehaviour {
        [ HarmonyPatch (
            typeof ( PlayerCustomizationController ),
            ( "JoinServerNotificationRPC_Original_1" ) )
        ] [ HarmonyPrefix ]
        internal static void SpawnHook ( ) {
            if ( !otAPI .initialized ) {
                GameObject mainUI = GameObject .Find ( "Canvas/GameUI/MainUI" );
                GameObject root = new GameObject (
                    "Custom Apps",
                    typeof ( RectTransform ),
                    typeof ( AudioSource )
                );
                root .transform .SetParent ( mainUI.transform, false );
                root .transform .localPosition = Vector2 .zero;
                root .transform .localScale = Vector3 .one;
                otAPI .rootHUD = root;
                otAPI .aus = root .GetComponent < AudioSource > ( );
                if ( otAPI .RoutineRunner == null ) {
                    otAPI .RoutineRunner =
                    new GameObject ( "otAPI RoutineRunner", typeof ( RoutineRunner ) )
                    .GetComponent < RoutineRunner > ( );
                    DontDestroyOnLoad ( otAPI .RoutineRunner );
                }
                otAPI .RunCoroutine ( otAPI .Initializer ( ) );
            }
        }
        [ HarmonyPatch (
            typeof ( PlayerController ),
            ( "OnDespawned" ) )
        ] [ HarmonyPrefix ]
        internal static void DespawnHook ( PlayerController __instance ) {
            if ( __instance .IsReturningMenu ) {
                otAPI .RunCoroutine ( otAPI .Despawner ( ) );
                otAPI .rootHUD = null;
                otAPI .mainTray = null;
                otAPI .updateCycles = [ ];
            }
        }

        [ HarmonyPatch ( 
            typeof ( MainMenuUIController ),
            "Update" )
        ] [ HarmonyPostfix ]
        internal static void DevLauncher ( ) {
            if ( Input .GetKey ( KeyCode .LeftShift ) &&
            Input .GetKey ( KeyCode .LeftControl ) &&
            Input .GetKeyDown ( KeyCode .B ) ) {
                Debug .Log ( "dev solo testing lobby created" );
                MultiplayerManager mm_instance = MonoSingleton < MultiplayerManager > .I;
                Traverse .Create ( mm_instance )
                    .Field ( "FilterPlayerValue" )
                    .SetValue ( 0 );
                Traverse .Create ( mm_instance )
                    .Field ( "FilterTypeValue" )
                    .SetValue ( FilterType .Private );
                MainMenuUIController ui_instance = MonoSingleton < MainMenuUIController > .I;
                TMP_InputField lobbyfield = AccessTools
                    .FieldRefAccess < MainMenuUIController, TMP_InputField >
                    ( "_createSessionNameInputField" )
                    ( ui_instance )
                ;
                lobbyfield .text = "dev testing solo";
                mm_instance .CreateLobby ( );
            }
        }

        [ HarmonyPatch (
            typeof( MultiplayerManager ),
            ( "CreateLobby" ) )
        ] [ HarmonyPrefix ]
        internal static void ApplyModdedTag ( ref List < bool > ___FilterSocialTags ) {
            var val = MonoSingleton < MultiplayerManager > .I .FilterPlayerValue;
            var text = MonoSingleton < MainMenuUIController > .I .CreateSessionNameInputField .text;
            ___FilterSocialTags [ 4 ] = true;
        }

        [ HarmonyPatch (
            typeof ( TextChannelManager ),
            ( "OnEnterPressed" ) )
        ] [ HarmonyPrefix ]
        internal static bool AliasCheck ( ) {
            string text = MonoSingleton < UIManager > .I ? .MessageInput .text;
            if ( text == null ) { return true; }
            string [ ] splitAlias = text.Split (
                new char [ ] { ' ' },
                StringSplitOptions .RemoveEmptyEntries
            );
            
            if ( splitAlias .Length == 0 ) { return true; }
            
            if ( otAPI .CheckAlias (
                splitAlias [ 0 ] .ToLower ( ),
                true,
                out List < Alias > aliases
            ) ) {
                string [ ] args = splitAlias .Length > 1
                    ? splitAlias .Skip ( 1 )
                                 .ToArray ( )
                    : [ "" ]
                ;
                foreach ( Alias alias in aliases ) {
                    if ( otAPI .Invoker ( alias, args ) ) {
                        bool pass = false;
                        if ( otAPI .passthroughs .Count > 0 ) {
                            foreach ( string pt in otAPI .passthroughs ) {
                                if ( text == pt ) { pass = true; }
                            }   
                        }
                        ResetPostMessage ( pass );
                        return false;
                    }
                    else {
                        ResetPostMessage ( );
                        return false;
                    }
                }
            }
            return true;
        }
        private static void ResetPostMessage ( bool skiptext = false ) {
            MonoSingleton < TaskManager > .I .SetLockState (
                NetworkSingleton < MusicManager > .I .IsActive
                ? LockState .Music
                : LockState .Free
            );
            EventSystem .current .SetSelectedGameObject ( null );
            if ( !skiptext ) { MonoSingleton < UIManager > .I .MessageInput .text = ""; }
        }

        [ HarmonyPatch (
            typeof ( TextChannelManager ),
            ( "SendMessageAsync" ) )
        ] [ HarmonyPrefix ]
        internal static bool HelpCloser ( byte [ ] textBytes ) {
            string text = Encoding .Unicode .GetString ( textBytes );
            if ( otAPI .passthroughs .Count > 0 ) {
                foreach ( string pt in otAPI .passthroughs ) {
                    if ( text == pt ) { return false; }
                }
            }
            return true;
        }
    }
}