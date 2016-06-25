using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("ANSWER_MATCHING_PAIR")]
    public class MatchingAnswer : IEntityBase
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("QUESTION_ID")]
        public int QuestionId { get; set; }
        [Column("CHOICE_ID")]
        public int ChoiceId { get; set; }
        [Column("CHOICE_A")]
        public string ChoiceA { get; set; }
        [Column("CHOICE_B")]
        public string ChoiceB { get; set; }

        public virtual Question Question { get; set; }
    }
}
