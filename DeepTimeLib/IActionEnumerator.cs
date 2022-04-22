using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib
{
    public interface IActionEnumerator<TAction>
    {
        int this[TAction item] { get; }
        TAction this[int index] { get; }

        int EnumCount { get; }
    }
}
