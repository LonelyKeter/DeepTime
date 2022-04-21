namespace DeepTime.Lib;

public interface IAgent<TState, TAction>
{
    void SetNext(TState state, double reward);
    TAction Eval();
    void EndEpisode(TState state, double reward);
}
