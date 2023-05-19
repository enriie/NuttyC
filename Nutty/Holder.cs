using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutty.Commands {
    public class Holder {
        private static Holder instance;
        private static readonly object lockObject = new object();

        public readonly IConfiguration channelIds = new ConfigurationBuilder().AddJsonFile("channelIds.json").Build();

        public static Holder Instance {
            get {
                if (instance == null) {
                    lock(lockObject) {
                        if (instance == null) {
                            instance = new Holder();
                        }
                    }
                }
                return instance;
            }
        }

    }
}
