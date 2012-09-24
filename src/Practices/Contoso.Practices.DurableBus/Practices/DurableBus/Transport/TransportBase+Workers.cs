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
using Contoso.Practices.DurableBus.Utilities;
using System.Collections.Generic;
namespace Contoso.Practices.DurableBus.Transport
{
    public abstract partial class TransportBase<T>
	{
		private readonly IList<WorkerThread> _workerThreads = new List<WorkerThread>();

        /// <summary>
        /// Gets or sets the process action.
        /// </summary>
        /// <value>
        /// The process action.
        /// </value>
        protected Action ProcessAction { get; set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
		public void Dispose()
		{
			lock (_workerThreads)
				for (var index = 0; index < _workerThreads.Count; index++)
					_workerThreads[index].Stop();
		}

		private WorkerThread AddWorkerThread()
		{
			lock (_workerThreads)
			{
                var result = new WorkerThread(ProcessAction);
				_workerThreads.Add(result);
				result.Stopped += (sender, e) =>
				{
					var workerThread = (sender as WorkerThread);
					lock (_workerThreads)
						_workerThreads.Remove(workerThread);
				};
				return result;
			}
		}

        /// <summary>
        /// Gets or sets the worker threads.
        /// </summary>
        /// <value>
        /// The worker threads.
        /// </value>
		public int WorkerThreads
		{
			get { return _workerThreads.Count; }
			set
			{
				lock (_workerThreads)
				{
					var current = _workerThreads.Count;
					if (value != current)
					{
						if (value < current)
							for (int index = value; index < current; index++)
								_workerThreads[index].Stop();
						else if (value > current)
							for (int index = current; index < value; index++)
								AddWorkerThread().Start();
					}
				}
			}
		}
	}
}