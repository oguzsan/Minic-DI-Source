using System;
using System.Reflection;
using System.Collections.Generic;


namespace Minic.DI
{
    public class ReflectionCache
    {
		//	MEMBERS
		public readonly Type ReflectedType;
		public LinkedList<FieldInfo> Fields { get; private set; }
		public LinkedList<PropertyInfo> Properties { get; private set; }


        //	CONSTRUCTOR
        public ReflectionCache( Type reflectedType)
		{
            ReflectedType = reflectedType;
			Fields = new LinkedList<FieldInfo> ();
			Properties = new LinkedList<PropertyInfo> ();
        }

    }
}
