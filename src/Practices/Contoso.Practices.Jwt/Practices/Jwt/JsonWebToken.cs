#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Contoso.Abstract.Micro;
using System.IO;
namespace Contoso.Practices.Jwt
{
    // [SPEC] http://self-issued.info/docs/draft-ietf-oauth-json-web-token.html
    public class JsonWebToken<T>
    {
        private readonly static Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithm.RS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        private readonly static JsonSerializer<JwtHeader> _headerSerializer = new JsonSerializer<JwtHeader>();
        private readonly static JsonSerializer<T> _payloadSerializer = new JsonSerializer<T>();

        public static string Encode(T payload, string key, JwtHashAlgorithm algorithm) { return Encode(payload, Encoding.UTF8.GetBytes(key), algorithm); }
        public static string Encode(T payload, byte[] keyBytes, JwtHashAlgorithm algorithm)
        {
            var segments = new List<string>();
            using (var s = new MemoryStream())
            using (var w = new StreamWriter(s))
            {
                // add header
                var header = new JwtHeader { Algorithm = algorithm.ToString(), Type = "JWT" };
                //var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
                _headerSerializer.Serialize(w, header); var headerBytes = s.ToArray();
                segments.Add(Base64UrlEncode(headerBytes));
                // add payload
                //var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));
                //var payloadBytes = Encoding.UTF8.GetBytes(@"{"iss":"761326798069-r5mljlln1rd4lrbhg75efgigp36m78j5@developer.gserviceaccount.com","scope":"https://www.googleapis.com/auth/prediction","aud":"https://accounts.google.com/o/oauth2/token","exp":1328554385,"iat":1328550785}");
                s.SetLength(0); _payloadSerializer.Serialize(w, payload); var payloadBytes = s.ToArray();
                segments.Add(Base64UrlEncode(payloadBytes));
            }
            // add signature
            var stringToSign = string.Join(".", segments.ToArray());
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            var signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
            segments.Add(Base64UrlEncode(signature));
            return string.Join(".", segments.ToArray());
        }

        public static T Decode(string token, string key) { return Decode(token, key, true); }
        public static T Decode(string token, string key, bool verify)
        {
            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            var crypto = Base64UrlDecode(parts[2]);
            // parse header
            //var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            //var headerData = JObject.Parse(headerJson);
            JwtHeader headerData;
            using (var s = new MemoryStream(Base64UrlDecode(header)))
            using (var r = new StreamReader(s))
                headerData = _headerSerializer.Deserialize(r);
            // parse payload
            //var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            //var payloadData = JObject.Parse(payloadJson);
            T payloadData;
            using (var s = new MemoryStream(Base64UrlDecode(payload)))
            using (var r = new StreamReader(s))
                payloadData = _payloadSerializer.Deserialize(r);
            // parse signautre
            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(header + "." + payload);
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var algorithm = headerData.Algorithm;
                //
                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);
                if (decodedCrypto != decodedSignature)
                    throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
            }
            return payloadData;
        }

        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "RS256": return JwtHashAlgorithm.RS256;
                case "HS384": return JwtHashAlgorithm.HS384;
                case "HS512": return JwtHashAlgorithm.HS512;
                default: throw new InvalidOperationException("Algorithm not supported.");
            }
        }

        // from JWT spec
        private static string Base64UrlEncode(byte[] input)
        {
            // Remove any trailing '='s, 62nd char of encoding, 63rd char of encoding
            return Convert.ToBase64String(input).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }

        // from JWT spec
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input.Replace('-', '+').Replace('_', '/'); // 62nd char of encoding, 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            return Convert.FromBase64String(output);
        }
    }
}
