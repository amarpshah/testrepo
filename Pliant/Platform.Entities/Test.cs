using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("TESTS")]
    public class Test : IEntityBase
    {
        public Test()
        {

        }
        public int ID { get; set; }
        [Column("CODE")]
        public string Code { get; set; }
        [Column("TEXT")]
        public string Text { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("OBJECTIVE")]
        public string Objective { get; set; }
        [Column("STATUS")]
        public int Status { get; set; }
        [Column("NEGATIVE_MARKS")]
        public int NegativeMarks { get; set; }
        [Column("POINTS_PER_QUESTION")]
        public int PointsPerQuestion { get; set; }
        [Column("TOTAL_MARKS")]
        public int TotalMarks { get; set; }
        [Column("PASSING_MARKS")]
        public int PassingMarks { get; set; }
        [Column("SCORING_MODE")]
        public int ScoringMode { get; set; }
        [Column("DIFFICULTY_LEVEL")]
        public int DifficultyLevel { get; set; }
        [Column("POOL_SEQUENCE")]
        public int PoolSequence { get; set; }
        [Column("SHOW_HINT")]
        public int ShowHint { get; set; }
        [Column("IS_LOCK")]
        public int Lock { get; set; }
        [Column("LOCKED_BY")]
        public int LockedBy { get; set; }
        [Column("ON_LOCKED")]
        public DateTime OnLocked { get; set; }
        [Column("ON_CREATED")]
        public DateTime OnCreated { get; set; }
        [Column("ON_UPDATED")]
        public DateTime OnUpdated { get; set; }
        public virtual ICollection<Pool> Pools { get; set; }
      
    }
}
