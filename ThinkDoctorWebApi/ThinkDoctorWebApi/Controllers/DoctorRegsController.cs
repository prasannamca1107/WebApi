using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThinkDoctorWebApi.Models;

namespace ThinkDoctorWebApi.Controllers
{
    public class DoctorRegsController : Controller
    {
        private DoctorRegContext db = new DoctorRegContext();

        // GET: DoctorRegs
        public ActionResult Index()
        {
            return View(db.DoctorRegs.ToList());
        }

        // GET: DoctorRegs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorReg doctorReg = db.DoctorRegs.Find(id);
            if (doctorReg == null)
            {
                return HttpNotFound();
            }
            return View(doctorReg);
        }

        // GET: DoctorRegs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoctorRegs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Title,FirstName,MiddleName,LastName,Dob,Gender,NHSNo,PostCode,AddressLine1,AddressLine2,AddressLine3,City,State,Country,HomeNo,WorkNo,MobileNo,EmailID1,Email2,Age,ParentName,Relation,ParentAddress,ParentContactNo,NxKintName,NxtKinAddress,NxtKinContactNo,FileName,FilePath,ActivationCode,Password,Uniquecode,Emailconform,Status,Logintime,Logouttime,Deviceid,temp_pasw_attempted")] DoctorReg doctorReg)
        {
            if (ModelState.IsValid)
            {
               
                db.DoctorRegs.Add(doctorReg);
                string tmp = temppassword();
                var password = Encrypt(tmp);
                doctorReg.Password = password;
                doctorReg.Emailconform = 0;
                doctorReg.Logintime = DateTime.Now;
                doctorReg.Logouttime = DateTime.Now;
                doctorReg.temp_pasw_attempted = 0;
                db.SaveChanges();
                Guid activationCode = Guid.NewGuid();
                doctorReg.ActivationCode = activationCode.ToString();
                string s = SendActivationEmail(doctorReg);

                string s1 = SendAdminEmail(doctorReg);
                
                return RedirectToAction("Index");
            }

            return View(doctorReg);
        }
        public string temppassword()
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,$,%,&";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string passwordString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < 7; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }
            return passwordString;
        }
        public string otpgen()
        {
            string allowedChars = "";

            allowedChars += "1,2,3,4,5,6,7,8,9,0";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string passwordString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < 7; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }
            return passwordString;
        }

        public  string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        //public  string Decrypt(string cipherText)
        //{
        //    string EncryptionKey = "MAKV2SPBNI99212";
        //    byte[] cipherBytes = Convert.FromBase64String(cipherText);
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                cs.Close();
        //            }
        //            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //        }
        //    }
        //    return cipherText;
        //}
        public string SendActivationEmail(DoctorReg doctorReg)
        {

            db.DoctorRegs.Attach(doctorReg);

            db.Entry(doctorReg).Property(x => x.ActivationCode).IsModified = true;
            db.SaveChanges();

            try
            {
                using (MailMessage mm = new MailMessage("prasanna1991mca@gmail.com", doctorReg.EmailID1))
                {
                    mm.Subject = "Account Activation";
                    string body = string.Empty;
                    //string body = "Hello " + doctorsReg.Fname + ",";
                    //body += "<br /><br />Please click the following link to activate your account";
                    //body += "<br /><a href = '" + string.Format("http://192.168.1.3/DoctorsRegs/ActivationMail/{0}", doctorsReg.ActivationCode) + "'>Click here to activate your account.</a>";
                    //body += "<br /><br />Thanks";
                    string name = doctorReg.FirstName.ToString();
                    string activationurl = string.Format("http://192.168.1.3/ThinkdocotorApi/DoctorRegs/ActivationMail/{0}", doctorReg.ActivationCode);
                    string wmail = doctorReg.EmailID1.ToString();
                    string temppasw =Encrypt( doctorReg.Password.ToString());
                    string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "MailHtml\\ConformationMail.html");
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{UserName}", name);
                    body = body.Replace("{Url}", activationurl);
                    body = body.Replace("{Usermail}", wmail);
                    body = body.Replace("{Password}", temppasw);
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

        public string SendAdminEmail(DoctorReg doctorsReg)
        {



            try
            {
                using (MailMessage mm = new MailMessage("prasanna1991mca@gmail.com", "prasannamca1107a@gmail.com"))
                {
                    mm.Subject = "One Account Created..";
                    string body = string.Empty;
                    //string body = "Hello " + doctorsReg.Fname + ",";
                    //body += "<br /><br />Please click the following link to activate your account";
                    //body += "<br /><a href = '" + string.Format("http://192.168.1.3/DoctorsRegs/ActivationMail", doctorsReg.ActivationCode) + "'>Click here to activate your account.</a>";
                    //body += "<br /><br />Thanks";
                    string name = doctorsReg.FirstName;
                    string deviceid = doctorsReg.Deviceid;
                    string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "MailHtml\\AdminKnowRegs.html");
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{UserName}", name);
                    body = body.Replace("{DeviceId}", deviceid);

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

        public ActionResult ActivationMail(DoctorReg doctorsReg)
        {
            ViewBag.Message = "Invalid Activation code.";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());

                var userActivation = db.DoctorRegs.Where(p => p.ActivationCode == activationCode.ToString() && p.Emailconform == 0).FirstOrDefault();
                if (userActivation != null)
                {
                    userActivation.Emailconform = 1;
                    db.DoctorRegs.Attach(userActivation);
                    db.Entry(userActivation).Property(x => x.Emailconform).IsModified = true;
                    db.SaveChanges();
                    return RedirectToAction("../HTML/MailActivated.html");

                    //db.UserActivation.Remove(userActivation);
                    //db.SaveChanges();
                    //ViewBag.Message = "Activation successful.";
                }
                else
                {
                    return RedirectToAction("../HTML/AlreadyActivated.html");
                }
            }

            return View();
        }
        // GET: DoctorRegs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorReg doctorReg = db.DoctorRegs.Find(id);
            if (doctorReg == null)
            {
                return HttpNotFound();
            }
            return View(doctorReg);
        }

        // POST: DoctorRegs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Title,FirstName,MiddleName,LastName,Dob,Gender,NHSNo,PostCode,AddressLine1,AddressLine2,AddressLine3,City,State,Country,HomeNo,WorkNo,MobileNo,EmailID1,Email2,Age,ParentName,Relation,ParentAddress,ParentContactNo,NxKintName,NxtKinAddress,NxtKinContactNo,FileName,FilePath,ActivationCode,Password,Uniquecode,Emailconform,Status,Logintime,Logouttime,Deviceid,temp_pasw_attempted")] DoctorReg doctorReg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctorReg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorReg);
        }

        // GET: DoctorRegs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorReg doctorReg = db.DoctorRegs.Find(id);
            if (doctorReg == null)
            {
                return HttpNotFound();
            }
            return View(doctorReg);
        }

        // POST: DoctorRegs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorReg doctorReg = db.DoctorRegs.Find(id);
            db.DoctorRegs.Remove(doctorReg);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
