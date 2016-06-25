using AutoMapper;
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

              //  PermissionSplit(permission);
                List<PermissionViewModel> newPermission = new List<PermissionViewModel>();
                List<string> ActionList = new List<string>();
                List<int> IsPermissionList = new List<int>();
                var count = 4;
                foreach (var p in permission)
                {
                    PermissionViewModel P = new PermissionViewModel();
                    P.ID = p.ID;
                    P.RoleID = p.RoleID;
                    P.FormID = p.FormID;
                    P.sRole = _roleRepository.GetAll().Single(r => r.ID == P.RoleID).Name.ToString();
                    var formName = _formRepository.GetAll().Where(r => r.FormID == P.FormID).ToList();
                    P.sForm = formName[0].Name;
                    
                        ActionList.Add(p.Action);
                        IsPermissionList.Add(p.IsPermission);
                        count--;
                    
                    
                if(count == 0)
                {
                    P.ActionList = new List<string>();
                    //P.ActionList = ActionList;
                    string[] tempAction = new string[4];
                    ActionList.CopyTo(tempAction);
                    int[] tempIsPermission = new int[4];
                    IsPermissionList.CopyTo(tempIsPermission);
                    P.ActionList = tempAction.ToList<string>();
                    P.IsPermissionList = tempIsPermission.ToList<int>();
                    
                    newPermission.Add(P);
                    ActionList.Clear();
                    IsPermissionList.Clear();
                    count = 4;
                }
                }

                IEnumerable<PermissionViewModel> permissionsVM = Mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionViewModel>>(permission);

                
               
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

        private void PermissionSplit(List<Permission> permission)
        {
            List<int> rollList = new List<int>();
            List<int> formList = new List<int>();

            foreach (var p in permission)
            {
                rollList.Add(p.RoleID);
                rollList.Add(p.FormID);
            
            }
        
        }

    }
}
