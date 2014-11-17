using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Elmah;

namespace RElmah.Elmah
{
    public class ErrorPostModule : HttpModuleBase
    {
        private Uri _targetUrl;
        private Uri _infoUrl;
        private string _secret;

        protected override void OnInit(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            // Get the configuration section of this module.
            // If it's not there then there is nothing to initialize or do.
            // In this case, the module is as good as mute.

            var config = (IDictionary)GetConfig();
            if (config == null)
                return;

            // The module is expecting one parameter,
            // called 'targetUrl', which identifies the destination
            // of the HTTP POST that the module will perform.
            // You can also optionally supply an 'infoUrl' which 
            // can be used to call back for infos (the most logical
            // use is to specify where elmah.axd can be reached).

            var targetUrl = new Uri(GetSetting(config, "targetUrl"), UriKind.Absolute);
            var infoUrlSetting = GetOptionalSetting(config, "infoUrl", string.Empty);
            var infoUrl = !string.IsNullOrEmpty(infoUrlSetting)
                                ? new Uri(infoUrlSetting, UriKind.Absolute)
                                : null;
            var secret = GetOptionalSetting(config, "secret", string.Empty);

            var errorLogModule = application.Modules.GetSingleModule<ErrorLogModule>();
            if (errorLogModule == null)
                return;

            errorLogModule.Logged += (_, args) =>
            {
                if (args == null) throw new ArgumentNullException("args");
                SetError(HttpContext.Current, args.Entry);
            };

            _targetUrl = targetUrl;
            _infoUrl = infoUrl;
            _secret = secret;
        }

        private void SetError(HttpContext context, ErrorLogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            var error = entry.Error;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_targetUrl);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // See http://blogs.msdn.com/shitals/archive/2008/12/27/9254245.aspx
                request.ServicePoint.Expect100Continue = false;

                // The idea is to post to an url the json representation
                // of the intercepted error. We do a base 64 encoding
                // to fool the other side just in case some sort of
                // automated post validation is performed (do we have a 
                // better way to do this?). We post also the application
                // name and the handshaking token.

                var token = GetSourceId(context);
                var payload = ErrorJson.EncodeString(error);
                payload = string.IsNullOrEmpty(_secret)
                        ? payload
                        : Crypto.EncryptStringAes(payload, _secret);

                var form = new NameValueCollection
                {
                    { "error",          Base64Encode(payload) },
                    { "errorId",        entry.Id },
                    { "sourceId",       token },
                    { "infoUrl",        _infoUrl != null ? _infoUrl.AbsoluteUri : null },
                };

                // Get the bytes to determine
                // and set the content length.

                var data = Encoding.ASCII.GetBytes(W3.ToW3FormEncoded(form));
                Debug.Assert(data.Length > 0);
                request.ContentLength = data.Length;

                // Post it! (asynchronously)

                request.BeginGetRequestStream(ErrorReportingAsyncCallback(ar =>
                {
                    using (var output = request.EndGetRequestStream(ar))
                        output.Write(data, 0, data.Length);
                    request.BeginGetResponse(ErrorReportingAsyncCallback(rar => request.EndGetResponse(rar).Close() /* Not interested; assume OK */), null);
                }), null);
            }
            catch (Exception e)
            {
                // IMPORTANT! We swallow any exception raised during the 
                // logging and send them out to the trace . The idea 
                // here is that logging of exceptions by itself should not 
                // be  critical to the overall operation of the application.
                // The bad thing is that we catch ANY kind of exception, 
                // even system ones and potentially let them slip by.

                OnWebPostError(/* request, */ e);
            }
        }

        private static AsyncCallback ErrorReportingAsyncCallback(AsyncCallback callback)
        {
            return ar =>
            {
                if (ar == null) throw new ArgumentNullException("ar");

                try
                {
                    callback(ar);
                }
                catch (Exception e)
                {
                    OnWebPostError(e);
                }
            };
        }

        protected virtual string GetSourceId(HttpContext context)
        {
            var config = (IDictionary)GetConfig();
            return config == null
                 ? string.Empty
                 : GetSetting(config, "sourceId");
        }

        public static string Base64Encode(string str)
        {
            var encbuff = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        private static void OnWebPostError(/* WebRequest request, */ Exception e)
        {
            Debug.Assert(e != null);
            Trace.WriteLine(e);
        }

        #region Configuration

        internal const string GroupName = "elmah";
        internal const string GroupSlash = GroupName + "/";

        public static object GetSubsection(string name)
        {
            return GetSection(GroupSlash + name);
        }

        public static object GetSection(string name)
        {
            return ConfigurationManager.GetSection(name);
        }

        protected virtual object GetConfig()
        {
            return GetSubsection("errorPost");
        }

        private static string GetSetting(IDictionary config, string name)
        {
            Debug.Assert(config != null);
            Debug.Assert(!string.IsNullOrEmpty(name));

            var value = ((string)config[name]) ?? string.Empty;

            if (value.Length == 0)
            {
                throw new global::Elmah.ApplicationException(string.Format(
                    "The required configuration setting '{0}' is missing for the error posting module.", name));
            }

            return value;
        }

        private static string GetOptionalSetting(IDictionary config, string name, string defaultValue = null)
        {
            Debug.Assert(config != null);
            Debug.Assert(!string.IsNullOrEmpty(name));

            var value = ((string)config[name]) ?? string.Empty;

            if (value.Length == 0)
                return defaultValue;

            return value;
        }

        #endregion
    }

    public static class W3
    {
        public static string ToW3FormEncoded(NameValueCollection collection)
        {
            return W3FormEncode(collection, null);
        }

        private static string W3FormEncode(NameValueCollection collection, string prefix)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            if (collection.Count == 0)
                return String.Empty;

            const int size = 32766;
            var sb = new StringBuilder();

            var names = collection.AllKeys;
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var values = collection.GetValues(i);

                if (values == null)
                    continue;

                foreach (var value in values)
                {
                    var current = value;

                    if (sb.Length > 0)
                        sb.Append('&');

                    if (!String.IsNullOrEmpty(name))
                        sb.Append(name).Append('=');

                    //  This portion of the original code has been modified 
                    //  to avoid System.UriFormatException if input string size 
                    //  exceeds a limit defined by the underlying .NET fw version

                    var chunks = from index in Enumerable.Range(0, current.Length / size + 1)
                                 let chunk = (index + 1) * size <= current.Length
                                           ? current.Substring(index * size, size)
                                           : current.Substring(index * size)
                                 where chunk.Length > 0
                                 select Uri.EscapeDataString(chunk);
                    chunks.Aggregate(sb, (acc, x) => acc.Append(x));

                    //  End of modified portion
                }
            }

            if (sb.Length > 0 && !String.IsNullOrEmpty(prefix))
                sb.Insert(0, prefix);

            return sb.ToString();
        }
    }

    static class HttpModuleCollectionExtensions
    {
        public static T GetSingleModule<T>(this HttpModuleCollection modules)
        {
            return Enumerable.Range(0, modules.Count)
                             .Select(i => modules[i])
                             .OfType<T>()
                             .SingleOrDefault();
        }
    }

    public static class Crypto
    {
        private static readonly byte[] salt = Encoding.ASCII.GetBytes("LMiOjlydD96zbFat");

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptStringAes(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // generate the key from the shared secret and the salt
            var key = new Rfc2898DeriveBytes(sharedSecret, salt);

            // Create a RijndaelManaged object
            using (var aesAlg = new RijndaelManaged())
            {
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptStringAes(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // generate the key from the shared secret and the salt
            var key = new Rfc2898DeriveBytes(sharedSecret, salt);

            // Create the streams used for decryption.                
            byte[] bytes = Convert.FromBase64String(cipherText);
            using (var msDecrypt = new MemoryStream(bytes))
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                using (var aesAlg = new RijndaelManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new SystemException("Stream did not contain properly formatted byte array");

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException("Did not read byte array properly");

            return buffer;
        }
    }
}
