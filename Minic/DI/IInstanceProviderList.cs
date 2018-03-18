using System;


namespace Minic.DI
{
    public interface IInstanceProviderList
    {
        //  METHODS
        IInstanceProvider AddValue(Type targetType, object value);
        IInstanceProvider AddType<T>(Type targetType) where T : new();
    }
}
