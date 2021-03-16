using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi.Models.Apis {
    public class ApiPlayerMove {
        public int X { get; set; }
        public int Y { get; set; }
        public string PlayerToken { get; set; }
        public string GameToken { get; set; }
    }
}
