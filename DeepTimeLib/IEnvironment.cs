namespace DeepTime.Lib;

public interface IEnvironment<TState, TAction>
{
    TState CurrentState { get; }
    (TState, double, bool) ChangeState(TAction action);
}
