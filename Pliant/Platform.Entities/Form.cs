using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Form : IEntityBase
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public virtual ICollection<Permission> Permission { get; set; }
    }
}
