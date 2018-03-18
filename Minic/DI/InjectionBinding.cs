using System;


namespace Minic.DI
{
    public class InjectionBinding : IInstanceProviderOptions
    {
        //	MEMBERS
        public readonly Type TargetType;
        public IInstanceProvider InstanceProvider{get; private set;}
        private IInstanceProviderList _InstanceProviderList;
        


        //	CONSTRUCTOR
        public InjectionBinding(Type targetType, IInstanceProviderList instanceProviderList)
        {
            TargetType = targetType;
            _InstanceProviderList = instanceProviderList;
        }


        //  METHODS
        public void ToValue(object value)
        {
            InstanceProvider = _InstanceProviderList.AddValue(TargetType, value);
        }

        public void ToType<T>() where T : new()
        {
            InstanceProvider = _InstanceProviderList.AddType<T>(TargetType);
        }
    }
}
