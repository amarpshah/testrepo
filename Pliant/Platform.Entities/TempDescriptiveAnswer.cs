using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("TEMP_ANSWER_DESCRIPTIVE")]
    public class TempDescriptiveAnswer : IEntityBase
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
        [Column("TEMP_TEST_QUESTION_ID")]
        public int TempTestQuestionID { get; set; }

       
        //[Key, ForeignKey("TempTestQuestion")]
        public virtual TempTestQuestion TempQuestion { get; set; }
     
       
    }
}
