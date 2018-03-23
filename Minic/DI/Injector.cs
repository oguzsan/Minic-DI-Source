using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Minic.DI
{
    public class Injector : IInjector, IInstanceProviderList, IMemberInjector
    {
        //  CONSTANTS
        private const string ERROR_ALREADY_ADDED_BINDING_FOR_TYPE   = "Injection Error:Already added binding for type [{0}]\n{1}";
        private const string ERROR_TYPE_NOT_ASSIGNABLE_TO_TARGET    = "Injection Error:Given type [{0}] is not assignable to target type [{1}]\n{2}";
        private const string ERROR_VALUE_NOT_ASSIGNABLE_TO_TARGET   = "Injection Error:Given value type [{0}] is not assignable to target type [{1}]\n{2}";
        private const string ERROR_CAN_NOT_FIND_BINDING_FOR_TYPE    = "Injection Error:Can not find binding for type [{0}]\n{1}";
        

        //  MEMBERS
        public int BindingCount{get{return _Bindings.Count;}}
        public int ProviderCount{get{return _Providers.Count;}}
        public int ErrorCount{get{return _Errors.Count;}}
        private Dictionary<Type, InjectionBinding> _Bindings;
        private Dictionary<Type, IInstanceProvider> _Providers;
        private Dictionary<Type, ReflectionCache> _Reflections;
        private List<InjectionError> _Errors;


        //	CONSTRUCTOR
        public Injector()
        {
            _Bindings = new Dictionary<Type, InjectionBinding>();
            _Providers = new Dictionary<Type, IInstanceProvider>();
            _Reflections = new Dictionary<Type, ReflectionCache>();
            _Errors = new List<InjectionError>();
        }


        //  METHODS
        #region IInjector implementations

        public IInstanceProviderOptions AddBinding<T>()
        {
            InjectionBinding binding;
            
            //  Check is there is an existing binding with given type
            if(_Bindings.TryGetValue(typeof(T),out binding))
            {
                //  Add error
                string typeAsString = typeof(T).ToString();
                string callerInfo = GetCallerInfo(1);
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

        public void InjectInto(object container)
        {
            //  Get reflection data for object. Will be performed once per type
            ReflectionCache classReflection = GetReflection(container.GetType());

            //  Inject into fields
            foreach (FieldInfo fieldInfo in classReflection.Fields)
            {
                if (InjectIntoField(fieldInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Add error
                    string typeAsString = fieldInfo.FieldType.ToString();
                    string callerInfo = GetCallerInfo(1);
                    string errorInfo = String.Format(ERROR_CAN_NOT_FIND_BINDING_FOR_TYPE,typeAsString, callerInfo);
                    _Errors.Add(new InjectionError(InjectionErrorType.CanNotFindBindingForType,errorInfo));

                    continue;
                }
            }

            //  Inject into properties
            foreach (PropertyInfo propertyInfo in classReflection.Properties)
            {
                if (InjectIntoProperty(propertyInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Add error
                    string typeAsString = propertyInfo.PropertyType.ToString();
                    string callerInfo = GetCallerInfo(1);
                    string errorInfo = String.Format(ERROR_CAN_NOT_FIND_BINDING_FOR_TYPE,typeAsString, callerInfo);
                    _Errors.Add(new InjectionError(InjectionErrorType.CanNotFindBindingForType,errorInfo));

                    continue;
                }
            }
        }

        #endregion

        #region IInstanceProviderList implementations

        public IInstanceProvider AddValue(Type targetType, object value)
        {
            //  Check if type of value is assignable to target type
            if (!targetType.IsAssignableFrom(value.GetType()))
            {
                //  Add error
                string typeAsString = value.GetType().ToString();
                string targetTypeAsString = targetType.ToString();
                string callerInfo = GetCallerInfo(2);
                string errorInfo = String.Format(ERROR_VALUE_NOT_ASSIGNABLE_TO_TARGET, typeAsString, targetTypeAsString, callerInfo);
                _Errors.Add(new InjectionError(InjectionErrorType.ValueNotAssignableToTarget, errorInfo));

                return null;
            }

            IInstanceProvider provider = new SingleInstanceProvider(value);
            _Providers.Add(targetType, provider);
            return provider;
        }

        public IInstanceProvider AddType<T>(Type targetType) where T : new()
        {
            //  Check if type T is assignable to target type
            if (!targetType.IsAssignableFrom(typeof(T)))
            {
                //  Add error
                string typeAsString = typeof(T).ToString();
                string targetTypeAsString = targetType.ToString();
                string callerInfo = GetCallerInfo(2);
                string errorInfo = String.Format(ERROR_TYPE_NOT_ASSIGNABLE_TO_TARGET, typeAsString, targetTypeAsString, callerInfo);
                _Errors.Add(new InjectionError(InjectionErrorType.TypeNotAssignableToTarget, errorInfo));

                return null;
            }
            
            IInstanceProvider provider = new NewInstanceProvider<T>();
            _Providers.Add(targetType,provider);

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

        private string GetCallerInfo(int upLevel=1)
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1+uppLevel)
            string info = String.Format("\tFilename:{0}\n\tMethod:{1}\n\tLine:{2}",
                sf.GetFileName(),
                sf.GetMethod(),
                sf.GetFileLineNumber()
                );

            return info;
        }
    }
}
