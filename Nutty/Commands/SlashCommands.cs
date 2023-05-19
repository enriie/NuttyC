using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands {
    public class SlashCommands : ApplicationCommandModule {

        ulong debug_id = ulong.Parse(Holder.Instance.channelIds["Debug_Channel"]);
        ulong boss_id = ulong.Parse(Holder.Instance.channelIds["Commands_Channel"]);
        ulong log_id = ulong.Parse(Holder.Instance.channelIds["Log_Channel"]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="score"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        [SlashCommand("score", "Calculates the score for Guild Boss.")]
        public async Task GBRScoreCommand(
            InteractionContext ctx,
            [Option("score", "Score you see during the fight")] long score,
            [Option("multiplier", "Bonus Multiplier you have from selected Challenges")] long multiplier) {
            if (ctx.Channel.Id == debug_id | ctx.Channel.Id == boss_id) {
                await ctx.CreateResponseAsync($"{ctx.Member.DisplayName}, your total score is: {CommandLogic.Score(score, multiplier)}. You had a base score of : {score}, and had the multipler of {multiplier}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <param name="g3"></param>
        /// <param name="g4"></param>
        /// <param name="g5"></param>
        /// <returns></returns>
        [SlashCommand("golem", "Calculates the total score for Golem Elimination")]
        public async Task GolemScoreCommand(
            InteractionContext ctx,
            [Option("Golem1", "Number of 1st golems destroyed")] long g1,
            [Option("Golem2", "Number of 2nd golems destroyed")] long g2,
            [Option("Golem3", "Number of 3rd golems destroyed")] long g3,
            [Option("Golem4", "Number of 4th golems destroyed")] long g4,
            [Option("Golem5", "Number of 5th golems destroyed")] long g5) {

            if (ctx.Channel.Id == debug_id | ctx.Channel.Id == boss_id) {

                await ctx.CreateResponseAsync($"{ctx.Member.DisplayName}, your total score is: {CommandLogic.GolemTotalScore(g1, g2, g3, g4, g5)}. " +
                    $"You scores per golem are : " +
                    $"{g1}:{CommandLogic.GolemSpecificScore(g1, 0)} / " +
                    $"{g2}:{CommandLogic.GolemSpecificScore(g2, 1)} / " +
                    $"{g3}:{CommandLogic.GolemSpecificScore(g3, 2)} / " +
                    $"{g4}:{CommandLogic.GolemSpecificScore(g4, 3)} / " +
                    $"{g5}:{CommandLogic.GolemSpecificScore(g5, 4)}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("frontlineToggle", "Enable/Disable Frontline Notifier")]
        public async Task ToggleFrontlineCommand(InteractionContext ctx) {
            if (Program.isUserAdmin(ctx.Member)) {


                //                var channel = await ctx.Client.GetChannelAsync(ulong.Parse(Holder.Instance.channelIds["Log_Channel"]));
                var channel = await ctx.Client.GetChannelAsync(617136791778230295);
                Program.frontlineActive = !Program.frontlineActive;

                var statusState = Program.frontlineActive ? "active" : "inactive";
                var logState = Program.frontlineActive ? "enabled" : "disabled";

                await Program.UpdateStatus($"Frontline is {statusState}");
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent($"Frontline was {logState}"));
            }
            else {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("You do not have permissions to run this command."));
            }

        }
    }
}
