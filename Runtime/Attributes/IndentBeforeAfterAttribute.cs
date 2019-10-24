using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 在字段之前设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentBeforeAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public int indentLevel;
#endif

        public IndentBeforeAttribute(int indentLevel = 1)
        {
#if UNITY_EDITOR
            this.indentLevel = indentLevel;
#endif
        }
    }


    /// <summary>
    /// 在字段之后设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentAfterAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public int indentLevel;
#endif

        public IndentAfterAttribute(int indentLevel = -1)
        {
#if UNITY_EDITOR
            this.indentLevel = indentLevel;
#endif
        }
    }

} // namespace UnityExtensions