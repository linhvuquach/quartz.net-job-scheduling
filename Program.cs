using Quartz;
using quartz_job;

var builder = WebApplication.CreateBuilder(args);
// Create a job key for our SimpleJob
var jobKey = new JobKey("SimpleJob");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    // Use a scoped container to create jobs. This makes it easier to use dependency injection.
    // q.UseMicrosoftDependencyInjection();
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Register the job and set the job data
    q.AddJob<SimpleJob>(opts => opts.WithIdentity(jobKey));

    // Create a trigger that runs next Monday
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SimpleJob-trigger")
        .WithCronSchedule("0 0 9 ? * MON") // Every Monday at 9:00 AM
        .WithDescription("Trigger for SimpleJob that runs every Monday at 9:00 AM"));
});

// Add the Quartz.NET hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Quartz.NET endpoints
app.MapPost("/jobs/trigger", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();

    // Trigger the job immediately
    await scheduler.TriggerJob(jobKey);
    
    return Results.Ok(new { message = "Job triggered successfully", jobKey = jobKey.ToString() });
})
.WithName("TriggerJob")
.WithOpenApi();

app.MapGet("/jobs/status", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();
    
    var jobDetail = await scheduler.GetJobDetail(jobKey);
    var triggers = await scheduler.GetTriggersOfJob(jobKey);
    var triggerInfo = triggers.Select(async t => new 
    {
        Key = t.Key.ToString(),
        State = await scheduler.GetTriggerState(t.Key),
        NextFireTime = t.GetNextFireTimeUtc(),
        PreviousFireTime = t.GetPreviousFireTimeUtc()
    });
    
    return Results.Ok(new 
    { 
        JobExists = jobDetail != null,
        JobKey = jobKey.ToString(),
        Triggers = await Task.WhenAll(triggerInfo)
    });
})
.WithName("GetJobStatus")
.WithOpenApi();

app.MapPost("/jobs/reschedule-tomorrow", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();

    // Get the existing trigger
    var triggerKey = new TriggerKey("SimpleJob-trigger");
    var existingTrigger = await scheduler.GetTrigger(triggerKey);

    if (existingTrigger == null)
    {
        return Results.NotFound(new { message = "Trigger not found" });
    }

    // Create a new trigger for tomorrow at 9:00 AM
    var tomorrow = DateTime.Today.AddDays(1).AddHours(9);
    var newTrigger = TriggerBuilder.Create()
        .WithIdentity("SimpleJob-trigger")
        .ForJob(jobKey)
        .StartAt(tomorrow)
        .WithDescription($"Trigger for SimpleJob rescheduled to run tomorrow at {tomorrow:yyyy-MM-dd HH:mm}")
        .Build();

    // Replace the existing trigger
    await scheduler.RescheduleJob(triggerKey, newTrigger);

    return Results.Ok(new
    {
        message = "Job rescheduled successfully",
        jobKey = jobKey.ToString(),
        newSchedule = tomorrow,
        triggerKey = triggerKey.ToString()
    });
})
.WithName("RescheduleJobTomorrow")
.WithOpenApi();

app.MapPost("/jobs/pause", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();

    // Pause the job - this prevents all triggers associated with the job from firing
    await scheduler.PauseJob(jobKey);

    return Results.Ok(new
    {
        message = "Job paused successfully",
        jobKey = jobKey.ToString(),
        description = "The job will not execute until it is resumed"
    });
})
.WithName("PauseJob")
.WithOpenApi();

app.MapPost("/jobs/resume", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();

    // Resume the job - this allows triggers to fire again
    await scheduler.ResumeJob(jobKey);

    return Results.Ok(new
    {
        message = "Job resumed successfully",
        jobKey = jobKey.ToString(),
        description = "The job will now execute according to its schedule"
    });
})
.WithName("ResumeJob")
.WithOpenApi();

app.MapDelete("/jobs/delete", async (ISchedulerFactory schedulerFactory) =>
{
    var scheduler = await schedulerFactory.GetScheduler();

    // Delete the job - this removes the job and all its triggers from the scheduler
    var deleted = await scheduler.DeleteJob(jobKey);

    if (!deleted)
    {
        return Results.NotFound(new { message = "Job not found or already deleted" });
    }

    return Results.Ok(new
    {
        message = "Job deleted successfully",
        jobKey = jobKey.ToString(),
        description = "The job and all its triggers have been permanently removed from the scheduler"
    });
})
.WithName("DeleteJob")
.WithOpenApi();

app.Run();
