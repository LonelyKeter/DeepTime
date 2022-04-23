using DeepTime.Lib.Agents;
using DeepTime.Lib.Aproximators;
using DeepTime.Lib.Data;
using DeepTime.Lib.Policies;
using DeepTime.Lib;

using System.Text.Json;
using System.Text.Json.Serialization;

var dqn = new DQN<EpsilonGreedy<State>>(
    new EpsilonGreedy<State>(0.05), 
    new QFunctionApproximator<State, StateConverter>(
        new StateConverter(), 
        new AdviceEnumerator().EnumCount
    ), 
    new HyperParameters(0.01f, 0.5f)
);

Console.WriteLine(JsonSerializer.Serialize<DQN<EpsilonGreedy<State>>>(dqn));