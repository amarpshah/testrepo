using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("TEMP_ANSWER_CHOICE_BASED")]
    public class TempChoiceAnswer:IEntityBase
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("QUESTION_ID")]
        public int QuestionId { get; set; }
        [Column("CHOICE_ID")]
        public int ChoiceId { get; set; }
        [Column("TEXT")]
        public string Text { get; set; }
        [Column("POINTS_PER_CHOICE")]
        public int PointsPerChoice { get; set; }
        [Column("DISPLAY_TYPE")]
        public int DisplayType { get; set; }
        [Column("IS_ANSWER")]
        public bool IsAnswer { get; set; }
        [Column("PAPER_ID")]
        public int PaperID { get; set; }
        [Column("TEMP_TEST_QUESTION_ID")]
        public int TempTestQuestionID { get; set; }

        public virtual TempTestQuestion TempQuestion { get; set; }
    }
}
