using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Nutty.Commands;
using Nutty.Commands.Jobs;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Nutty
{

    class Program {

        public static DiscordClient discord;

        public static int timeSlot = 2;
        public static bool frontlineActive = false;

        public static JObject config;

        public static async Task Main(string[] args) {
            Console.WriteLine("Setting up Quartz Scheduler!");
            // Setup Quartz Scheduler Factory & Scheduler
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            // Making Jobs & Triggers for Frontline
            var utcHour = 11 + (6 * (timeSlot - 1));
            if (utcHour > 24) { utcHour -= 24; }

            var jobEarly = JobBuilder.Create<FrotnlineNotifier>()
                                .WithIdentity("frontline_notifier_early", "frontline")
                                .Build();

            var triggerEarly = TriggerBuilder.Create()
                                        .WithIdentity("1h", "frontline")
                                        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(utcHour, 0).InTimeZone(TimeZoneInfo.Utc))
                                        .ForJob(jobEarly)
                                        .Build();

            var jobReminder = JobBuilder.Create<FrotnlineNotifier>()
                                .WithIdentity("frontline_notifier_reminder", "frontline")
                                .Build();

            var triggerReminder = TriggerBuilder.Create()
                                        .WithIdentity("30min", "frontline")
                                        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(utcHour, 30).InTimeZone(TimeZoneInfo.Utc))
                                        .ForJob(jobReminder)
                                        .Build();

            var jobStarting = JobBuilder.Create<FrotnlineNotifier>()
                                .WithIdentity("frontline_notifier_starting", "frontline")
                                .Build();

            var triggerStarting = TriggerBuilder.Create()
                                        .WithIdentity("5min", "frontline")
                                        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(utcHour, 55).InTimeZone(TimeZoneInfo.Utc))
                                        .ForJob(jobStarting)
                                        .Build();

            var jobCleanup = JobBuilder.Create<Cleaner>()
                .WithIdentity("fl_cleanup", "cleaner")
                .Build();

            var triggerCleanup = TriggerBuilder.Create()
                .WithIdentity("cfl_cleanup", "cleaner")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(utcHour + 1, 35).InTimeZone(TimeZoneInfo.Utc))
                .ForJob(jobCleanup)
                .Build();

            // Registering Jobs and their Triggers for Frontline.
            await scheduler.ScheduleJob(jobEarly, triggerEarly);
            await scheduler.ScheduleJob(jobReminder, triggerReminder);
            await scheduler.ScheduleJob(jobStarting, triggerStarting);
            await scheduler.ScheduleJob(jobCleanup, triggerCleanup);

            // Loading Discord Bot settings.
            config = JObject.Parse(File.ReadAllText("appSettings.json"));

            // Setting up Discord Bot Configuration
            Console.WriteLine("Setting up Discord Bot!");
            discord = new DiscordClient(new DiscordConfiguration() {
                Token = (string)config["Token"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.None
            });

            frontlineActive = (bool)config["Frontline_State"];

            Console.WriteLine("Registering Traditional & Slash Commands!");
            // Registering Traditional and Slash Commands
            var commandsNext = discord.UseCommandsNext(new CommandsNextConfiguration() { 
                StringPrefixes = new[] { "-" },
                EnableDms = false

            });

            var commandsSlash = discord.UseSlashCommands();

            commandsNext.RegisterCommands<TraditionalCommands>();
//            commandsSlash.RegisterCommands<SlashCommands>();

            // Upadting Discord Bot Activity/Status on Ready.
            discord.Ready += async (s, e) => {
                var state = frontlineActive ? "active" : "inactive";
                await UpdateStatus($"Frontline is {state}");
            };

            Console.WriteLine("Nutty is Starting!");

            // Running Discord Bot
            await discord.ConnectAsync();
            Console.WriteLine("Nutty has Started!");

            // Exit with Confirmation
            EndPoint:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();

            Console.WriteLine("Are you Sure? Y/N");
            var input = Console.ReadKey().Key;
            bool isValidInput = input != null && (input == ConsoleKey.Y ||
                                                  input == ConsoleKey.N);

            if (isValidInput) {
                bool shouldContinue = input == ConsoleKey.Y;
                if (shouldContinue) {
                    scheduler.Shutdown();
                    discord.DisconnectAsync().GetAwaiter().GetResult();
                }
                else {
                    goto EndPoint;
                }
            }
            else {
                Console.WriteLine("Invalid input.");
                goto EndPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task UpdateStatus(string message) {
            await discord.UpdateStatusAsync(new DiscordActivity(message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool isUserAdmin(DiscordMember member) {
            return member.Roles.Any(role => role.Permissions.HasPermission(Permissions.Administrator));
        }

    }
}