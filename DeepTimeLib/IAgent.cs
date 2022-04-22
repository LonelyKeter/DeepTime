namespace DeepTime.Lib;

public interface IAgent<TState, TAction>
{
    void SetNext(TState state, float reward);
    TAction Eval();

    void StartEpisode(TState state);
    void EndEpisode(TState state, float reward);
}
