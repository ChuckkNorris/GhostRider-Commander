using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRider.Domain.Models {
    public class AddTaskRequest {
        public string TaskName { get; set; }
        public string UserFirstName { get; set; }
    }
}
