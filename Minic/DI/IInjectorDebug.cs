// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Minic.DI
{
    public interface IInjectorTester : IInjector
    {
        //  MEMBERS
        int BindingCount{get;}
        int ProviderCount{get;}
    }
}
