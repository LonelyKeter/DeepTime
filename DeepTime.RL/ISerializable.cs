using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.RL;

public interface ISerializable<T> where T: ISerializable<T>
{
    static abstract T Deserialize(Stream stream);
    void Serialize(Stream stream);
    void LoadFrom(Stream stream);
    
}