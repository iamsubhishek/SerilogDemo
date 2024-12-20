using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewSerilogApplication.Interface;
using NewSerilogApplication.Models;
using Serilog;

namespace NewSerilogApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog<HomeController> traceInfo;
        
        public HomeController(ILog<HomeController> logger)
        {
            traceInfo = logger;
        }

        public IActionResult Index()
        {
            try
            {
                traceInfo.TraceMessage();
                int a = 10, b = 10;
                var result = (a / b);
                return View();
            }
            catch (DivideByZeroException ex)
            {
                traceInfo.TraceError(ex);
                throw;
            }
            catch (Exception ex)
            {
                traceInfo.TraceError(ex);
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
