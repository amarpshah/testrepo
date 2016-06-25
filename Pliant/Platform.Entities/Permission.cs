using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Permission : IEntityBase
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int FormID { get; set; }
        public string Action { get; set; }
        public int IsPermission { get; set; }
       
    }
}
