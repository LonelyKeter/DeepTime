namespace DeepTime.Advisor.Data;
using static Data.Types;

public static class AdviceEnumerator
{
    static readonly int MaxLength = PriorityCount * AttractivenessCount;  

    public static int EnumCount => MaxLength + 1;

    public static int Enumerate(Advice advice) => advice.Rest ? 
        MaxLength : advice.Priority.AsIndex() * AttractivenessCount + advice.Attractiveness.AsIndex();

    public static Advice GetValue(int index) => index < 0 || index > MaxLength ? throw new ArgumentOutOfRangeException(nameof(index)) :
        index == MaxLength ? 
            new(Priority.VeryLow, Attractiveness.VeryLow, true) :
            new(PriorityFromIndex(index / AttractivenessCount), AttractivenessFromIndex(index % AttractivenessCount), false);
}
