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
        IInjectionValueOption AddBinding<T>();
        bool HasBindingForType(Type type);
        InjectionError GetError(int index);
    }
}
