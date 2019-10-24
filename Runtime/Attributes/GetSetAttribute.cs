using System.Reflection;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 使用属性替换字段显示和编辑
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class GetSetAttribute : PropertyAttribute
    {
#if UNITY_EDITOR

        public string propertyName;
        public string label;

        public Object target;
        public PropertyInfo propertyInfo;

#endif

        public GetSetAttribute(string property, string label = null)
        {
#if UNITY_EDITOR
            this.propertyName = property;
            this.label = label;
#endif
        }

    } // class GetSetAttribute

} // namespace UnityExtensions