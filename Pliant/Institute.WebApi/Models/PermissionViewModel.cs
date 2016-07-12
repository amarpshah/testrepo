using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class PermissionViewModel
    {
        public PermissionViewModel()
        {
        APVMList = new List<ActionPermissionViewModel>();
        
        }
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int FormID { get; set; }
        public string FormName { get; set; }
        public string Action { get; set; }
        public int IsPermission { get; set; }
        public string sRole { get; set; }
        public string sForm { get; set; }
        public List<ActionPermissionViewModel> APVMList { get; set; }
        
    }

    public class ActionPermissionViewModel
    {
        
        public int Permission { get; set; }
        public string Action { get; set; }
    
    
    }

    public class UpdatePermissionViewModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public int FormID { get; set; }
        public string Action { get; set; }
        public int IsPermission { get; set; }
    }


    public class UserFormActionPermissionViewModels
    {
        public string FormName { get; set; }
        public string Action { get; set; }
        public int IsPermission { get; set; }
    
    }
 
}