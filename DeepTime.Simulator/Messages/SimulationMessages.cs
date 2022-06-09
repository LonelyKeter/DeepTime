namespace DeepTime.Simulator.Messages;

using DeepTime.Advisor.Data;
using DeepTime.Advisor.Statistics;

using DeepTime.RL;
using DeepTime.RL.Agents;

using DeepTime.Simulation;

using DeepTime.Simulator.ViewModels;

using System.Collections.Generic;

public record ScheduleContextChangedMessage(ScheduleContext New);
public record UserStateChangedChangedMessage(UserState New);
public record AgentChangedMessage(IAgent New);
public record DQNLoadedMessage(DQN New);
public record DayStartedMessage(IReadOnlyCollection<TaskVM> Tasks);
public record DayFinishedMessage(uint EpisodeNumber, StatisticsEntry Entry);
public record SimulationStartedMessage();
public record SimulationEndedMessage();