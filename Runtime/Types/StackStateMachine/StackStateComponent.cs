using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    /// <summary>
    /// BaseStackStateComponent
    /// </summary>
    public abstract class BaseStackStateComponent : ScriptableComponent, IStackState
    {
        public virtual void OnReset() { }
        public virtual void OnEnter(StackAction stackAction) { }
        public virtual void OnExit(StackAction stackAction) { }
        public virtual void OnUpdate(float deltaTime) { }
    }


    /// <summary>
    /// StackStateComponent. OnEnter & OnExit can be serialized.
    /// </summary>
    [AddComponentMenu("State Machines/Stack State Component")]
    [DisallowMultipleComponent]
    public class StackStateComponent : BaseStackStateComponent
    {
        [SerializeField]
        StackStateEvent _onEnter = default;

        [SerializeField]
        StackStateEvent _onExit = default;

        public event UnityAction<StackAction> onEnter
        {
            add
            {
                if (_onEnter == null) _onEnter = new StackStateEvent();
                _onEnter.AddListener(value);
            }
            remove { _onEnter?.RemoveListener(value); }
        }

        public event UnityAction<StackAction> onExit
        {
            add
            {
                if (_onExit == null) _onExit = new StackStateEvent();
                _onExit.AddListener(value);
            }
            remove { _onExit?.RemoveListener(value); }
        }


        public override void OnEnter(StackAction stackAction)
        {
            _onEnter?.Invoke(stackAction);
        }


        public override void OnExit(StackAction stackAction)
        {
            _onExit?.Invoke(stackAction);
        }

    } // class StackStateComponent

} // namespace UnityExtensions
