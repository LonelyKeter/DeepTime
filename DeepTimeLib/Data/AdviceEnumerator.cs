namespace DeepTime.Lib.Data;

using static Data.Types;

public struct AdviceEnumerator : IActionEnumerator<Advice>
{
    static readonly int MaxLength = PriorityCount * AttractivenessCount;  

    public int EnumCount => MaxLength + 1;

    public int this[Advice advice] => advice.Rest ? 
        MaxLength : advice.Priority.AsIndex() * AttractivenessCount + advice.Attractiveness.AsIndex();

    public Advice this[int index] => index < 0 || index > MaxLength ? throw new ArgumentOutOfRangeException(nameof(index)) :
        index == MaxLength ? 
            new(Priority.VeryLow, Attractiveness.VeryLow, true) :
            new(PriorityFromIndex(index / AttractivenessCount), AttractivenessFromIndex(index % AttractivenessCount), false);
}
