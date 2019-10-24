﻿using System;
using System.Collections;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace UnityExtensions
{
    /// <summary>
    /// 运行时工具箱
    /// </summary>
    public struct RuntimeUtilities
    {
        static GameObject _globalGameObject;


        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            _globalGameObject = new GameObject("GlobalGameObject");
            _globalGameObject.AddComponent<GlobalComponent>();
            _globalGameObject.hideFlags = HideFlags.HideInHierarchy;
            UObject.DontDestroyOnLoad(_globalGameObject);
        }


        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action fixedUpdate;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action waitForFixedUpdate;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action update;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action lateUpdate;
        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static event Action onGUI;


        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static void AddUpdate(UpdateMode mode, Action action)
        {
            switch (mode)
            {
                case UpdateMode.FixedUpdate: fixedUpdate += action; return;
                case UpdateMode.WaitForFixedUpdate: waitForFixedUpdate += action; return;
                case UpdateMode.Update: update += action; return;
                case UpdateMode.LateUpdate: lateUpdate += action; return;
                case UpdateMode.OnGUI: onGUI += action; return;
            }
        }


        /// <summary>
        /// 仅用于运行时
        /// </summary>
        public static void RemoveUpdate(UpdateMode mode, Action action)
        {
            switch (mode)
            {
                case UpdateMode.FixedUpdate: fixedUpdate -= action; return;
                case UpdateMode.WaitForFixedUpdate: waitForFixedUpdate -= action; return;
                case UpdateMode.Update: update -= action; return;
                case UpdateMode.LateUpdate: lateUpdate -= action; return;
                case UpdateMode.OnGUI: onGUI -= action; return;
            }
        }


        /// <summary>
        /// 添加全局组件
        /// </summary>
        public static T AddGlobalComponent<T>() where T : Component
        {
            return _globalGameObject.AddComponent<T>();
        }


        public class GlobalComponent : ScriptableComponent
        {
            void Start()
            {
                StartCoroutine(WaitForFixedUpdate());

                IEnumerator WaitForFixedUpdate()
                {
                    var wait = new WaitForFixedUpdate();
                    while (true)
                    {
                        yield return wait;
                        waitForFixedUpdate?.Invoke();
                    }
                }
            }

            void FixedUpdate()
            {
                fixedUpdate?.Invoke();
            }

            void Update()
            {
                update?.Invoke();
            }

            void LateUpdate()
            {
                lateUpdate?.Invoke();
            }

            void OnGUI()
            {
                onGUI?.Invoke();
            }

        } // class GlobalComponent


        /// <summary>
        /// 循环利用的 MaterialPropertyBlock 池
        /// </summary>
        public static readonly ObjectPool<MaterialPropertyBlock> materialPropertyBlockPool = new ObjectPool<MaterialPropertyBlock>();


        /// <summary>
        /// 交换两个变量的值
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }


        /// <summary>
        /// 判断集合是否为 null 或元素个数是否为 0
        /// </summary>
        public static bool IsNullOrEmpty(ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }


        /// <summary>
        /// 同时设置 Unity 时间缩放和 FixedUpdate 频率
        /// </summary>
        /// <param name="timeScale"> 要设置的时间缩放 </param>
        /// <param name="fixedFrequency"> 要设置的 FixedUpdate 频率 </param>
        public static void SetTimeScaleAndFixedFrequency(float timeScale, float fixedFrequency)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = timeScale / fixedFrequency;
        }


        /// <summary>
        /// 使用恰当的方式 Destroy Object
        /// </summary>
        public static void DestroySafely(UObject obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UObject.DestroyImmediate(obj);
                else
#endif
                    UObject.Destroy(obj);
            }
        }

    } // struct RuntimeUtilities

} // namespace UnityExtensions