namespace DeepTime.Lib.Data;

public struct AdviceEnumerator : IActionEnumerator<Advice>
{
    static readonly int PrioirtyDim = Enum.GetNames<Priority>().Length;
    static readonly int AttractivenessDim = Enum.GetNames<Attractiveness>().Length;

    static readonly int MaxLength = PrioirtyDim * AttractivenessDim;

    public int EnumCount => MaxLength + 1;

    public int this[Advice advice] => advice.Rest ? 
        MaxLength : (int)advice.Priority * AttractivenessDim + (int)advice.Attractiveness;

    public Advice this[int index] => index < 0 || index > MaxLength ? throw new ArgumentOutOfRangeException(nameof(index)) :
        index == MaxLength ? 
            new(Priority.Low, Attractiveness.Low, true) :
            new((Priority)(index / AttractivenessDim), (Attractiveness)(index % AttractivenessDim), false);
}
