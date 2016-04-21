using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRest.Api.Attributes {

    public class RestProperty : Attribute {
        public string PropertyName { get; set; }
    }
    
}
