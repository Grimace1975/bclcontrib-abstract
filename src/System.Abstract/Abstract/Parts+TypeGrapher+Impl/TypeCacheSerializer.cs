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
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
namespace System.Abstract.Parts
{
    /// <summary>
    /// TypeCacheSerializer
    /// </summary>
    public class TypeCacheSerializer : ITypeCacheSerializer
    {
        private static readonly Guid _versionId = typeof(TypeCacheSerializer).Module.ModuleVersionId;

        /// <summary>
        /// Deserializes the types.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public IEnumerable<Type> DeserializeTypes(TextReader i)
        {
            var document = new XmlDocument();
            document.Load(i);
            var documentElement = document.DocumentElement;
            var guid = new Guid(documentElement.Attributes["versionId"].Value);
            if (guid != _versionId)
                return null;
            var list = new List<Type>();
            foreach (XmlNode node in documentElement.ChildNodes)
            {
                var assembly = Assembly.Load(node.Attributes["name"].Value);
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    var guid2 = new Guid(node2.Attributes["versionId"].Value);
                    foreach (XmlNode node3 in node2.ChildNodes)
                    {
                        var innerText = node3.InnerText;
                        var type = assembly.GetType(innerText);
                        if (type == null || type.Module.ModuleVersionId != guid2)
                            return null;
                        list.Add(type);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Serializes the types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="o">The o.</param>
        public void SerializeTypes(IEnumerable<Type> types, TextWriter o)
        {
            var document = new XmlDocument();
            document.AppendChild(document.CreateComment("TypeCache_DoNotModify"));
            var newChild = document.CreateElement("typeCache");
            document.AppendChild(newChild);
            newChild.SetAttribute("lastModified", CurrentDate.ToString());
            newChild.SetAttribute("versionId", _versionId.ToString());
            foreach (var grouping in types
                .GroupBy(type => type.Module)
                .GroupBy(groupedByModule => groupedByModule.Key.Assembly))
            {
                var element2 = document.CreateElement("assembly");
                newChild.AppendChild(element2);
                element2.SetAttribute("name", grouping.Key.FullName);
                foreach (var grouping2 in grouping)
                {
                    var element3 = document.CreateElement("module");
                    element2.AppendChild(element3);
                    element3.SetAttribute("versionId", grouping2.Key.ModuleVersionId.ToString());
                    foreach (var type in grouping2)
                    {
                        var element4 = document.CreateElement("type");
                        element3.AppendChild(element4);
                        element4.AppendChild(document.CreateTextNode(type.FullName));
                    }
                }
            }
            document.Save(o);
        }

        private DateTime CurrentDate
        {
            get
            {
                var currentDateOverride = CurrentDateOverride;
                return (!currentDateOverride.HasValue ? DateTime.Now : currentDateOverride.GetValueOrDefault());
            }
        }

        internal DateTime? CurrentDateOverride { get; set; }
    }
}