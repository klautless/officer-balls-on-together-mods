using System;
using System .Collections .Generic;
using System .Text .RegularExpressions;

using BepInEx .Configuration;

using UnityEngine;

namespace _otAPI {
    public partial class otAPI {
        public static Depot CreateDepot (
            string Name,
            string ShortName,
            string Author,
            string Description,
            UIPackage ? App,
            string Prefix
        ) {
            if ( !Regex.IsMatch ( Name, @"^[\p{L}\s'-]+$" ) ) {
                Debug .Log ( $"{ errPrefix }Bad Depot name! Only letters, spaces, hyphens, and apostrophes allowed." );
                return null;
            }
            else if ( Name .Length > 18 ) {
                Debug.Log ( $"{ errPrefix }Depot name too long! Must be 18 characters or under." );
                return null;
            }
            else if ( !Regex .IsMatch ( ShortName, @"^[\p{L}\s'-]+$" ) ) {
                Debug.Log ( $"{ errPrefix }Bad Short name! Only letters, spaces, hyphens, and apostrophes allowed." );
                return null;
            }
            else if ( ShortName .Length > 15 ) {
                Debug.Log ( $"{ errPrefix }Short name must be 15 characters or under." );
                return null;
            }
            Depot depot = new Depot( Name, ShortName, Author, Description, App, Prefix );
            depots .Add ( depot );
            Debug .Log ( $"otAPI Depot created: { depot .name }" );
            return depot;
        }
        public static void Register (
            string name,
            string description,
            Depot depot,
            Action< string[] > action,
            bool frontEnd,
            bool passThrough,
            Arg [ ] args
        ) {
            Depot _depot = null;
            foreach ( Depot d in depots ) {
                if ( d.name == depot.name ) _depot = d;
            }
            if ( _depot == null ) {
                Debug .Log ( "Invalid depot!" );
                return;
            }
            _depot.aliases [ name ] = new Alias (
                name, depot,
                description, action,
                frontEnd, passThrough,
                args
            );
        }
        public static void AddCfg (
            string name,
            string description,
            Depot depot,
            Arg [ ] args,
            CfgLink cfgLink,
            Action < string [ ] > auxMethod = null,
            AuxTiming auxTiming = AuxTiming .Before
        ) {
            Depot _depot = null;
            foreach ( Depot d in depots ) {
                if ( d.name == depot.name ) _depot = d;
            }
            if ( _depot == null ) {
                Debug .Log ( $"{ errPrefix }Invalid depot!" );
                return;
            }
            _depot.aliases[ name ] = new Alias (
                name, depot,
                description, args,
                cfgLink, auxMethod,
                auxTiming
            );
        }
        internal static void CfgAlias (
            CfgLink cfgLink,
            string name,
            Action < string [ ] > act,
            bool during,
            string [ ] args
        ) {
            bool playNoti = !cfgLink .skipNoti;
            if ( args .Length == 0 ) args = [ "" ];
            switch ( cfgLink .valueType ) {
                case ArgType.Bool:
                    if ( cfgLink .boolLink != null ) {
                        ConfigEntry < bool > boolLink = cfgLink .boolLink;
                        if ( args [ 0 ] == "" ) {
                            boolLink .Value = !boolLink .Value;
                            string boolState =
                                boolLink .Value
                                ? "true."
                                : "false."
                            ;
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } { boolState }" );
                        }
                        else if ( args [ 0 ] == "true" ) {
                            boolLink .Value = true;
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } true." );
                        }
                        else if ( args[ 0 ] == "false" ) {
                            boolLink .Value = false;
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } false." );
                        }
                        else { 
                            Dictionary < int, VerificationError > dict =
                                new Dictionary < int, VerificationError > {
                                { 0, VerificationError .BoolExpected }
                            }; 
                            ErrorMsg ( dict );
                        }
                    };
                break;
                case ArgType .Int:
                    if ( cfgLink .intLink != null ) {
                        ConfigEntry < int > intLink = cfgLink .intLink;
                        if ( args[ 0 ] == "" ) {
                            if ( during ) act .Invoke( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .currentString } { intLink .Value }." );
                        }
                        else if ( int.TryParse( args[ 0 ], out int intOut ) ) {
                            intLink .Value = intOut;
                            if ( during ) act.Invoke( args );
                            if ( playNoti ) Notify( $"{ name } { cfgLink .changeString } { intLink .Value }." );
                        }
                        else {
                            Dictionary < int, VerificationError > dict =
                                new Dictionary < int, VerificationError > {
                                { 0, VerificationError .IntExpected }
                            };
                            ErrorMsg ( dict );
                        }
                    }
                break;
                case ArgType .Float:
                    if ( cfgLink.floatLink != null ) {
                        ConfigEntry < float > floatLink = cfgLink .floatLink;
                        if ( args [ 0 ] == "" ) {
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .currentString } { floatLink .Value }." );
                        }
                        else if ( float .TryParse ( args [ 0 ], out float floatOut ) ) {
                            floatLink .Value = floatOut;
                            if ( during ) act.Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } { floatLink .Value }." );
                        }
                        else {
                            Dictionary < int, VerificationError > dict =
                                new Dictionary < int, VerificationError > {
                                { 0, VerificationError .FloatExpected }
                            };
                            ErrorMsg ( dict );
                        }
                    }
                break;
                case ArgType .String:
                    if ( cfgLink .stringLink != null ) {
                        ConfigEntry < string > stringLink = cfgLink .stringLink;
                        if ( args [ 0 ] == "" ) {
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .currentString } \"{ stringLink .Value }\"." );
                        }
                        else {
                            stringLink .Value = args [ 0 ];
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } \"{ stringLink .Value }\"." );
                        }
                    }
                break;
                case ArgType .HexColor:
                    if ( cfgLink .stringLink != null ) {
                        ConfigEntry < string > colorLink = cfgLink .stringLink;
                        if ( args [ 0 ] == "" ) {
                            if ( during ) act .Invoke( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .currentString } \"<#{ colorLink .Value }>{ colorLink .Value }</mark>\"." );
                        }
                        else {
                            if ( args [ 0 ] [ 0 ] == '#' ) args [ 0 ] = args [ 0 ] .Substring ( 1 );
                            colorLink.Value = args [ 0 ];
                            if ( during ) act .Invoke ( args );
                            if ( playNoti ) Notify ( $"{ name } { cfgLink .changeString } \"<#{ colorLink .Value }>{ colorLink .Value }</mark>\"." );
                        }
                    }
                break;
            }
        }
        public static bool CheckAlias (
            string name,
            bool needFrontEnd,
            out List < Alias > aliases
        ) { 
            bool foundAny = false;
            aliases = [ ];
            foreach ( Depot d in depots) {
                string prefix = d .prefix;
                if ( name .Substring ( 0, prefix .Length ) == prefix ) {
                    string checkname = name .Substring ( prefix .Length );
                    if ( d .aliases .ContainsKey ( checkname ) ) {
                        if ( d .aliases [ checkname ] .frontEnd | !needFrontEnd ) {
                            aliases .Add( d .aliases [ checkname ] ); foundAny = true;
                        }
                    } 
                }
            }
            return foundAny;
        }
        public static bool Invoker (
            Alias alias,
            string [ ] args
        ) {
            if ( alias
                .Verify (
                    args,
                    out Dictionary < int, VerificationError > errors,
                    out string clarifier
                )
            ) {
                try {
                    if ( alias .auxTiming == AuxTiming .Before && alias .action != null ) {
                        alias .action .Invoke ( args );
                    }
                    if ( alias .cfgLink != null ) {
                        alias .cfgAction .Invoke (
                            alias .cfgLink,
                            alias .name, alias .action,
                            alias .auxTiming == AuxTiming .During, args
                        );
                    }
                    if ( alias .auxTiming == AuxTiming .After && alias .action != null ) {
                        alias .action .Invoke ( args );
                    }
                    return true;
                }
                catch ( Exception ex ) {
                    Debug .Log ( $"otAPI Invoker failure with alias { alias .name }: { ex .Message }" );
                    return false;
                }
            }
            else { 
                ErrorMsg ( errors, clarifier );
                return false;
            }
        }
    }
}