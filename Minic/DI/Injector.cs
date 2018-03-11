using System;
using System.Collections.Generic;


namespace Minic.DI
{
    public class Injector : IInjector, IInjectionInstanceProvider
    {
        //  MEMBERS
        public int BindingCount{get{return _Bindings.Count;}}
        public int ProviderCount{get{return _Providers.Count;}}
        public int ErrorCount{get{return _Errors.Count;}}
        private Dictionary<Type, InjectionBinding> _Bindings;
        private Dictionary<Type, IInstanceProvider> _Providers;
        private List<InjectionError> _Errors;


        //	CONSTRUCTOR
        public Injector()
        {
            _Bindings = new Dictionary<Type, InjectionBinding>();
            _Providers = new Dictionary<Type, IInstanceProvider>();
            _Errors = new List<InjectionError>();
        }


        //  METHODS
        #region IInjector implementations

        public IInjectionValueOption AddBinding<T>()
        {
            InjectionBinding binding;
            
            if(_Bindings.TryGetValue(typeof(T),out binding))
            {
                _Errors.Add(new InjectionError(InjectionErrorType.AlreadyAddedBindingForType,""));
            }
            else
            {
                binding = new InjectionBinding(typeof(T), this);
                _Bindings.Add(typeof(T), binding);
            }
            
            return binding;
        }

        public bool HasBindingForType(Type type)
        {
            return _Bindings.ContainsKey(type);
        }

        public InjectionError GetError(int index)
        {
            return _Errors[index];
        }

        #endregion

        #region IInjectionInstanceProvider implementations

        public void AddValue(Type targetType, object value)
        {
            _Providers.Add(targetType, new SingleInstanceProvider(value));
        }

        public void AddType<T>(Type targetType) where T : new()
        {
            _Providers.Add(targetType,new NewInstanceProvider<T>());
        }

        #endregion
    }
}
