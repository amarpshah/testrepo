﻿using AutoMapper;
using Institute.BizComponent.Infrastructure;
using Institute.BizComponent.Repositories;
using Institute.Entities;
using Institute.WebApi.Infrastructure.Core;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Institute.WebApi.Controllers;

namespace Institute.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/permissions")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PermissionController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Permission> _permissionRepository;
        private readonly IEntityBaseRepository<Role> _roleRepository;
        private readonly IEntityBaseRepository<Form> _formRepository;
            

        public PermissionController(IEntityBaseRepository<Permission> permissionRepository,
            IEntityBaseRepository<Role> roleRepository,
            IEntityBaseRepository<Form> formRepository,
             IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _formRepository = formRepository;

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Permission> permission = null;
                int totalPermission = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    permission = _permissionRepository.FindBy(c => c.Action.ToLower().Contains(filter))
                                 .OrderBy(c => c.ID)
                                .Skip(currentPage * currentPageSize)
                                .Take(currentPageSize)
                                .ToList();

                    totalPermission = _permissionRepository.GetAll()
                        .Where(c => c.Action.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    permission = _permissionRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalPermission = _permissionRepository.GetAll().Count();
                }
        
             

                List<PermissionViewModel> newPermission =  PermissionSplit(permission);


             
             //   IEnumerable<PermissionViewModel> permissionsVM = Mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionViewModel>>(permission);

                
               
                PaginationSet<PermissionViewModel> pagedSet = new PaginationSet<PermissionViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalPermission,
                    TotalPages = (int)Math.Ceiling((decimal)totalPermission / currentPageSize),
                    Items = newPermission
                };

                response = request.CreateResponse<PaginationSet<PermissionViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        private List<PermissionViewModel> PermissionSplit(List<Permission> permission)
        {
            List<PermissionViewModel> newPermission = new List<PermissionViewModel>();
            List<string> ActionList = new List<string>();
            List<int> IsPermissionList = new List<int>();

            var tempRoleID = 0;
            var tempFormID = 0;


            var temp1RoleID = 0;
            var temp1FormID = 0;

            List<ActionPermissionViewModel> tempAPVMList = new List<ActionPermissionViewModel>();
            foreach (var p in permission)
            {


                PermissionViewModel P = new PermissionViewModel();
                P.ID = p.ID;
                //P.RoleID = p.RoleID;
                //P.FormID = p.FormID;


                if (tempRoleID == 0 || tempRoleID == p.RoleID && tempFormID == 0 || tempFormID == p.Form.FormID)
                {
                    ActionPermissionViewModel APVM = new ActionPermissionViewModel();

                    APVM.Action = p.Action;
                    APVM.Permission = p.IsPermission;


                    P.RoleID = p.RoleID;
                    P.FormID = p.Form.FormID;

                    tempAPVMList.Add(APVM);
                    P.APVMList = tempAPVMList.ToList();
                    P.sRole = _roleRepository.GetAll().Single(r => r.ID == P.RoleID).Name.ToString();
                    var formName = _formRepository.GetAll().Where(r => r.FormID == P.FormID).ToList();
                    P.sForm = formName[0].Name;
                    if (newPermission.Count() == 0 || p.RoleID != temp1RoleID || p.Form.FormID != temp1FormID)
                    {
                        newPermission.Add(P);
                    }
                    else
                    {
                        newPermission.RemoveAt(newPermission.Count - 1);
                        newPermission.Add(P);
                    }
                    tempRoleID = p.RoleID;
                    tempFormID = p.Form.FormID;

                    temp1RoleID = p.RoleID;
                    temp1FormID = p.Form.FormID;
                }
                else
                {




                    tempAPVMList.Clear();
                    ActionPermissionViewModel APVM = new ActionPermissionViewModel();

                    APVM.Action = p.Action;
                    APVM.Permission = p.IsPermission;

                    tempAPVMList.Add(APVM);

                    tempRoleID = p.RoleID;
                    tempFormID = p.Form.FormID;


                }
            }

            return newPermission;

        }



           
        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, List<UpdatePermissionViewModel> permissionVM)
          {
            return CreateHttpResponse(request, () =>
         {
             HttpResponseMessage response = null;
             if (!ModelState.IsValid)
             {
                 response = request.CreateResponse(HttpStatusCode.BadRequest,
                     ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                           .Select(m => m.ErrorMessage).ToArray());
             }
             else
             {
                
                
                 foreach (var p in permissionVM)
                 {
                     Permission permission = new Permission();
                     var permissions = _permissionRepository.FindBy(
                         pp => pp.Form.FormID == p.FormID && pp.RoleID == p.RoleID && pp.Action == p.Action);
                     foreach (var t in permissions)
                     {

                         permission = t;
                     }
                     permission.IsPermission = p.IsPermission;
                     _permissionRepository.Edit(permission);
                 }

                                 
                 _unitOfWork.Commit();
                 response = request.CreateResponse(HttpStatusCode.OK);
             }
             return response;
         });
        }
        
        
          //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, int? roleid, int? formid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Permission> permissions = null;
                int totalPermissions = new int();

                permissions = _permissionRepository.GetAll()
                    .Where(q => (roleid != -1 ? q.RoleID == roleid : 1 == 1) &&
                                (formid != -1 ? q.Form.FormID == formid : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalPermissions = _permissionRepository.GetAll()
                       .Where(q => (roleid != -1 ? q.RoleID == roleid : 1 == 1) &&
                                   (formid != -1 ? q.Form.FormID == formid : 1 == 1)

                        )
                        .Count();

                List<PermissionViewModel> newPermission = PermissionSplit(permissions);
              //  IEnumerable<PermissionViewModel> permissionsVM = Mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionViewModel>>(permissions);


                PaginationSet<PermissionViewModel> pagedSet = new PaginationSet<PermissionViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalPermissions,
                    TotalPages = (int)Math.Ceiling((decimal)totalPermissions / currentPageSize),
                    Items = newPermission
                };

                response = request.CreateResponse<PaginationSet<PermissionViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("forms")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<FormViewModel> formVM = new List<FormViewModel>();
                var forms = _formRepository.GetAll().ToList();

                int tempFormID = 0;
                foreach (var f in forms)
                {
                    FormViewModel form = new FormViewModel();
                    form.FormID = f.FormID;
                    form.Name = f.Name;

                    if (tempFormID == 0 || tempFormID != form.FormID)
                    {
                        formVM.Add(form);
                        tempFormID = form.FormID;
                    }
                }

              

          //      IEnumerable<FormViewModel> formVM = Mapper.Map<IEnumerable<Form>, IEnumerable<FormViewModel>>(forms);

                 response = request.CreateResponse<IEnumerable<FormViewModel>>(HttpStatusCode.OK, formVM);

                return response;
            });
        }

        [AllowAnonymous]
        [Route("getpermissions")]
        public HttpResponseMessage GetPermissions(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<UserFormActionPermissionViewModels> UFVMList = new List<UserFormActionPermissionViewModels>();
                var permissions = _permissionRepository.GetAll().ToList();
                //var permissions = _permissionRepository.GetAll().Where(x => x.RoleID == id).ToList();
             
            
                foreach (var p in permissions)
                {
                    UserFormActionPermissionViewModels UFVM = new UserFormActionPermissionViewModels();
                    UFVM.FormName = p.Form.Name;
                    UFVM.Action = p.Action;
                    UFVM.IsPermission = p.IsPermission;
                    UFVMList.Add(UFVM);
                                    
                }
                
                
                
               // IEnumerable<PermissionViewModel> PermissionVM = Mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionViewModel>>(permissions);

                response = request.CreateResponse<IEnumerable<UserFormActionPermissionViewModels>>(HttpStatusCode.OK, UFVMList);

                return response;
            });
        }
    }
}
