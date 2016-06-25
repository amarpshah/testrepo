using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class TestQuestion : IEntityBase
    {
        public TestQuestion()
        {
            Choices = new List<FinalChoiceAnswer>();
            Matches = new List<FinalMatchingAnswer>();
            Descriptive = new List<FinalDescriptiveAnswer>();
            
        }
        
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public int PoolID { get; set; }
        public string PoolName { get; set; }
        public int QuestionID { get; set; }
        public int QuestionType { get; set; }
        public string QuestionText { get; set; }
        public int PaperID { get; set; }
        public int TestSetNo { get; set; }
        public int SequenceNo { get; set; }

        public virtual ICollection<FinalChoiceAnswer> Choices { get; set; }
        public virtual ICollection<FinalDescriptiveAnswer> Descriptive { get; set; }
        public virtual ICollection<FinalMatchingAnswer> Matches { get; set; }
    }
}
