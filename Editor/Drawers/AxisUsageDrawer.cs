﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(AxisUsageAttribute))]
    class AxisUsageDrawer : BasePropertyDrawer<AxisUsageAttribute>
    {
        static string[][] _axisNames =
        {
                new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z" },
                new string[] { "X", "Y", "Z" },
                new string[] { "XY", "YZ", "XZ" },
            };


        static Axis[][] _axisValues =
        {
                new Axis[] { Axis.PositiveX, Axis.NegativeX, Axis.PositiveY, Axis.NegativeY, Axis.PositiveZ, Axis.NegativeZ },
                new Axis[] { Axis.X, Axis.Y, Axis.Z },
                new Axis[] { Axis.XY, Axis.YZ, Axis.XZ },
            };


        int AxisToIndex(Axis axis)
        {
            switch (attribute.usage)
            {
                case AxisUsage.Direction6: return AxisDirectionToIndex6();
                case AxisUsage.Axis3: return AxisToIndex3();
                case AxisUsage.Plane3: return AxisPlaneToIndex3();
                default: return -1;
            }

            int AxisToIndex3()
            {
                switch (axis)
                {
                    case Axis.PositiveX: case Axis.NegativeX: case Axis.X: return 0;
                    case Axis.PositiveY: case Axis.NegativeY: case Axis.Y: return 1;
                    case Axis.PositiveZ: case Axis.NegativeZ: case Axis.Z: return 2;
                    default: return -1;
                }
            }

            int AxisDirectionToIndex6()
            {
                switch (axis)
                {
                    case Axis.PositiveX: return 0;
                    case Axis.NegativeX: return 1;
                    case Axis.PositiveY: return 2;
                    case Axis.NegativeY: return 3;
                    case Axis.PositiveZ: return 4;
                    case Axis.NegativeZ: return 5;
                    default: return -1;
                }
            }

            int AxisPlaneToIndex3()
            {
                switch (axis)
                {
                    case Axis.XY: return 0;
                    case Axis.YZ: return 1;
                    case Axis.XZ: return 2;
                    default: return -1;
                }
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            int type = (int)attribute.usage / 2;

            if ((int)attribute.usage % 2 == 0)
            {
                int index = AxisToIndex((Axis)property.intValue);
                index = GUI.Toolbar(position, index, _axisNames[type], EditorStyles.miniButton);

                property.intValue = index < 0 ? 0 : (int)_axisValues[type][index];
            }
            else
            {
                position.width = position.width / _axisNames[type].Length - 2;
                int mask = property.intValue;

                for (int i = 0; i < _axisNames[type].Length; i++)
                {
                    int item = (int)_axisValues[type][i];
                    if (GUI.Toggle(position, (mask & item) == item, _axisNames[type][i], EditorStyles.miniButton))
                    {
                        mask |= item;
                    }
                    else
                    {
                        mask &= ~item;
                    }
                    position.x = position.xMax + 2;
                }

                property.intValue = mask;
            }
        }

    } // class AxisUsageDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR