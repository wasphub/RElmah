using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace RElmah.Server.Middleware.Handlers
{
    public class DispatcherBase
    {
        public static Task End(OwinResponse response, string data)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            var bytes = Encoding.UTF8.GetBytes(data);
            var d = new ArraySegment<byte>(bytes, 0, bytes.Length);
            response.Write(d.Array, d.Offset, d.Count);

            var completionSource = new TaskCompletionSource<object>();
            completionSource.SetResult(null);
            return completionSource.Task;
        }
    }
}