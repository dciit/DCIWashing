using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCIWashing.Props
{
    internal class PropTransaction
    {
        public string time { get; set; } = "";
        public string partno { get; set; } = "";
        public string cm { get; set; } = "";
        public string type { get; set; } = "";
        public decimal qty { get; set; } = 0;
        public string createBy { get; set; } = "";
    }
}
