using Nutty.Commands;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands.Jobs
{
    public class FrotnlineNotifier : IJob
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Frontline Notifier triggered");

            // Checks if Frontline is Currently Active
            if (Program.frontlineActive)
            {
                Console.WriteLine("Frontline is Active, sending the reminder in #frontline channel");

                // Get the Frontline Channel
                var channel = await Program.discord.GetChannelAsync(ulong.Parse(Holder.Instance.channelIds["Frontline_Channel"]));

                // Calculate Frontline Starting Time
                int hour = 12 + 6 * (Program.timeSlot - 1);
                var addDay = false;
                if (hour > 24)
                {
                    hour -= 24;
                    addDay = true;
                }

                // Get DateTime data used for Time Difference calculation
                DateTime now = DateTime.UtcNow;
                DateTime frontline = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hour, 0, 0, DateTimeKind.Utc);
                if (addDay) { frontline.AddDays(1); }

                // Get time left until Frontline
                TimeSpan timeDif = frontline.Subtract(now);

                // Send a message in Frontline Channel informing uses of time until Frontline.
                if (timeDif.Hours > 0)
                {
                    await channel.SendMessageAsync($"Frontline is starting in {timeDif.Hours}h {timeDif.Minutes}min!");
                }
                else
                {
                    await channel.SendMessageAsync($"Frontline is starting in {timeDif.Minutes}min!");
                }
            }

            Console.WriteLine("Frontline is Inactive");
        }
    }
}
