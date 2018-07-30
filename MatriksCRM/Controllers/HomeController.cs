using MatriksCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatriksCRM.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            if (Session.Count == 0)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Proje(string bolum)
        {
            List<Proje> ProjeListesi = new List<Proje>();

            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "ProjeCek"
            };

            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));
            command.Parameters.Add(new SqlParameter("@Bolum", bolum));

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    try
                    {
                        Proje YeniProje = new Proje();
                        YeniProje.ProjeID = reader.GetInt32(0);
                        YeniProje.FirmaAdi = reader.GetString(1);
                        YeniProje.ProjeAdi = reader.GetString(2);
                        YeniProje.ProjeYeri = reader.GetString(3);
                        YeniProje.TeklifTarihi = reader.GetDateTime(4).ToShortDateString();
                        YeniProje.TeklifIcerigi = reader.GetString(5);
                        YeniProje.ProjeDurum = reader.GetString(6);
                        ProjeListesi.Add(YeniProje);
                    }
                    catch
                    {
                        Console.WriteLine("A null value has reached");
                    }

                }
            }
            ViewBag.Bolum = bolum;
            return View(ProjeListesi);
        }

        #region Project Operations

        [HttpPost]
        public JsonResult CreateProject(Proje proje, HttpPostedFileBase file)
        {
            return Json(AddModifyParameters(proje, file, "AddProject"));
        }

        [HttpPost]
        public JsonResult ModifyProject(Proje proje, HttpPostedFileBase file)
        {
            return Json(AddModifyParameters(proje, file, "ProjeDegistir"));
        }

        [HttpPost]
        public JsonResult DeleteProject(int id)
        {
            int retval;
            bool returnValue = false;
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("ProjeSil", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("ProjeID", id);
                connection.Open();
                retval = command.ExecuteNonQuery();
                connection.Close();
            }
            if (retval >= 1)
            {
                returnValue = true;

            }
            return Json(returnValue);

        }


        public bool AddModifyParameters(Proje proje, HttpPostedFileBase file, string procedureName)
        {
            bool returnValue = false;

            MemoryStream memoryStream = new MemoryStream();
            file.InputStream.CopyTo(memoryStream);
            byte[] teklifIcerigi = memoryStream.ToArray();

            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = procedureName
            };

            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));

            if (procedureName == "ProjeDegistir")
            {
                command.Parameters.Add(new SqlParameter("@ProjeID", proje.ProjeID));
            }
            command.Parameters.Add(new SqlParameter("@FirmaAdi", proje.FirmaAdi));
            command.Parameters.Add(new SqlParameter("@ProjeAdi", proje.ProjeAdi));
            command.Parameters.Add(new SqlParameter("@ProjeYeri", proje.ProjeYeri));

            DateTime TeklifTarihi = DateTime.Parse(proje.TeklifTarihi);
            command.Parameters.Add(new SqlParameter("@TeklifTarihi", TeklifTarihi));
            command.Parameters.Add(new SqlParameter("@IcerikAdi", file.FileName));

            command.Parameters.Add(new SqlParameter("@TeklifIcerigi", teklifIcerigi));
            command.Parameters.Add(new SqlParameter("@ProjeDurum", proje.ProjeDurum));
            command.Parameters.Add(new SqlParameter("@Bolum", proje.Bolum));

            connection.Open();
            if (command.ExecuteNonQuery() != 0)
            {
                returnValue = true;
            }

            return returnValue;
        }

        #endregion

        public FileStreamResult TeklifIcerigiIndir(int projeId)
        {
            string fileName = "";
            Stream stream;
            FileStreamResult streamResult;

            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "TeklifIcerigiIndir"
            };

            command.Parameters.Add(new SqlParameter("@ProjeID", projeId));

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                fileName = reader.GetString(0);
                stream = reader.GetStream(1);
                string fileNameExtension = Path.GetExtension(fileName);

                if (Path.GetExtension(fileName) == ".pdf")
                {
                    streamResult = new FileStreamResult(stream, "application/pdf");
                }
                else if (Path.GetExtension(fileName) == ".xls")
                {
                    streamResult = new FileStreamResult(stream, "application/vnd.ms-excel");
                }
                else if (Path.GetExtension(fileName) == ".xlsx")
                {
                    streamResult = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
                else
                {
                    return null;
                }

                streamResult.FileDownloadName = fileName;
                return streamResult;
            }

            return null;
        }

        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }
        public ActionResult Ajanda()
        {
            

            return View();
        }
    }
}