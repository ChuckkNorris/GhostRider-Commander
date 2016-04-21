using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRider.Domain.Models {
    public class SharePointTask {
        public string Title { get; set; }
        public SharePointUser AssignedTo { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}
