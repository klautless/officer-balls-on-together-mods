
using System;
using System .Linq;
using UnityEngine;

namespace _otAPI {
    public class UITheme {
        public string name { get; private set; } = "";
        public string author { get; private set; } = "";
        public Color borderColor { get; private set; }
        public Color bodyColor { get; private set; }
        public Color headerColor { get; private set; }
        public Color textColor { get; private set; }
        public Color buttonColor { get; private set; }
        public Color hoverColor { get; private set; }
        public Color systemButtonColor { get; private set; }
        public Color systemHoverColor { get; private set; }
        private UITheme extension = null; // do not use yet
        public UITheme ( 
            string _name, string _author,
            string _border, string _body,
            string _header, string _text,
            string _button, string _hover,
            string _system, string _systemhover,
            UITheme _extension = null
        ) {
            name = _name; author = _author;
            borderColor = GetTextColor ( _border ); bodyColor = GetTextColor ( _body );
            headerColor = GetTextColor ( _header ); textColor = GetTextColor ( _text );
            buttonColor = GetTextColor ( _button ); hoverColor = GetTextColor ( _hover );
            systemButtonColor = GetTextColor ( _system ); systemHoverColor = GetTextColor ( _systemhover );
            extension = _extension;
        }
        public UITheme (
            string _name, string _author,
            Color _border, Color _body,
            Color _header, Color _text,
            Color _button, Color _hover,
            Color _system, Color _systemhover,
            UITheme _extension = null
        ) {
            name = _name; author = _author;
            borderColor = _border; bodyColor = _body; headerColor = _header;
            textColor = _text;
            buttonColor = _button; hoverColor = _hover;
            systemButtonColor = _system; systemHoverColor = _systemhover;
            extension = _extension;
        }
        public Color GetChannel (
            ThemeChannel type,
            UITheme theme
        ) {
            switch ( type ) {
                case ThemeChannel .Border: return theme .borderColor;
                case ThemeChannel .Body: return theme .bodyColor;
                case ThemeChannel .Header: return theme .headerColor;
                case ThemeChannel .Text: return theme .textColor;
                case ThemeChannel .Button: return theme .buttonColor;
                case ThemeChannel .Hover: return theme .hoverColor;
                case ThemeChannel .System: return theme .systemButtonColor;
                case ThemeChannel .SystemHover: return theme .systemHoverColor;
            }
            return Color .clear;
        }
        public string GetChannelAsTextTag (
            ThemeChannel ? type,
            bool unwrapped = false
        ) {
            if ( type == null ) return "";
            char [ ] unwanted = { '<', '>' };
            string color = "";
            switch ( type ) {
                case ThemeChannel .Border: color = ColorUtility .ToHtmlStringRGB ( borderColor ); break;
                case ThemeChannel .Body: color = ColorUtility .ToHtmlStringRGB ( bodyColor ); break;
                case ThemeChannel .Header: color = ColorUtility .ToHtmlStringRGB ( headerColor ); break;
                case ThemeChannel .Text: color = ColorUtility .ToHtmlStringRGB ( textColor ); break;
                case ThemeChannel .Button: color = ColorUtility .ToHtmlStringRGB ( buttonColor ); break;
                case ThemeChannel .Hover: color = ColorUtility .ToHtmlStringRGB ( hoverColor ); break;
                case ThemeChannel .System: color = ColorUtility .ToHtmlStringRGB ( systemButtonColor ); break;
                case ThemeChannel .SystemHover: color = ColorUtility .ToHtmlStringRGB ( systemHoverColor ); break;
            }
            return unwrapped ? string .Concat ( color .Where ( c => !unwanted .Contains ( c ) ) ) : color;
        }
        public Color GetTextColor ( string color = null) {
            if ( color != null ) {
                if ( ColorUtility .TryParseHtmlString ( color, out Color processed ) ) { return processed; }
            } return textColor; 
        }
    }
}