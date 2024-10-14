using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tools
{
    public static class TypeTools
    {
        private static Dictionary<Type, List<MethodInfo>> _operatorsCache = new Dictionary<Type, List<MethodInfo>>();

        public static T Convert<T>(object value)
        {
            return (T)Convert(value, typeof(T));
        }

        public static object Convert(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            var valueType = value.GetType();
            if (valueType == targetType || valueType.IsInstanceOfType(targetType))
            {
                return value;
            }

            if (targetType == typeof(string))
            {
                return value.ToString();
            }

            if (targetType.IsGenericType)
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    return Activator.CreateInstance(targetType, Convert(value, underlyingType));
                }
            }

            try
            {
                return System.Convert.ChangeType(value, targetType);
            }
            catch
            {
                if (!_operatorsCache.TryGetValue(targetType, out List<MethodInfo> operators))
                {
                    operators = new List<MethodInfo>();
                    foreach (var method in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name.StartsWith("op_Explicit") || method.Name.StartsWith("op_Implicit"))
                        {
                            operators.Add(method);
                        }
                    }
                    _operatorsCache.Add(targetType, operators);
                }

                foreach (var oper in operators)
                {
                    if (oper.ReturnType == targetType)
                    {
                        return oper.Invoke(null, new object[] { value });
                    }
                }
            }

            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        public static bool IsDerivedFrom<T>(object value)
        {
            return IsDerivedFrom(value, typeof(T));
        }

        public static bool IsDerivedFrom(object value, Type type)
        {
            var valueType = value.GetType();
            while (valueType != null)
            {
                if (valueType == type)
                {
                    return true;
                }

                valueType = valueType.BaseType;
            }

            return valueType != null;
        }

        public static bool Implements(object value, Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException("Interface type expected", nameof(type));
            }
            
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueType = value.GetType();
            var interfaces = valueType.GetInterfaces();

            return interfaces.Contains(type);
        }
    }
}
