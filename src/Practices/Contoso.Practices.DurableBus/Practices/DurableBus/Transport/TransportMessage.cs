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
using System.IO;
using System.Collections.Generic;
using System.Abstract;
using System.Security.Principal;
namespace Contoso.Practices.DurableBus.Transport
{
	/// <summary>
	/// TransportMessage
	/// </summary>
	[Serializable]
	public class TransportMessage
	{
		private object[] _body;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportMessage"/> class.
        /// </summary>
		public TransportMessage()
		{
			Lifetime = TimeSpan.MaxValue;
		}

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
		public List<KeyValuePair<string, string>> Headers { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
		public string Id { get; set; }
        /// <summary>
        /// Gets or sets the message intent.
        /// </summary>
        /// <value>
        /// The message intent.
        /// </value>
		public MessageIntent MessageIntent { get; set; }
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
		public List<object> Messages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TransportMessage"/> is recoverable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recoverable; otherwise, <c>false</c>.
        /// </value>
		public bool Recoverable { get; set; }
        /// <summary>
        /// Gets or sets the return info.
        /// </summary>
        /// <value>
        /// The return info.
        /// </value>
		public string ReturnInfo { get; set; }
        /// <summary>
        /// Gets or sets the time sent.
        /// </summary>
        /// <value>
        /// The time sent.
        /// </value>
		public DateTime TimeSent { get; set; }
        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
		public TimeSpan Lifetime { get; set; }
        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
		public IIdentity Identity { get; set; }

		#region Body

        /// <summary>
        /// Copies the messages to body.
        /// </summary>
		public void CopyMessagesToBody()
		{
            _body = new object[Messages.Count];
			Messages.CopyTo(_body);
		}

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public object[] Body
		{
			get { return _body; }
			set
			{
				_body = value;
				Messages = new List<object>(_body);
			}
		}

        /// <summary>
        /// Gets or sets the body stream.
        /// </summary>
        /// <value>
        /// The body stream.
        /// </value>
		public Stream BodyStream { get; set; }

		#endregion
	}
}