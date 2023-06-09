﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands {
    public class TraditionalCommands : BaseCommandModule {


        ulong debug_id = ulong.Parse(Holder.Instance.channelIds["Debug_Channel"]);
        ulong boss_id = ulong.Parse(Holder.Instance.channelIds["Commands_Channel"]);
        ulong log_id = ulong.Parse(Holder.Instance.channelIds["Log_Channel"]);
        ulong frontline_id = ulong.Parse(Holder.Instance.channelIds["Frontline_Channel"]);

        /// <summary>
        /// Traditional Command using Command Prefix that calculates Score for Guild Boss Rush.
        /// </summary>
        /// <param name="ctx">Command Context from Discord Message</param>
        /// <param name="score">Base score of GBR during battle</param>
        /// <param name="multiplier">Multiplier from selected Challenges.</param>
        /// <returns></returns>
        [Command("score")]
        public async Task Score(CommandContext ctx, int score, int multiplier) {
            await ctx.Message.DeleteAsync();
            if (ctx.Channel.Id == boss_id | ctx.Channel.Id == debug_id) {
                await ctx.RespondAsync($"{ctx.Member.DisplayName}, your total score is: {CommandLogic.Score(score, multiplier)}. You had a base score of : {score}, and had the multipler of {multiplier}.");
            }
            else {
                await ctx.RespondAsync("This is not the appropriate channel for this command.");
            }
        }

        /// <summary>
        /// Traditional command using Command Prefix that calculates Score for Golem Elimination.
        /// </summary>
        /// <param name="ctx">Command Context from Discord Message</param>
        /// <param name="g1">Amount of 1st Golems destroyed</param>
        /// <param name="g2">Amount of 2nd Golems destroyed</param>
        /// <param name="g3">Amount of 3rd Golems destroyed</param>
        /// <param name="g4">Amount of 4th Golems destroyed</param>
        /// <param name="g5">Amount of 5th Golems destroyed</param>
        /// <returns></returns>
        [Command("golem")]
        public async Task GolemScore(CommandContext ctx, int g1, int g2 = 0, int g3 = 0, int g4 = 0, int g5 = 0) {
            await ctx.Message.DeleteAsync();
            if (ctx.Channel.Id == boss_id | ctx.Channel.Id == debug_id) {
                await ctx.RespondAsync($"{ctx.Member.DisplayName}, your total score is: {CommandLogic.GolemTotalScore(g1, g2, g3, g4, g5)}. " +
                    $"You scores per golem are : " +
                    $"{g1}:{CommandLogic.GolemSpecificScore(g1, 0)} / " +
                    $"{g2}:{CommandLogic.GolemSpecificScore(g2, 1)} / " +
                    $"{g3}:{CommandLogic.GolemSpecificScore(g3, 2)} / " +
                    $"{g4}:{CommandLogic.GolemSpecificScore(g4, 3)} / " +
                    $"{g5}:{CommandLogic.GolemSpecificScore(g5, 4)}");
            }
            else {
                await ctx.RespondAsync("This is not the appropriate channel for this command.");
            }
        }

        /// <summary>
        /// Traditional Command with Command Prefix used for Toggling Frontline Status.
        /// Only useable by Members with Administrative Permissions.
        /// </summary>
        /// <param name="ctx">Command Context from Discord Message</param>
        /// <returns></returns>
        [Command("frontlineToggle")]
        public async Task FrontlineToggle(CommandContext ctx) {
            await ctx.Message.DeleteAsync();
            if (Program.isUserAdmin(ctx.Member)) {
                CommandLogic.ToggleFrontlineState();

                var state = Program.frontlineActive ? "active" : "inactive";
                await Program.UpdateStatus($"Frontine is {state}");
            }
        }

        /// <summary>
        /// Traditional Command with Command Prefix used for changing current Frontlien Timezone.
        /// Only usable by Members with Administrative Permissions.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        [Command("frontlineTimeslot")]
        public async Task FrontlineTimeslot(CommandContext ctx, int slot) {
            await ctx.Message.DeleteAsync();
            if (Program.isUserAdmin(ctx.Member)) {
                var result = CommandLogic.ChangeFrontlineTimeslot(slot);

                if (result) {
                    await ctx.RespondAsync($"You have successfully changed frontline timeslot to {Program.timeSlot}.");
                }
                else {
                    await ctx.RespondAsync($"Timeslot {slot} does not exist. Time slots available are 1, 2, 3 & 4.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("frontlineInfo")]
        public async Task FrontlineInfo(CommandContext ctx) {
            await ctx.Message.DeleteAsync();
            await ctx.RespondAsync($"Frontline Info: \nFrontline Status:{Program.frontlineActive}\nFrontline Timeslot:{Program.timeSlot}");
        }

        /// <summary>
        /// Store plan for Frontline.
        /// </summary>
        /// <param name="ctx"> Discord Command Context </param>
        /// <param name="plan">  </param>
        /// <returns></returns>
        [Command("sPlan")]
        public async Task StorePlan(CommandContext ctx, params string[] args) {
            await ctx.Message.DeleteAsync();
            if (Program.isUserAdmin(ctx.Member)) {
                File.WriteAllText(@"plan.txt", string.Join("", args));

                var response = await ctx.RespondAsync("Plan was stored");

                await Task.Delay(10000);
                await response.DeleteAsync();
            }
        }

        /// <summary>
        /// Clear the plan for Frontline.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("cPlan")]
        public async Task ClearPlan(CommandContext ctx) {
            await ctx.Message.DeleteAsync();
            if (Program.isUserAdmin(ctx.Member)) {
                File.WriteAllText(@"plain.txt", "");
            }
        }

        /// <summary>
        /// Show the plan for frontlien in #Frontlien channel.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("plan")]
        public async Task ShowPlan(CommandContext ctx) {
            await ctx.Message.DeleteAsync();
            if (ctx.Channel.Id != frontline_id | ctx.Channel.Id != debug_id) {
                var response = await ctx.RespondAsync($"Wrong Channel");
                await Task.Delay(5000);
                await response.DeleteAsync();

                return;
          }

            var planText = "";
            planText = File.ReadAllText(@"plan.txt");
            if (planText.Length > 0) {
                ctx.RespondAsync(planText);
            }
        }

        // --- Seperator --- This shouldn't be done this way lol


        [Command("helpMe")]
        public async Task Help(CommandContext ctx) {
            await ctx.Message.DeleteAsync();
            await ctx.RespondAsync(Program.helpEmbedBuilder.Build());
        }

    }
}
