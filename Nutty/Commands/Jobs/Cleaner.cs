using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands.Jobs {
    public class Cleaner : IJob {

        public async Task Execute(IJobExecutionContext context) {
            if (Program.frontlineActive) {
                var channel = await Program.discord.GetChannelAsync(ulong.Parse(Holder.Instance.channelIds["Frontline_Channel"]));

                var msgHistory = await channel.GetMessagesAsync();

                var messages = msgHistory.Where(m => m.Author.Id == 1068944385867251712).ToList(); ;

                channel.DeleteMessagesAsync(messages);
            }
        }
    }
}
