using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute.Entities
{
    public class Topic : IEntityBase
    {
        public Topic()
        {
        }
        public int ID { get; set; }
        [Column("STANDARD_SUBJECT_MAPPING_ID")]
        public int MappingID { get; set; }
        public virtual StandardSubjectMapping Mapping { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Objective { get; set; }
        [Column("Is_Active")]
        public bool IsActive { get; set; }
        
        public virtual ICollection<Question> Questions { get; set; }
    }
}
