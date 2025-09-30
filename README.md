# Quartz.NET Job Scheduling - Getting Started

This project demonstrates a simple Quartz.NET job scheduling implementation to help understand the core concepts and workflow.

## 🔧 What is Quartz.NET?

Quartz.NET is a powerful, feature-rich, job scheduling library for .NET applications. It allows you to schedule jobs to run at specific times, intervals, or based on complex scheduling patterns.

## 🏗️ Core Concepts

### 1. **Job** (`IJob`)
- A job is the work that you want to perform
- Implements the `IJob` interface with an `Execute` method
- Contains the business logic that runs when triggered
- **Example**: Sending emails, processing data, cleaning up files

### 2. **JobDetail**
- Describes the job instance and its properties
- Contains metadata about the job (name, group, description)
- Links to the job class that will be executed

### 3. **Trigger**
- Defines WHEN a job should run
- Contains scheduling information (start time, end time, repeat intervals)
- **Types**: SimpleTrigger, CronTrigger, CalendarIntervalTrigger

### 4. **Scheduler**
- The main engine that manages jobs and triggers
- Responsible for executing jobs at the right time
- Can start, stop, pause, and resume job execution

## 🚀 Workflow: Schedule → Execute

```
1. Define Job Class (IJob) → 2. Create JobDetail → 3. Create Trigger → 4. Schedule with Scheduler → 5. Job Executes
```

## 📁 Project Structure

```
├── Program.cs          # Main application with Quartz configuration
├── SimpleJob.cs        # Example job implementation
├── quartz-job.csproj  # Project file with Quartz.NET dependencies
└── README.md          # This file
```

## 🔄 How This Example Works

### The Job (`SimpleJob.cs`)
```csharp
public class SimpleJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Your business logic goes here
        // This example just logs information and simulates work
    }
}
```

### The Configuration (`Program.cs`)
1. **Register Quartz services** with dependency injection
2. **Create JobDetail** for our SimpleJob
3. **Create Trigger** that runs every 10 seconds using cron expression
4. **Start the scheduler** as a hosted service

### The Cron Expression
- `*/10 * * * * ?` = Every 10 seconds
- Format: `second minute hour day-of-month month day-of-week year`

## 🎯 Available Endpoints

Once running, you can interact with the scheduler via HTTP endpoints:

### Get Job Status
```bash
GET http://localhost:5000/jobs/status
```
Returns information about the job and its triggers.

### Manually Trigger Job
```bash
POST http://localhost:5000/jobs/trigger
```
Executes the job immediately, outside of its normal schedule.

## 🏃‍♂️ Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Watch the logs**: You'll see job executions every 10 seconds:
   ```
   === JOB EXECUTION STARTED ===
   Job: DEFAULT.SimpleJob executed at [timestamp]
   Processing... (simulating 2 seconds of work)
   Job completed successfully!
   === JOB EXECUTION FINISHED ===
   ```

## 📚 Key Benefits of Quartz.NET

- **Reliable**: Enterprise-grade job scheduling
- **Flexible**: Simple intervals to complex cron expressions
- **Scalable**: Supports clustering for high-availability
- **Integration**: Works seamlessly with .NET dependency injection
- **Persistent**: Can store job data in databases
- **Monitoring**: Rich APIs for job management and monitoring

## 🔧 Common Trigger Types

### Simple Trigger (Fixed Intervals)
```csharp
q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(30)
        .RepeatForever()));
```

### Cron Trigger (Complex Schedules)
```csharp
q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithCronSchedule("0 0 12 * * ?")); // Daily at noon
```

## 🎯 Next Steps

1. **Experiment** with different cron expressions
2. **Add job parameters** using JobDataMap
3. **Implement multiple jobs** with different schedules
4. **Add persistence** with database job store
5. **Explore clustering** for high-availability scenarios

## 📖 Useful Cron Examples

- `0 0 12 * * ?` - Daily at noon
- `0 15 10 ? * MON-FRI` - 10:15 AM every weekday
- `0 0/5 * * * ?` - Every 5 minutes
- `0 0 0 1 * ?` - First day of every month at midnight

---

**Happy Scheduling!** 🎯