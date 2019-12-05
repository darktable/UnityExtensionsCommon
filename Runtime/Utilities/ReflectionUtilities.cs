using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityExtensions
{
    /// <summary>
    /// Reflection 工具箱
    /// </summary>
    public struct ReflectionUtilities
    {
        static IEnumerable<Type> _allAssemblyTypes;


        /// <summary>
        /// 类型表（可通过 Where 查找特定类型）
        /// </summary>
        public static IEnumerable<Type> allAssemblyTypes
        {
            get
            {
                if (_allAssemblyTypes == null)
                {
                    _allAssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                        t =>
                        {
                            try
                            {
                                return t.GetTypes();
                            }
                            catch
                            {
                                return new Type[0];
                            }
                        });
                }
                return _allAssemblyTypes;
            }
        }


        public static Type GetTypeInSameAssembly(string typeName, Type otherType)
        {
            var assemblyQualifiedName = otherType.AssemblyQualifiedName;
            typeName += assemblyQualifiedName.Substring(assemblyQualifiedName.IndexOf(','));
            return Type.GetType(typeName);
        }


        static FieldInfo InternalGetFieldInfo(Type type, string fieldName, BindingFlags flags)
        {
            while (type != null)
            {
                var info = type.GetField(fieldName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取对象的成员字段信息。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static FieldInfo GetFieldInfo(object instance, string fieldName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetFieldInfo(instance.GetType(), fieldName, flags);
        }


        /// <summary>
        /// 获取类型的静态字段信息。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static FieldInfo GetFieldInfo<T>(string fieldName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetFieldInfo(typeof(T), fieldName, flags);
        }


        /// <summary>
        /// 获取类型的静态字段信息。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetFieldInfo(type, fieldName, flags);
        }


        static PropertyInfo InternalGetPropertyInfo(Type type, string propertyName, BindingFlags flags)
        {
            while (type != null)
            {
                var info = type.GetProperty(propertyName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取对象的成员属性信息。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetPropertyInfo(instance.GetType(), propertyName, flags);
        }


        /// <summary>
        /// 获取类型的静态属性信息。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static PropertyInfo GetPropertyInfo<T>(string propertyName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetPropertyInfo(typeof(T), propertyName, flags);
        }


        /// <summary>
        /// 获取类型的静态属性信息。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetPropertyInfo(type, propertyName, flags);
        }


        static MethodInfo InternalGetMethodInfo(Type type, string methodName, BindingFlags flags)
        {
            while (type != null)
            {
                var info = type.GetMethod(methodName, flags);
                if (info != null) return info;
                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// 获取对象的成员方法信息。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static MethodInfo GetMethodInfo(object instance, string methodName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetMethodInfo(instance.GetType(), methodName, flags);
        }


        /// <summary>
        /// 获取类型的静态方法信息。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static MethodInfo GetMethodInfo<T>(string methodName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetMethodInfo(typeof(T), methodName, flags);
        }


        /// <summary>
        /// 获取类型的静态方法信息。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            return InternalGetMethodInfo(type, methodName, flags);
        }


        /// <summary>
        /// 获取对象的成员字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static object GetFieldValue(object instance, string fieldName)
        {
            return GetFieldInfo(instance, fieldName).GetValue(instance);
        }


        /// <summary>
        /// 获取类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static object GetFieldValue<T>(string fieldName)
        {
            return GetFieldInfo<T>(fieldName).GetValue(null);
        }


        /// <summary>
        /// 获取类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static object GetFieldValue(Type type, string fieldName)
        {
            return GetFieldInfo(type, fieldName).GetValue(null);
        }


        /// <summary>
        /// 设置对象的成员字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static void SetFieldValue(object instance, string fieldName, object value)
        {
            GetFieldInfo(instance, fieldName).SetValue(instance, value);
        }


        /// <summary>
        /// 设置类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static void SetFieldValue<T>(string fieldName, object value)
        {
            GetFieldInfo<T>(fieldName).SetValue(null, value);
        }


        /// <summary>
        /// 设置类型的静态字段值。从最终类型开始向上查找，忽略字段的可见性。
        /// </summary>
        public static void SetFieldValue(Type type, string fieldName, object value)
        {
            GetFieldInfo(type, fieldName).SetValue(null, value);
        }


        /// <summary>
        /// 获取对象的成员属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static object GetPropertyValue(object instance, string propertyName)
        {
            return GetPropertyInfo(instance, propertyName).GetValue(instance, null); 
        }


        /// <summary>
        /// 获取类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static object GetPropertyValue<T>(string propertyName)
        {
            return GetPropertyInfo<T>(propertyName).GetValue(null, null);
        }


        /// <summary>
        /// 获取类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static object GetPropertyValue(Type type, string propertyName)
        {
            return GetPropertyInfo(type, propertyName).GetValue(null, null);
        }


        /// <summary>
        /// 设置对象的成员属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static void SetPropertyValue(object instance, string propertyName, object value)
        {
            GetPropertyInfo(instance, propertyName).SetValue(instance, value, null);
        }


        /// <summary>
        /// 设置类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static void SetPropertyValue<T>(string propertyName, object value)
        {
            GetPropertyInfo<T>(propertyName).SetValue(null, value, null);
        }


        /// <summary>
        /// 设置类型的静态属性值。从最终类型开始向上查找，忽略属性的可见性。
        /// </summary>
        public static void SetPropertyValue(Type type, string propertyName, object value)
        {
            GetPropertyInfo(type, propertyName).SetValue(null, value, null);
        }


        /// <summary>
        /// 调用对象的成员方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod(object instance, string methodName, params object[] param)
        {
            return GetMethodInfo(instance, methodName).Invoke(instance, param);
        }


        /// <summary>
        /// 调用类型的静态方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod<T>(string methodName, params object[] param)
        {
            return GetMethodInfo<T>(methodName).Invoke(null, param);
        }


        /// <summary>
        /// 调用类型的静态方法。从最终类型开始向上查找，忽略方法的可见性。
        /// </summary>
        public static object InvokeMethod(Type type, string methodName, params object[] param)
        {
            return GetMethodInfo(type, methodName).Invoke(null, param);
        }

    } // struct ReflectionUtilities

} // namespace UnityExtensions