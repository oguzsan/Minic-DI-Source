using System;


namespace Minic.DI.Provider
{
    public interface IInstanceProviderOptions
    {
        //	METHODS
        void ToValue(object value);
        void ToType<T>() where T : new();
    }
}
