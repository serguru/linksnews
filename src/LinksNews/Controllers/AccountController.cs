using LinksNews.Core;
using LinksNews.Services;
using LinksNews.Services.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LinksNews.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService accountService;
        private readonly ExecutionService es;
        private readonly FileService fs;

        public AccountController(
           AccountService accountService,
           ExecutionService es,
           FileService fs
            )
        {
            this.accountService = accountService;
            this.es = es;
            this.fs = fs;
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] LoginData loginData)
        {
            return es.Execute(
                () =>
                {
                    Account account = accountService.LogAccountIn(loginData);

                    if (account == null)
                    {
                        es.ThrowInfoException("Cannot login {0}", loginData.Login);
                    }
                    if (account.Locked)
                    {
                        es.ThrowInfoException("Account {0} is locked", account.Login);
                    }

                    return new JsonResult(new GenericResult(account));
                },
                false
            );
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return es.Execute(() =>
            {
                accountService.Logout();
                return new JsonResult(new GenericResult());
            }, false);
        }

        [Route("register")]
        [HttpPost]
        public IActionResult RegisterNewAccount([FromBody] RegistrationData registration)
        {
            return es.Execute(() =>
            {
                Account account = accountService.RegisterNewAccount(registration);
                account.Password = null;

                LoginData loginData = new LoginData();
                loginData.Login = registration.Login;
                loginData.Password = registration.Password;
                accountService.PutLoginDataToCookies(loginData);

                return new JsonResult(new GenericResult(account));
            }, false);
        }

        [Route("account")]
        [HttpPost]
        public IActionResult GetAccount()
        {
            return es.Execute(() =>
            {
                if (string.IsNullOrWhiteSpace(accountService.Login))
                {
                    return new JsonResult(new GenericResult());
                }

                Account account = accountService.GetFullAccountByLogin(accountService.Login);
                if (account == null)
                {
                    return new JsonResult(new GenericResult());
                }

                if (account.Locked)
                {
                    es.ThrowInfoException("Account {0} is locked", account.Login);
                }

                account.Password = null;

                return new JsonResult(new GenericResult(account));
            }, false);
        }

        [Route("saveAccount")]
        [HttpPost]
        public IActionResult SaveAccount([FromBody] Account account)
        {
            return es.Execute(() =>
            {
                if (account == null || string.IsNullOrWhiteSpace(account.Login))
                {
                    es.ThrowException("No account to save");
                }

                // accountService.Login already checked and cannot be null here
                if (!accountService.Login.Equals(account.Login, StringComparison.OrdinalIgnoreCase))
                {
                    es.ThrowInfoException("Account to save does not match logged in account;");
                }

                accountService.SaveAccount(account);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [Route("savePassword")]
        [HttpPost]
        public IActionResult SavePassword([FromBody]ChangePasswordData data)
        {
            return es.Execute(() =>
            {
                if (data == null || !data.Valid())
                {
                    es.ThrowInfoException("No data to change password");
                }

                if (string.Equals(data.OldPassword, data.NewPassword))
                {
                    es.ThrowInfoException("Passwords are the same");
                }

                if (!accountService.Login.Equals(data.Login, StringComparison.OrdinalIgnoreCase))
                {
                    es.ThrowInfoException("Account to save does not match logged in account;");
                }

                accountService.SavePassword(data);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [Route("deleteAccount")]
        [HttpPost]
        public IActionResult DeleteAccount([FromBody] string login)
        {
            return es.Execute(() =>
            {
                if (string.IsNullOrWhiteSpace(login) || !string.Equals(login, accountService.Login))
                {
                    es.ThrowInfoException("No account to delete");
                }

                if (!accountService.Login.Equals(login, StringComparison.OrdinalIgnoreCase))
                {
                    es.ThrowInfoException("Account to delete does not match logged in account;");
                }

                accountService.DeleteAccount(login);
                return new JsonResult(new GenericResult());
            }, true);
        }

        [HttpPost]
        [Route("uploadImage4Account")]
        public IActionResult UploadImage4Account(IFormFile file)
        {
            return es.Execute(() =>
            {
                string path = fs.UploadAccountImage(accountService.Login, file).Result;
                return new JsonResult(new GenericResult(new { imageUrl = path }));
            }, true);
        }

        [HttpPost]
        [Route("deleteImage4Account")]
        public IActionResult DeleteImage4Account([FromBody] string login)
        {
            return es.Execute(() =>
            {
                fs.DeleteAccountImage(login);
                return new JsonResult(new GenericResult());
            }, true);
        }
    }
}