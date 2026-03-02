namespace _otAPI {
    public class Arg
    {
        public ArgType type { get; private set; } //what type to expect
        public bool optional { get; private set; } = false; //whether the parameter is mandatory
        public object minIn { get; private set; } = null; //minimum input (must have a maximum if used)
        public object maxIn { get; private set; } = null; //maximum input (must have a minimum if used)
        public Arg(
            ArgType _type,
            bool _optional = false,
            object _minIn = null,
            object _maxIn = null
        ) {
            type = _type; optional = _optional;
            if ( _minIn != null ) minIn = _minIn;
            if ( _maxIn != null ) maxIn = _maxIn;
        }
    }
}