using System;
using System.Diagnostics;
using System.Reflection;

namespace VectorExpressionEngine
{
    [DebuggerDisplay("CachedProperty: {PropertyInfo}")]
    internal class CachedProperty
    {
        public CachedProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var attribute = propertyInfo.GetCustomAttribute<ExpressionAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException("property has no Expression attribute", nameof(propertyInfo));
            }

            Name = attribute.ExpressionName ?? propertyInfo.Name;
            IsConstant = attribute.IsConstant;
            PropertyInfo = propertyInfo;
        }

        public string Name { get; }

        public bool IsConstant { get; }

        public PropertyInfo PropertyInfo { get; }
    }
}
