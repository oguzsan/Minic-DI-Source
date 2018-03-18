using System;
using System.Reflection;


namespace Minic.DI
{
	public interface IMemberInjector
    {
        //  METHODS
        bool InjectIntoField(FieldInfo fieldInfo, object container);
		bool InjectIntoProperty(PropertyInfo propertyInfo, object container);
	}
}