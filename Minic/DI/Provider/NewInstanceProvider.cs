using System;


namespace Minic.DI.Provider
{
    public class NewInstanceProvider<T> : IInstanceProvider where T : new()
    {
        //	MEMBERS
        private object _Instance;


        //  CONSTRUCTOR
        public NewInstanceProvider()
        {}


        //  METHODS
        public void GetInstance(out object value, out bool isNew)
        {
            if(_Instance==null)
            {
                _Instance = new T();
                isNew = true;
            }
            else
            {
                isNew = false;
            }
            value = _Instance;
        }
    }
}
