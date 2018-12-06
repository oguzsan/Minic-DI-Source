// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Minic.DI.Provider
{
    public interface IInstanceProviderList
    {
        //  METHODS
        IInstanceProvider AddValueProvider(Type targetType, object value);
        IInstanceProvider AddTypedProvider<T>(Type targetType) where T : new();
    }
}
