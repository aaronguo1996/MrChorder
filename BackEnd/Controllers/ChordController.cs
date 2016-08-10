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
            return View();
        }

        [HttpPost]
        public void GetFile(HttpPostedFileBase file)
        {
            string path = System.IO.Path.Combine(Server.MapPath("~/Images"), System.IO.Path.GetFileName(file.FileName));
            file.SaveAs(path);
            ViewBag.Message = "File uploaded successfully";
        }
    }
}