using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ThinkDoctorWebApi.Models
{
    public class Otp_pasw
    {
        
       
        public int? id { get; set; }
        public int userid { get; set; }
        public string otp { get; set; }
        public int status { get; set; }
        public DateTime expiry_time { get; set; }
        public DateTime  createed { get; set; }
       

    }
}