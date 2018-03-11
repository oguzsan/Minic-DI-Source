using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Minic.DI
{
    public class Injector : IInjector, IInjectionInstanceProvider
    {
        //  CONSTANTS
        private const string ERROR_ALREADY_ADDED_BINDING_FOR_TYPE = "Injection Error:Already added binding for type {0}\n{1}";
        

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
            
            //  Check is there is an existing binding with given type
            if(_Bindings.TryGetValue(typeof(T),out binding))
            {
                //  Add error
                string typeAsString = typeof(T).ToString();
                string callerInfo = GetCallerInfo();
                string errorInfo = String.Format(ERROR_ALREADY_ADDED_BINDING_FOR_TYPE,typeAsString, callerInfo);
                _Errors.Add(new InjectionError(InjectionErrorType.AlreadyAddedBindingForType,errorInfo));
            }
            else
            {
                //  Add binding
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
        
        private string GetCallerInfo()
        {
            StackTrace st = new StackTrace(true);
            string info = String.Format("\tFilename:{0}\n\tMethod:{1}\n\tLine:{2}",
                st.GetFrame(2).GetFileName(),
                st.GetFrame(2).GetMethod(),
                st.GetFrame(2).GetFileLineNumber()
                );

            return info;
        }
    }
}
