using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Minic.DI.Error;
using Minic.DI.Provider;
using Minic.DI.Reflection;


namespace Minic.DI
{
    public class Injector : IInjectorTester, IInstanceProviderList, IMemberInjector
    {
        //  MEMBERS
        public int BindingCount{get{return _Bindings.Count;}}
        public int ProviderCount{get{return _Providers.Count;}}
        public int ErrorCount{get{return _Errors.Count;}}
        private Dictionary<Type, InjectionBinding> _Bindings;
        private Dictionary<Type, IInstanceProvider> _Providers;
        private Dictionary<Type, ReflectionCache> _Reflections;
        private bool _ShouldThrowException;
        private List<InjectionError> _Errors;
        private string[] _ErrorMessages;


        //	CONSTRUCTOR
        public Injector(bool shouldThrowException=false)
        {
            _Bindings = new Dictionary<Type, InjectionBinding>();
            _Providers = new Dictionary<Type, IInstanceProvider>();
            _Reflections = new Dictionary<Type, ReflectionCache>();
            _ShouldThrowException = shouldThrowException;
            _Errors = new List<InjectionError>();
            _ErrorMessages = new string[Enum.GetValues(typeof(InjectionErrorType)).Length];
            _ErrorMessages[(int)InjectionErrorType.AlreadyAddedBindingForType           ] = "Injection Error:Already added binding for type [{1}]\n{0}";
            _ErrorMessages[(int)InjectionErrorType.AlreadyAddedTypeWithDifferentProvider] = "Injection Error:Requested provider with type [{2}] already added with a different provider\n[{0}]]";
            _ErrorMessages[(int)InjectionErrorType.TypeNotAssignableToTarget            ] = "Injection Error:Given type [{2}] is not assignable to binding type [{1}]\n{0}";
            _ErrorMessages[(int)InjectionErrorType.ValueNotAssignableToBindingType      ] = "Injection Error:Given value of type [{2}] is not assignable to binding type [{1}]\n{0}";
            _ErrorMessages[(int)InjectionErrorType.CanNotFindBindingForType             ] = "Injection Error:Can not find binding for type [{1}]\n{0}";
        }


        //  METHODS
        #region IInjectorTester implementations

        public bool HasBindingForType(Type type)
        {
            return _Bindings.ContainsKey(type);
        }

        #endregion

        #region IInjector implementations

        public IInstanceProviderOptions AddBinding<T>()
        {
            Type bindingType = typeof(T);

            InjectionBinding binding;
            
            //  Check is there is an existing binding with given type
            if(_Bindings.TryGetValue(bindingType,out binding))
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.AlreadyAddedBindingForType, bindingType, null, 1);
                if(_ShouldThrowException)
                {
                    throw new InjectionException(error.Error,error.Message);
                }
            }
            else
            {
                //  Add binding
                binding = new InjectionBinding(bindingType, this);
                _Bindings.Add(bindingType, binding);
            }
            
            return binding;
        }

        public InjectionError GetError(int index)
        {
            return _Errors[index];
        }

        public void InjectInto(object container, IMemberInjector injectionOverride = null)
        {
            //  Get reflection container for object. Will be performed once per type
            ReflectionCache classReflection = GetReflection(container.GetType());

            //  Inject into fields
            foreach (FieldInfo fieldInfo in classReflection.Fields)
            {
                if(injectionOverride!=null && injectionOverride.InjectIntoField(fieldInfo, container))
                {
                    continue;
                }
                else if (InjectIntoField(fieldInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, fieldInfo.FieldType, null, 1);
                    if(_ShouldThrowException)
                    {
                        throw new InjectionException(error.Error,error.Message);
                    }

                    continue;
                }
            }

            //  Inject into properties
            foreach (PropertyInfo propertyInfo in classReflection.Properties)
            {
                
                if(injectionOverride!=null && injectionOverride.InjectIntoProperty(propertyInfo, container))
                {
                    continue;
                }
                else if (InjectIntoProperty(propertyInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, propertyInfo.PropertyType, null, 1);
                    if(_ShouldThrowException)
                    {
                        throw new InjectionException(error.Error,error.Message);
                    }

                    continue;
                }
            }
        }

        public T GetInstance<T>()
        {
            Type bindingType = typeof(T);

            object value = null;
            InjectionBinding binding = null;
            if (_Bindings.TryGetValue(bindingType, out binding) == true)
            {
                bool isNew;
                binding.InstanceProvider.GetInstance(out value, out isNew);
                if (isNew)
                {
                    InjectInto(value);
                }
            }
            else
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, 1);
                if(_ShouldThrowException)
                {
                    throw new InjectionException(error.Error,error.Message);
                }
            }

            return (T)value;
        }
        #endregion

        #region IInstanceProviderList implementations

        public IInstanceProvider AddValue(Type bindingType, object value)
        {
            Type providerType = value.GetType();

            //  Check if type of value is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.ValueNotAssignableToBindingType, bindingType, providerType, 2);
                if(_ShouldThrowException)
                {
                    throw new InjectionException(error.Error,error.Message);
                }

                return null;
            }

            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if(_Providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if(provider.GetType()!=typeof(SingleInstanceProvider))
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, 2);
                    if(_ShouldThrowException)
                    {
                        throw new InjectionException(error.Error,error.Message);
                    }
                }
            }
            else
            {
                provider = new SingleInstanceProvider(value);
                _Providers.Add(providerType,provider);
            }

            return provider;
        }

        public IInstanceProvider AddType<T>(Type bindingType) where T : new()
        {
            Type providerType = typeof(T);

            //  Check if type T is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.TypeNotAssignableToTarget, bindingType, providerType, 2);
                if(_ShouldThrowException)
                {
                    throw new InjectionException(error.Error,error.Message);
                }

                return null;
            }
            
            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if(_Providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if(provider.GetType()!=typeof(NewInstanceProvider<T>))
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, 2);
                    if(_ShouldThrowException)
                    {
                        throw new InjectionException(error.Error,error.Message);
                    }
                }
            }
            else
            {
                provider = new NewInstanceProvider<T>();
                _Providers.Add(providerType,provider);
            }

            return provider;
        }

        #endregion
        
        #region IMemberInjector implementations
        
        public bool InjectIntoField(FieldInfo fieldInfo, object container)
        {
            InjectionBinding binding = null;
            if (_Bindings.TryGetValue(fieldInfo.FieldType, out binding) == true)
            {
                object value;
                bool isNew;
                binding.InstanceProvider.GetInstance(out value, out isNew);
                if (isNew)
                {
                    InjectInto(value);
                }
                fieldInfo.SetValue(container, value);
                return true;
            }
            return false;
        }
		
        public bool InjectIntoProperty(PropertyInfo propertyInfo, object container)
        {
            InjectionBinding binding = null;
            if (_Bindings.TryGetValue(propertyInfo.PropertyType, out binding) == true)
            {
                object value;
                bool isNew;
                binding.InstanceProvider.GetInstance(out value, out isNew);
                if (isNew)
                {
                    InjectInto(value);
                }
                propertyInfo.SetValue(container, value, null);
                return true;
            }
            return false;
        }
        
        #endregion

        private ReflectionCache GetReflection(Type type)
        {
            ReflectionCache reflection = null;

            if (_Reflections.TryGetValue(type, out reflection) == false)
            {
                reflection = new ReflectionCache(type);

                MemberInfo[] fieldInfoList = type.FindMembers( MemberTypes.Field, 
                    BindingFlags.Instance | 
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.SetField | BindingFlags.SetProperty , null, null);

                foreach (MemberInfo fieldInfo in fieldInfoList)
                {
                    object[] attributeList = fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                    if (attributeList.Length > 0)
                    {
                        reflection.Fields.AddLast((FieldInfo)fieldInfo);
                    }
                }

                MemberInfo[] propertyInfoList = type.FindMembers( MemberTypes.Property,
                    BindingFlags.Instance | 
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.SetField | BindingFlags.SetProperty , null, null);

                foreach (MemberInfo propertyInfo in propertyInfoList)
                {
                    object[] attributeList = propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                    if (attributeList.Length > 0)
                    {
                        reflection.Properties.AddLast((PropertyInfo)propertyInfo);
                    }
                }

                _Reflections[type] = reflection;
            }

            return reflection;
        }

        private InjectionError CreateError(InjectionErrorType errorType, Type bindingType=null, Type providerType=null, int callerLevel=0)
        {
            string callerInfo = GetCallerInfo(1+callerLevel);
            string bindingTypeAsString = (bindingType!=null)?(bindingType.ToString()):("");
            string providerTypeAsString = (providerType!=null)?(providerType.ToString()):("");
            object[] args = new object[]{ callerInfo, bindingTypeAsString, providerTypeAsString};
            string errorMessage = String.Format(_ErrorMessages[(int)errorType], args);

            InjectionError error = new InjectionError(errorType,errorMessage);
            _Errors.Add(error);

            return error;
        }

        private string GetCallerInfo(int callerLevel=0)
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1+callerLevel);
            string info = String.Format("\tFilename:{0}\n\tMethod:{1}\n\tLine:{2}", sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber() );

            return info;
        }

    }
}
