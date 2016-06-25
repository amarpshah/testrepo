using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Institute.Entities
{
    public class Question : IEntityBase
    {
        public Question()
        {
            Choices = new List<ChoiceAnswer>();
            Matches = new List<MatchingAnswer>();
            Descriptive = new List<DescriptiveAnswer>();
            QuestionPoolMaps = new List<PoolQuestionMapping>();
        }
        [Column("Id")]
        public int ID { get; set; }
        [Column("Topic_Id")]
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        //public virtual Pool Pool { get; set; }
        public int Type { get; set; }
        [Column("Difficulty_Level")]
        public int DifficultyLevel { get; set; }
        public int Points { get; set; }
        public int Status { get; set; }
        public int Randomize { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        [Column("OBJECTIVE")]
        public string Objective { get; set; }
        public virtual ICollection<ChoiceAnswer> Choices { get; set; }
        public virtual ICollection<DescriptiveAnswer> Descriptive { get; set; }
        public virtual ICollection<MatchingAnswer> Matches { get; set; }
        public virtual ICollection<PoolQuestionMapping> QuestionPoolMaps { get; set; }
        [Column("Is_Active")]
        public bool IsActive { get; set; }
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
    }
}
