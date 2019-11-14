﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class BaseState : IState
    {
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate(float deltaTime);
    }


    /// <summary>
    /// 可序列化状态. 状态的 Enter 和 Exit 事件可序列化
    /// </summary>
    [Serializable]
    public class State : BaseState
    {
        [SerializeField]
        UnityEvent _onEnter = default;

        [SerializeField]
        UnityEvent _onExit = default;


        /// <summary>
        /// 添加或移除更新状态触发的事件
        /// </summary>
        public event Action<float> onUpdate;


        /// <summary>
        /// 添加或移除进入状态触发的事件
        /// </summary>
        public event UnityAction onEnter
        {
            add
            {
                if (_onEnter == null) _onEnter = new UnityEvent();
                _onEnter.AddListener(value);
            }
            remove { _onEnter?.RemoveListener(value); }
        }


        /// <summary>
        /// 添加或移除离开状态触发的事件
        /// </summary>
        public event UnityAction onExit
        {
            add
            {
                if (_onExit == null) _onExit = new UnityEvent();
                _onExit.AddListener(value);
            }
            remove { _onExit?.RemoveListener(value); }
        }


        public override void OnEnter()
        {
            _onEnter?.Invoke();
        }


        public override void OnExit()
        {
            _onExit?.Invoke();
        }


        public override void OnUpdate(float deltaTime)
        {
            onUpdate?.Invoke(deltaTime);
        }

    } // class State

} // namespace UnityExtensions
