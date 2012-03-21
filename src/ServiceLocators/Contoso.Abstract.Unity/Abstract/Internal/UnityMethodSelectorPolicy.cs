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
using System.Abstract;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
namespace Contoso.Abstract.Internal
{
    /// <summary>
    /// UnityMethodSelectorPolicy
    /// </summary>
    internal class UnityMethodSelectorPolicy : MethodSelectorPolicyBase<ServiceInjectionMethodAttribute>
    {
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            var dependency = ServiceDependencyAttribute.GetServiceDependencies(parameter)
                .SingleOrDefault();
            if (dependency != null)
                return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, dependency.Name);
            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }
    }
}
