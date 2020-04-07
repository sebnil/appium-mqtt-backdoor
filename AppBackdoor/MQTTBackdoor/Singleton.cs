namespace MQTTBackdoor
{
    /// <summary>
    /// Reusable singleton design pattern. Allows maximum of one instance of an object, and provides easy access to the class without instantiation of the class.
    /// BTownTKD at http://stackoverflow.com/questions/16745629/how-to-abstract-a-singleton-class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>, new()
    {
        private static T _instance = new T();
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
