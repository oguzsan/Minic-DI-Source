using System;


namespace Minic.DI
{
    public interface IInjectionInstanceProvider
    {
        //  METHODS
        void AddValue(Type targetType, object value);
        void AddType<T>(Type targetType) where T : new();
    }
}