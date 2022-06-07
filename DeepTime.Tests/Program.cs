using DeepTime.Lib.Agents;
using DeepTime.Lib.Aproximators;
using DeepTime.Lib.Data;
using DeepTime.Lib.Policies;
using DeepTime.Lib;

using DeepTime.Simulation;


using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

var gen = new User(UserConfig.Default);

var stream = new MemoryStream(256);
var formatter = new BinaryFormatter();

gen.Serialize(stream);
stream.Position = 0;
var deserialized = User.Deserialize(stream);
var a = 0;