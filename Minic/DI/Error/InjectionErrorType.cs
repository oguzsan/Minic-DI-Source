// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Minic.DI.Error
{
    public enum InjectionErrorType
    {
        AlreadyAddedBindingForType,
        AlreadyAddedTypeWithDifferentProvider,
        TypeNotAssignableToTarget,
        ValueNotAssignableToBindingType,
        CanNotFindBindingForType,
    }
}
