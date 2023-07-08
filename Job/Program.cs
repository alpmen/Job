using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Job
{
    internal class Program
    {
        static async Task Main()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<MakeRequestJob>()
                .WithIdentity("RequestJob")
                .Build();

            var dailyTrigger = TriggerBuilder.Create()
                .WithIdentity("DailyTrigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(14, 28))
                .Build();

            var weeklyTrigger = TriggerBuilder.Create()
                .WithIdentity("WeeklyTrigger")
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 15, 0))
                .Build();

            var monthlyTrigger = TriggerBuilder.Create()
                .WithIdentity("MonthlyTrigger")
                .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 15, 0))
                .Build();

            await scheduler.ScheduleJob(job, dailyTrigger);
            //await scheduler.ScheduleJob(job, weeklyTrigger);
            //await scheduler.ScheduleJob(job, monthlyTrigger);

            await scheduler.Start();

            Console.WriteLine("Console uygulaması çalışıyor...");
            Console.ReadLine();

            await scheduler.Shutdown();
        }

        public class MakeRequestJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync("https://localhost:7267/api/ConsumerExpence/listTotal");
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var path = "HashedData.txt";

                        using (StreamWriter streamWriter = new StreamWriter(path, true, Encoding.UTF8))
                        {
                            streamWriter.WriteLine(content);
                        }
                        Console.WriteLine(content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }

                Console.WriteLine($"İstek gönderildi: {DateTime.Now}");
                await Task.CompletedTask;
            }
        }
    }
}




//class Program
//{
//    static async Task Main()
//    {
//        while (true)
//        {
//            await MakeRequest();
//            Thread.Sleep(TimeSpan.FromMinutes(5));
//        }
//    }

//    static async Task MakeRequest()
//    {
//        using (var client = new HttpClient())
//        {
//            try
//            {
//                var response = await client.GetAsync("http://localhost:5000/api/consumer");
//                response.EnsureSuccessStatusCode();
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine(content);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"An error occurred: {ex.Message}");
//            }
//        }
//    }
//}





//using Quartz;
//using Quartz.Impl;
//using System;
//using System.Threading.Tasks;

//class Program
//{
//    static async Task Main()
//    {
//        var schedulerFactory = new StdSchedulerFactory();
//        var scheduler = await schedulerFactory.GetScheduler();

//        var job = JobBuilder.Create<MakeRequestJob>()
//            .WithIdentity("RequestJob")
//            .Build();

//        var dailyTrigger = TriggerBuilder.Create()
//            .WithIdentity("DailyTrigger")
//            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(15, 0))
//            .Build();

//        var weeklyTrigger = TriggerBuilder.Create()
//            .WithIdentity("WeeklyTrigger")
//            .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 15, 0))
//            .Build();

//        var monthlyTrigger = TriggerBuilder.Create()
//            .WithIdentity("MonthlyTrigger")
//            .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 15, 0))
//            .Build();

//        await scheduler.ScheduleJob(job, dailyTrigger);
//        await scheduler.ScheduleJob(job, weeklyTrigger);
//        await scheduler.ScheduleJob(job, monthlyTrigger);

//        await scheduler.Start();

//        Console.WriteLine("Console uygulaması çalışıyor...");
//        Console.ReadLine();

//        await scheduler.Shutdown();
//    }
//}

//public class MakeRequestJob : IJob
//{
//    public async Task Execute(IJobExecutionContext context)
//    {
//        // Belirtilen URL'ye istek gönderme işlemleri burada gerçekleştirilebilir

//        Console.WriteLine($"İstek gönderildi: {DateTime.Now}");
//        await Task.CompletedTask;
//    }
//}