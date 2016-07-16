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
using System.Web.Http;
using System.Web.Http.Cors;

namespace Institute.WebApi.Controllers
{
    //[Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RolesController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Role> _rolesRepository;
        private readonly IEntityBaseRepository<UserRole> _userRolesRepository;
        private readonly IEntityBaseRepository<Form> _formRepository;
        private readonly IEntityBaseRepository<Permission> _permissionRepository;

        public RolesController(IEntityBaseRepository<Role> rolesRepository,
            IEntityBaseRepository<UserRole> userRolesRepository,
            IEntityBaseRepository<Form> formRepository,
            IEntityBaseRepository<Permission> permissionRepository,
             IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _rolesRepository = rolesRepository;
            _userRolesRepository = userRolesRepository;
            _formRepository = formRepository;
            _permissionRepository = permissionRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var standards = _rolesRepository.GetAll().ToList();

                IEnumerable<RoleViewModel> standardsVM = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(standards);

                response = request.CreateResponse<IEnumerable<RoleViewModel>>(HttpStatusCode.OK, standardsVM);

                return response;
            });
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
                List<Role> role = null;
                int totalRoles = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    role = _rolesRepository.FindBy(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalRoles = _rolesRepository.GetAll()
                        .Where(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    role = _rolesRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalRoles = _rolesRepository.GetAll().Count();
                }

                IEnumerable<RoleViewModel> studentsVM = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(role);

                PaginationSet<RoleViewModel> pagedSet = new PaginationSet<RoleViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalRoles,
                    TotalPages = (int)Math.Ceiling((decimal)totalRoles / currentPageSize),
                    Items = studentsVM
                };

                response = request.CreateResponse<PaginationSet<RoleViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string name)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Role> roles = null;
                int totalRoles = new int();

                roles = _rolesRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (name != null ? q.Name.Contains(name) : 1 == 1)
                    //(division != null ? q.Division.Contains(division) : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalRoles = _rolesRepository.GetAll()
                .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                            (name != null ? q.Name.Contains(name) : 1 == 1)
                    //(division != null ? q.Division.Contains(division) : 1 == 1)

                )
                .Count();

                IEnumerable<RoleViewModel> roleVM = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(roles);



                PaginationSet<RoleViewModel> pagedSet = new PaginationSet<RoleViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalRoles,
                    TotalPages = (int)Math.Ceiling((decimal)totalRoles / currentPageSize),
                    Items = roleVM
                };

                response = request.CreateResponse<PaginationSet<RoleViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, RoleViewModel role)
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
                    Role newRole = new Role();

                    newRole.Code = role.Code;
                    newRole.Name = role.Name;


                    var roles = _rolesRepository.FindBy(c => c.Code.ToLower().Contains(newRole.Code.ToLower())).ToList();
                    if (roles != null && roles.Count > 0)
                    {

                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {
                     
                        _rolesRepository.Add(newRole);
                      
                        _unitOfWork.Commit();
                        AddRoleInPermission();
                        // Update view model
                        role = Mapper.Map<Role, RoleViewModel>(newRole);
                        response = request.CreateResponse<RoleViewModel>(HttpStatusCode.Created, role);
                    }
                }

                return response;
            });
        }
        private void AddRoleInPermission()
        {
            List<Form> forms = null;
            Role role = null;
            forms = _formRepository.GetAll().ToList();
            
            role = _rolesRepository.GetAll().OrderByDescending(r => r.ID) //Get Last Record from table
                .First();

                           
           
            foreach (var f in forms)
            {
                Permission permission = new Permission();
                permission.RoleID = role.ID;
                permission.FormID = f.ID;
                permission.Action = f.Action;
                permission.IsPermission = 0;
              
               _permissionRepository.Add(permission);
                
            }

            _unitOfWork.Commit();
       
       
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, RoleViewModel roleVM)
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
                    Role role = new Role();

                    role = _rolesRepository.GetSingle(roleVM.ID);

                    role.Code = roleVM.Code;
                    role.Name = roleVM.Name;
                    //standard.Division = standardVM.Division;


                    _rolesRepository.Edit(role);
                    _unitOfWork.Commit();

                    // Update view model
                    roleVM = Mapper.Map<Role, RoleViewModel>(role);
                    response = request.CreateResponse<RoleViewModel>(HttpStatusCode.Created, roleVM);

                }


                return response;
            });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
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
                    var UserRoles = _userRolesRepository.GetAll()
                        .Where(u => u.RoleId == id);
                    if (UserRoles.Count() > 0)
                    {

                        response = request.CreateResponse<RoleViewModel>(HttpStatusCode.NotAcceptable, new RoleViewModel());
                    }
                    else
                    {

                        Role deleteRole = _rolesRepository.GetSingle(id);
                        _rolesRepository.Delete(deleteRole);

                        _unitOfWork.Commit();

                        // Update view model
                        response = request.CreateResponse<RoleViewModel>(HttpStatusCode.Created, new RoleViewModel());
                    }
                }

                return response;
            });
        }

    }
}
