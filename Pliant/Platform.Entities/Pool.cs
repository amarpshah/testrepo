using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("POOLS")]
    public class Pool : IEntityBase
    {
        public Pool()
        {

        }
        public int ID { get; set; }
        [Column("TEST_ID")]
        public int TestID { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("STATUS")]
        public int Status { get; set; }
        [Column("IS_MANDATORY_TO_PASS")]
        public int IsMandatoryToPass { get; set; }
        [Column("NO_OF_QUESTION_OUT_OF")]
        public int NoOfQuestionsOutOf { get; set; }
        [Column("PASSING_SCORE")]
        public int PassingScore { get; set; }
        [Column("NEGATIVE_MARKS")]
        public int NegativeMarks { get; set; }
        [Column("POOL_TOTAL_MARKS")]
        public int PoolTotalMarks { get; set; }
        [Column("RANDOMIZE_CHOICE")]
        public int RandomizeChoice { get; set; }
        [Column("RANDOMIZE_QUESTION")]
        public int RandomizeQuestion { get; set; }
        [Column("DIFFICULTY_LEVEL")]
        public int DifficultyLevel { get; set; }
        [Column("IS_LOCK")]
        public int IsLock { get; set; }
        [Column("LOCKED_BY")]
        public int LockedBy { get; set; }
        [Column("ON_CREATED")]
        public DateTime OnCreated { get; set; }//TODO: Change ti CreatedOn
        [Column("ON_LOCKED")]
        public DateTime OnLocked { get; set; }
        [Column("ON_UPDATED")]
        public DateTime OnUpdated { get; set; }

        public virtual Test Test { get; set; }
        //public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<PoolQuestionMapping> PoolQuestionMapping { get; set; }

    }
}
