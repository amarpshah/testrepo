using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("FINAL_ANSWER_DESCRIPTIVE")]
    public class FinalDescriptiveAnswer : IEntityBase
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("QUESTION_ID")]
        public int QuestionId { get; set; }
        [Column("KEYWORDS")]
        public string Keywords { get; set; }
        [Column("POSITIVE_POINTS")]
        public int PositivePoints { get; set; }
        [Column("NEGATIVE_POINTS")]
        public int NegativePoints { get; set; }
        [Column("IS_ANSWER")]
        public int IsAnswer { get; set; }
        [Column("PAPER_ID")]
        public int PaperID { get; set; }
        [Column("TEST_QUESTION_ID")]
        public int TestQuestionID { get; set; }

        //[Key, ForeignKey("TestQuestion")]
        public virtual TestQuestion TestQuestion { get; set; }


    }
}
