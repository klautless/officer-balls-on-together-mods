# otAPI
  
ob / officer balls  
feb - march 2026  
documentation is wip  
  
The goals of otAPI are to make it easy to link methods to user-accessible settings and create a UI ecosystem.  
It is not meant to be used on its own, but rather in tandem with Harmony or MonoMod, and is aimed at providing good controls for the end-user.  
  
Parameters denoted with asterisks are optional.  
Documentation is split into foundational systems:  
  
[Depots](#depots)  
[Aliases](#aliases)  
[UI](#ui)  
[Themes](#themes)  
[Helper Methods](#helper-methods)  
[Misc Classes](#misc-classes)  
  
## Depots
  
[Return to top](#otapi)  
A depot functions as a container for your mod's contents.  
Depending on whether or not you want to use an app, you will create a depot in one of two ways:  
  
``otAPI .CreateDepot ( string Name, string ShortName, string Author, string Description, string Prefix );``  
  
will create a depot without an app, and  
  
``...string Description, string Prefix, UIPackage App, UIPackage Icon, List < UITheme> ThemeList );``  
  
will allow you to integrate an app.  
ThemeList is an optional list of UIThemes - the App and Icon are non-optional if you choose this creator.  
Always store the depot you create for your mod as you will have to reference it to add your aliases.  
  
Example:  
``Depot Huckeys = otAPI .CreateDepot ( "Huckeys", "Huc", "John Huckey", "A mod that makes you brave?", "/" );``  
``Depot Chonston = otAPI .CreateDepot ( "Chonston Mod", "CHO", "The Grape", "Have you ever believed in ham?", MyApp, MyIcon, MyList );`` 
  
## Aliases
  
[Return to top](#otapi)  
Aliases are the core of routing methods into either chat commands or managable settings from the modPhone.  
The best practice is to define a List < Alias > of new aliases and loop through them for registration.  
  
Basic aliases are for simple chat commands such as a /help list or a /coinflip command.  
They can take arguments as a list of strings which will be parsed and verified for content.  
  
``otAPI .Register ( string Name, string Description, Depot depot, Action< string[] > action, bool frontEnd, bool passThrough, Arg [ ] args );``  
  
All actions you setup with this must have a string array as the parameter, whether it is utilized by the method or not.  
frontEnd represents whether or not you want the alias to appear as a chat command. At this time, general aliases are not routed to the phone, so leave this at true.  
passThrough represents whether or not you want this to be a shared-functionality alias, i.e. a generic /help command that other mods could read from as well.  
  
If you want to use a CFGAlias (these will appear in the phone and map to ConfigEntry references), observe the following method:  
  
``otAPI .AddCfg ( string Name, string Description, Depot depot, Arg [ ] args, CfgLink cfgLink, Action < string [ ] > auxMethod, AuxTiming auxTiming );``  
  
auxMethod is an optional parameter to trigger an additional method on your side after CFG settings are changed.  
auxTiming is an optional parameter to set whether it is invoked before or after the CFG setting is changed (and during runs it after, but before the Notification).  
  
Example:  
Case 1: Basic aliases  
``List < Alias > BasicAliases = [ new Alias ( "Lasagna", lasagnaDepot, "To go, please.", LasagnaMethod, true, false, null ),``  
`` new Alias ( "Macaroni", lasagnaDepot, "Just don't.", MacaroniMethod, true, false, null ) ];``  
  
``foreach ( Alias alias in BasicAliases) {``  
``  otAPI .Register (``  
``  alias .name, alias.description, alias .depot,``  
``  alias .action, alias .frontEnd, alias .passThrough,``  
``  alias .args );``  
``}``  
  
Case 2: CFG aliases  
Start with existing ConfigEntry < T > entries.  
Bind them in Awake or similar as per usual.  
Valid types include string (functions as hexadecimal color as well), bool, float, and int.  
``List < Alias > cfgAliases = [``  
`` new Alias ( "My Setting", lasagnaDepot, "Sets the lasagna to green.",``  
`` new Arg [ ] { new Arg ( ArgType .Bool, true) },``  
`` new CfgLink ( ArgType .Bool, MySettingConfigEntry ) ) ];``  
``foreach ( Alias alias in cfgAliases ) {``  
`` otAPI .AddCfg (``  
``  alias .name, alias .description, alias .depot,``  
``  alias .args, alias .cfgLink, alias .action, alias .auxTiming``  
`` );``  
``}``  
  
## UI
  
[Return to top](#otapi)  
  
There are currently UITypes included in otAPI: UIPanel, UIButton, UIText, UIScrollable, UIImage, UIInput, UISlider, UINotificationTray
Foundationally, all UI elements are created with a relative proportions system.  
Position ranges from -Vector2 .one to Vector2 .one with Vector2 .zero being the center of the screen/object, 1 being top-right corner, and -1 being bottom-left.  
Scale ranges from Vector2 .zero to Vector2 .one within the parent's bounds, unless Unclamped is set to true.  
  
UI functionality is all guided through UIPackages at the core. UIPackages contain transform data as well as various bools, flags, and links to other GameObjects and components to enable tree-like structured UI creation.  
The root App Package will transfer out several contents if provided - including Bools, a dictionary of Bools, Ints, a dict of strings, etc - Prefabs ( blueprints for instantiating sub-elements of your UI), Tasks and Actions to run methods, etc.  
Many of these components are optional and will allow you to embed systems inside your UI, but you don't need to build in this way.  
  
The list below may not be 100% comprehensive.  
Core UIPackage parameters are:  
  
**Functional:**  
ObjectName ( string ) - names the object.  
DepotFolder ( string ) - set to the same name as your depot's.  
Mark ( bool ) - whether the created GameObject will be added to ``otAPI .AppList [ YourModID ] .UI``, a dictionary of gameobjects by name.  
StorePackage ( bool ) - whether to store the UIPackage on the object, if you need to reference items again post-creation.  
Type ( UIType ) - determines what kind of element you will create. Defaults to panel if left blank.  
Parent ( GameObject ) - determines the object your element spawns under. Defaults to rootHUD (custom apps canvas) if left blank.  
Children ( List < UIPackage > ) - child elements to spawn.  
  
Action ( Action ) - action for Button elements.  
PersistentUI ( List < string > ) - elements that will be preserved during CleanChildren.  
PersistentUpdates ( List < string > ) - update cycles that will be preserved during CleanChildren.  
PostBuild ( Action < TaskCompletionSource < Bool > > ) - sets up actions that are executed after the element is created. Functionally you just have to name a parameter alongside the action ( i.e. ``PostBuild = async ( locker ) => { }``) and by the end of the method call ``locker .SetResult ( true )`` to release the thread.  
  
**Transforms:**  
Size ( Vector2 ) - size of the element, 0-1 if Unclamped is not set to true.  
TextSize ( int ) - sets the textsize of the element.  
Position ( Vector2 ) - position relative to the parent object, -1 to 1 in both axes.  
Radius ( float ) - sets the % of the element that will be rounded. Set to 1 for a fully rounded rectangle and 0 for squared edges.  
Width ( float ) - sets the % of the parent's width for sliders and text inputs.  
Expands ( bool ) - defaults to true. Set false for parent objects within containers.  

  
**Colors:**  
Channel1 ( ThemeChannel ) - sets the main color slot for the element.  
Channel2 ( ThemeChannel ) - sets the secondary color slot for the element.  
TextChannel ( ThemeChannel ) - sets the text color for the element.  
Theme ( UITheme ) - can be included if you want to override system default.  
  
**Images:**  
ImgSize ( Vector2Int ) - the dimensions in pixels of your imported image.  
ImgScale ( float ) - the screenscale for your image.  
Path ( string ) - the path to the image. csproj must have files included as embedded resources for integration with the API methods.  
Assembly ( Assembly ) - the executing assembly the image is embedded within. ``Assembly .GetExecutingAssembly ( )`` with ``using System .Reflection`` directive included.  
*For an app icon, a UIPackage with all four parameters + DepotFolder + ObjectName is necessary.*  
  
**Misc**:  
ScrollRect ( ScrollRect ) - route a ScrollRect to child elements to ensure the scrolling behavior forwards appropriately on creation.  
Aspect ( AspectGroup ) - sets a desired AspectRatio. best used on root object.  


*UI-Related Methods*  
  
**MakeGrabber**  
``otAPI .MakeGrabber ( GameObject Handle, GameObject Target, *bool LimitRangeByHandle*, *Action ReleaseAction* )`` - makes an object move another (or itself). Optional action trigger on release ie. settings update for the position.  
  
**AddClickAction**  
``otAPI .AddClickAction ( GameObject Object, Action Action, *bool UseClick* );`` - adds a click behavior to a given GameObject.  
  
## Themes  
  
[Return to top](#otapi)  
  
Themes are relatively simple to create and should be fed in alongside your app details in the depot. Constructor method allows for creation with HTML/hexadecimal colorcode ( include the `#` sign ) or for Colors constructed with new Color (r, g, b), etc.  
Hexadecimal is easier.  
``new UITheme ( name, author, BorderColor, BodyColor, HeaderColor, TextColor, ButtonColor, ButtonHover, SystemColor, SystemHover );``  
``new ( "meteorite", "ob", "#0f241d", "#323240", "#443e4f", "#b0a9a0", "#3e465c", "#404e6b", "#B1555B", "#C36A6D" );``  
  
## Helper Methods  
  
[Return to top](#otapi)  
  
**RunCoroutine**  
`` otAPI .RunCoroutine ( )
**Notify**  
``otAPI .Notify ( string notification, *UINotificationTray tray* );`` - sends a notification to the tray.  
  
**Edit**  
``otAPI .Edit ( type Instance, string FieldName, value Value );`` - directly edits a private/hidden parameter on an instance.  
  
**Call**  
``otAPI .Call ( type Instance, string MethodName, *object [ ] Parameters* );`` - directly calls a private/hidden method from an instance with optional parameters.  
  
**AddUpdateCycle**  
``otAPI .AddUpdateCycle ( string DepotFolder, string Name, Action Action );`` - adds a method to recurring update calls.  
  
**AddOrGet**  
``otAPI .AddOrGet < type > ( GameObject obj );`` - Gets a component of a given type off a gameobject, or adds it and returns it if it doesn't yet exist.  
  
**SafishDeleter**  
``otAPI .SafishDeleter ( GameObject obj );`` - Deletes a gameobject maybe safely?  
  
**CleanChildren**  
``otAPI .CleanChildren ( GameObject Target, string DepotFolder, *List < string > UI_to_keep*, *List < string > Update_Cycles_To_Keep*, *Action Post_Clear_action* );`` - Deletes GameObject children, preserving elements as intended throughout.
  
**Click**  
``otAPI .Click ( );`` - click!  
  
**ValidateHex**  
``otAPI .ValidateHex ( string Input )`` - validates a string as hex input (no #'s allowed!)  

## Misc Classes
  
[Return to top](#otapi)  
  
Args represent expected parameters from users. They have types such as float, int, string, bool, or hex color; they can be set as optional, and they can have min/max values.  
It is easiest to construct them in the context of an alias to get intellisense guidance on parameters.  
``new Arg ( ArgType .Float, false, 0.1f, 0.8f );``  
  
CfgLinks route a ConfigEntry into your CfgAliases. Again, construct in the context of an alias for best practice.  
``new CfgLink ( ArgType .Float, MyConfigBool );``  



  
