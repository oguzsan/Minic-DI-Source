using System;


namespace Minic.DI.Error
{
    public class InjectionError
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;
        public readonly string Message;


        //  CONSTRUCTOR
        public InjectionError(InjectionErrorType error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
