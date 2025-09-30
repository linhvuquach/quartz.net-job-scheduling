using Quartz;

namespace quartz_job;

/// <summary>
/// A simple job that demonstrates basic Quartz.NET functionality
/// </summary>
public class SimpleJob : IJob
{
    private readonly ILogger<SimpleJob> _logger;

    public SimpleJob(ILogger<SimpleJob> logger)
    {
        _logger = logger;
    }

    // This method is called when the job is executed by Quartz
    public async Task Execute(IJobExecutionContext context)
    {
        // Get job details from the context
        var jobKey = context.JobDetail.Key;
        var triggerKey = context.Trigger.Key;
        var fireTime = context.FireTimeUtc;

        _logger.LogInformation("=== JOB EXECUTION STARTED ===");
        _logger.LogInformation("Job: {JobKey} executed at {FireTime}", jobKey, fireTime);
        _logger.LogInformation("Trigger: {TriggerKey}", triggerKey);

        // Simulate some work
        _logger.LogInformation("Processing... (simulating 2 seconds of work)");
        await Task.Delay(2000);

        _logger.LogInformation("Job completed successfully!");
        _logger.LogInformation("=== JOB EXECUTION FINISHED ===");
    }
}