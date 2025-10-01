There are multiple ways to stop jobs in Quartz.NET. Here are the different approaches:

## 1. By JobKey
   await scheduler.PauseJob(jobKey);

## 2. By TriggerKey - Pause specific triggers instead of the entire job
   var triggerKey = new TriggerKey("SimpleJob-trigger");
   await scheduler.PauseTrigger(triggerKey);

## 3. By Group - Pause all jobs in a group
   await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals("myGroup"));

## 4. Pause All Jobs
   await scheduler.PauseAll();

## 5. Unschedule Trigger - Remove a specific trigger without deleting the job
   await scheduler.UnscheduleJob(triggerKey);

## 6. Shutdown Scheduler - Stop everything
   await scheduler.Shutdown();

## Key Differences:

- PauseJob(jobKey): Pauses ALL triggers for that job
- PauseTrigger(triggerKey): Pauses only that specific trigger (useful if a job has multiple triggers)
- DeleteJob(jobKey): Removes job and all triggers permanently
- UnscheduleJob(triggerKey): Removes trigger but keeps the job definition
