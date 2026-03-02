using BepInEx .Configuration;

namespace _otAPI {
    public class CfgLink
    {
        public ArgType valueType { get; private set; } = ArgType .Null; //type of ConfigEntry to map out to.
        public string changeString { get; private set; } = "was changed to"; //string shown when parameter is changed
        public string currentString { get; private set; } = "is currently set to"; //string shown when parameter is checked
        public bool skipNoti { get; private set; } = false; //skip the automated change-notifier system
        public ConfigEntry< bool > boolLink { get; private set; } = null; //links to a ConfigEntry<bool>
        public ConfigEntry < int > intLink { get; private set; } = null; //links to a ConfigEntry<int>
        public ConfigEntry < float > floatLink { get; private set; } = null; //links to a ConfigEntry<float>
        public ConfigEntry < string > stringLink { get; private set; } = null; //links to a ConfigEntry<string>
        public CfgLink (
            ArgType type,
            ConfigEntry < bool > inBool,
            string _changedString = "was changed to",
            string _currentString = "is currently set to",
            bool _skipNoti = false
        ) {
            CommonInit ( type, _changedString, _currentString, _skipNoti );
            boolLink = inBool;
        }
        public CfgLink (
            ArgType type,
            ConfigEntry < int > inInt,
            string _changedString = "was changed to",
            string _currentString = "is currently set to",
            bool _skipNoti = false
        ) {
            CommonInit ( type, _changedString, _currentString, _skipNoti );
            intLink = inInt;
        }
        public CfgLink (
            ArgType type,
            ConfigEntry < float > inFloat,
            string _changedString = "was changed to",
            string _currentString = "is currently set to",
            bool _skipNoti = false
        ) {
            CommonInit ( type, _changedString, _currentString, _skipNoti );
            floatLink = inFloat;
        }
        public CfgLink (
            ArgType type,
            ConfigEntry < string > inString,
            string _changedString = "was changed to",
            string _currentString = "is currently set to",
            bool _skipNoti = false
        ) {
            CommonInit ( type, _changedString, _currentString, _skipNoti );
            stringLink = inString;
        }
        private void CommonInit (
            ArgType type,
            string _changedString,
            string _currentString,
            bool _skipNoti
        ) {
            valueType = type;
            changeString = _changedString;
            currentString = _currentString;
            skipNoti = _skipNoti;
        }
    }
}