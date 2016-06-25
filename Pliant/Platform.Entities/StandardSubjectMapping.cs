using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute.Entities
{
    [Table("Standard_Subject_Mapping")]
    public class StandardSubjectMapping : IEntityBase
    {
        public StandardSubjectMapping()
        {
        }
        public int ID { get; set; }
        [Column("Standard_Id")]
        public int StandardId { get; set; }
        public virtual Standard Standard { get; set; }
        [Column("Subject_Id")]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        [Column("Is_Active")]
        public bool IsActive { get; set; }
        public virtual ICollection<Topic> Topic { get; set; }
    }
}
