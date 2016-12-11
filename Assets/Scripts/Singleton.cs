using UnityEngine;

namespace IndieYP.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object Lock = new object();

        private void Awake()
        {
            DontDestroyOnLoad( this );
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        public static T Instance
        {
            get
            {
                lock ( Lock )
                {
                    if ( _instance != null )
                        return _instance;

                    _instance = (T)FindObjectOfType( typeof ( T ) );

                    if ( _instance != null )
                        return _instance;

                    var singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = typeof ( T ).ToString();

                    DontDestroyOnLoad( singleton );

                    return _instance;
                }
            }
        }
    }
}