using System;


namespace Minic.DI.Provider
{
    public interface IInstanceProvider
    {
        //  METHODS
        void GetInstance( out object instance, out bool isNew );
    }
}
