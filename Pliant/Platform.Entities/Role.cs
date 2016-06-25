using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    /// <summary>
    /// Platform Role
    /// </summary>
    public class Role : IEntityBase
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<UserRole> RoleMapping { get; set; }
    }
}
