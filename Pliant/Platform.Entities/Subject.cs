using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Subject : IEntityBase
    {
        public Subject()
        {
            //StandardMapping = new List<StandardSubjectMapping>();
        }
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<StandardSubjectMapping> StandardMapping { get; set; }
        
        
    }
}
