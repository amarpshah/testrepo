using Institute.WebApi.Infrastructure.Core;
using Institute.WebApi.Models;
using Institute.BizComponent.Infrastructure;
using Institute.BizComponent.Repositories;
using Institute.Entities;
using Institute.Services;
using Institute.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;

namespace Institute.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/Account")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : ApiControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IEntityBaseRepository<User> _usersRepository;

        public AccountController(IMembershipService membershipService, IEntityBaseRepository<User> usersRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _membershipService = membershipService;
            _usersRepository = usersRepository;
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public HttpResponseMessage Login(HttpRequestMessage request, LoginViewModel user)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (ModelState.IsValid)
                {
                    MembershipContext _userContext = _membershipService.ValidateUser(user.Username, user.Password);

                    if (_userContext.User != null)
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true, userid = _userContext.User.ID });
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = false, userid = -1 });
                    }
                }
                else
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = false, userid = -1 });

                return response;
            });
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public HttpResponseMessage Register(HttpRequestMessage request, RegistrationViewModel user)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, userid = -1 });
                }
                else
                {
                    Institute.Entities.User _user = _membershipService.CreateUser(user.Username, user.Email, user.Password, new int[] { user.RoleID });

                    if (_user != null)
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true, userid = _user.ID });
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = false, userid = -1 });
                    }
                }

                return response;
            });
        }



        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, RegistrationViewModel regVM)
        {

            return CreateHttpResponse(request, () =>
               {
                   HttpResponseMessage response = null;

                  
                   if (!ModelState.IsValid)
                   {
                       response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, userid = -1 });
                   }
                   else
                   {
                       Institute.Entities.User _user = _membershipService.UpdateUser(regVM.ID, regVM.Username, regVM.Email, regVM.Password, new int[] { regVM.RoleID });

                       if (_user != null)
                       {
                           response = request.CreateResponse(HttpStatusCode.OK, new { success = true, userid = _user.ID });
                       }
                       else
                       {
                           response = request.CreateResponse(HttpStatusCode.OK, new { success = false, userid = -1 });
                       }
                   }

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
                List<User> user = null;
                int totalUsers = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    user = _usersRepository.FindBy(c => c.Username.ToLower().Contains(filter) ||
                            c.Email.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalUsers = _usersRepository.GetAll()
                        .Where(c => c.Username.ToLower().Contains(filter) ||
                            c.Email.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    user = _usersRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalUsers = _usersRepository.GetAll().Count();
                }
                List<int> roleList = new List<int>();
                foreach (var u in user)
                {
                    roleList.Add(u.UserRoles.ElementAt(0).RoleId);

                }
                IEnumerable<RegistrationViewModel> studentsVM = Mapper.Map<IEnumerable<User>, IEnumerable<RegistrationViewModel>>(user);
                int i = 0;
                foreach (var u in studentsVM)
                {
                    u.sRole = user.ElementAt(i).UserRoles.ElementAt(0).Role.Name.ToString();
                    u.RoleID = roleList[i];
                    i++;
                }

             

                PaginationSet<RegistrationViewModel> pagedSet = new PaginationSet<RegistrationViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalUsers,
                    TotalPages = (int)Math.Ceiling((decimal)totalUsers / currentPageSize),
                    Items = studentsVM
                };

                response = request.CreateResponse<PaginationSet<RegistrationViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, int? roleid, string username, string email)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<User> users = null;
                int totalusers = new int();

                users = _usersRepository.GetAll()
                    .Where(q => (username != null ? q.Username.Contains(username) : 1 == 1) &&
                                (roleid != -1 ? q.UserRoles.Any(x => x.RoleId == roleid) : 1 == 1) &&
                                (email != null ? q.Username.Contains(email) : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalusers = _usersRepository.GetAll()
                                  .Where(q => (username != null ? q.Username.Contains(username) : 1 == 1) &&
                                              (roleid != -1 ? q.UserRoles.Any(x => x.RoleId == roleid) : 1 == 1) &&
                                              (email != null ? q.Username.Contains(email) : 1 == 1)

                      )
                      .Count();

                List<int> roleList = new List<int>();
                foreach (var u in users)
                {
                    roleList.Add(u.UserRoles.ElementAt(0).RoleId);

                }
                IEnumerable<RegistrationViewModel> usersVM = Mapper.Map<IEnumerable<User>, IEnumerable<RegistrationViewModel>>(users);
                int i = 0;
                foreach (var u in usersVM)
                {
                    u.sRole = users.ElementAt(i).UserRoles.ElementAt(0).Role.Name.ToString();
                    u.RoleID = roleList[i];
                    i++;
                }

               
                PaginationSet<RegistrationViewModel> pagedSet = new PaginationSet<RegistrationViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalusers,
                    TotalPages = (int)Math.Ceiling((decimal)totalusers / currentPageSize),
                    Items = usersVM
                };

                response = request.CreateResponse<PaginationSet<RegistrationViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


    }
}
