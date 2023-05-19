using Nutty.Commands;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty {
    public class FrotnlineNotifier : IJob {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context) {
            Console.WriteLine("Triggered");

            // Checks if Frontline is Currently Active
            if (Program.frontlineActive) {

                // Get the Frontline Channel
                var channel = await Program.discord.GetChannelAsync(ulong.Parse(Holder.Instance.channelIds["Frontlne_Channel"]));

                // Calculate Frontline Starting Time
                int hour = 4 * Program.timeSlot;

                // Get DateTime data used for Time Difference calculation
                DateTime now = DateTime.UtcNow;
                DateTime frontline = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hour, 0, 0, DateTimeKind.Utc);

                // Get time left until Frontline
                TimeSpan timeDif = frontline.Subtract(now);

                // Send a message in Frontline Channel informing uses of time until Frontline.
                await channel.SendMessageAsync($"Frontline is Starting in {timeDif.TotalMinutes}!");
            }
        }
    }
}
