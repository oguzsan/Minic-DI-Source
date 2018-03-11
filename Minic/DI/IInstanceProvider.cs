using System;


namespace Minic.DI
{
    public interface IInstanceProvider
    {
        //  METHODS
        void GetInstance( out object instance, out bool isNew );
    }
}
