﻿using System.Web;
using System.Web.Mvc;
using Lyra.WaveParser;
using PDF;

namespace MrChorder.Controllers
{
    public class ChordController : Controller
    {
        private string filename;
        //[HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void GetFile()
        {
            foreach (string eachfile in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[eachfile] as HttpPostedFileBase;
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload"), System.IO.Path.GetFileName(file.FileName));
                filename = path;
                file.SaveAs(path);
            }
            string genPath = System.IO.Path.Combine(Server.MapPath("~/Generate"), System.IO.Path.GetFileName("t1.pdf"));
            if (System.IO.File.Exists(genPath))
            {
                System.IO.File.Delete(genPath);
            }
            Response.Write("File upload successfully!");
            Response.End();
        }

        [HttpGet]
        public void SendFile()
        {
            //todo this filename
            string resultFilePath = System.IO.Path.Combine(Server.MapPath("~/Generate/"), System.IO.Path.GetFileName("t1.pdf"));
            string imgPath = System.IO.Path.Combine(Server.MapPath("~/Images/"));
            //[TODO] file name
            while (!System.IO.File.Exists(resultFilePath))//wait here
            {
                //[TODO] audio file name
                Audio audio = new Audio("test file name");
                float[] notes = { 1, 2 };// audio.GetNotes();
                //[TODO] three name
                ToPDF.ScoreCreation(imgPath, resultFilePath, notes, notes.Length, "todoname", "todoformname", "todotoname");
            }
            Response.Write("Return file successfully!");
            Response.End();
        }
    }
}