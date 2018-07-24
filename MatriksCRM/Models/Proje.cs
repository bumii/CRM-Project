using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatriksCRM.Models
{
    public class Proje
    {
        public int ProjeID { get; set; }
        public string FirmaAdi { get; set; }
        public string ProjeAdi { get; set; }
        public string ProjeYeri { get; set; }
        public DateTime TeklifTarihi { get; set; }
        public Byte[] TeklifIcerigi { get; set; }
        public string ProjeDurum { get; set; }
        public string Bolum { get; set; }
    }
}