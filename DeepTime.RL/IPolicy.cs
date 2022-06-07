namespace DeepTime.RL;

public interface IPolicy
{
    public int Eval(double[] actionValues);
}
