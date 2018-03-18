using System;


namespace Minic.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int BindingCount{get;}
        int ProviderCount{get;}
        int ErrorCount{get;}


        //  METHODS
        IInstanceProviderOptions AddBinding<T>();
        bool HasBindingForType(Type type);
        InjectionError GetError(int index);
        void InjectInto(object container);
    }
}
