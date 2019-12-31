using System;

namespace UnityExtensions
{
    /// <summary>
    /// 状态机, 可作为一般状态机或子状态机使用
    /// </summary>
    public class StateMachine<T> : BaseState where T : class, IState
    {
        T _currentState;
        double _currentStateTime;


        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        public float currentStateTime => (float)_currentStateTime;


        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        public double currentStateTimeDouble => _currentStateTime;


#if DEBUG
        bool _duringSetting = false;
#endif


        /// <summary>
        /// 当前状态
        /// </summary>
        public T currentState
        {
            get => _currentState;
            set
            {
#if DEBUG
                if (_duringSetting)
                {
                    throw new Exception("Shouldn't change state inside OnExit or OnEnter!");
                }
                _duringSetting = true;
#endif

                _currentState?.OnExit();

                T lastState = _currentState;
                _currentState = value;
                _currentStateTime = 0;

                _currentState?.OnEnter();

#if DEBUG
                _duringSetting = false;
#endif

                StateChanged(lastState, _currentState);
            }
        }


        /// <summary>
        /// 状态变化后触发
        /// </summary>
        protected virtual void StateChanged(T lastState, T currentState)
        {
        }


        /// <summary>
        /// 更新当前状态
        /// 注意: 顶层状态机需要主动调用
        /// </summary>
        public override void OnUpdate(float deltaTime)
        {
            _currentStateTime += deltaTime;
            _currentState?.OnUpdate(deltaTime);
        }

    } // class StateMachine<T>

} // namespace UnityExtensions
