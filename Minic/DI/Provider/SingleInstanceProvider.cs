using System;


namespace Minic.DI.Provider
{
    public class SingleInstanceProvider : IInstanceProvider
    {
        //  MEMBERS
        private object _Instance;
        private bool _IsNew;


        //  CONSTRUCTOR
        public SingleInstanceProvider( object instance)
        {
            _Instance = instance;
            _IsNew = true;
        }


        //  METHODS
        public void GetInstance(out object instance, out bool isNew)
        {
            instance = _Instance;
            isNew = _IsNew;
            _IsNew = false;
        }
    }
}