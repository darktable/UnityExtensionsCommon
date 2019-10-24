using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 标记在 Quaternion 字段上, 将其显示为欧拉角
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class EulerAnglesAttribute : PropertyAttribute
    {
#if UNITY_EDITOR

        public Quaternion quaternion = Quaternion.identity;
        public Vector3 eulerAngles = Vector3.zero;

#endif

    } // class EulerAnglesAttribute

} // namespace UnityExtensions