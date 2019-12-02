using System;
using System.Collections;
using System.IO;
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
            _globalGameObject.transform.ResetLocal();
            UObject.DontDestroyOnLoad(_globalGameObject);
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void InitializeEditor()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                if (!Application.isPlaying) unitedUpdate?.Invoke();
            };
        }
#endif


        public static Transform globalTransform => _globalGameObject.transform;


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
        public static event Action waitForEndOfFrame;


        /// <summary>
        /// 一个通用的 Update，编辑器和运行时都会触发
        /// 注意：应该同时搭配使用 unitedDeltaTime
        /// </summary>
        public static event Action unitedUpdate;


        public static float unitedDeltaTime
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return Editor.EditorUtilities.unscaledDeltaTime * Time.timeScale;
                else
#endif
                    return Time.deltaTime;
            }
        }


        public static float unitedUnscaledDeltaTime
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return Editor.EditorUtilities.unscaledDeltaTime;
                else
#endif
                    return Time.unscaledDeltaTime;
            }
        }


        public static float GetUnitedDeltaTime(TimeMode mode)
        {
            return mode == TimeMode.Normal ? unitedDeltaTime : unitedUnscaledDeltaTime;
        }


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
                case UpdateMode.WaitForEndOfFrame: waitForEndOfFrame += action; return;
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
                case UpdateMode.WaitForEndOfFrame: waitForEndOfFrame -= action; return;
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
                StartCoroutine(WaitForEndOfFrame());

                IEnumerator WaitForFixedUpdate()
                {
                    var wait = new WaitForFixedUpdate();
                    while (true)
                    {
                        yield return wait;
                        waitForFixedUpdate?.Invoke();
                    }
                }

                IEnumerator WaitForEndOfFrame()
                {
                    var wait = new WaitForEndOfFrame();
                    while (true)
                    {
                        yield return wait;
                        waitForEndOfFrame?.Invoke();
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
                unitedUpdate?.Invoke();
            }

            void LateUpdate()
            {
                lateUpdate?.Invoke();
            }

        } // class GlobalComponent


        /// <summary>
        /// Support binary reading/writing for Vector2, Vector3, Vector4, byte[]
        /// </summary>
        public static void RegisterCommonBinaryReadWrite()
        {
            if (_commonBinaryReadWriteRegistered) return;
            _commonBinaryReadWriteRegistered = true;

            ReaderWriterExtensions.Register((BinaryWriter writer, Vector2 value) =>
            {
                writer.Write(value.x);
                writer.Write(value.y);
            });

            ReaderWriterExtensions.Register((BinaryReader reader) =>
            {
                Vector2 value;
                value.x = reader.ReadSingle();
                value.y = reader.ReadSingle();
                return value;
            });

            ReaderWriterExtensions.Register((BinaryWriter writer, Vector3 value) =>
            {
                writer.Write(value.x);
                writer.Write(value.y);
                writer.Write(value.z);
            });

            ReaderWriterExtensions.Register((BinaryReader reader) =>
            {
                Vector3 value;
                value.x = reader.ReadSingle();
                value.y = reader.ReadSingle();
                value.z = reader.ReadSingle();
                return value;
            });

            ReaderWriterExtensions.Register((BinaryWriter writer, Vector4 value) =>
            {
                writer.Write(value.x);
                writer.Write(value.y);
                writer.Write(value.z);
                writer.Write(value.w);
            });

            ReaderWriterExtensions.Register((BinaryReader reader) =>
            {
                Vector4 value;
                value.x = reader.ReadSingle();
                value.y = reader.ReadSingle();
                value.z = reader.ReadSingle();
                value.w = reader.ReadSingle();
                return value;
            });

            ReaderWriterExtensions.Register((BinaryWriter writer, byte[] value) =>
            {
                writer.Write(value.Length);
                writer.Write(value);
            });

            ReaderWriterExtensions.Register((BinaryReader reader) =>
            {
                int length = reader.ReadInt32();
                return reader.ReadBytes(length);
            });
        }
        static bool _commonBinaryReadWriteRegistered;


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
        public static bool IsNullOrEmpty<T>(T collection) where T : ICollection
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