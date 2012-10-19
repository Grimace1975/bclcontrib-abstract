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
using System.Security.Cryptography.X509Certificates;
namespace Contoso.Practices.Jwt
{
    public class GoogleJsonWebToken
    {
        public static string Encode(string email, string certificateFilePath)
        {
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.Now;

            var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int)issueTime.AddMinutes(55).Subtract(utc0).TotalSeconds; // Expiration time is up to 1 hour, but lets play on safe side

            var payload = new JwtPayload
            {
                iss = email,
                scope = "https://www.googleapis.com/auth/gan.readonly",
                aud = "https://accounts.google.com/o/oauth2/token",
                exp = exp,
                iat = iat
            };

            var certificate = new X509Certificate2(certificateFilePath, "notasecret");
            var privateKey = certificate.Export(X509ContentType.Cert);
            return JsonWebToken<JwtPayload>.Encode(payload, privateKey, JwtHashAlgorithm.RS256);
        }
    }
}
