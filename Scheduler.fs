module Scheduler
open Quartz
open Quartz.Impl
open System.Threading.Tasks

let private functionKey = "function"

type private FunctionContainer(job: unit -> unit) =
    member __.Job = job

type private JobContainer () =
  interface IJob with
      member __.Execute(context) =
        async {
            let actualJob = context.JobDetail.JobDataMap.Get(functionKey) :?> FunctionContainer
            actualJob.Job()            
        } |> Async.StartAsTask :> Task

module private JobFactory =
    let private triggerJob cronExpression =
        TriggerBuilder.Create()
            .WithCronSchedule(cronExpression)
            .Build()

    let scheduleJob cronExpression job =        
        let schedulerFactory = StdSchedulerFactory()
        async {
            let! scheduler = schedulerFactory.GetScheduler() |> Async.AwaitTask
            scheduler.Start() |> ignore
            
            let jobDataMap = new JobDataMap()
            jobDataMap.Add(functionKey, new FunctionContainer(job))

            let builder = JobBuilder.Create<JobContainer>()
                                    .SetJobData(jobDataMap)

            let jobContainer = builder.Build()
            scheduler.ScheduleJob(jobContainer, (triggerJob cronExpression)) |> ignore
        }

let scheduleJob cronExpression job =
    JobFactory.scheduleJob cronExpression job |> Async.RunSynchronously