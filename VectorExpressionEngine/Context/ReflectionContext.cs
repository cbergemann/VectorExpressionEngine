using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VectorExpressionEngine
{
    public class ReflectionContext : IContext
    {
        public ReflectionContext(object targetObject)
        {
            _targetObject = targetObject;

            var pis = _targetObject.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            _reflectedProperties = pis.Where(mi => mi.CustomAttributes.Any(a => a.AttributeType == typeof(ExpressionAttribute))).Select(mi => new CachedProperty(mi)).ToList();

            var mis = _targetObject.GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            _reflectedMethods = mis.Where(mi => mi.CustomAttributes.Any(a => a.AttributeType == typeof(ExpressionAttribute))).Select(k => new CachedMethod(k)).ToList();
        }

        private readonly object _targetObject;

        private readonly List<CachedProperty> _reflectedProperties;

        private readonly List<CachedMethod> _reflectedMethods;

        public object ResolveVariable(string name)
        {
            var cachedProperty = _reflectedProperties.FirstOrDefault(p => p.Name == name);
            if (cachedProperty != null)
            {
                return cachedProperty.PropertyInfo.GetValue(_targetObject);
            }

            var cachedMethod = _reflectedMethods.FirstOrDefault(m => m.Name == name && m.ArgumentTypes.Length == 0);
            if (cachedMethod != null)
            {
                try
                {
                    return cachedMethod.MethodInfo.Invoke(_targetObject, Array.Empty<object>());
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException ?? ex;
                }
            }

            throw new SyntaxException($"Unknown variable or function: '{name}'");
        }

        public object CallFunction(string name, object[] arguments)
        {
            var argumentTypes = arguments.Select(a => a.GetType()).ToArray();

            var cachedMethod = _reflectedMethods.FirstOrDefault(m => m.IsMatch(name, argumentTypes));
            if (cachedMethod == null)
            {
                throw new SyntaxException($"No function '{name}' found for arguments of type '{string.Join(", ", arguments.Select(a => a.GetType().Name))}'");
            }

            try
            {
                return cachedMethod.MethodInfo.Invoke(_targetObject, arguments);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public bool IsConstantExpressionVariable(string name)
        {
            var cachedProperty = _reflectedProperties.FirstOrDefault(p => p.Name == name);
            if (cachedProperty == null)
            {
                throw new SyntaxException($"Unknown variable: '{name}'");
            }

            return cachedProperty.IsConstant;
        }

        public bool IsConstantExpressionCall(string name, Type[] argumentTypes)
        {
            var cachedMethod = _reflectedMethods.FirstOrDefault(m => m.IsMatch(name, argumentTypes));
            if (cachedMethod == null)
            {
                throw new SyntaxException($"No function '{name}' found for arguments of type '{string.Join(", ", argumentTypes.Select(t => t.Name))}'");
            }

            return cachedMethod.IsConstant;
        }

        public void AssignVariable(string name, object value)
        {
            throw new SyntaxException("cannot assign variable - context is read-only");
        }
    }
}
