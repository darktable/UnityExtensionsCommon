
namespace UnityExtensions
{
    /// <summary>
    /// State stack action.
    /// </summary>
    public enum StackAction
    {
        Push,
        Pop,
    }


    /// <summary>
    /// Stack state.
    /// </summary>
    public interface IStackState
    {
        /// <summary>
        /// Called when StateMachine resets stack.
        /// </summary>
        void OnReset();

        /// <summary>
        /// Called when StateMachine enters this state.
        /// </summary>
        void OnEnter(StackAction stackAction);

        /// <summary>
        /// Called when StateMachine quits this state.
        /// </summary>
        void OnExit(StackAction stackAction);

        /// <summary>
        /// Called when StateMachine updates this state.
        /// </summary>
        void OnUpdate(float deltaTime);

    } // interface IStackState

} // namespace UnityExtensions
