using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semicolon.OnlineJudge.Models.Problemset
{
    public class PassRate
    {
        public long Submit { get; set; }

        public long Pass { get; set; }

        public double GetPassRate()
        {
            return Pass / Submit;
        }

        public override string ToString()
        {
            if (Submit == 0)
            {
                return "0/0 - 0%";
            }

            return Pass + "/" + Submit + " - " + GetPassRate() * 100 + "%";
        }
    }
}
