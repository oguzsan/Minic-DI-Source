using System;


namespace Minic.DI
{
    public interface IInjectorTester : IInjector
    {
        //  MEMBERS
        int BindingCount{get;}
        int ProviderCount{get;}


        //  METHODS
        bool HasBindingForType(Type type);
    }
}
