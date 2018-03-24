using System;
using Minic.DI.Error;
using Minic.DI.Provider;


namespace Minic.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int ErrorCount{get;}


        //  METHODS
        IInstanceProviderOptions AddBinding<T>();
        InjectionError GetError(int index);
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
    }
}
