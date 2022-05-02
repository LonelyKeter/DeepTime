using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Aproximators
{
    public interface IStateConverter<TInput>
    {
        double[] ToInput(TInput input);
        int InputSize { get; }
    }
}
