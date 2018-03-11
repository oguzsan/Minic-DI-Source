using System;


namespace Minic.DI
{
    public class InjectionBinding : IInjectionValueOption
    {
        //	MEMBERS
        public readonly Type TargetType;
        private IInjectionInstanceProvider _InstanceProvider;


        //	CONSTRUCTOR
        public InjectionBinding(Type targetType, IInjectionInstanceProvider instanceProvider)
        {
            TargetType = targetType;
            _InstanceProvider = instanceProvider;
        }


        //  METHODS
        public void ToValue(object value)
        {
            _InstanceProvider.AddValue(TargetType, value);
        }

        public void ToType<T>() where T : new()
        {
            _InstanceProvider.AddType<T>(TargetType);
        }
    }
}