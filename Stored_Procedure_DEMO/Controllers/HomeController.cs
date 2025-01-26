using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Stored_Procedure_DEMO.Models;

namespace Stored_Procedure_DEMO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SqlHelper _sqlHelper;

        public HomeController(ILogger<HomeController> logger, SqlHelper sqlHelper)
        {
            _logger = logger;
            _sqlHelper = sqlHelper;
        }

        public IActionResult Index()
        {
            var dataTable = GetUser(5);
            return View(dataTable);
        }
        public DataTable GetUser(int id)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeId", id)
            };
            var database = _sqlHelper.ExecuteStoredProcedureQuery("sp_GetEmployeeInfo", parameters);
            return database;
        }
        [HttpPost]
        public bool AddEmployee(string name, decimal salary)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name",name),
                new SqlParameter("@Salary",salary)
            };
            var result = _sqlHelper.ExecuteStoredProcedure("sp_AddEmployee", parameters);
            
            return result != 0;
        }
        [HttpPost]
        public bool UpdateEmployee(int id,string name, decimal salary)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@EmployeeID", id),
            new SqlParameter("@Name", name),
            new SqlParameter("@Salary", salary)
                };

                var result = _sqlHelper.ExecuteStoredProcedure_Status("sp_UpdateEmployee", parameters);

                // �ھڹw�s�{�Ǫ���^�ȧP�_��s�O�_���\
                switch (result)
                {
                    case 0:     // ��s���\
                        return true;
                    case -1:    // �䤣����u
                        return false;
                    case -2:    // �o�Ϳ��~
                        return false;
                    case -3:    // ����s�]��ƥ����ܡ^
                        return true;    // �]����ƥ����ܡA�ڭ̤��M�i�H����"���\"
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteEmployee(int id)
        {
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@EmployeeID",id)
            };
            var result = _sqlHelper.ExecuteStoredProcedure_Status("sp_DeleteEmployee", parameter);
            // �ھڹw�s�{�Ǫ���^�ȧP�_��s�O�_���\
            switch (result)
            {
                case 0:     // ��s���\
                    return true;
                case -1:    // �䤣����u
                    return false;
                case -2:    // �o�Ϳ��~
                    return false;
                case -3:    // ���R���]��ƥ����ܡ^
                    return true;    // �]����ƥ����ܡA�ڭ̤��M�i�H����"���\"
                default:
                    return false;
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
