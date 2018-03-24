// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;
using System.Reflection;
using System.Collections.Generic;


namespace Minic.DI.Reflection
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
