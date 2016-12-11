
namespace IndieYP.Delegates
{
    public class Shared
    {
        public delegate void VoidDelegate();

        public delegate void VectorDelegate( UnityEngine.Vector3 vector );

        public delegate void FloatDelegate( float val );

        public delegate void IntDelegate( int val );

        public delegate void BoolDelegate( bool flag );

        public delegate void StringDelegate( string data );
    }
}
