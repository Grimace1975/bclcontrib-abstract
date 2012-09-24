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
namespace System
{
    /// <summary>
    /// EnvironmentEx
    /// </summary>
    public static partial class EnvironmentEx
    {
        private static DeploymentEnvironment _deploymentEnvironment = DeploymentEnvironment.Production;
        private static DevelopmentStage _developmentStage = DevelopmentStage.Release;

        #region MockBase

        /// <summary>
        /// MockBase
        /// </summary>
        public abstract class MockBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockBase"/> class.
            /// </summary>
            protected MockBase() { }

            /// <summary>
            /// Gets the deployment environment.
            /// </summary>
            public virtual DeploymentEnvironment DeploymentEnvironment
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets the development stage.
            /// </summary>
            public virtual DevelopmentStage DevelopmentStage
            {
                get { throw new NotImplementedException(); }
            }

#if !CLRSQL
            /// <summary>
            /// Nexts the ID.
            /// </summary>
            /// <returns></returns>
            public virtual int NextID() { throw new NotImplementedException(); }
#endif
        }

        #endregion

        static EnvironmentEx()
        {
            ApplicationID = "NONE";
        }

        [ThreadStatic]
        private static MockBase _mock;

        /// <summary>
        /// Gets or sets the mock.
        /// </summary>
        /// <value>
        /// The mock.
        /// </value>
        public static MockBase Mock
        {
            get { return _mock; }
            set { _mock = value; }
        }

        /// <summary>
        /// Gets the application ID.
        /// </summary>
        public static string ApplicationID { get; private set; }

        /// <summary>
        /// Gets or sets the deployment environment.
        /// </summary>
        /// <value>
        /// The deployment environment.
        /// </value>
        public static DeploymentEnvironment DeploymentEnvironment
        {
            get { return (_mock == null ? _deploymentEnvironment : _mock.DeploymentEnvironment); }
            set
            {
                if (_mock == null)
                    _deploymentEnvironment = value;
                else
                    throw new InvalidOperationException("Mocked");
            }
        }

        /// <summary>
        /// Gets or sets the development stage.
        /// </summary>
        /// <value>
        /// The development stage.
        /// </value>
        public static DevelopmentStage DevelopmentStage
        {
            get { return (_mock == null ? _developmentStage : _mock.DevelopmentStage); }
            set
            {
                if (_mock == null)
                    _developmentStage = value;
                else
                    throw new InvalidOperationException("Mocked");
            }
        }

#if !CLRSQL
        private static readonly object _nextIDLock = new object();
        private static int _nextID;

        /// <summary>
        /// Gets the next id in the sequence.
        /// </summary>
        /// <returns></returns>
        public static int NextID()
        {
            if (_mock == null)
            {
                int nextID;
                lock (_nextIDLock)
                {
                    nextID = ++_nextID;
                    if (nextID == (1 >> 30))
                        nextID = _nextID = 0;
                }
                return nextID;
            }
            return _mock.NextID();
        }
#endif
    }
}
