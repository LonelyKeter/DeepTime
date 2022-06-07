namespace DeepTime.RL;

public interface IEnvironment
{
    int StateVecLength { get; }
    int ActionCount { get; }

    double[] CurrentState { get; }
    (double[], double, bool) ChangeState(int action);
}
