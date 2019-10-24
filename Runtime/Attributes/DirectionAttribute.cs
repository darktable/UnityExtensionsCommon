using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 让 Vector3 代表方向向量, Inspector 上显示的是 XY 轴的欧拉角
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DirectionAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public float length;
        public Vector3 direction;
        public Vector2 eulerAngles;
#endif

        public DirectionAttribute(float length = 1f)
        {
#if UNITY_EDITOR
            this.length = Mathf.Clamp(length, MathUtilities.OneMillionth, MathUtilities.Million);
            this.direction = new Vector3(0, 0, length);
            this.eulerAngles = new Vector2();
#endif
        }

    } // class DirectionAttribute

} // namespace UnityExtensions