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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Contoso.Abstract.Micro.Internal;
namespace Contoso.Abstract.Micro
{
    /// <summary>
    /// JsonSerializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonSerializer<T> : JsonSerializer
    {
        private readonly Dictionary<String, JsonMemberSerializationInfo> _memberSerializers = new Dictionary<String, JsonMemberSerializationInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer&lt;T&gt;"/> class.
        /// </summary>
        public JsonSerializer()
            : this(true) { }

        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <returns></returns>
        public static JsonSerializer<T> CreateSerializer() { return CreateSerializer(JsonValueType.Unknown); }
        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <param name="serializeAs">The serialize as.</param>
        /// <returns></returns>
        public static JsonSerializer<T> CreateSerializer(JsonValueType serializeAs)
        {
            var serializer = CreateSerializer(typeof(T), serializeAs);
            return (serializer is JsonSerializer<T> ? (JsonSerializer<T>)serializer : new JsonGenericSerializerAdapter<T>(serializer));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="createSerializers">if set to <c>true</c> [create serializers].</param>
        protected internal JsonSerializer(bool createSerializers)
        {
            if (createSerializers)
            {
                var markedSerializeable = Attribute.IsDefined(typeof(T), typeof(JsonSerializableAttribute));
                if (markedSerializeable)
                {
                    foreach (PropertyInfo pInfo in typeof(T).GetProperties())
                        if (Attribute.IsDefined(pInfo, typeof(JsonPropertyAttribute)))
                        {
                            var serializationInfo = new JsonMemberSerializationInfo(pInfo);
                            _memberSerializers.Add(serializationInfo.Name, serializationInfo);
                        }
                    foreach (FieldInfo fInfo in typeof(T).GetFields())
                        if (Attribute.IsDefined(fInfo, typeof(JsonPropertyAttribute)))
                        {
                            var serializationInfo = new JsonMemberSerializationInfo(fInfo);
                            _memberSerializers.Add(serializationInfo.Name, serializationInfo);
                        }
                }
                else
                    foreach (PropertyInfo pInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var serializationInfo = new JsonMemberSerializationInfo(pInfo);
                        _memberSerializers.Add(serializationInfo.Name, serializationInfo);
                    }
            }
        }

        /// <summary>
        /// Deserializes the specified r.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public T Deserialize(TextReader r) { return Deserialize(WrapTextReader.Wrap(r, WrapTextReader.WrapOptions.Default), string.Empty); }
        /// <summary>
        /// Deserializes the specified r.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected internal virtual T Deserialize(TextReader r, string path)
        {
            var result = Activator.CreateInstance<T>();
            var parens = JsonParserUtil.ReadStartObject(r);
            var hasMembers = (JsonParserUtil.PeekNextChar(r, true) != '}');
            while (hasMembers)
            {
                ReadMemeber(r, path, result);
                hasMembers = (JsonParserUtil.PeekNextChar(r, true) == ',');
                if (hasMembers)
                    JsonParserUtil.ReadNextChar(r, true);
            }
            JsonParserUtil.ReadEndObject(r, parens);
            return result;
        }

        private void ReadMemeber(TextReader r, string path, T result)
        {
            var name = JsonParserUtil.ReadMemberName(r);
            var sInfo = _memberSerializers[name];
            var memberValue = sInfo.Serializer.BaseDeserialize(r, path + "/" + name);
            var targetType = sInfo.MemberType;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                targetType = targetType.GetGenericArguments()[0];
            if (memberValue == null)
                sInfo.SetValue(result, null);
            else
            {
                if (targetType.IsEnum)
                {
                    var integerType = Enum.GetUnderlyingType(targetType);
                    if (integerType == typeof(UInt64))
                        sInfo.SetValue(result, Convert.ToUInt64(memberValue));
                    else if (integerType == typeof(Int64))
                        sInfo.SetValue(result, Convert.ToInt64(memberValue));
                    else if (integerType == typeof(UInt32))
                        sInfo.SetValue(result, Convert.ToUInt32(memberValue));
                    else if (integerType == typeof(Int32))
                        sInfo.SetValue(result, Convert.ToInt32(memberValue));
                    else if (integerType == typeof(UInt16))
                        sInfo.SetValue(result, Convert.ToUInt16(memberValue));
                    else if (integerType == typeof(Int16))
                        sInfo.SetValue(result, Convert.ToInt16(memberValue));
                    else if (integerType == typeof(Byte))
                        sInfo.SetValue(result, Convert.ToByte(memberValue));
                    else if (integerType == typeof(SByte))
                        sInfo.SetValue(result, Convert.ToSByte(memberValue));
                }
                else
                {
                    try { sInfo.SetValue(result, Convert.ChangeType(memberValue, targetType)); }
                    catch
                    {
                        if (memberValue != null)
                        {
                            if (memberValue is string && targetType == typeof(Uri))
                                sInfo.SetValue(result, new Uri((string)memberValue, UriKind.RelativeOrAbsolute));
                            else
                            {
                                var ctorInfo = targetType.GetConstructor(new[] { memberValue.GetType() });
                                if (ctorInfo != null)
                                    sInfo.SetValue(result, ctorInfo.Invoke(new[] { memberValue }));
                                else
                                    throw;
                            }
                        }
                        else
                            throw;
                    }
                }
            }
        }

        /// <summary>
        /// Serializes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="obj">The obj.</param>
        public void Serialize(TextWriter w, T obj) { Serialize(w, obj, JsonOptions.None, null, 0); }
        /// <summary>
        /// Serializes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="options">The options.</param>
        public void Serialize(TextWriter w, T obj, JsonOptions options) { Serialize(w, obj, options, null, 0); }
        /// <summary>
        /// Serializes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="options">The options.</param>
        /// <param name="format">The format.</param>
        public void Serialize(TextWriter w, T obj, JsonOptions options, string format) { Serialize(w, obj, options, format, 0); }
        /// <summary>
        /// Serializes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="options">The options.</param>
        /// <param name="format">The format.</param>
        /// <param name="tabDepth">The tab depth.</param>
        protected internal virtual void Serialize(TextWriter w, T obj, JsonOptions options, string format, int tabDepth)
        {
            if ((options & JsonOptions.EnclosingParens) != 0)
                w.Write('(');
            w.Write('{');
            var first = true;
            foreach (JsonMemberSerializationInfo sInfo in _memberSerializers.Values)
            {
                var value = sInfo.GetValue(obj);
                if (value != null || (options & JsonOptions.IncludeNulls) != 0)
                {
                    if (!first)
                        w.Write(',');
                    else
                        first = false;
                    if ((options & JsonOptions.Formatted) != 0)
                    {
                        w.WriteLine();
                        w.Write(new string(' ', tabDepth * 2));
                    }
                    if ((options & JsonOptions.QuoteNames) != 0)
                        w.Write('"');
                    w.Write(sInfo.Name);
                    if ((options & JsonOptions.QuoteNames) != 0)
                        w.Write('"');
                    w.Write(':');
                    if ((options & JsonOptions.Formatted) != 0)
                        w.Write(' ');
                    if (value == null)
                        w.Write("null");
                    else
                        if (sInfo.JsonProperty == null)
                            sInfo.Serializer.BaseSerialize(w, value, options, null, tabDepth + 1);
                        else
                            sInfo.Serializer.BaseSerialize(w, value, options, sInfo.JsonProperty.Format, tabDepth + 1);
                }
            }
            w.Write('}');
            if ((options & JsonOptions.EnclosingParens) != 0)
                w.Write(')');
        }

        internal override object BaseDeserialize(TextReader r, string path) { return (JsonParserUtil.PeekIsNull(r, true, path) ? null : (object)Deserialize(r, path)); }
        internal override void BaseSerialize(TextWriter w, object obj, JsonOptions options, string format, int tabDepth) { Serialize(w, (T)obj, options, format, tabDepth); }
    }
}
