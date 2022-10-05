using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotData.Entity
{
    public class BotUser
    {
        public long Id { get; set; }
        public int Score { get; set; }
        public int? LastBet { get; set; }
    }
}
