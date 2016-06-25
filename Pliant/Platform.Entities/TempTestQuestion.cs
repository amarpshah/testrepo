using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class TempTestQuestion : IEntityBase
    {

        public TempTestQuestion()
        {
            Choices = new List<TempChoiceAnswer>();
            Matches = new List<TempMatchingAnswer>();
            Descriptive = new List<TempDescriptiveAnswer>();
            
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
        
        public virtual ICollection<TempChoiceAnswer> Choices { get; set; }
        public virtual ICollection<TempDescriptiveAnswer> Descriptive { get; set; }
        public virtual ICollection<TempMatchingAnswer> Matches { get; set; }
    }
}
