using System .Collections .Generic;

using UnityEngine;

namespace _otAPI {
    public partial class otAPI {
        
        private static Dictionary < string, Color > washingMachine =
        new Dictionary < string, Color >
        {
            { "border", new Color ( 0.4235f, 0.4392f, 0.7686f ) },
            { "body", new Color ( 0.0706f, 0.1216f, 0.2157f ) },
            { "header", new Color ( 0.4157f, 0.3020f, 0.3255f ) },
            { "text", new Color ( 0.945f, 0.965f, 0.941f ) },
            { "button", new Color ( 0.5529f, 0.5569f, 0.6314f ) },
            { "hover", new Color ( 0.3647f, 0.3843f, 0.7137f ) },
            { "system", new Color ( 0.3647f, 0.6902f, 0.6706f ) },
            { "systemhover", new Color ( 0.4157f, 0.6118f, 0.7059f ) }
        };
        internal static List < UITheme > themes = new ( ) {
            new (
                "meteorite", "ob",
                "#0f241d", "#323240", "#443e4f", "#b0a9a0",
                "#3e465c", "#404e6b", "#B1555B", "#C36A6D"
            ),
            new (
                "chocolate", "ob",
                "#5f3131", "#4f281c", "#5c372d", "#d3e6dd",
                "#693a26", "#7f462e", "#ce9278", "#d3a28d"
            ),
            new (
                "materwelon", "ob",
                "#3a5e32", "#ff577e", "#f78fb6", "#343842",
                "#2e8e66", "#4aa562", "#d9215b", "#e63970"
            ),
            new (
                "...and dragons", "ob",
                "#aa6950", "#c3876f", "#4f827d", "#f5e1a8",
                "#84ac7b", "#7dbd82", "#84333b", "#91343d"
            ),
            new (
                "banana andy", "ob",
                "#ffd45d", "#fbdc98", "#9e7f67", "#f4f7d9",
                "#e8c963", "#ebd27f", "#d4b042", "#dcbe65"
            ),
            new (
                "orange creamsicle", "ob",
                "#e97d45", "#d5d6db", "#d75e40", "#f1ac7b",
                "#ed682b", "#fe8a24", "#EA6D29", "#FF6029"
            ),
            new (
                "knockout", "ob",
                "#8b232c", "#631831", "#b12c3a", "#f2c9cd",
                "#9a1d2e", "#b83336", "#9B1342", "#E5135B"
            ),
            new (
                "2am", "ob",
                "#1e4771", "#4b54aa", "#383A69", "#ceb79f",
                "#4e2cab", "#4c3bad", "#403458", "#504566"
            ),
            new (
                "ghost house", "ob",
                "#f5ede1", "#6a4d52", "#633E6C", "#dac6c3",
                "#766b8c", "#877aa1", "#D09C83", "#E2AA83"
            ),
            new (
                "washing machine", "ob",
                washingMachine [ "border" ], washingMachine [ "body" ],
                washingMachine [ "header" ], washingMachine [ "text" ],
                washingMachine [ "button" ], washingMachine [ "hover" ],
                washingMachine [ "system" ], washingMachine [ "systemhover" ]
            ),
            new (
                "headline (light)", "ob",
                "#2f383c", "#e2e6e1", "#4390d9", "#e2e6e1",
                "#5aafd3", "#75c7db", "#34a853", "#49bc67"
            ),
            new (
                "headline (dark)", "ob",
                "#2f383c", "#262226", "#4390d9", "#e2e6e1",
                "#5aafd3", "#75c7db", "#34a853", "#49bc67"
            ),
        };
    }
}