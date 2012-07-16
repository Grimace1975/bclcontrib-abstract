//#region License
//
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//using System;
//using System.IO;
//using System.Abstract;
//using System.Abstract.Parts;
//using System.Collections.Generic;
//using System.Reflection;
//using Contoso.Abstract.Abstract;
//using System.Xml;
//using System.Runtime.Serialization;
//using System.Security;
//using System.Globalization;
//using System.Collections;
//using System.Xml.Serialization;

//namespace System.Abstract.Parts
//{
//    public class MessageSerializer
//    {
//        private const string BASETYPE = "baseType";
//        [ThreadStatic]
//        private static string defaultNameSpace;
//        private static readonly Dictionary<FieldInfo, LateBoundField> fieldInfoToLateBoundField = new Dictionary<FieldInfo, LateBoundField>();
//        private static readonly Dictionary<FieldInfo, LateBoundFieldSet> fieldInfoToLateBoundFieldSet = new Dictionary<FieldInfo, LateBoundFieldSet>();
//        private IMessageMapper mapper;
//        [ThreadStatic]
//        private static List<Type> messageBaseTypes;
//        private List<Type> messageTypes;
//        private string nameSpace = "http://tempuri.net";
//        [ThreadStatic]
//        private static List<Type> namespacesToAdd;
//        [ThreadStatic]
//        private static IDictionary<string, string> namespacesToPrefix;
//        [ThreadStatic]
//        private static IDictionary<string, string> prefixesToNamespaces;
//        private static readonly Dictionary<PropertyInfo, LateBoundProperty> propertyInfoToLateBoundProperty = new Dictionary<PropertyInfo, LateBoundProperty>();
//        private static readonly Dictionary<PropertyInfo, LateBoundPropertySet> propertyInfoToLateBoundPropertySet = new Dictionary<PropertyInfo, LateBoundPropertySet>();
//        private static readonly List<Type> typesBeingInitialized = new List<Type>();
//        private static readonly Dictionary<Type, Type> typesToCreateForArrays = new Dictionary<Type, Type>();
//        private static readonly Dictionary<Type, IEnumerable<FieldInfo>> typeToFields = new Dictionary<Type, IEnumerable<FieldInfo>>();
//        private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> typeToProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
//        private const string XMLPREFIX = "d1p1";
//        private const string XMLTYPE = "d1p1:type";

//        public T[] Deserialize<T>(Stream s)
//            where T : class
//        {
//            prefixesToNamespaces = new Dictionary<string, string>();
//            messageBaseTypes = new List<Type>();
//            var result = new List<T>();
//            var doc = new XmlDocument();
//            doc.Load(XmlReader.Create(s, new XmlReaderSettings { CheckCharacters = false }));
//            if (doc.DocumentElement != null)
//            {
//                foreach (XmlAttribute a in doc.DocumentElement.Attributes)
//                {
//                    if (a.Name == "xmlns")
//                        defaultNameSpace = a.Value.Substring(a.Value.LastIndexOf("/") + 1);
//                    else if (a.Name.Contains("xmlns:"))
//                    {
//                        int colonIndex = a.Name.LastIndexOf(":");
//                        string prefix = a.Name.Substring(colonIndex + 1);
//                        if (prefix.Contains("baseType"))
//                        {
//                            Type baseType = this.MessageMapper.GetMappedTypeFor(a.Value);
//                            if (baseType != null)
//                                messageBaseTypes.Add(baseType);
//                            continue;
//                        }
//                        prefixesToNamespaces[prefix] = a.Value;
//                    }
//                }
//                if (doc.DocumentElement.Name.ToLower() != "messages")
//                {
//                    object m = Process(doc.DocumentElement, null);
//                    if (m == null)
//                        throw new SerializationException("Could not deserialize message.");
//                    result.Add(m as T);
//                }
//                else
//                {
//                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
//                    {
//                        object m = this.Process(node, null);
//                        result.Add(m as T);
//                    }
//                }
//                defaultNameSpace = null;
//            }
//            return result.ToArray();
//        }

//        private static string FormatAsString(object value)
//        {
//            if (value == null)
//                return string.Empty;
//            if (value is bool)
//                return value.ToString().ToLower();
//            if (value is string)
//                return SecurityElement.Escape(value as string);
//            if (value is DateTime)
//            {
//                var valueAsDateTime = (DateTime)value;
//                return valueAsDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture);
//            }
//            if (value is TimeSpan)
//            {
//                var valueAsTimeSpan = (TimeSpan)value;
//                return string.Format("{0}P0Y0M{1}DT{2}H{3}M{4}.{5:000}S", new object[] { valueAsTimeSpan.TotalSeconds < 0.0 ? "-" : string.Empty, Math.Abs(valueAsTimeSpan.Days), Math.Abs(valueAsTimeSpan.Hours), Math.Abs(valueAsTimeSpan.Minutes), Math.Abs(valueAsTimeSpan.Seconds), Math.Abs(valueAsTimeSpan.Milliseconds) });
//            }
//            if (value is DateTimeOffset)
//            {
//                var valueAsDateTimeOffset = (DateTimeOffset)value;
//                return valueAsDateTimeOffset.ToString("o", CultureInfo.InvariantCulture);
//            }
//            if (value is Guid)
//            {
//                var valueAsGuid = (Guid)value;
//                return valueAsGuid.ToString();
//            }
//            return value.ToString();
//        }

//        private IEnumerable<FieldInfo> GetAllFieldsForType(Type t)
//        {
//            return t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
//        }

//        private IEnumerable<PropertyInfo> GetAllPropertiesForType(Type t, bool isKeyValuePair)
//        {
//            var results = new List<PropertyInfo>();
//            foreach (var p in t.GetProperties())
//            {
//                var genericArgs = p.PropertyType.GetGenericArguments();
//                if (typeof(IList) == p.PropertyType)
//                    throw new NotSupportedException("IList is not a supported property type for serialization, use List instead. Type: " + t.FullName + " Property: " + p.Name);
//                if ((genericArgs.Length == 1) && (typeof(IList<>).MakeGenericType(genericArgs) == p.PropertyType))
//                    throw new NotSupportedException("IList<T> is not a supported property type for serialization, use List<T> instead. Type: " + t.FullName + " Property: " + p.Name);
//                if ((genericArgs.Length == 2) && (typeof(IDictionary<,>).MakeGenericType(genericArgs) == p.PropertyType))
//                    throw new NotSupportedException("IDictionary<T, K> is not a supported property type for serialization, use Dictionary<T,K> instead. Type: " + t.FullName + " Property: " + p.Name);
//                if ((p.CanWrite || isKeyValuePair) && (p.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length <= 0))
//                    results.Add(p);
//            }
//            if (t.IsInterface)
//                foreach (var interfaceType in t.GetInterfaces())
//                    results.AddRange(GetAllPropertiesForType(interfaceType, false));
//            return results;
//        }

//        private static List<string> GetBaseTypes(IMessage[] messages, IMessageMapper mapper)
//        {
//            var result = new List<string>();
//            foreach (IMessage m in messages)
//            {
//                var t = mapper.GetMappedTypeFor(m.GetType());
//                for (var baseType = t.BaseType; (baseType != typeof(object)) && (baseType != null); baseType = baseType.BaseType)
//                    if (typeof(IMessage).IsAssignableFrom(baseType) && !result.Contains(baseType.FullName))
//                        result.Add(baseType.FullName);
//                foreach (var i in t.GetInterfaces())
//                    if (((i != typeof(IMessage)) && typeof(IMessage).IsAssignableFrom(i)) && !result.Contains(i.FullName))
//                        result.Add(i.FullName);
//            }
//            return result;
//        }

//        private FieldInfo GetField(Type t, string name)
//        {
//            IEnumerable<FieldInfo> fields;
//            typeToFields.TryGetValue(t, out fields);
//            if (fields != null)
//                foreach (FieldInfo f in fields)
//                    if (f.Name == name)
//                        return f;
//            return null;
//        }

//        private static string GetNameAfterColon(string name)
//        {
//            string n = name;
//            if (name.Contains(":"))
//                n = name.Substring(name.IndexOf(":") + 1, (name.Length - name.IndexOf(":")) - 1);
//            return n;
//        }

//        private static List<string> GetNamespaces(IMessage[] messages, IMessageMapper mapper)
//        {
//            List<string> result = new List<string>();
//            foreach (IMessage m in messages)
//            {
//                string ns = mapper.GetMappedTypeFor(m.GetType()).Namespace;
//                if (!result.Contains(ns))
//                    result.Add(ns);
//            }
//            return result;
//        }

//        private object GetObjectOfTypeFromNode(Type t, XmlNode node)
//    {
//        // This item is obfuscated and can not be translated.
//        if (t.IsSimpleType() || (t == typeof(Uri)))
//            return this.GetPropertyValue(t, node);
//        //if (t == typeof(WireEncryptedString))
//        //{
//        //    if (this.EncryptionService != null)
//        //    {
//        //        EncryptedValue encrypted = this.GetObjectOfTypeFromNode(typeof(EncryptedValue), node) as EncryptedValue;
//        //        string s = EncryptionService.Decrypt(encrypted);
//        //        WireEncryptedString <>g__initLocal1 = new WireEncryptedString();
//        //        <>g__initLocal1.Value = s;
//        //        return <>g__initLocal1;
//        //    }
//        //    foreach (XmlNode n in node.ChildNodes)
//        //    {
//        //        if (n.Name.ToLower() == "encryptedbase64value")
//        //        {
//        //            WireEncryptedString wes = new WireEncryptedString();
//        //            wes.Value = this.GetPropertyValue(typeof(string), n) as string;
//        //            return wes;
//        //        }
//        //    }
//        //}
//        if (typeof(IEnumerable).IsAssignableFrom(t))
//            return this.GetPropertyValue(t, node);
//        object result = this.MessageMapper.CreateInstance(t);
//        foreach (XmlNode n in node.ChildNodes)
//        {
//            Type type = null;
//            if (n.Name.Contains(":"))
//                type = Type.GetType("System." + n.Name.Substring(0, n.Name.IndexOf(":")), false, true);
//            var prop = GetProperty(t, n.Name);
//            if (prop != null)
//            {
//                if (type != null)
//                    goto Label_01CA;
//                var val = GetPropertyValue(prop.PropertyType, n);
//                if (val != null)
//                {
//                    propertyInfoToLateBoundPropertySet[prop](result, val);
//                    continue;
//                }
//            }
//            var field = GetField(t, n.Name);
//            if (field != null)
//            {
//                if (type != null)
//                    goto Label_0222;
//                var val = GetPropertyValue(field.FieldType, n);
//                if (val != null)
//                    fieldInfoToLateBoundFieldSet[field](result, val);
//            }
//        }
//        return result;
//    }

//        private static PropertyInfo GetProperty(Type t, string name)
//        {
//            IEnumerable<PropertyInfo> props;
//            typeToProperties.TryGetValue(t, out props);
//            if (props != null)
//            {
//                string n = GetNameAfterColon(name);
//                foreach (PropertyInfo prop in props)
//                {
//                    if (prop.Name == n)
//                    {
//                        return prop;
//                    }
//                }
//            }
//            return null;
//        }

//        private object GetPropertyValue(Type type, XmlNode n)
//        {
//            if ((n.ChildNodes.Count == 1) && (n.ChildNodes[0] is XmlText))
//            {
//                Type[] args = type.GetGenericArguments();
//                if (args.Length == 1)
//                {
//                    Type nullableType = typeof(Nullable<>).MakeGenericType(args);
//                    if (type == nullableType)
//                    {
//                        if (n.ChildNodes[0].InnerText.ToLower() == "null")
//                        {
//                            return null;
//                        }
//                        return this.GetPropertyValue(args[0], n);
//                    }
//                }
//                if (type == typeof(string))
//                {
//                    return n.ChildNodes[0].InnerText;
//                }
//                if (type.IsPrimitive || (type == typeof(decimal)))
//                {
//                    return Convert.ChangeType(n.ChildNodes[0].InnerText, type);
//                }
//                if (type == typeof(Guid))
//                {
//                    return new Guid(n.ChildNodes[0].InnerText);
//                }
//                if (type == typeof(DateTime))
//                {
//                    return XmlConvert.ToDateTime(n.ChildNodes[0].InnerText, XmlDateTimeSerializationMode.Utc);
//                }
//                if (type == typeof(TimeSpan))
//                {
//                    return XmlConvert.ToTimeSpan(n.ChildNodes[0].InnerText);
//                }
//                if (type == typeof(DateTimeOffset))
//                {
//                    return DateTimeOffset.Parse(n.ChildNodes[0].InnerText, null, DateTimeStyles.RoundtripKind);
//                }
//                if (type.IsEnum)
//                {
//                    return Enum.Parse(type, n.ChildNodes[0].InnerText);
//                }
//                if (type == typeof(byte[]))
//                {
//                    return Convert.FromBase64String(n.ChildNodes[0].InnerText);
//                }
//                if (type == typeof(Uri))
//                {
//                    return new Uri(n.ChildNodes[0].InnerText);
//                }
//            }
//            if (!typeof(IDictionary).IsAssignableFrom(type))
//            {
//                if (typeof(IEnumerable).IsAssignableFrom(type) && (type != typeof(string)))
//                {
//                    bool isArray = type.IsArray;
//                    Type typeToCreate = type;
//                    if (isArray)
//                    {
//                        typeToCreate = typesToCreateForArrays[type];
//                    }
//                    if (typeof(IList).IsAssignableFrom(typeToCreate))
//                    {
//                        IList list = Activator.CreateInstance(typeToCreate) as IList;
//                        if (list != null)
//                        {
//                            foreach (XmlNode xn in n.ChildNodes)
//                            {
//                                object m = this.Process(xn, list);
//                                list.Add(m);
//                            }
//                            if (isArray)
//                            {
//                                return typeToCreate.GetMethod("ToArray").Invoke(list, null);
//                            }
//                        }
//                        return list;
//                    }
//                }
//                if (n.ChildNodes.Count != 0)
//                {
//                    return this.GetObjectOfTypeFromNode(type, n);
//                }
//                if (type == typeof(string))
//                {
//                    return string.Empty;
//                }
//                return null;
//            }
//            IDictionary result = Activator.CreateInstance(type) as IDictionary;
//            Type keyType = typeof(object);
//            Type valueType = typeof(object);
//            foreach (Type interfaceType in type.GetInterfaces())
//            {
//                Type[] args = interfaceType.GetGenericArguments();
//                if ((args.Length == 2) && typeof(IDictionary<,>).MakeGenericType(args).IsAssignableFrom(type))
//                {
//                    keyType = args[0];
//                    valueType = args[1];
//                    break;
//                }
//            }
//            foreach (XmlNode xn in n.ChildNodes)
//            {
//                object key = null;
//                object value = null;
//                foreach (XmlNode node in xn.ChildNodes)
//                {
//                    if (node.Name == "Key")
//                    {
//                        key = this.GetObjectOfTypeFromNode(keyType, node);
//                    }
//                    if (node.Name == "Value")
//                    {
//                        value = this.GetObjectOfTypeFromNode(valueType, node);
//                    }
//                }
//                if ((result != null) && (key != null))
//                {
//                    result[key] = value;
//                }
//            }
//            return result;
//        }

//        public void InitType(Type t)
//        {
//            logger.Debug("Initializing type: " + t.AssemblyQualifiedName);
//            if (!t.IsSimpleType())
//            {
//                if (typeof(IEnumerable).IsAssignableFrom(t))
//                {
//                    if (t.IsArray)
//                    {
//                        typesToCreateForArrays[t] = typeof(List<>).MakeGenericType(new Type[] { t.GetElementType() });
//                    }
//                    foreach (Type g in t.GetGenericArguments())
//                    {
//                        this.InitType(g);
//                    }
//                    foreach (Type interfaceType in t.GetInterfaces())
//                    {
//                        Type[] arr = interfaceType.GetGenericArguments();
//                        if ((arr.Length == 1) && typeof(IEnumerable<>).MakeGenericType(new Type[] { arr[0] }).IsAssignableFrom(t))
//                        {
//                            this.InitType(arr[0]);
//                        }
//                    }
//                }
//                else
//                {
//                    bool isKeyValuePair = false;
//                    Type[] args = t.GetGenericArguments();
//                    if (args.Length == 2)
//                    {
//                        isKeyValuePair = typeof(KeyValuePair<,>).MakeGenericType(args) == t;
//                    }
//                    if (((args.Length == 1) && args[0].IsValueType) && (typeof(Nullable<>).MakeGenericType(args) == t))
//                    {
//                        this.InitType(args[0]);
//                    }
//                    else if (!typesBeingInitialized.Contains(t))
//                    {
//                        typesBeingInitialized.Add(t);
//                        IEnumerable<PropertyInfo> props = this.GetAllPropertiesForType(t, isKeyValuePair);
//                        typeToProperties[t] = props;
//                        IEnumerable<FieldInfo> fields = this.GetAllFieldsForType(t);
//                        typeToFields[t] = fields;
//                        foreach (PropertyInfo p in props)
//                        {
//                            logger.Debug("Handling property: " + p.Name);
//                            propertyInfoToLateBoundProperty[p] = DelegateFactory.Create(p);
//                            if (!isKeyValuePair)
//                            {
//                                propertyInfoToLateBoundPropertySet[p] = DelegateFactory.CreateSet(p);
//                            }
//                            this.InitType(p.PropertyType);
//                        }
//                        foreach (FieldInfo f in fields)
//                        {
//                            logger.Debug("Handling field: " + f.Name);
//                            fieldInfoToLateBoundField[f] = DelegateFactory.Create(f);
//                            if (!isKeyValuePair)
//                            {
//                                fieldInfoToLateBoundFieldSet[f] = DelegateFactory.CreateSet(f);
//                            }
//                            this.InitType(f.FieldType);
//                        }
//                    }
//                }
//            }
//        }

//        private object Process(XmlNode node, object parent)
//        {
//            string name = node.Name;
//            string typeName = defaultNameSpace + "." + name;
//            if (name.Contains(":"))
//            {
//                int colonIndex = node.Name.IndexOf(":");
//                name = name.Substring(colonIndex + 1);
//                string prefix = node.Name.Substring(0, colonIndex);
//                string ns = prefixesToNamespaces[prefix];
//                typeName = ns.Substring(this.nameSpace.LastIndexOf("/") + 1) + "." + name;
//            }
//            if (name.Contains("NServiceBus."))
//            {
//                typeName = name;
//            }
//            if (parent != null)
//            {
//                if (parent is IEnumerable)
//                {
//                    if (parent.GetType().IsArray)
//                    {
//                        return this.GetObjectOfTypeFromNode(parent.GetType().GetElementType(), node);
//                    }
//                    Type[] args = parent.GetType().GetGenericArguments();
//                    if (args.Length == 1)
//                    {
//                        return this.GetObjectOfTypeFromNode(args[0], node);
//                    }
//                }
//                PropertyInfo prop = parent.GetType().GetProperty(name);
//                if (prop != null)
//                {
//                    return this.GetObjectOfTypeFromNode(prop.PropertyType, node);
//                }
//            }
//            Type t = this.MessageMapper.GetMappedTypeFor(typeName);
//            if (t != null)
//            {
//                return this.GetObjectOfTypeFromNode(t, node);
//            }
//            logger.Debug("Could not load " + typeName + ". Trying base types...");
//            foreach (Type baseType in messageBaseTypes)
//            {
//                try
//                {
//                    logger.Debug("Trying to deserialize message to " + baseType.FullName);
//                    return this.GetObjectOfTypeFromNode(baseType, node);
//                }
//                catch
//                {
//                    continue;
//                }
//            }
//            throw new TypeLoadException("Could not handle type '" + typeName + "'.");
//        }

//        public void Serialize(IMessage[] messages, Stream stream)
//        {
//            namespacesToPrefix = new Dictionary<string, string>();
//            namespacesToAdd = new List<Type>();
//            List<string> namespaces = GetNamespaces(messages, this.MessageMapper);
//            for (int i = 0; i < namespaces.Count; i++)
//            {
//                string prefix = "q" + i;
//                if (i == 0)
//                {
//                    prefix = "";
//                }
//                if (namespaces[i] != null)
//                {
//                    namespacesToPrefix[namespaces[i]] = prefix;
//                }
//            }
//            StringBuilder messageBuilder = new StringBuilder();
//            foreach (IMessage m in messages)
//            {
//                Type t = this.MessageMapper.GetMappedTypeFor(m.GetType());
//                this.WriteObject(t.Name, t, m, messageBuilder);
//            }
//            StringBuilder builder = new StringBuilder();
//            List<string> baseTypes = GetBaseTypes(messages, this.MessageMapper);
//            builder.AppendLine("<?xml version=\"1.0\" ?>");
//            builder.Append("<Messages xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"");
//            for (int i = 0; i < namespaces.Count; i++)
//            {
//                string prefix = "q" + i;
//                if (i == 0)
//                {
//                    prefix = "";
//                }
//                builder.AppendFormat(" xmlns{0}=\"{1}/{2}\"", (prefix != "") ? (":" + prefix) : prefix, this.nameSpace, namespaces[i]);
//            }
//            foreach (Type type in namespacesToAdd)
//            {
//                builder.AppendFormat(" xmlns:{0}=\"{1}\"", type.Name.ToLower(), type.Name);
//            }
//            for (int i = 0; i < baseTypes.Count; i++)
//            {
//                string prefix = "baseType";
//                if (i != 0)
//                {
//                    prefix = prefix + i;
//                }
//                builder.AppendFormat(" xmlns:{0}=\"{1}\"", prefix, baseTypes[i]);
//            }
//            builder.Append(">\n");
//            builder.Append(messageBuilder.ToString());
//            builder.AppendLine("</Messages>");
//            byte[] buffer = Encoding.UTF8.GetBytes(builder.ToString());
//            stream.Write(buffer, 0, buffer.Length);
//        }

//        private void Write(StringBuilder builder, Type t, object obj)
//        {
//            if (obj != null)
//            {
//                if (!typeToProperties.ContainsKey(t))
//                {
//                    throw new InvalidOperationException("Type " + t.FullName + " was not registered in the serializer. Check that it appears in the list of configured assemblies/types to scan.");
//                }
//                foreach (PropertyInfo prop in typeToProperties[t])
//                {
//                    this.WriteEntry(prop.Name, prop.PropertyType, propertyInfoToLateBoundProperty[prop](obj), builder);
//                }
//                foreach (FieldInfo field in typeToFields[t])
//                {
//                    this.WriteEntry(field.Name, field.FieldType, fieldInfoToLateBoundField[field](obj), builder);
//                }
//            }
//        }

//        private void WriteEntry(string name, Type type, object value, StringBuilder builder)
//        {
//            if (value == null)
//            {
//                if (!typeof(IEnumerable).IsAssignableFrom(type))
//                {
//                    Type[] args = type.GetGenericArguments();
//                    if (args.Length == 1)
//                    {
//                        Type nullableType = typeof(Nullable<>).MakeGenericType(args);
//                        if (type == nullableType)
//                        {
//                            this.WriteEntry(name, typeof(string), "null", builder);
//                        }
//                    }
//                }
//            }
//            else if (type.IsValueType || (type == typeof(string)))
//            {
//                builder.AppendFormat("<{0}>{1}</{0}>\n", name, FormatAsString(value));
//            }
//            else if (!typeof(IEnumerable).IsAssignableFrom(type))
//            {
//                this.WriteObject(name, type, value, builder);
//            }
//            else
//            {
//                builder.AppendFormat("<{0}>\n", name);
//                if (type == typeof(byte[]))
//                {
//                    string str = Convert.ToBase64String((byte[])value);
//                    builder.Append(str);
//                }
//                else
//                {
//                    Type baseType = typeof(object);
//                    foreach (Type interfaceType in type.GetInterfaces())
//                    {
//                        Type[] arr = interfaceType.GetGenericArguments();
//                        if ((arr.Length == 1) && typeof(IEnumerable<>).MakeGenericType(new Type[] { arr[0] }).IsAssignableFrom(type))
//                        {
//                            baseType = arr[0];
//                            break;
//                        }
//                    }
//                    foreach (object obj in (IEnumerable)value)
//                    {
//                        if (obj.GetType().IsSimpleType())
//                        {
//                            this.WriteEntry(obj.GetType().Name, obj.GetType(), obj, builder);
//                        }
//                        else
//                        {
//                            this.WriteObject(baseType.SerializationFriendlyName(), baseType, obj, builder);
//                        }
//                    }
//                }
//                builder.AppendFormat("</{0}>\n", name);
//            }
//        }

//        private void WriteObject(string name, Type type, object value, StringBuilder builder)
//        {
//            if (value is WireEncryptedString)
//            {
//                if (this.EncryptionService == null)
//                {
//                    throw new InvalidOperationException(string.Format("Cannot encrypt field {0} because no encryption service was configured.", name));
//                }
//                EncryptedValue encryptedValue = this.EncryptionService.Encrypt((value as WireEncryptedString).Value);
//                this.WriteObject(name, typeof(EncryptedValue), encryptedValue, builder);
//            }
//            else if (value is Uri)
//            {
//                builder.AppendFormat("<{0}>{1}</{0}>\n", name, value);
//            }
//            else
//            {
//                string prefix;
//                string element = name;
//                namespacesToPrefix.TryGetValue(type.Namespace, out prefix);
//                if ((string.IsNullOrEmpty(prefix) && (type == typeof(object))) && value.GetType().IsSimpleType())
//                {
//                    if (!namespacesToAdd.Contains(value.GetType()))
//                    {
//                        namespacesToAdd.Add(value.GetType());
//                    }
//                    builder.AppendFormat("<{0}>{1}</{0}>\n", value.GetType().Name.ToLower() + ":" + name, FormatAsString(value));
//                }
//                else
//                {
//                    if (!string.IsNullOrEmpty(prefix))
//                    {
//                        element = prefix + ":" + name;
//                    }
//                    builder.AppendFormat("<{0}>\n", element);
//                    this.Write(builder, type, value);
//                    builder.AppendFormat("</{0}>\n", element);
//                }
//            }
//        }

//        public IEncryptionService EncryptionService { get; set; }

//        public IMessageMapper MessageMapper
//        {
//            get { return this.mapper; }
//            set
//            {
//                this.mapper = value;
//                if (this.messageTypes != null)
//                    this.mapper.Initialize(this.messageTypes);
//            }
//        }

//        public List<Type> MessageTypes
//        {
//            get { return this.messageTypes; }
//            set
//            {
//                this.messageTypes = value;
//                if (!this.messageTypes.Contains(typeof(EncryptedValue)))
//                    this.messageTypes.Add(typeof(EncryptedValue));
//                if (this.MessageMapper != null)
//                    this.MessageMapper.Initialize(this.messageTypes.ToArray());
//                foreach (Type t in this.messageTypes)
//                    this.InitType(t);
//            }
//        }

//        public string Namespace
//        {
//            get { return this.nameSpace; }
//            set { this.nameSpace = value; }
//        }
//    }
//}