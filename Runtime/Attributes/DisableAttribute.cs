using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个字段上, 根据指定的 Property 或 Field 决定是否禁用编辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DisableAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public string name;
        public bool value;
        public object fieldOrProp;
#endif


        public DisableAttribute(string fieldOrProperty, bool disableValue)
        {
#if UNITY_EDITOR
            this.name = fieldOrProperty;
            this.value = disableValue;
#endif
        }

    } // class DisableAttribute

} // namespace UnityExtensions