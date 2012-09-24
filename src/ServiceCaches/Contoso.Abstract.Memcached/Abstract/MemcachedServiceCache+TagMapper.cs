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
// https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
using System;
using Enyim.Caching.Memcached;
using System.Collections.Generic;
namespace Contoso.Abstract
{
    public partial class MemcachedServiceCache
    {
        /// <summary>
        /// ITagMapper
        /// </summary>
        public interface ITagMapper
        {
            /// <summary>
            /// Toes the add opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="name">The name.</param>
            /// <param name="cas">The cas.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <returns></returns>
            TagMapper.AddOpcode ToAddOpcode(object tag, ref string name, out ulong cas, out object opvalue);
            /// <summary>
            /// Toes the set opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="name">The name.</param>
            /// <param name="cas">The cas.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <param name="storeMode">The store mode.</param>
            /// <returns></returns>
            TagMapper.SetOpcode ToSetOpcode(object tag, ref string name, out ulong cas, out object opvalue, out StoreMode storeMode);
            /// <summary>
            /// Toes the get opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <returns></returns>
            TagMapper.GetOpcode ToGetOpcode(object tag, out object opvalue);
        }

        /// <summary>
        /// TagMapper
        /// </summary>
        public class TagMapper : ITagMapper
        {
            /// <summary>
            /// AddOpcode
            /// </summary>
            public enum AddOpcode
            {
                /// <summary>
                /// Append
                /// </summary>
                Append,
                /// <summary>
                /// AppendCas
                /// </summary>
                AppendCas,
                /// <summary>
                /// Store
                /// </summary>
                Store,
                /// <summary>
                /// Cas
                /// </summary>
                Cas,
            }

            /// <summary>
            /// SetOpcode
            /// </summary>
            public enum SetOpcode
            {
                /// <summary>
                /// Prepend
                /// </summary>
                Prepend,
                /// <summary>
                /// PrependCas
                /// </summary>
                PrependCas,
                /// <summary>
                /// Store
                /// </summary>
                Store,
                /// <summary>
                /// Cas
                /// </summary>
                Cas,
                /// <summary>
                /// Decrement
                /// </summary>
                Decrement,
                /// <summary>
                /// DecrementCas
                /// </summary>
                DecrementCas,
                /// <summary>
                /// Increment
                /// </summary>
                Increment,
                /// <summary>
                /// IncrementCas
                /// </summary>
                IncrementCas
            }

            /// <summary>
            /// GetOpcode
            /// </summary>
            public enum GetOpcode
            {
                /// <summary>
                /// Get
                /// </summary>
                Get,
                //PerformMultiGet,
            }

            /// <summary>
            /// Toes the add opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="name">The name.</param>
            /// <param name="cas">The cas.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <returns></returns>
            public AddOpcode ToAddOpcode(object tag, ref string name, out ulong cas, out object opvalue)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                // determine flag, striping name if needed
                bool flag = name.StartsWith("#");
                if (flag)
                    name = name.Substring(1);
                // store
                if (tag == null)
                {
                    cas = 0;
                    opvalue = null;
                    return AddOpcode.Store;
                }
                //
                if (tag is CasResult<object>)
                {
                    var plainCas = (CasResult<object>)tag;
                    cas = plainCas.Cas;
                    opvalue = null;
                    return AddOpcode.Cas;
                }
                // append
                if (tag is ArraySegment<byte>)
                {
                    cas = 0;
                    opvalue = tag;
                    return AddOpcode.Append;
                }
                else if (tag is CasResult<ArraySegment<byte>>)
                {
                    var appendCas = (CasResult<ArraySegment<byte>>)tag;
                    cas = appendCas.Cas;
                    opvalue = appendCas.Result;
                    return AddOpcode.AppendCas;
                }
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Toes the set opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="name">The name.</param>
            /// <param name="cas">The cas.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <param name="storeMode">The store mode.</param>
            /// <returns></returns>
            public SetOpcode ToSetOpcode(object tag, ref string name, out ulong cas, out object opvalue, out StoreMode storeMode)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                // determine flag, striping name if needed
                bool flag = name.StartsWith("#");
                if (flag)
                {
                    storeMode = StoreMode.Replace;
                    name = name.Substring(1);
                }
                else
                    storeMode = StoreMode.Set;
                // store
                if (tag == null)
                {
                    cas = 0;
                    opvalue = null;
                    return SetOpcode.Store;
                }
                //
                if (tag is CasResult<object>)
                {
                    var plainCas = (CasResult<object>)tag;
                    cas = plainCas.Cas;
                    opvalue = null;
                    return SetOpcode.Cas;
                }
                // prepend
                if (tag is ArraySegment<byte>)
                {
                    cas = 0;
                    opvalue = tag;
                    return SetOpcode.Prepend;
                }
                else if (tag is CasResult<ArraySegment<byte>>)
                {
                    var appendCas = (CasResult<ArraySegment<byte>>)tag;
                    cas = appendCas.Cas;
                    opvalue = appendCas.Result;
                    return SetOpcode.PrependCas;
                }
                // decrement
                else if (tag is DecrementTag)
                {
                    cas = 0;
                    opvalue = (DecrementTag)tag;
                    return SetOpcode.Decrement;
                }
                else if (tag is CasResult<DecrementTag>)
                {
                    var decrementCas = (CasResult<DecrementTag>)tag;
                    cas = decrementCas.Cas;
                    opvalue = decrementCas.Result;
                    return SetOpcode.DecrementCas;
                }
                // increment
                else if (tag is IncrementTag)
                {
                    cas = 0;
                    opvalue = (IncrementTag)tag;
                    return SetOpcode.Increment;
                }
                else if (tag is CasResult<IncrementTag>)
                {
                    var incrementCas = (CasResult<IncrementTag>)tag;
                    cas = incrementCas.Cas;
                    opvalue = incrementCas.Result;
                    return SetOpcode.IncrementCas;
                }
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Toes the get opcode.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="opvalue">The opvalue.</param>
            /// <returns></returns>
            public GetOpcode ToGetOpcode(object tag, out object opvalue)
            {
                opvalue = tag;
                return GetOpcode.Get; // (tag is Func<IMultiGetOperation, KeyValuePair<string, CacheItem>, object> ? GetOpcode.PerformMultiGet : GetOpcode.Get);
            }
        }
    }
}
