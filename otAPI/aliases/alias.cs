using System;
using System .Collections .Generic;

namespace _otAPI {
    public class Alias {
        public string name; //alias name
        public string description; //brief description
        public Depot depot;//category to file alias under
        public CfgLink cfgLink = null; // maps out to cfg params if alias is a cfg alias
        public Action < CfgLink, string, Action < string [ ] >, bool, string [ ] >
            cfgAction = otAPI .CfgAlias; // preconfigured settings method for cfg aliases
        public Action < string [ ] > action; //callable
        public AuxTiming auxTiming = AuxTiming .Before; //determines when aux methods are called for cfg aliases
        public bool frontEnd; //whether it's a user-accessible from text aliases, or backend
        public bool passThrough; //whether this should be accessible to other mods (ie. a universal /help)
        public Arg [ ] args; //arguments (optional, etc)

        // classic constructor: feeds info about which method to alias to in your own plugin
        public Alias (
            string _name,
            Depot _depot,
            string _desc,
            Action < string [ ] > _action,
            bool _frontEnd,
            bool _passThrough,
            Arg [ ] _args
        ) {
            name = _name;
            description = _desc;
            depot = _depot;
            action = _action;
            frontEnd = _frontEnd;
            passThrough = _passThrough;
            args =
                _args != null
                ? _args
                : Array .Empty < Arg > ( );
            if ( _passThrough ) {
                bool found = false;
                if ( otAPI .passthroughs .Count > 0 ) {
                    foreach ( string pt in otAPI .passthroughs ) { if ( pt == "_name" ) found = true; }
                }
                if ( !found ) otAPI .passthroughs .Add ( "_name" );
            }
        }
        // cfg constructor: uses the CfgAlias method
        public Alias (
            string _name,
            Depot _depot,
            string _desc,
            Arg [ ] _args,
            CfgLink _cfgLink
        ) {
            name = _name;
            description = _desc;
            depot = _depot;
            action = null;
            frontEnd = true;
            passThrough = false;
            args =
                _args != null
                ? _args
                : Array .Empty < Arg > ( );
            cfgLink = _cfgLink;
        }
        // cfg constructor w/ optional aux method when you need to append extras
        public Alias (
            string _name,
            Depot _depot,
            string _desc,
            Arg [ ] _args,
            CfgLink _cfgLink,
            Action < string [ ] >
            _action = null,
            AuxTiming _auxTiming = AuxTiming.Before
        ) {
            name = _name;
            description = _desc;
            depot = _depot;
            action = _action;
            cfgLink = _cfgLink;
            auxTiming = _auxTiming;
            frontEnd = false;
            passThrough = false;
            args =
                _args != null
                ? _args
                : Array .Empty < Arg > ( );
        }

        public bool Verify (
            string [ ] _args,
            out Dictionary < int, VerificationError > err,
            out string clarifier
        ) {
            clarifier = "";
            err = new ( );
            if ( _args .Length == 0 ) { return true; }
            if ( args .Length > 0 ) {
                for ( int i = 0; i < args .Length; i++ ) {
                    if ( !args [ i ] .optional && _args .Length < i ) {
                        err [ i ] = VerificationError .NonOptionalOmitted;
                        string type = "";
                        switch ( args [ i ] .type) {
                            case ArgType .Bool: type = $"bool ( true or false)."; break;
                            case ArgType .Int: type = $"int (examples: 1, 5, 67)."; break;
                            case ArgType .Float: type = $"float (examples: 0.5, 1, 2.73)."; break;
                            case ArgType .String: type = $"string (examples: bwa, guh)."; break;
                            case ArgType .HexColor: type = $"hex color (examples: FFCA36, bc1120)."; break;
                        }
                        clarifier = $"Must include a { type }";
                        return false;
                    }
                    string arg = "";
                    try { arg = _args [ i ]; }
                    catch ( Exception ) { arg = ""; }
                    if ( args [ i ] .optional && arg == "" ) continue;
                    bool valid = !args [ i ] .optional | ( args [ i ] .optional && arg .Length != 0 );
                    switch ( args [ i ] .type ) {
                        case ArgType .HexColor:
                            if ( arg != "" ) { if ( arg [ 0 ] == '#' ) { arg = arg .Substring ( 1 ); } }
                            if ( ( !otAPI .ValidateHex ( arg ) | arg .Length != 6 ) && valid ) {
                                err [ i ] = VerificationError .BadHexColor;
                                clarifier = $"Valid examples: FFCA36, bc1120.";
                                return false;
                            }
                        break;
                        case ArgType .String:
                            if ( args [ i ] .minIn != null && args [ i ] .maxIn != null && valid ) {
                                if ( arg .Length != ( int ) args[ i ] .minIn &&
                                    args [ i ] .minIn == args [ i ] .maxIn ) {
                                    err [ i ] = VerificationError .BadStringSize;
                                    clarifier = $"Must be exactly { args [ i ] .minIn } characters.";
                                    return false;
                                }
                                if ( arg .Length < ( int ) args [ i ] .minIn |
                                    arg .Length > ( int ) args [ i ] .maxIn ) {
                                    err [ i ] = VerificationError .BadStringSize;
                                    clarifier = $"Must be between { args [ i ] .minIn } and { args [ i ] .maxIn } characters.";
                                    return false;
                                }
                            }
                        break;
                        case ArgType .Int:
                            if ( int .TryParse ( arg, out int _intresult ) && valid ) {
                                if ( args [ i ] .minIn != null && args [ i ] .maxIn != null) {
                                    try { if ( _intresult < ( int ) args [ i ] .minIn ) {
                                            err [ i ] = VerificationError .OutsideRange;
                                            clarifier = $"Must be between { args [ i ] .minIn} and { args [ i ] .maxIn }.";
                                            return false;
                                        }
                                    }
                                    catch ( Exception ) {
                                        err [ i ] = VerificationError .BadTypeComparison; return false;
                                    }
                                    try { if ( _intresult > ( int ) args [ i ] .maxIn ) {
                                            err [ i ] = VerificationError .OutsideRange;
                                            clarifier = $"Must be between { args [ i ].minIn } and { args [ i ] .maxIn }.";
                                            return false;
                                        }
                                    }
                                    catch ( Exception ) {
                                        err [ i ] = VerificationError .BadTypeComparison; return false;
                                    }
                                }
                            }
                            else {
                                err [ i ] = VerificationError .IntExpected;
                            return false; }
                        break;
                        case ArgType .Float:
                            if ( float .TryParse ( arg, out float _floatresult ) && valid ) 
                            {
                                if ( args [ i ] .minIn != null && args [ i ] .maxIn != null )
                                {
                                    try { if ( _floatresult < ( float ) args [ i ] .minIn ) {
                                            err [ i ] = VerificationError .OutsideRange;
                                            clarifier = $"Must be between { args [ i ] .minIn } and { args [ i ] .maxIn }.";
                                            return false;
                                        }
                                    } catch ( Exception ) { err [ i ] = VerificationError .BadTypeComparison; return false; }
                                    try { if ( _floatresult > ( float ) args [ i ] .maxIn ) {
                                            err [ i ] = VerificationError .OutsideRange;
                                            clarifier = $"Must be between { args [ i ] .minIn } and { args [ i ] .maxIn }.";
                                            return false;
                                        }
                                    }
                                    catch ( Exception ) {
                                        err [ i ] = VerificationError .BadTypeComparison; return false;
                                    }
                                }
                            }
                            else {
                                err [ i ] = VerificationError .FloatExpected;
                            return false; }
                        break;
                        case ArgType .Bool:
                            if ( ( arg != "true" | arg != "false" ) && valid ) {
                                err [ i ] = VerificationError .BoolExpected; return false;
                            }
                        break;
                    }
                }
            }
            return true;
        }
    }
}