using System .Collections .Generic;

namespace _otAPI {
    public class Depot
    {
        internal string name { get; private set; } //Name of your mod's depot
        internal string shortName { get; private set; } //Shortend name for menus
        internal string author { get; private set; } //Author of the depot
        internal string description { get; private set; } //Description of what this alias group does

        internal UIPackage ? app; internal bool UsesApp = false;
        internal UIPackage ? icon; internal bool UsesIcon = false;
        internal string prefix { get; private set; } //Prefix for the depot's aliases
        internal Dictionary < string, Alias > aliases { get; private set; } = new ( ); //Aliases stored within this depot
        public Depot (
            string Name,
            string ShortName,
            string Author,
            string Description,
            string Prefix
        ) {
            name = Name;
            shortName = ShortName;
            author = Author;
            description = Description;
            prefix = Prefix;
        }
        public Depot (
            string Name,
            string ShortName,
            string Author,
            string Description,
            string Prefix,
            UIPackage App,
            UIPackage Icon
        ) {
            name = Name;
            shortName = ShortName;
            author = Author;
            description = Description;
            prefix = Prefix;
            app = App;
            icon = Icon;
            UsesApp = true; UsesIcon = true;
        }
        public bool GetAlias(string name, out Alias alias) {
            alias = null;
            if ( aliases .ContainsKey ( name ) )
                { alias = aliases [ name ]; return true; }
            return false;
        }
    }
}