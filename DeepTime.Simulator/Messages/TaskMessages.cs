namespace DeepTime.Simulator.Messages;
using DeepTime.Simulation;

using Simulator.Model;
using Simulator.ViewModels;

using System.Collections.Generic;


public record DoTaskMessage(int TaskId, int MinutesSpent, int? NewEstimate, bool Finished);
public record RestMessage();
public record NewTaskProposedMessage(UserFeedback? Feedback);
public record TaskSelectedMessage(TaskVM? Task);
public record TasksAddedMessage(IReadOnlyCollection<TaskVM> Tasks);
public record TasksGeneratedMessage(IReadOnlyCollection<TaskVM> Tasks);
public record TaskDeletedMessage(TaskVM Task);
public record TasksClearedMessage(IReadOnlyCollection<TaskVM> Tasks);