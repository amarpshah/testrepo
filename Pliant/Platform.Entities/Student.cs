using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.Entities
{
    public class Student : IEntityBase
    {
        public int ID
        {
            get;
            set;
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Image { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StandardId { get; set; }
        public virtual Standard Standard { get; set; }
    }
}
