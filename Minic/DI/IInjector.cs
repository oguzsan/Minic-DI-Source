// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;
using System.Collections.Generic;
using Minic.DI.Error;
using Minic.DI.Provider;


namespace Minic.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int ErrorCount{get;}


        //  METHODS
        IInstanceProviderSetter AddBinding<T>();
        InjectionError GetError(int index);
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
        IEnumerator<T> GetAssignableInstances<T>();
    }
}
