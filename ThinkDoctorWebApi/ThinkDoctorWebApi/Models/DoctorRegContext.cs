using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ThinkDoctorWebApi.Models
{
    public class DoctorRegContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public DoctorRegContext() : base("name=DoctorRegContext")
        {
           Database.SetInitializer<DoctorRegContext>(null);
        }
        public System.Data.Entity.DbSet<ThinkDoctorWebApi.Models.DoctorReg> DoctorRegs { get; set; }
        public System.Data.Entity.DbSet<ThinkDoctorWebApi.Models.Otp_pasw > Otp_pasw { get; set; }
    }
}
