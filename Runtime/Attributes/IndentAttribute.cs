using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 设置缩进
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IndentAttribute : PropertyAttribute
    {
        public int indentLevel;

        public IndentAttribute(int indentLevel = 1)
        {
            this.indentLevel = indentLevel;
        }

    } // class IndentAttribute

} // namespace UnityExtensions