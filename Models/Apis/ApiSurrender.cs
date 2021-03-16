using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi.Models.Apis {
    public class ApiSurrender {
        public string GameToken { get; set; }
        public string PlayerToken { get; set; }
    }
}
