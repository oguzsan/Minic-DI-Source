// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Minic.DI.Provider
{
    public interface IInstanceProviderOptions
    {
        //	METHODS
        void ToValue(object value);
        void ToType<T>() where T : new();
    }
}
