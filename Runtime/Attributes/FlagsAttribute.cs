using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在一个枚举字段上, 将其作为选择掩码使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class FlagsAttribute : PropertyAttribute
    {
        public bool includeObsolete;

        public FlagsAttribute(bool includeObsolete = false)
        {
            this.includeObsolete = includeObsolete;
        }

    } // class FlagsAttribute

} // namespace UnityExtensions