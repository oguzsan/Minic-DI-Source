using System;


namespace Minic.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int ErrorCount{get;}


        //  METHODS
        IInstanceProviderOptions AddBinding<T>();
        InjectionError GetError(int index);
        void InjectInto(object container);
    }
}
