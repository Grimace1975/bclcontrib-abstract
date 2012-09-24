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
using System.Linq;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.Logging;
namespace Contoso.Abstract.SPG2010
{
    /// <summary>
    /// LoggingConfiguration
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Configures the area.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="categories">The categories.</param>
        public static void ConfigureArea(string areaName, string[] categories)
        {
            var diagnosticsArea = new DiagnosticsArea(areaName);
            foreach (string category in categories)
                diagnosticsArea.DiagnosticsCategories.Add(new DiagnosticsCategory(category));
            AddArea(diagnosticsArea);
        }

        private static DiagnosticsAreaCollection GetAreas()
        {
            return new DiagnosticsAreaCollection(new ConfigManager());
        }

        private static bool HasArea(DiagnosticsAreaCollection collection, string areaName)
        {
            return collection.Any(x => string.Equals(x.Name.Trim(), areaName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private static void AddArea(DiagnosticsArea newArea)
        {
            var areas = GetAreas();
            if (!areas.Contains(newArea))
                areas.Add(newArea);
            else
            {
                var index = areas.IndexOf(newArea);
                foreach (DiagnosticsCategory item in newArea.DiagnosticsCategories)
                    if (!areas[index].DiagnosticsCategories.Contains(item))
                        areas[index].DiagnosticsCategories.Add(item);
            }
            areas.SaveConfiguration();
        }

        /// <summary>
        /// Removes the area.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        public static void RemoveArea(string areaName)
        {
            var areas = GetAreas();
            if (HasArea(areas, areaName))
            {
                while (areas[areaName].DiagnosticsCategories.Count != 0)
                    areas[areaName].DiagnosticsCategories.Clear();
                areas.RemoveAt(areas.IndexOf(areas[areaName]));
                areas.SaveConfiguration();
            }
        }
    }
}
