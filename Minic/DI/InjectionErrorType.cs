using System;


namespace Minic.DI
{
    public enum InjectionErrorType
    {
        AlreadyAddedBindingForType,
        TypeNotAssignableToTarget,
        ValueNotAssignableToTarget,
        CanNotFindBindingForType,
    }
}
