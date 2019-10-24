using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个字段上, 根据指定的 Property 或 Field 决定是否隐藏编辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class HideAttribute : PropertyAttribute
    {
#if UNITY_EDITOR

        public string name;
        public bool value;

        public object fieldOrProp;
        public int result; // 0-hide, 1-show, -1-error

#endif


        public HideAttribute(string fieldOrProperty, bool hideValue)
        {
#if UNITY_EDITOR
            this.name = fieldOrProperty;
            this.value = hideValue;
#endif
        }



    } // class HideAttribute

} // namespace UnityExtensions