using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class PermissionViewModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int FormID { get; set; }
        public string Action { get; set; }
        public int IsPermission { get; set; }
        public List<string> ActionList;
        public List<int> IsPermissionList;
        public string sRole { get; set; }
        public string sForm { get; set; }
    }

    public class FormViewModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int FormID { get; set; }
    
    
    }
    public class FormPermissionViewModel
    {
        public int ID { get; set; }
        public int Add { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
        public int View { get; set; }
        public int Print { get; set; }

        public bool AddViewable { get; set; }
        public bool UpdateViewable { get; set; }
        public bool DeleteViewable { get; set; }
        public bool ViewViewable { get; set; }
        public bool PrintViewable { get; set; }
        

    }
}