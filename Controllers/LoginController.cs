
using CrudMap.Helpers;
using CrudMap.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CrudMap.Controllers
{
    public class LoginController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["image"].ConnectionString;

        private static Dictionary<string, string> verificationCodes = new Dictionary<string, string>();

        // GET: Login
        public ActionResult Login()
        {
            var model = new LoginModel();

            if (Request.Cookies["UserEmail"] != null)
            {
                model.EmailId = Request.Cookies["UserEmail"].Value;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string RememberMe)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SP_GetLoginDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmailId", model.EmailId);
                cmd.Parameters.AddWithValue("@Password", model.Password);


                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result is byte[] dbPasswordBytes)
                {
                    byte[] inputHashedBytes = PasswordHelper.HashPasswordAsBytes(model.Password);

                    if (dbPasswordBytes.SequenceEqual(inputHashedBytes))
                    {
                        Session["UserEmail"] = model.EmailId;

                        if (!string.IsNullOrEmpty(RememberMe) && RememberMe.ToLower() == "true")
                        {
                            HttpCookie cookie = new HttpCookie("UserEmail");
                            cookie.Value = model.EmailId;
                            cookie.Expires = DateTime.Now.AddDays(7);
                            Response.Cookies.Add(cookie);
                        }

                        return RedirectToAction("Lists", "Teachers");
                    }
                }
            }

            ViewBag.Message = "Invalid Email or Password";
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.CountryList = GetCountries();
            return View();
        }

        [HttpPost]
        public ActionResult Create(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CountryList = GetCountries();
                return View(model);
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SP_InsertRegisterDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@EmailId", model.EmailId);

                // ✅ Hash password to bytes
                byte[] hashedPassword = PasswordHelper.HashPasswordAsBytes(model.Password);
                cmd.Parameters.Add("@Password", SqlDbType.VarBinary, 64).Value = hashedPassword;

                cmd.Parameters.AddWithValue("@Date", model.Date);
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);

                con.Open();
                string resultMessage = "";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resultMessage = reader[0].ToString();
                    }
                }

                TempData["Message"] = resultMessage;

                if (resultMessage == "Email already registered.")
                {
                    ViewBag.CountryList = GetCountries();
                    ModelState.AddModelError("EmailId", "Email already registered.");
                    return View(model);
                }
            }

            return RedirectToAction("Login");
        }




        private List<SelectListItem> GetCountries()
        {
            List<SelectListItem> countries = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT CountryId, CountryName FROM Country", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new SelectListItem
                    {
                        Value = reader["CountryId"].ToString(),
                        Text = reader["CountryName"].ToString()
                    });
                }
            }
            return countries;
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Login/Forgot
        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }

        // POST: Login/Forgot
        public ActionResult Forgot(string email)
        {
            string resetLink = Url.Action("ResetPassword", "Login", new { email = email }, protocol: Request.Url.Scheme);

            EmailHelper emailHelper = new EmailHelper();
            EmailHelper.SendVerificationEmail(email, resetLink);

            TempData["Message"] = "Reset link has been sent to your email.";
            return RedirectToAction("Login");
        }
        // GET: Login/ResetPassword?email=xyz@example.com
        [HttpGet]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Message"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            ViewBag.Email = email;
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
            {
                TempData["Message"] = "Password reset failed. Please check the fields.";
                return RedirectToAction("ResetPassword", new { email });
            }

            // Hash new password to bytes using PasswordHelper
            byte[] hashedPassword = PasswordHelper.HashPasswordAsBytes(newPassword);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["image"].ConnectionString))
            {
                string query = "UPDATE Register SET Password = @Password WHERE EmailId = @EmailId";  // ✅ TABLE: Register
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@Password", SqlDbType.VarBinary, 64).Value = hashedPassword;
                cmd.Parameters.AddWithValue("@EmailId", email);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "Password reset successful. Please log in.";
            return RedirectToAction("Login");
        }







    }
}
