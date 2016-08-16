using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FFT;
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
            string genPath = System.IO.Path.Combine(Server.MapPath("~/Generate"), System.IO.Path.GetFileName("t1.pdf"));
            string imgPath = System.IO.Path.Combine(Server.MapPath("~/Images"), System.IO.Path.GetFileName("sign.png"));
            //do something
            while (!System.IO.File.Exists(genPath))//wait here
            {
                FFT.FFT fft = new FFT.FFT();
                float[] notes = fft.fft("filename");
                ToPDF tp = new ToPDF();
                tp.GeneratePDF(imgPath,genPath, notes, notes.Length);
            }
            Response.Write("Return file successfully!");
            Response.End();
        }
    }
}