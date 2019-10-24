using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个 int 字段上, 使其作为 Layer 显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class LayerAttribute : PropertyAttribute
    {
    }

} // namespace UnityExtensions