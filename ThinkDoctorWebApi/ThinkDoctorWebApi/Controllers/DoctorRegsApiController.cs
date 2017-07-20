using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Description;
using ThinkDoctorWebApi.Models;

namespace ThinkDoctorWebApi.Controllers
{
    public class DoctorRegsApiController : ApiController
    {
        private DoctorRegContext db = new DoctorRegContext();
        private DoctorRegsController dc = new DoctorRegsController();

        // GET: api/DoctorRegsApi
        [ResponseType(typeof(DoctorReg))]
        public  IQueryable<DoctorReg> GetDoctorRegs()
        {
           
                return db.DoctorRegs;
           
        }

        //public IHttpActionResult GetCompanies()
        //{
        //    var companies = db.DoctorRegs.ToList();
        //    return Ok(new { results = companies });
        //}
        // GET: api/DoctorRegsApi/email/pasw
        [ResponseType(typeof(DoctorReg))]
        [Route("api/DoctorRegsApi/{Wemail}/{Password}")]
        public IHttpActionResult GetDoctorsRegs(string Wemail, string Password)
        {
            var emailchk = db.DoctorRegs.Where(p =>p.EmailID1 == Wemail).FirstOrDefault();
            if (emailchk == null)
            {

                // return Ok("user_not_exits");
                return Ok(new Responseerror() { Status = "Fail", Message = "user_not_exits" });

            }

            var password = dc.Encrypt(Password);
            var chkpass = db.DoctorRegs.Where(p => p.Password == password && p.EmailID1==Wemail ).FirstOrDefault();
            if (chkpass == null)
            {
                // return Ok("worng_Password");
                return Ok(new Responseerror() { Status = "Fail", Message = "worng_password" });
            }

           
            var emailconfchk = db.DoctorRegs.Where(p => p.Emailconform == 1 && p.EmailID1==Wemail ).FirstOrDefault();
            if (emailconfchk == null)
            {
                // return Ok("email_not_confirm");
                return Ok(new Responseerror() { Status = "Fail", Message = "email_not_confirm" });
            }
           

           
            var userActivation = db.DoctorRegs.Where(p => p.EmailID1 == Wemail.ToString()).FirstOrDefault();
            if (userActivation != null)
            {
                userActivation.Status = 1;
                userActivation.temp_pasw_attempted = 1;
                db.DoctorRegs.Attach(userActivation);
                db.Entry(userActivation).Property(x => x.Status).IsModified = true;
                db.Entry(userActivation).Property(x => x.temp_pasw_attempted).IsModified = true;
                db.SaveChanges();
            }

            //return Ok("Pass=" + userActivation.id);
            return Ok(new loginResponse() { Status = "success", Userid = userActivation.id.ToString() });
        }


        [HttpGet]
        [ResponseType(typeof(DoctorReg))]
        [Route("api/DoctorRegsApi/{Wemail}/")]
        public IHttpActionResult Userexits(string Wemail)
        {
            var emailchk = db.DoctorRegs.Where(p => p.EmailID1 == Wemail).FirstOrDefault();
            if (emailchk == null)
            {

                return Ok("fail");

            }
            else
            {
                return Ok("pass");
            }
        }

        [HttpGet]
        public IHttpActionResult forgotpasw(string email,string forgot)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userActivation = db.DoctorRegs.Where(p => p.EmailID1 == email.ToString()).FirstOrDefault();
            if (userActivation == null)
            {
                return Ok(new Responseerror() { Status = "fail", Message = "user_not_exits" });
            }
            else
            {
                try
                {
                    Otp_pasw otpdata = new Otp_pasw();
                    db.Otp_pasw.Add(otpdata);
                    string otp = dc.otpgen();
                    otpdata.otp = otp;
                    otpdata.userid = userActivation.id;
                    otpdata.status = 1;
                    otpdata.expiry_time = DateTime.Now.AddMinutes(10);
                    otpdata.createed = DateTime.Now;
                    var sent=sendotptomail(otpdata,email );
                    if(sent != "succ")
                    {
                        return Ok(new Responseerror() { Status = "fail", Message = "error" });
                    }
                    db.SaveChanges();
                    return Ok(new otpResponse { Status = "success", otp = otpdata.otp,userid=userActivation.id, exptime = otpdata.expiry_time });
                }

                catch (Exception ex)
                {
                    return Ok(new Responseerror() { Status = "fail", Message = "error" });
                }

               
            }
           

        }
        public string sendotptomail(Otp_pasw otpdata,string email)
        {

                        try
            {
                using (MailMessage mm = new MailMessage("prasanna1991mca@gmail.com", email))
                {
                    mm.Subject = "Think Docotor -Forgot Password";
                    string body = string.Empty;
                    body = "Hi ";
                    body += "<br /><br />Please use OTP <B>'"+otpdata.otp +"'</B> to reset your password to access Think Doctor";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("prasanna1991mca@gmail.com", "prasannamca1107");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);

                }
                return "succ";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        [HttpGet]
        public IHttpActionResult otpvalidation(string otpval, int userid,string exptime)
        {

            DateTime todaydate = DateTime.Now;
            DateTime inputdate = Convert.ToDateTime(exptime);
            if (todaydate >= inputdate)
            {
                return Ok(new Responseerror { Status = "fail", Message = "expired" });
            }
            else
            {
                var otpchk = db.Otp_pasw.Where(p => p.otp == otpval && p.userid == userid &p.status==1).FirstOrDefault();
                if (otpchk == null)
                {
                    return Ok(new Responseerror { Status = "fail", Message = "invalid" });
                }
                else
                {
                    
                    otpchk.status = 0;
                    db.SaveChanges();
                    return Ok(new otpsuccResponse { Status = "success", message = "valid" });

                }
               
            }
            
           


        }


        [HttpGet]
        public IHttpActionResult ChangePassword(string email, string password)
        {
            try
            {
                var emailchk = db.DoctorRegs.Where(p => p.EmailID1 == email).FirstOrDefault();
                if (emailchk == null)
                {
                    return Ok(new Responseerror() { Status = "fail", Message = "user_not_exits" });
                }
                else
                {
                    emailchk.Password = dc.Encrypt(password);
                    db.Entry(emailchk).Property(x => x.Password).IsModified = true;
                    db.SaveChanges();
                    return Ok(new passchangeResponse() { Status = "success", message = "Password_changed" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new Responseerror() { Status = "fail", Message = "error" });
            }
        }

        // PUT: api/DoctorRegsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDoctorReg(int id, DoctorReg doctorReg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctorReg.id)
            {
                return BadRequest();
            }

            db.Entry(doctorReg).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorRegExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DoctorRegsApi
        [ResponseType(typeof(DoctorReg))]
        public IHttpActionResult PostDoctorReg(DoctorReg doctorReg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DoctorRegs.Add(doctorReg);
            string tmp =dc. temppassword();
            doctorReg.Password = tmp;
            doctorReg.Emailconform = 0;
            doctorReg.Logintime = DateTime.Now;
            doctorReg.Logouttime = DateTime.Now;
            doctorReg.temp_pasw_attempted = 0;
            db.SaveChanges();
            Guid activationCode = Guid.NewGuid();
            doctorReg.ActivationCode = activationCode.ToString();
            string s = dc.SendActivationEmail(doctorReg);
            string s1 =dc. SendAdminEmail(doctorReg);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = doctorReg.id }, doctorReg);
        }

        // DELETE: api/DoctorRegsApi/5
        [ResponseType(typeof(DoctorReg))]
        public IHttpActionResult DeleteDoctorReg(int id)
        {
            DoctorReg doctorReg = db.DoctorRegs.Find(id);
            if (doctorReg == null)
            {
                return NotFound();
            }

            db.DoctorRegs.Remove(doctorReg);
            db.SaveChanges();

            return Ok(doctorReg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DoctorRegExists(int id)
        {
            return db.DoctorRegs.Count(e => e.id == id) > 0;
        }
    }
}