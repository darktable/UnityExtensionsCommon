using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个字段上, 可以替换显示的 Label
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class LabelAttribute : PropertyAttribute
    {
        public string label;

        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }

} // namespace UnityExtensions