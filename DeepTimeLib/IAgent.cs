namespace DeepTime.Lib;

public interface IAgent<TState, TAction>
{
    void SetNext(TState state, double reward);
    TAction Eval();

    void StartEpisode(TState state);
    void EndEpisode(TState state, double reward);

    string ToJson();
}
