
namespace MongoJobScheduler.Tasks
{
    public abstract class ScheduleTask : ITask
    {
        public abstract void Execute();
    }
}
