using System;


namespace Minic.DI
{
    public interface IInjectionValueOption
    {
        //	METHODS
        void ToValue(object value);
        void ToType<T>() where T : new();
    }
}
