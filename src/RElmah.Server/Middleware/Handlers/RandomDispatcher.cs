using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RElmah.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public class RandomDispatcher : PostDispatcher
    {
        private readonly Exception[] _randoms =
        {
            new OutOfMemoryException("No more memory..."), 
            new ArgumentNullException("foo"), 
            new InvalidCastException("What more are you trying to achieve??"), 
            new StackOverflowException("Tail recursion rulez!"), 
            new AccessViolationException("Scary...!")
        };
        private readonly Random _randomizer = new Random();

        public RandomDispatcher(IErrorsInbox inbox) : base(inbox) { }

        protected override async Task<ErrorPayload> OnProcessRequest(IDictionary<string, object> environment)
        {
            var exception = _randoms[_randomizer.Next(_randoms.Length)];

            return await Task.FromResult(new ErrorPayload
            {
                ApplicationName = exception.GetType().Name,
                Error = new ErrorDetail
                {
                    Message = exception.Message
                }
            });
        }
    }
}