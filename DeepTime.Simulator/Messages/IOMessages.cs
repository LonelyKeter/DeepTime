namespace DeepTime.Simulator.Messages;

using Model;
using Simulation;
using ViewModels;
using RL;

public record TaskGeneratorLoadedMessage(TaskVMGenerator Generator);
public record UserConfigLoadedMessage(UserConfig Config);
public record AgentLoadedMessage(IAgent Agent);
