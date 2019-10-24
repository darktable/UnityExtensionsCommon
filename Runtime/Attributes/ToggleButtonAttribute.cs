using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个 bool 字段上, 将其显示为按钮
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ToggleButtonAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public string label;
        public string trueText;
        public string falseText;
        public bool indent;
#endif

        public ToggleButtonAttribute(string label, string trueText, string falseText)
        {
#if UNITY_EDITOR
            this.label = label;
            this.trueText = trueText;
            this.falseText = falseText;
#endif
        }

        public ToggleButtonAttribute(string text, bool indent = true)
        {
#if UNITY_EDITOR
            this.label = text;
            this.indent = indent;
#endif
        }

    } // class ToggleButtonAttribute

} // namespace UnityExtensions