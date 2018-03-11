using System;


namespace Minic.DI
{
    public class NewInstanceProvider<T> : IInstanceProvider where T : new()
    {
        //	MEMBERS
        public readonly Type TargetType;
        private object _Value;


        //  CONSTRUCTOR
        public NewInstanceProvider()
        {}


        //  METHODS
        public void GetInstance(out object value, out bool isNew)
        {
            if(_Value==null)
            {
                _Value = new T();
                isNew = true;
            }
            else
            {
                isNew = false;
            }
            value = _Value;
        }
    }
}
