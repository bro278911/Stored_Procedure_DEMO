using System.Data;
using System.Diagnostics;
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
             var dataTable = GetUser(3);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
