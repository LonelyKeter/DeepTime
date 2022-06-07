namespace DeepTime.RL;

public interface IQFunction
{
    int StateVecLength { get; }
    int ActionCount { get; }

    double[] GetQValues(double[] state);
    void CorrectQValue(double[] state, int takenAction, double correction);
}
