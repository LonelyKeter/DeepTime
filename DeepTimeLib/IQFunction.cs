using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib;

public interface IQFunction<TState>
{
    float[] GetQValues(TState state);
    void CorrectQValue(TState state, int takenAction, float correction);

    int ActionCount { get; }
}
