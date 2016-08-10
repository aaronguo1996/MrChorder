using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MrChorder.Controllers
{
    public class ChordController : Controller
    {
        //[HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "No file uploaded";
            return View();
        }

        [HttpPost]
        public ActionResult GetFile()
        {
            foreach (string eachfile in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[eachfile] as HttpPostedFileBase;
                string path = System.IO.Path.Combine(Server.MapPath("~/Images"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(path);
            }
            Response.Write("File uploaded successfully");
            return View();
        }
    }
}