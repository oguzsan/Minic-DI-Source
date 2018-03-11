using System;


namespace Minic.DI
{
    public class InjectionError
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;
        public readonly string Info;


        //  CONSTRUCTOR
        public InjectionError(InjectionErrorType error, string info)
        {
            Error = error;
            Info = info;
        }
    }
}
