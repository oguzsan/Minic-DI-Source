using System;


namespace Minic.DI
{
    public interface IInstanceProviderOptions
    {
        //	METHODS
        void ToValue(object value);
        void ToType<T>() where T : new();
    }
}
