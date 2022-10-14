using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace VectorExpressionEngine
{
    [DebuggerDisplay("CachedMethod: {MethodInfo}")]
    internal class CachedMethod
    {
        public CachedMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var attribute = methodInfo.GetCustomAttribute<ExpressionAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException("method has no Expression attribute", nameof(methodInfo));
            }

            Name = attribute.ExpressionName ?? methodInfo.Name;
            IsConstant = attribute.IsConstant;
            ArgumentTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            MethodInfo = methodInfo;
        }

        public string Name { get; }

        public bool IsConstant { get; }

        public Type[] ArgumentTypes { get; }

        public MethodInfo MethodInfo { get; }

        public bool IsMatch(IReadOnlyList<Type> argumentTypes)
        {
            if (argumentTypes.Count != ArgumentTypes.Length)
            {
                return false;
            }

            for (int it = 0; it < ArgumentTypes.Length; it++)
            {
                if (!ArgumentTypes[it].IsAssignableFrom(argumentTypes[it]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}