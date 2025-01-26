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

                // 根據預存程序的返回值判斷更新是否成功
                switch (result)
                {
                    case 0:     // 更新成功
                        return true;
                    case -1:    // 找不到員工
                        return false;
                    case -2:    // 發生錯誤
                        return false;
                    case -3:    // 未更新（資料未改變）
                        return true;    // 因為資料未改變，我們仍然可以視為"成功"
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
            // 根據預存程序的返回值判斷更新是否成功
            switch (result)
            {
                case 0:     // 更新成功
                    return true;
                case -1:    // 找不到員工
                    return false;
                case -2:    // 發生錯誤
                    return false;
                case -3:    // 未刪除（資料未改變）
                    return true;    // 因為資料未改變，我們仍然可以視為"成功"
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
