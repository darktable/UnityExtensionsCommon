using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// ScriptableAssetSingleton
    /// 在编辑器和运行时提供访问资源单例的方法
    /// 注意：创建的资源需要加入项目的 Preloaded Assets 列表
    /// </summary>
    public class ScriptableAssetSingleton<T> : ScriptableAsset where T : ScriptableAssetSingleton<T>
    {
        static T _instance;


        /// <summary>
        /// 访问单例
        /// </summary>
        public static T instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    _instance = AssetUtilities.FindAsset<T>();
                    if (!_instance)
                    {
                        _instance = CreateInstance<T>();
                        Debug.LogWarning(string.Format("No asset of {0} loaded, a temporary instance was created. Use {0}.CreateOrSelectAsset to create an asset.", typeof(T).Name));
                    }
#else
                    _instance = CreateInstance<T>();
                    Debug.LogWarning(string.Format("No asset of {0} loaded, a temporary instance was created. Do you forget to add the asset to \"Preloaded Assets\" list?", typeof(T).Name));
#endif
                }
                return _instance;
            }
        }


        protected ScriptableAssetSingleton()
        {
            _instance = this as T;
        }


#if UNITY_EDITOR

        /// <summary>
        /// 创建单例资源, 如果已经存在则选中该资源
        /// </summary>
        public static void CreateOrSelectAsset()
        {
            bool needCreate = false;

            if (!_instance)
            {
                _instance = AssetUtilities.FindAsset<T>();
                if (!_instance)
                {
                    _instance = CreateInstance<T>();
                    needCreate = true;
                }
            }
            else needCreate = !AssetDatabase.IsNativeAsset(_instance);

            if (needCreate)
            {
                AssetUtilities.CreateAsset(_instance);
            }

            Selection.activeObject = instance;
        }

#endif

    } // class ScriptableAssetSingleton

} // namespace UnityExtensions