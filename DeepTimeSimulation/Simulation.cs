namespace DeepTime.Simulation;

using DeepTime.Lib;
using DeepTime.Lib.Data;

public class Simulation<TAgent, TScheduleSource, TUser>
    where TAgent: IAgent<State, Action>
    where TScheduleSource : ISimulatedScheduleSource
    where TUser: IUser
{
    private Advisor<TAgent, TScheduleSource> _advisor;

    private readonly TUser _user;    
    private readonly TScheduleSource _scheduleSource;

    public void StepForward(int minutes)
    {
        while(true)
        {
            _scheduleSource.StepForward(minutes);
            var currentTime = _scheduleSource.GetCurrentTime();

            var proposedTasks = _advisor.GetAdvice();
            var feedback = _user.GetFeedback(proposedTasks, currentTime);

            if (!feedback.HasValue)
            {
                break;
            } 
            
            var value = feedback.Value;

            _advisor.SubmitProgress(value.TaskId, value.MinutesSpent, value.Done, value.NewEstimate);
            minutes = value.MinutesSpent;
        }
    }    

    public void SimulateDay(int timeStep)
    {
        while(!_scheduleSource.DayHasPassed())
        {
            StepForward(timeStep);
        }
    }

    public void SimulateDays(int timeStep, int dayCount) 
    {
        for (var i = 0; i < dayCount; i++)
        {
            StepForward(timeStep);
        }
    }
}
