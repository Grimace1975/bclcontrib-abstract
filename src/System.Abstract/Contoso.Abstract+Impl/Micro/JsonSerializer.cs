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
using System.Text;
using Contoso.Abstract.Micro.Internal;
namespace Contoso.Abstract.Micro
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class JsonSerializer
    {
        private static readonly Type[] _numberTypes = new[] { 
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(IntPtr), typeof(UIntPtr), typeof(float), typeof(double), typeof(decimal), 
            typeof(byte?), typeof(sbyte?), typeof(short?), typeof(ushort?), typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?), typeof(IntPtr?), typeof(UIntPtr?), typeof(float?), typeof(double?), typeof(decimal?)
        };
        private static readonly Type[] _stringTypes = new[] { 
            typeof(string), typeof(char), typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(Uri), typeof(StringBuilder), typeof(UriBuilder),
            typeof(char?), typeof(DateTime?), typeof(TimeSpan?), typeof(Guid?) 
        };
        private static Dictionary<Type, JsonSerializer> _serializers = new Dictionary<Type, JsonSerializer>();
        private static Dictionary<Type, JsonSerializer> _arraySerializers = new Dictionary<Type, JsonSerializer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        protected JsonSerializer()
            : this(JsonValueType.Object, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="serializerType">Type of the serializer.</param>
        /// <param name="defaultFormat">The default format.</param>
        protected JsonSerializer(JsonValueType serializerType, string defaultFormat)
        {
            SerializerType = serializerType;
            DefaultFormat = defaultFormat;
        }

        internal static JsonSerializer CreateSerializer(Type t) { return CreateSerializer(t, JsonValueType.Unknown); }
        internal static JsonSerializer CreateSerializer(Type t, JsonValueType serializeAs)
        {
            var serializableAttribute = (JsonSerializableAttribute)Attribute.GetCustomAttribute(t, typeof(JsonSerializableAttribute));
            if (serializeAs == JsonValueType.Unknown && serializableAttribute != null)
                serializeAs = serializableAttribute.SerializeAs;
            if (serializeAs == JsonValueType.Boolean || (serializeAs == JsonValueType.Unknown && t == typeof(Boolean)))
                return (serializableAttribute == null ? new JsonBooleanSerializer() : new JsonBooleanSerializer(serializableAttribute.Format));
            else if (serializeAs == JsonValueType.Number || (serializeAs == JsonValueType.Unknown && (Array.IndexOf(_numberTypes, t) >= 0 || t.IsEnum)))
                return (serializableAttribute == null ? new JsonNumberSerializer() : new JsonNumberSerializer(serializableAttribute.Format));
            else if (serializeAs == JsonValueType.String || (serializeAs == JsonValueType.Unknown && Array.IndexOf(_stringTypes, t) >= 0))
                return (serializableAttribute == null ? new JsonStringSerializer() : new JsonStringSerializer(serializableAttribute.Format));
            else if ((serializeAs == JsonValueType.Array || serializeAs == JsonValueType.Unknown) && t.GetInterface("IEnumerable`1") != null)
            {
                Type elementType;
                if (t.IsArray)
                    elementType = t.GetElementType();
                else
                {
                    var genericEnumerable = t.GetInterface("IEnumerable`1");
                    elementType = genericEnumerable.GetGenericArguments()[0];
                }
                if (!_arraySerializers.ContainsKey(t))
                {
                    var serializerType = typeof(JsonArraySerializer<,>);
                    serializerType = serializerType.MakeGenericType(t, elementType);
                    _arraySerializers.Add(t, (JsonSerializer)Activator.CreateInstance(serializerType));
                }
                return _arraySerializers[t];
            }
            else if (serializeAs == JsonValueType.Object || serializeAs == JsonValueType.Unknown)
            {
                if (!_serializers.ContainsKey(t))
                {
                    var serializerType = typeof(JsonSerializer<>);
                    serializerType = serializerType.MakeGenericType(t);
                    _serializers.Add(t, (JsonSerializer)Activator.CreateInstance(serializerType));
                }
                return _serializers[t];
            }
            throw new JsonSerializationException("Unable to create serializer");
        }

        /// <summary>
        /// Gets the type of the serializer.
        /// </summary>
        /// <value>
        /// The type of the serializer.
        /// </value>
        public virtual JsonValueType SerializerType { get; private set; }
        /// <summary>
        /// Gets the default format.
        /// </summary>
        public virtual string DefaultFormat { get; private set; }

        internal abstract object BaseDeserialize(TextReader r, string path);

        internal void BaseSerialize(TextWriter w, object obj) { BaseSerialize(w, obj, JsonOptions.None, null, 0); }
        internal void BaseSerialize(TextWriter w, object obj, JsonOptions options) { BaseSerialize(w, obj, JsonOptions.None, null, 0); }
        internal void BaseSerialize(TextWriter w, object obj, JsonOptions options, string format) { BaseSerialize(w, obj, options, format, 0); }
        internal abstract void BaseSerialize(TextWriter w, object obj, JsonOptions options, string format, int tabDepth);
    }
}
