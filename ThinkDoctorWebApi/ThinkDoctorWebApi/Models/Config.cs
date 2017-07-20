using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThinkDoctorWebApi.Models
{
    public class Config
    {
    }
    public class Responseerror
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class loginResponse
    {
        public string Status { get; set; }
        public string Userid { get; set; }
    }
    public class otpResponse
    {
        public string Status { get; set; }
        public string otp { get; set; }
        public int userid { get; set; }
        public DateTime exptime { get; set; }

    }
    public class otpsuccResponse
    {
        public string Status { get; set; }
        public string message { get; set; }
    }


public class passchangeResponse
{
    public string Status { get; set; }
    public string message { get; set; }
}
    }