using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Paper : IEntityBase
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public int NoOfSets { get; set; }
        public bool IsFinalized { get; set; }
        public int CreatedBy { get; set; }
        public DateTime OnCreated { get; set; }



    }
}
