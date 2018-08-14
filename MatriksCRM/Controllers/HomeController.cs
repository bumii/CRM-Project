using MatriksCRM.Models;
using MatriksCRM.Views.Home;
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
            if(proje.ProjeDurum == "Teklif Asamasinda"){
            DateTime TeklifTarihi = DateTime.Parse(proje.TeklifTarihi);
            DBConnection connect = new DBConnection();
                try
                {
                    var kullaniciid = Session["ID"].ToString();
                    var ProjeDetay = " <h5> Proje Detayları : </h5> Proje Adı : " + proje.ProjeAdi + " <br> Firma Adı : " + proje.FirmaAdi + " <br> Proje Yeri : " + proje.ProjeYeri;
                    connect.OpenConnection();
                    List<SqlParameter> param = new List<SqlParameter>();
                    param.Add(new SqlParameter("@KullaniciID", kullaniciid));
                    string projeIsmi = proje.ProjeAdi + " Proje Teklifi";
                    param.Add(new SqlParameter("@Title", projeIsmi));
                    param.Add(new SqlParameter("@StartDate", TeklifTarihi.ToString("yyyyMMdd")));
                    param.Add(new SqlParameter("@EndTime", TeklifTarihi.AddDays(8).ToString("yyyyMMdd")));
                    param.Add(new SqlParameter("@Color", "blue"));
                    param.Add(new SqlParameter("@AllDay", "1"));
                    param.Add(new SqlParameter("@NOTLAR", ProjeDetay));
                    string sql = "Insert into tAgenda.tAgenda(KullaniciID,Title,StartDate,EndTime,Color,AllDay,NOTLAR) ";
                    sql += "Values(@KullaniciID,@Title,@StartDate,@EndTime,@Color,@AllDay,@NOTLAR) ";

                    connect.RunSqlCommand(sql, param);

                }
            finally
            {
                connect.CloseConnection();
            }
            }

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


        public int AddModifyParameters(Proje proje, HttpPostedFileBase file, string procedureName)
        {
            byte[] teklifIcerigi = null;
            string fileName = "";

            if (file != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                file.InputStream.CopyTo(memoryStream);
                teklifIcerigi = memoryStream.ToArray();
                fileName = file.FileName;
            }
            else
            {
                teklifIcerigi = new byte[0];
            }

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
            command.Parameters.Add(new SqlParameter("@IcerikAdi", fileName));
            command.Parameters.Add(new SqlParameter("@TeklifIcerigi", teklifIcerigi));
            command.Parameters.Add(new SqlParameter("@ProjeDurum", proje.ProjeDurum));
            command.Parameters.Add(new SqlParameter("@Bolum", proje.Bolum));

            if (procedureName == "ProjeDegistir")
            {
                connection.Open();
                int effectedRows = command.ExecuteNonQuery();
                if (effectedRows != 0)
                {
                    return effectedRows;
                }


            }

            if (procedureName == "AddProject")
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    int projeId = reader.GetInt32(0);
                    connection.Close();
                    return projeId;
                }

            }

            return 0;
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
        public ActionResult IsTakip()
        {
            List<IsTakip> IsListesi = new List<IsTakip>();

            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "IsListele"
            };
            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    try
                    {
                        IsTakip YeniProje = new IsTakip();
                        YeniProje.IsID = reader.GetInt32(0);
                        YeniProje.FirmaAdi = reader.GetString(1);
                        YeniProje.ProjeAd = reader.GetString(2);
                        YeniProje.ProjeYeri = reader.GetString(3);
                        YeniProje.TeklifTarihi = reader.GetDateTime(4).ToShortDateString();
                        YeniProje.SonGorusmeTarihi = reader.GetDateTime(5).ToShortDateString();
                        YeniProje.ProjeVadesi = reader.GetString(6);
                        IsListesi.Add(YeniProje);
                    }
                    catch
                    {
                        Console.WriteLine("A null value has reached");
                    }

                }
            }
            return View(IsListesi);
        }

        public int Modify(IsTakip isTakip, string procedureName)
        {
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = procedureName
            };

            if (procedureName == "IsDegistir")
            {
                command.Parameters.Add(new SqlParameter("@IsID", isTakip.IsID));
            }
            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));
            command.Parameters.Add(new SqlParameter("@FirmaAdi", isTakip.FirmaAdi));
            command.Parameters.Add(new SqlParameter("@ProjeAd", isTakip.ProjeAd));
            command.Parameters.Add(new SqlParameter("@ProjeYeri", isTakip.ProjeYeri));
            DateTime TeklifTarihi = DateTime.Parse(isTakip.TeklifTarihi);
            command.Parameters.Add(new SqlParameter("@TeklifTarihi", TeklifTarihi));
            DateTime SonGorusmeTarihi = DateTime.Parse(isTakip.TeklifTarihi);
            command.Parameters.Add(new SqlParameter("@SonGorusmeTarihi", isTakip.SonGorusmeTarihi));
            command.Parameters.Add(new SqlParameter("@ProjeVadesi", isTakip.ProjeVadesi));

            if (procedureName == "IsDegistir")
            {
                connection.Open();
                int effectedRows = command.ExecuteNonQuery();
                if (effectedRows != 0)
                {
                    return effectedRows;
                }
            }
            if (procedureName == "TakipIs")
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    int IsId = reader.GetInt32(0);
                    connection.Close();
                    return IsId;
                }
            }

            return 0;
        }
        [HttpPost]
        public JsonResult CreateIs(IsTakip isTakip)
        {
            return Json(Modify(isTakip, "TakipIs"));
        }
        [HttpPost]
        public JsonResult ModifyIs(IsTakip isTakip)
        {
            return Json(Modify(isTakip, "IsDegistir"));
        }
        [HttpPost]
        public JsonResult IsSil(int id)
        {
            int retval;
            bool returnValue = false;
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("isSil", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("IsID", id);
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
        public ActionResult Musteri(string durum)
        {
            List<Musteri> MusteriListesi = new List<Musteri>();

            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "MusteriListele"
            };
            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    try
                    {

                        Musteri YeniMusteri = new Musteri();
                        YeniMusteri.MusteriID = reader.GetInt32(0);
                        YeniMusteri.FirmaAdi = reader.GetString(1);
                        YeniMusteri.YetkiliAd = reader.GetString(2);
                        YeniMusteri.YetkiliPoz = reader.GetString(3);
                        YeniMusteri.MusteriTel = reader.GetString(4);
                        YeniMusteri.MusteriMail = reader.GetString(5);
                        YeniMusteri.MusteriDurum = reader.GetString(6);
                        MusteriListesi.Add(YeniMusteri);
                    }
                    catch
                    {
                        Console.WriteLine("A null value has reached");
                    }

                }
            }
            return View(MusteriListesi);
        }
        public int ModifyCustomer(Musteri musteri, string procedureName)
        {
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            SqlConnection connection = new SqlConnection(connString);

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = procedureName
            };

            if (procedureName == "MusteriGuncelle")
            {
                command.Parameters.Add(new SqlParameter("@MusteriID", musteri.MusteriID));
            }
            command.Parameters.Add(new SqlParameter("@KullaniciID", Session["ID"]));
            command.Parameters.Add(new SqlParameter("@FirmaAdi", musteri.FirmaAdi));
            command.Parameters.Add(new SqlParameter("@YetkiliAd", musteri.YetkiliAd));
            command.Parameters.Add(new SqlParameter("@YetkiliPoz", musteri.YetkiliPoz));
            command.Parameters.Add(new SqlParameter("@MusteriTel", musteri.MusteriTel));
            command.Parameters.Add(new SqlParameter("@MusteriMail", musteri.MusteriMail));
            command.Parameters.Add(new SqlParameter("@MusteriDurum", musteri.MusteriDurum));

            if (procedureName == "MusteriGuncelle")
            {
                connection.Open();
                int effectedRows = command.ExecuteNonQuery();
                if (effectedRows != 0)
                {
                    return effectedRows;
                }
            }
            if (procedureName == "MusteriEkle")
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    int MusteriId = reader.GetInt32(0);
                    connection.Close();
                    return MusteriId;
                }
            }

            return 0;
        }
        [HttpPost]
        public JsonResult CreateCustomer(Musteri musteri)
        {
            return Json(ModifyCustomer(musteri, "MusteriEkle"));
        }
        [HttpPost]
        public JsonResult ModifyCustomer(Musteri musteri)
        {
            return Json(ModifyCustomer(musteri, "MusteriGuncelle"));
        }
        public JsonResult DeleteCustomer(int id)
        {
            int retval;
            bool returnValue = false;
            string connString = ConfigurationManager.ConnectionStrings["MatriksStajCRM"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("musteriSil", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("MusteriID", id);
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
        public ActionResult Ajanda()
        {
            return View();
        }
    }
}