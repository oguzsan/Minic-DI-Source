using System;
using Minic.DI.Provider;


namespace Minic.DI
{
    public class InjectionBinding : IInstanceProviderOptions
    {
        //	MEMBERS
        public readonly Type BindingType;
        public IInstanceProvider InstanceProvider{get; private set;}
        private IInstanceProviderList _InstanceProviderList;
        


        //	CONSTRUCTOR
        public InjectionBinding(Type targetType, IInstanceProviderList instanceProviderList)
        {
            BindingType = targetType;
            _InstanceProviderList = instanceProviderList;
        }


        //  METHODS
        public void ToValue(object value)
        {
            InstanceProvider = _InstanceProviderList.AddValue(BindingType, value);
        }

        public void ToType<T>() where T : new()
        {
            InstanceProvider = _InstanceProviderList.AddType<T>(BindingType);
        }
    }
}
