namespace DeepTime.RL;

public interface IAgent
{
    int StateVecLength { get; }
    int ActionCount { get; }

    void SetNext(double[] state, double reward);
    int Eval();

    void StartEpisode(double[] state);
    void EndEpisode(double[] state, double reward);
}
