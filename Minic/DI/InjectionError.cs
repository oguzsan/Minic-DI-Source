using System;


namespace Minic.DI
{
    public class InjectionError
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;
        public readonly string Info;


        //  CONSTRUCTOR
        public InjectionError(InjectionErrorType errorType, string info)
        {
            Error = errorType;
            Info = info;
        }
    }
}
