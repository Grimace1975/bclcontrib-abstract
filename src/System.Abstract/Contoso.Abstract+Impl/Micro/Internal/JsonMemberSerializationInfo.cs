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
using System.Text;
using System.Reflection;
namespace Contoso.Abstract.Micro.Internal
{
    internal class JsonMemberSerializationInfo
    {
        public JsonMemberSerializationInfo(FieldInfo field)
            : this(field, null, field.FieldType, (JsonPropertyAttribute)Attribute.GetCustomAttribute(field, typeof(JsonPropertyAttribute))) { }
        public JsonMemberSerializationInfo(PropertyInfo property)
            : this(null, property, property.PropertyType, (JsonPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))) { }
        private JsonMemberSerializationInfo(FieldInfo field, PropertyInfo property, Type memberType, JsonPropertyAttribute jsonProperty)
        {
            Field = field;
            Property = property;
            MemberType = memberType;
            JsonProperty = jsonProperty;
            Serializer = (jsonProperty != null ? JsonSerializer.CreateSerializer(memberType, jsonProperty.SerializeAs) : JsonSerializer.CreateSerializer(memberType));
            Name = (jsonProperty != null && !string.IsNullOrEmpty(jsonProperty.Name) ? jsonProperty.Name : (property != null ? property.Name : field.Name));
        }

        public bool IsProperty
        {
            get { return Property != null; }
        }

        public string Name { get; private set; }
        public Type MemberType { get; private set; }
        public PropertyInfo Property { get; private set; }
        public FieldInfo Field { get; private set; }
        public JsonSerializer Serializer { get; private set; }
        public JsonPropertyAttribute JsonProperty { get; private set; }

        public object GetValue(Object instance)
        {
            return (IsProperty ? Property.GetValue(instance, new object[] { }) : Field.GetValue(instance));
        }

        public void SetValue(object instance, object value)
        {
            if (IsProperty)
                Property.SetValue(instance, value, new object[] { });
            else
                Field.SetValue(instance, value);
        }
    }
}
