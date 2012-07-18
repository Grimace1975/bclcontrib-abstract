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
#if !CLR4
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// MergeableFilterInfo
    /// </summary>
    [Serializable]
    public class MergeableFilterInfo : FilterInfo
    {
        /// <summary>
        /// Adds the action filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        protected virtual void AddActionFilter(IActionFilter filter)
        {
            if (!ActionFilters.Contains(filter))
                ActionFilters.Add(filter);
        }

        /// <summary>
        /// Adds the authorization filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        protected virtual void AddAuthorizationFilter(IAuthorizationFilter filter)
        {
            if (!AuthorizationFilters.Contains(filter))
                AuthorizationFilters.Add(filter);
        }

        /// <summary>
        /// Adds the exception filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        protected virtual void AddExceptionFilter(IExceptionFilter filter)
        {
            if (!ExceptionFilters.Contains(filter))
                ExceptionFilters.Add(filter);
        }

        /// <summary>
        /// Adds the result filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        protected virtual void AddResultFilter(IResultFilter filter)
        {
            if (!ResultFilters.Contains(filter))
                ResultFilters.Add(filter);
        }

        /// <summary>
        /// Merges the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public virtual MergeableFilterInfo Merge(FilterInfo filter)
        {
            if (filter != null)
            {
                foreach (var f in filter.ActionFilters)
                    AddActionFilter(f);
                foreach (var f in filter.AuthorizationFilters)
                    AddAuthorizationFilter(f);
                foreach (var f in filter.ExceptionFilters)
                    AddExceptionFilter(f);
                foreach (var f in filter.ResultFilters)
                    AddResultFilter(f);
            }
            return this;
        }

        /// <summary>
        /// Merges the specified action filters.
        /// </summary>
        /// <param name="actionFilters">The action filters.</param>
        /// <param name="authorizationFilters">The authorization filters.</param>
        /// <param name="exceptionFilters">The exception filters.</param>
        /// <param name="resultFilters">The result filters.</param>
        /// <returns></returns>
        public virtual MergeableFilterInfo Merge(IEnumerable<IActionFilter> actionFilters, IEnumerable<IAuthorizationFilter> authorizationFilters, IEnumerable<IExceptionFilter> exceptionFilters, IEnumerable<IResultFilter> resultFilters)
        {
            foreach (var f in actionFilters)
                AddActionFilter(f);
            foreach (var f in authorizationFilters)
                AddAuthorizationFilter(f);
            foreach (var f in exceptionFilters)
                AddExceptionFilter(f);
            foreach (var f in resultFilters)
                AddResultFilter(f);
            return this;
        }
    }
}
#endif