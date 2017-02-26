using LinksNews.Services.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using LinksNews.Core;
//using Microsoft.Framework.ConfigurationModel;
using Microsoft.Extensions.Options;
using LinksNews.Services.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;


namespace LinksNews.Services
{
    public class AccountService
    {
        private readonly EncryptionService encryptionService;
        private readonly IOptions<LinksOptions> op;
        private readonly LinksContext db;
        private readonly ExecutionService es;
        private readonly FileService fs;
        private readonly PagesService ps;
        private readonly IHttpContextAccessor httpContext;
        private readonly MailService mas;


        public AccountService(
            EncryptionService encryptionService, 
            IOptions<LinksOptions> op,
            LinksContext db,
            ExecutionService es,
            FileService fs,
            PagesService ps,
            IHttpContextAccessor httpContext,
            MailService mas
            )
        {
            this.encryptionService = encryptionService;
            this.op = op;
            this.db = db;
            this.es = es;
            this.fs = fs;
            this.ps = ps;
            this.mas = mas;
            this.httpContext = httpContext;
        }

        public LoginData GetLoginDataFromCookies()
        {
            string lp = httpContext.HttpContext.Request.Cookies["lp"];

            if (string.IsNullOrWhiteSpace(lp))
            {
                return null;
            }

            string json = encryptionService.Decrypt(lp);

            try
            {
                LoginData result = JsonConvert.DeserializeObject<LoginData>(json);
                return result;
            }
            catch
            {
                Logout();
                return null;
            }
        }

        public void PutLoginDataToCookies(LoginData loginData)
        {
            if (loginData == null || !loginData.Valid)
            {
                es.ThrowException("Wrong login data");
            }
            string json = JsonConvert.SerializeObject(loginData);
            string lp = encryptionService.Encrypt(json);
            httpContext.HttpContext.Response.Cookies.Append("lp", lp,
                new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTimeOffset.MaxValue
                });
        }

        public string Login
        {
            get
            {
                LoginData loginData = GetLoginDataFromCookies();

                if (loginData == null)
                {
                    return null;
                }

                if (AccountValid(loginData))
                {
                    return loginData.Login;
                }

                Logout();
                return null;
            }
        }
        
        public Account LogAccountIn(LoginData loginData)
        {
            if (loginData == null || !loginData.Valid)
            {
                es.ThrowException("Wrong login data");
            }

            if (!AccountValid(loginData))
            {
                return null;
            }
            PutLoginDataToCookies(loginData);
            Account result = GetFullAccountByLogin(loginData.Login);
            return result;
        }

        public bool AccountValid(LoginData loginData)
        {
            Object result = null;

            using (SqlConnection connection = new SqlConnection(op.Value.Data.LinksConnection.ConnectionString))
            {
                string sql = "spLogin";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", loginData.Login);
                    command.Parameters.AddWithValue("@Password", loginData.Password);
                    connection.Open();
                    result = command.ExecuteScalar();
                }
            }

            return result != null;
        }

        public long CreateNewAccount(RegistrationData registrationData)
        {
            Object result = null;

            using (SqlConnection connection = new SqlConnection(op.Value.Data.LinksConnection.ConnectionString))
            {
                string sql = "spCreateAccount";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", registrationData.Login);
                    command.Parameters.AddWithValue("@Password", registrationData.Password);
                    command.Parameters.AddWithValue("@Email", registrationData.Email);
                    command.Parameters.AddWithValue("@FirstName", (object)registrationData.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", (object)registrationData.LastName ?? DBNull.Value);
                    connection.Open();
                    result = command.ExecuteScalar();
                }
            }

            return Convert.ToInt64(result);
        }

        public void UpdatePassword (ChangePasswordData data)
        {
            using (SqlConnection connection = new SqlConnection(op.Value.Data.LinksConnection.ConnectionString))
            {
                string sql = "spUpdatePassword";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", data.Login);
                    command.Parameters.AddWithValue("@OldPassword", data.OldPassword);
                    command.Parameters.AddWithValue("@NewPassword", data.NewPassword);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Logout()
        {
            httpContext.HttpContext.Response.Cookies.Delete("lp");
        }


        public Account RegisterNewAccount(RegistrationData registrationData)
        {
            long accountId = CreateNewAccount(registrationData);
            if (accountId == -1)
            {
                es.ThrowInfoException("Login {0} is already in use", registrationData.Login);
            }
            if (accountId == -2)
            {
                es.ThrowInfoException("Email {0} is already in use", registrationData.Email);
            }

            Account result = GetFullAccountByLogin(registrationData.Login);

            if (op.Value.Emails.SendRegisterEmailConfirmaion)
            {
                mas.SendRegisterConfirmation(result);
            }

            if (op.Value.Notifications.NotifyAdminAccountRegistered)
            {
                mas.NotifyAdminAccountRegistered(result);
            }


            return result;
        }

        public Account GetAccountByLogin(string login)
        {
            Account result = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            if (result == null)
            {
                return null;
            }
            if (result.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", result.Login);
            }
            return result;
        }

        public Account GetFullAccountByLogin(string login)
        {
            Account result = GetAccountByLogin(login);
            if (result == null)
            {
                return null;
            }
            if (result.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", result.Login);
            }
            result.ImageUrl = fs.GetAccountImageUrl(login);
            return result;
        }

        public Account SaveAccount(Account account)
        {
            Account existing = db.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == account.Id);
            if (existing == null)
            {
                es.ThrowInfoException("Account to update does not exist");
            }
            if (existing.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", existing.Login);
            }
            account.Password = existing.Password;
            db.Accounts.Update(account);
            db.SaveChanges();
            return GetFullAccountByLogin(account.Login);
        }

        public void SavePassword(ChangePasswordData data)
        {
            Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(data.Login, StringComparison.OrdinalIgnoreCase));

            if (account == null)
            {
                es.ThrowInfoException("Account to change password does not exist");
            }
            if (account.Locked)
            {
                es.ThrowInfoException("Account {0} is locked", account.Login);
            }

            LoginData loginData = new LoginData() { Login = data.Login, Password = data.OldPassword };

            if (!AccountValid(loginData))
            {
                es.ThrowInfoException("Old password is not valid");
            }

            UpdatePassword(data);            

        }

        public void DeleteAccount(string login)
        {
            db.Database.BeginTransaction();
            try
            {
                Account account = db.Accounts.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    es.ThrowInfoException("Account to delete does not exist");
                }

                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                db.AccountPages.RemoveRange(db.AccountPages.Where(x => x.AccountId == account.Id));
                db.SaveChanges();

                db
                    .Pages
                    .Where(x => x.AccountId == account.Id)
                    .Select(x => x.Id.Value)
                    .ToList()
                    .ForEach(x => ps.DeletePageFromDb(x, account.Id.Value));

                db.Visits.RemoveRange(db.Visits.Where(x => x.AccountId == account.Id));
                db.SaveChanges();

                db.MessageRecipients.RemoveRange(db.MessageRecipients.Where(x => x.RecipientAccountId == account.Id));
                db.SaveChanges();

                db.Messages.RemoveRange(db.Messages.Where(x => x.AuthorAccountId == account.Id));
                db.SaveChanges();

                db.Remove(account);
                db.SaveChanges();

                fs.DeleteAccountFolder(login);
                Logout();
                db.Database.CommitTransaction();
            }
            catch (Exception) 
            {
                db.Database.RollbackTransaction();
                throw;
            }
        }

        public string GetLoginById(long accountId)
        {
            string result = db.Accounts.Where(x => x.Id == accountId).Select(x => x.Login).FirstOrDefault();
            return result;
        }
    }
}
