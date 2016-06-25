using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("POOL_QUESTION_MAPPING")]
    public class PoolQuestionMapping : IEntityBase
    {
        public PoolQuestionMapping()
        {

        }
        public int ID { get; set; }
        [Column("POOL_ID")]
        public int PoolId { get; set; }
        [Column("QUESTION_ID")]
        public int QuestionId { get; set; }
        [Column("IS_MANDATORY")]
        public bool IsMandatory { get; set; }

        public virtual Pool Pool { get; set; }
        public virtual Question Question { get; set; }
    }
}
