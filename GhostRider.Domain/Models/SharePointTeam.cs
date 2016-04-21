using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRider.Domain.Models {
    public class SharePointTeam {
        public string Title { get; set; }
    //    public IEnumerable<SharePointUser> TeamMembers { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
