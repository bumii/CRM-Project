using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatriksCRM.Models
{
    public class IsTakip
    {
        public int IsID { get; set; }
        public string FirmaAdi { get; set; }
        public string ProjeAd { get; set; }
        public string ProjeYeri { get; set; }
        [DataType(DataType.Date)]
        public string TeklifTarihi { get; set; }
        public string SonGorusmeTarihi { get; set; }
        public string ProjeVadesi { get; set; }
    }
}