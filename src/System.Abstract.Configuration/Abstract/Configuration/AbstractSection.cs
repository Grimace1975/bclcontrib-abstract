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
using System.Configuration;
namespace System.Abstract.Configuration
{
    /// <summary>
    /// AbstractSection
    /// </summary>
    public partial class AbstractSection
    {
        /// <summary>
        /// Gets the event source.
        /// </summary>
        [ConfigurationProperty("eventSource", DefaultValue = null)]
        public EventSourceConfiguration EventSource
        {
            get { return (EventSourceConfiguration)base["eventSource"]; }
        }

        /// <summary>
        /// Gets the service bus.
        /// </summary>
        [ConfigurationProperty("serviceBus", DefaultValue = null)]
        public ServiceBusConfiguration ServiceBus
        {
            get { return (ServiceBusConfiguration)base["serviceBus"]; }
        }

        /// <summary>
        /// Gets the service cache.
        /// </summary>
        [ConfigurationProperty("serviceCache", DefaultValue = null)]
        public ServiceCacheConfiguration ServiceCache
        {
            get { return (ServiceCacheConfiguration)base["serviceCache"]; }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        [ConfigurationProperty("serviceLocator", DefaultValue = null)]
        public ServiceLocatorConfiguration ServiceLocator
        {
            get { return (ServiceLocatorConfiguration)base["serviceLocator"]; }
        }

        /// <summary>
        /// Gets the service log.
        /// </summary>
        [ConfigurationProperty("serviceLog", DefaultValue = null)]
        public ServiceLogConfiguration ServiceLog
        {
            get { return (ServiceLogConfiguration)base["serviceLog"]; }
        }
    }
}
