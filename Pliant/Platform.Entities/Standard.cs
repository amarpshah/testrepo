using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Standard : IEntityBase
    {
        public Standard()
        {
            //SubjectMapping = new List<StandardSubjectMapping>();
        }
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Division { get; set; }
        public virtual ICollection<StandardSubjectMapping> SubjectMapping { get; set; }
        
    }
}
