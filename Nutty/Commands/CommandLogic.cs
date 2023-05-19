using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands {
    public class CommandLogic {

        ///
        public static readonly int[] golemPoints = { 4, 6, 8, 8, 16 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="score"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static long Score(long score, long multiplier) {
            float multi = (float)multiplier / 100 + 1;
            long result = (long)(score * multi);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static long GolemTotalScore(long x, long y, long z, long i, long j) {
            long xTotal = x * golemPoints[0];
            long yTotal = y * golemPoints[1];
            long zTotal = z * golemPoints[2];
            long iTotal = i * golemPoints[3];
            long jTotal = j * golemPoints[4];

            long result = xTotal + yTotal + zTotal + iTotal + jTotal;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static long GolemSpecificScore(long x, int id) {
            if(id < 0 | id > golemPoints.Length) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"There is no golem point value for [{id}]");
                Console.ResetColor();

                return -1;
            }

            return x * golemPoints[id];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool ChangeFrontlineTimeslot(int slot) {
            if (slot >= 0 && slot < 5) {
                Program.timeSlot = slot;
                return true;
            }
            else {
                Program.timeSlot = 0;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ToggleFrontlineState() {
            Program.frontlineActive = !Program.frontlineActive;
            Program.config["Frontline_State"] = Program.frontlineActive;

            File.WriteAllText("appSettings.json", Program.config.ToString());
        }
    }
}
