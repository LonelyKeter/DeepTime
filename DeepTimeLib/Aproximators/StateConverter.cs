using DeepTime.Lib.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Aproximators
{
    public struct StateConverter : IStateConverter<Data.State>
    {
        public int InputSize => throw new NotImplementedException();

        public float[] ToInput(State input)
        {
            throw new NotImplementedException();
        }
    }
}
