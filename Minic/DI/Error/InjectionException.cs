using System;


namespace Minic.DI.Error
{
    public class InjectionException : Exception
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;


        //  CONSTRUCTOR
        public InjectionException(InjectionErrorType error,string message) : base(message)
        {
            Error = error;
        }
    }
}
