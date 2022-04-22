using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib
{
    public interface IPolicy<TState>
    {
        public int Eval<TQTable>(TState state, TQTable qTable) 
            where TQTable : IQFunction<TState>;
    }
}
