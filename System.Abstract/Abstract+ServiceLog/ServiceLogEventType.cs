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
namespace System.Abstract
{
    /// <summary>
    /// Category of events for classifying logging actions.
    /// </summary>
    public enum ServiceLogEventType
    {
        /// <summary>
        /// Generic logging action indicating tracking information.
        /// </summary>
        Trace,
        /// <summary>
        /// Logging action involving application debugging information.
        /// </summary>
        Debug,
        /// <summary>
        /// Logging action involving non-specific information of general purpose.
        /// </summary>
        Information,
        /// <summary>
        /// Logging action involving information of concern.
        /// </summary>
        Warning,
        /// <summary>
        /// Logging action involving information regarding a system or application error of a non-critical nature.
        /// </summary>
        Error,
        /// <summary>
        /// Logging action involving information regarding a critical system or application error.
        /// </summary>
        Fatal
    }

}
