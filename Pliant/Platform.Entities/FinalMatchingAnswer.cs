﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    [Table("FINAL_ANSWER_MATCHING_PAIR")]
    public class FinalMatchingAnswer : IEntityBase
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
        [Column("DISPLAY_B")]
        public string DisplayB { get; set; }
        [Column("PAPER_ID")]
        public int PaperID { get; set; }
        [Column("TEST_QUESTION_ID")]
        public int TestQuestionID { get; set; }

        public virtual TestQuestion TestQuestion { get; set; }
    }
}