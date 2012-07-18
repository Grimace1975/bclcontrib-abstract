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
using System.Threading;
using System.Abstract;
namespace Contoso.Practices.DurableBus.Utilities
{
	/// <summary>
	/// WorkerThread
	/// </summary>
	public class WorkerThread
	{
		private static readonly IServiceLog ServiceLog = ServiceLogManager.Get<WorkerThread>();
		private readonly Action _action;
		private volatile bool _stopRequested;
		private readonly Thread _thread;
		private readonly object _lock = new object();

        /// <summary>
        /// Occurs when [stopped].
        /// </summary>
		public event EventHandler Stopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerThread"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
		public WorkerThread(Action action)
		{
			_action = action;
			_thread = new Thread(new ThreadStart(Loop))
			{
				IsBackground = true,
			};
			_thread.SetApartmentState(ApartmentState.MTA);
			_thread.Name = string.Format("Worker.{0}", _thread.ManagedThreadId);
		}

        /// <summary>
        /// Loops this instance.
        /// </summary>
		protected void Loop()
		{
			while (!StopRequested)
			{
				try { _action(); continue; }
				catch (Exception ex) { ServiceLog.Error("Exception reached top level.", ex); continue; }
			}
			var stopped = Stopped;
			if (stopped != null)
				stopped(this, null);
		}

        /// <summary>
        /// Starts this instance.
        /// </summary>
		public void Start()
		{
			if (!_thread.IsAlive)
				_thread.Start();
		}

        /// <summary>
        /// Stops this instance.
        /// </summary>
		public void Stop()
		{
			lock (_lock)
				_stopRequested = true;
		}

        /// <summary>
        /// Gets a value indicating whether [stop requested].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [stop requested]; otherwise, <c>false</c>.
        /// </value>
		protected bool StopRequested
		{
			get
			{
				lock (_lock)
					return _stopRequested;
			}
		}
	}
}