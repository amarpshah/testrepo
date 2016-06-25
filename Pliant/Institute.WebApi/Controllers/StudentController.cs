using Institute.BizComponent.Infrastructure;
using Institute.BizComponent.Repositories;
using Institute.Entities;
using Institute.WebApi.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Institute.WebApi.Models;
using System.IO;
using System.Web;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Http.Cors;

namespace Institute.WebApi.Controllers
{

    [RoutePrefix("api/students")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StudentsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Student> _studentsRepository;

        public StudentsController(IEntityBaseRepository<Student> studentsRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _studentsRepository = studentsRepository;
        }
/*
        [AllowAnonymous]
        [Route("{page:int=0}/{pageSize=3}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, string filter)
        {
            filter = filter.ToLower().Trim();

            return CreateHttpResponse(request, () =>
                {
                    HttpResponseMessage response = null;

                    var students = _studentsRepository.GetAll().Where(s => s.Firstname.ToLower().Contains(filter) ||
                        s.Lastname.ToLower().Contains(filter) 
                        ).ToList();

                    var studentvm = Mapper.Map<IEnumerable<Student>, IEnumerable<StudentViewModel>>(students);

                    response = request.CreateResponse<IEnumerable<StudentViewModel>>(HttpStatusCode.OK, studentvm);
                    return response;
                });
        }
*/
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
                List<Student> students = null;
                int totalCustomers = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    students = _studentsRepository.FindBy(c => c.Lastname.ToLower().Contains(filter) ||
                            c.Firstname.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalCustomers = _studentsRepository.GetAll()
                        .Where(c => c.Lastname.ToLower().Contains(filter) ||
                            c.Firstname.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    students = _studentsRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalCustomers = _studentsRepository.GetAll().Count();
                }

                IEnumerable<StudentViewModel> studentsVM = Mapper.Map<IEnumerable<Student>, IEnumerable<StudentViewModel>>(students);

                PaginationSet<StudentViewModel> pagedSet = new PaginationSet<StudentViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalCustomers,
                    TotalPages = (int)Math.Ceiling((decimal)totalCustomers / currentPageSize),
                    Items = studentsVM
                };

                response = request.CreateResponse<PaginationSet<StudentViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var student = _studentsRepository.GetSingle(id);

                StudentViewModel studentVM = Mapper.Map<Student, StudentViewModel>(student);

                response = request.CreateResponse<StudentViewModel>(HttpStatusCode.OK, studentVM);

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, StudentViewModel student)
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
                    Student _student = _studentsRepository.GetSingle(student.ID);
                    //_student.UpdateStudent(student);
                    _student.Firstname = student.Firstname;
                    _student.Lastname = student.Lastname;
                    _student.DateOfBirth = student.DateOfBirth;
                    _student.StandardId = student.StandardId;

                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }


        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(HttpRequestMessage request, StudentViewModel student)
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
                    //if (_studentsRepository.StudentExists(student.Firstname, student.Lastname))
                    //{
                    //    ModelState.AddModelError("Invalid user", "Email or Identity Card number already exists");
                    //    response = request.CreateResponse(HttpStatusCode.BadRequest,
                    //    ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                    //          .Select(m => m.ErrorMessage).ToArray());
                    //}
                    //else
                    {
                        Student newStudent = new Student();
                        //newStudent.Update(student);
                        newStudent.Firstname = student.Firstname;
                        newStudent.Lastname = student.Lastname;
                        newStudent.DateOfBirth = student.DateOfBirth;
                        newStudent.StandardId = student.StandardId;

                        _studentsRepository.Add(newStudent);

                        _unitOfWork.Commit();

                        // Update view model
                        student = Mapper.Map<Student, StudentViewModel>(newStudent);
                        response = request.CreateResponse<StudentViewModel>(HttpStatusCode.Created, student);
                    }
                }

                return response;
            });
        }


        [MimeMultipart]
        [Route("images/upload")]
        public HttpResponseMessage Post(HttpRequestMessage request, int studentId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var studentOld = _studentsRepository.GetSingle(studentId);
                if (studentOld == null)
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid student.");
                else
                {
                    var uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/students");

                    var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);

                    // Read the MIME multipart asynchronously 
                    Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                    string _localFileName = multipartFormDataStreamProvider
                        .FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

                    // Create response
                    FileUploadResult fileUploadResult = new FileUploadResult
                    {
                        LocalFilePath = _localFileName,

                        FileName = Path.GetFileName(_localFileName),

                        FileLength = new FileInfo(_localFileName).Length
                    };

                    // update database
                    studentOld.Image = fileUploadResult.FileName;
                    _studentsRepository.Edit(studentOld);
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, fileUploadResult);
                }

                return response;
            });
        }


        [Route("files/upload")]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                //var studentOld = _studentsRepository.GetSingle(studentId);
                //if (studentOld == null)
                //    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid student.");
                //else
                {
                    var uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/students");

                    var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);

                    // Read the MIME multipart asynchronously 
                    Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                    string _localFileName = multipartFormDataStreamProvider
                        .FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

                    // Create response
                    FileUploadResult fileUploadResult = new FileUploadResult
                    {
                        LocalFilePath = _localFileName,

                        FileName = Path.GetFileName(_localFileName),

                        FileLength = new FileInfo(_localFileName).Length
                    };

                    // update database
                    //studentOld.Image = fileUploadResult.FileName;
                    //_studentsRepository.Edit(studentOld);
                    //_unitOfWork.Commit();

                    /////////////
                    SqlBulkCopy oSqlBulk = null;

                    
                    OleDbConnection myExcelConn = new OleDbConnection
                    ("Provider=Microsoft.ACE.OLEDB.12.0; " +
                        "Data Source=" + HttpContext.Current.Server.MapPath(".") + "\\" + uploadPath + //FileUpload.FileName +
                        ";Extended Properties=Excel 12.0;");
                    try
                    {
                        myExcelConn.Open();

                        // GET DATA FROM EXCEL SHEET.
                        OleDbCommand objOleDB =
                            new OleDbCommand("SELECT *FROM [Sheet1$]", myExcelConn);

                        // READ THE DATA EXTRACTED FROM THE EXCEL FILE.
                        OleDbDataReader objBulkReader = null;
                        objBulkReader = objOleDB.ExecuteReader();

                        //// SET THE CONNECTION STRING.
                        //string sCon = "Data Source=DNA;Persist Security Info=False;" +
                        //    "Integrated Security=SSPI;" +
                        //    "Initial Catalog=DNA_Classified;User Id=sa;Password=;" +
                        //    "Connect Timeout=30;";

                        //using (SqlConnection con = new SqlConnection(sCon))
                        //{
                        //    con.Open();

                        //    // FINALLY, LOAD DATA INTO THE DATABASE TABLE.
                        //    oSqlBulk = new SqlBulkCopy(con);
                        //    oSqlBulk.DestinationTableName = "EmployeeDetails"; // TABLE NAME.
                        //    oSqlBulk.WriteToServer(objBulkReader);
                        //}

                        ////lblConfirm.Text = "DATA IMPORTED SUCCESSFULLY.";
                        ////lblConfirm.Attributes.Add("style", "color:green");

                    }
                    catch (Exception ex)
                    {

                        //lblConfirm.Text = ex.Message;
                        //lblConfirm.Attributes.Add("style", "color:red");

                    }
                    finally
                    {
                        // CLEAR.
                        oSqlBulk.Close();
                        oSqlBulk = null;
                        myExcelConn.Close();
                        myExcelConn = null;
                    }


                    ////////////
                    response = request.CreateResponse(HttpStatusCode.OK, fileUploadResult);
                }

                return response;
            });
        }

    }
}
