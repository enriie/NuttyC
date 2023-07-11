using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands.Jobs {
    public class Cleaner : IJob {

        public async Task Execute(IJobExecutionContext context) {
            Console.WriteLine("Cleaner Triggered");
            if (Program.frontlineActive) {
                var channel = await Program.discord.GetChannelAsync(ulong.Parse(Holder.Instance.channelIds["Frontline_Channel"]));

                var msgHistory = await channel.GetMessagesAsync(20);

                var filteredMessages = msgHistory.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14 & x.Author.IsBot);
                var count = filteredMessages.Count();

                if (count > 0) {
                await channel.DeleteMessagesAsync(filteredMessages);
                Console.WriteLine($"Removed {count} messages.");
                }
            }
            else {
                Console.WriteLine("Frontline is not active");
            }
        }
    }
}
