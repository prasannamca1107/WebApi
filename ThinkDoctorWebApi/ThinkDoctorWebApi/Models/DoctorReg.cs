using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThinkDoctorWebApi.Models
{
    public class DoctorReg
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
        public string NHSNo { get; set; }
        public string PostCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string HomeNo { get; set; }
        public string WorkNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailID1 { get; set; }
        public string Email2 { get; set; }
        public string Age { get; set; }
        public string ParentName { get; set; }
        public string Relation { get; set; }
        public string ParentAddress { get; set; }
        public string ParentContactNo { get; set; }
        public string NxKintName { get; set; }
        public string NxtKinAddress { get; set; }
        public string NxtKinContactNo { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string ActivationCode { get; set; }
        public string Password { get; set; }
        public string Uniquecode { get; set; }
        public int? Emailconform { get; set; }
        public int? Status { get; set; }
        public DateTime? Logintime { get; set; }
        public DateTime? Logouttime { get; set; }
        public string Deviceid { get; set; }
        public int? temp_pasw_attempted { get; set; }
    }
}