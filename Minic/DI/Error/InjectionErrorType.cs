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
