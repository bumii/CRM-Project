using MatriksCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
                        YeniProje.TeklifIcerigi = null;
                        YeniProje.ProjeDurum = reader.GetString(6);
                        ProjeListesi.Add(YeniProje);
                    }
                    catch
                    {
                        Console.WriteLine("A null value has reached");
                    }
                    
                }
            }
            return View(ProjeListesi);
        }

        [HttpPost]
        public JsonResult ModifyProject(Proje proje)
        {
            bool returnValue = false;



            return Json(returnValue);
        }

        [HttpPost]
        public JsonResult DeleteProject(int id)
        {
            int retval;
            bool returnValue=false;
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("DeleteProject", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("ProjeID", id);
                connection.Open();
                retval =command.ExecuteNonQuery();
                connection.Close();
            }
            if (retval >= 1)
            {
                returnValue = true;
 
            }
            return Json(returnValue);

        }

        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }
    }
}