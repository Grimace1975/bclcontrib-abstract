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
using System.Reflection;
using System.Collections;
using System.Security;
namespace System.Runtime.CompilerServices
{
#if COREINTERNAL
    internal
#else
    public
#endif
 static class CompilerServicesExtensions
    {
#if CLR4
        private static readonly Type _dependentHandleType = Type.GetType("System.Runtime.CompilerServices.DependentHandle");
        private static readonly PropertyInfo _isAllocatedProperty = _dependentHandleType.GetProperty("IsAllocated");
        private static readonly MethodInfo _getPrimaryMethod = _dependentHandleType.GetMethod("GetPrimary");
        private static readonly MethodInfo _getPrimaryAndSecondaryMethod = _dependentHandleType.GetMethod("GetPrimaryAndSecondary");

        private delegate void GetPrimaryAndSecondaryDelegate(out object primary, out object secondary);

        public static bool TryGetValueByLazyValue<TKey, TValue>(this ConditionalWeakTable<Lazy<TKey>, TValue> table, TKey key, out TValue value)
            where TKey : class
            where TValue : class { return LazyValueHelper<TKey, TValue>.TryGetValueWorker(table, key, out value); }

        private class LazyValueHelper<TLazyKey, TValue>
            where TLazyKey : class
            where TValue : class
        {
            private static readonly FieldInfo _entriesField = typeof(ConditionalWeakTable<Lazy<TLazyKey>, TValue>).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
            private static readonly FieldInfo _bucketsField = typeof(ConditionalWeakTable<Lazy<TLazyKey>, TValue>).GetField("_buckets", BindingFlags.NonPublic | BindingFlags.Instance);
            private static readonly Type _entryType = typeof(ConditionalWeakTable<Lazy<TLazyKey>, TValue>).GetNestedType("Entry", BindingFlags.NonPublic).MakeGenericType(new[] { typeof(Lazy<TLazyKey>), typeof(TValue) });
            private static readonly FieldInfo _entryDepHndField = _entryType.GetField("depHnd", BindingFlags.Public | BindingFlags.Instance);

            [SecurityCritical]
            public static bool TryGetValueWorker(ConditionalWeakTable<Lazy<TLazyKey>, TValue> table, TLazyKey key, out TValue value)
            {
                var entries = (_entriesField.GetValue(table) as IList);
                var buckets = (int[])_bucketsField.GetValue(table);
                int index = FindEntry(key, buckets, entries);
                if (index != -1)
                {
                    object primary = null;
                    object secondary = null;
                    var depHnd = _entryDepHndField.GetValue(entries[index]);
                    var getPrimaryAndSecondary = (GetPrimaryAndSecondaryDelegate)Delegate.CreateDelegate(typeof(GetPrimaryAndSecondaryDelegate), depHnd, _getPrimaryAndSecondaryMethod);
                    getPrimaryAndSecondary(out primary, out secondary);
                    // Now that we've secured a strong reference to the secondary, must check the primary again to ensure it didn't expire
                    // (otherwise, we open a ---- where TryGetValue misreports an expired key as a live key with a null value.) 
                    if (primary != null)
                    {
                        value = (TValue)secondary;
                        return true;
                    }
                }
                value = default(TValue);
                return false;
            }

            private static int FindEntry(TLazyKey key, int[] buckets, IList entries)
            {
                Lazy<TLazyKey> lazy;
                for (int entriesIndex = 0; entriesIndex < entries.Count; entriesIndex++)
                {
                    var depHnd = _entryDepHndField.GetValue(entries[entriesIndex]);
                    if (((bool)_isAllocatedProperty.GetValue(depHnd, null) == true)
                        && (lazy = (Lazy<TLazyKey>)_getPrimaryMethod.Invoke(depHnd, null)).IsValueCreated
                        && (lazy.Value == key))
                        return entriesIndex;
                }
                return -1;
            }

            //private static int FindEntry(TKey key, int[] buckets, IList entries)
            //{
            //    int hashCode = RuntimeHelpers.GetHashCode(key) & 0x7fffffff;
            //    for (int entriesIndex = buckets[hashCode % buckets.Length]; entriesIndex != -1; entriesIndex = entries[entriesIndex].next)
            //        if ((entries[entriesIndex].hashCode == hashCode) && (entries[entriesIndex].depHnd.GetPrimary() == key))
            //            return entriesIndex;
            //    return -1;
            //}
        }
#else
        public static bool TryGetValueByLazyValue<TKey, TValue>(this ConditionalWeakTable<Lazy<TKey>, TValue> table, TKey key, out TValue value)
            where TKey : class
            where TValue : class { return table.TryGetValueByLazyValue(key, out value); }
#endif
    }
}
