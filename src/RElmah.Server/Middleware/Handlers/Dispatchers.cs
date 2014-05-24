using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RElmah.Domain;

namespace RElmah.Server.Middleware.Handlers
{
    public static class Dispatchers
    {
        private static readonly Exception[] Randoms =
        {
            new OutOfMemoryException("No more memory..."), 
            new ArgumentNullException("foo"), 
            new InvalidCastException("What more are you trying to achieve??"), 
            new StackOverflowException("Tail recursion rulez!"), 
            new AccessViolationException("Scary...!")
        };
        private static readonly Random Randomizer = new Random();

        public async static Task PostError(
            IErrorsInbox inbox, 
            Func<IDictionary<string, object>, Task<ErrorPayload>> executor, 
            IDictionary<string, object> environment)
        {
            inbox.Post(await executor(environment));
        }

        public async static Task<ErrorPayload> Elmah(IDictionary<string, object> environment)
        {
            var @params = await new OwinContext(environment).Request.ReadFormAsync();

            var errorText = Decode(@params["error"]);
            var sourceId  = @params["sourceId"];
            var errorId   = @params["errorId"];
            var infoUrl   = @params["infoUrl"];

            return new ErrorPayload(sourceId, JsonConvert.DeserializeObject<ErrorDetail>(errorText));
        }

        public static async Task<ErrorPayload> Random(IDictionary<string, object> environment)
        {
            var exception = Randoms[Randomizer.Next(Randoms.Length)];

            return await Task.FromResult(
                new ErrorPayload(
                    Guid.NewGuid().ToString(), 
                    new ErrorDetail
                    {
                        Message = exception.Message
                    }));
        }

        public async static Task Configuration(
            IConfigurationProvider configuration,
            IDictionary<string, object> environment)
        {
            var request = new OwinRequest(environment);
            if (request.Method == "POST")
            {
                configuration.AddCluster(await BuildCluster(request));
                return;
            }

            var response = new OwinResponse(environment);
            await response.WriteAsync(JsonConvert.SerializeObject(
                request.Uri.Segments.Count() > 3
                ? (dynamic)configuration.GetCluster(request.Uri.Segments.Skip(3).First())
                : configuration.Clusters));
        }

        public static async Task<string> BuildCluster(OwinRequest request)
        {
            var @params = await request.ReadFormAsync();
            return await Task.FromResult(@params["name"]);
        }

        static string Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
