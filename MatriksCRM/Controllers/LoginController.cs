using MatriksCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

namespace MatriksCRM.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Kullanici kullanici)
        {
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;

            SqlConnection connection = new SqlConnection(connString);
            
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "KullaniciLogin"
            };
            
            command.Parameters.Add(new SqlParameter("@Email", kullanici.Email));
            command.Parameters.Add(new SqlParameter("@Sifre", kullanici.Sifre));

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Session.Add("ID", reader.GetInt32(0));
                    Session.Add("Isim", reader.GetInt32(3));
                    Session.Add("Soyad", reader.GetInt32(4));
                }
                connection.Close();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                connection.Close();
                return View();
            }
        }
    }
}