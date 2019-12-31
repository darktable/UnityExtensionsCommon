﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    /// <summary>
    /// 状态组件基类
    /// </summary>
    public abstract class BaseStateComponent : ScriptableComponent, IState
    {
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate(float deltaTime) { }
}


    /// <summary>
    /// 状态组件. 状态的 Enter 和 Exit 事件可序列化
    /// </summary>
    [AddComponentMenu("State Machines/State Component")]
    [DisallowMultipleComponent]
    public class StateComponent : BaseStateComponent
    {
        [SerializeField]
        UnityEvent _onEnter = default;

        [SerializeField]
        UnityEvent _onExit = default;


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

    } // class StateComponent

} // namespace UnityExtensions
