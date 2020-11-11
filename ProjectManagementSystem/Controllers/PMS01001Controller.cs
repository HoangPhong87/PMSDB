using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Resources;
using ProjectManagementSystem.ViewModels.PMS01001;
using ProjectManagementSystem.WorkerServices;
using ProjectManagementSystem.WorkerServices.Impl;

namespace ProjectManagementSystem.Controllers
{
    public class PMS01001Controller : ControllerBase
    {
        private readonly IPMS01001Service _service;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PMS01001Controller()
            : this(new PMS01001Service())
        {
        }

        /// <summary>
        /// Declare variable tempData email
        /// </summary>
        private string TEMPDATA_EMAIL = "EMAIL";

        /// <summary>
        /// TempData storage
        /// </summary>
        [System.Serializable]
        private class TmpValues
        {
            public string Email { get; set; }
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service">IPMS01001Service</param>
        public PMS01001Controller(IPMS01001Service service)
        {
            this._service = service;
        }

        /// <summary>
        /// Function login into system
        /// </summary>
        /// <returns>View Login</returns>
        [AllowAnonymous]
        public ActionResult Login(string id)
        {
            if (GetLoginUser() != null && (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "PMS08001");
            }

            ViewBag.TimeOut = !string.IsNullOrEmpty(id) && id == "timeout" ? true : false;
            return View();
        }

        /// <summary>
        /// Function login into system[POST]
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string reg = @"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$";

                //Check min length user acount and password
                if (model.UserAccount.Length > 32 || model.Password.Length < 6 || model.Password.Length > 32
                    || !Regex.IsMatch(model.UserAccount, reg) || !Regex.IsMatch(model.Password, reg))
                {
                    ModelState.AddModelError("", string.Format(Messages.E029));
                    return View(model);
                }

                // Check if account has been locked
                if (this._service.IsLockedUser(model.CompanyCode, model.UserAccount))
                {
                    ModelState.AddModelError("", Messages.E014);
                    return View(model);
                }

                // Check UserAccount and Password
                LoginUser user = _service.Login(model.CompanyCode, model.UserAccount, SafePassword.GetSaltedPassword(model.Password));
                if (user != null)
                {
                    if (this._service.CheckLicense(model.CompanyCode) == 0)
                    {
                        ModelState.AddModelError("", string.Format(Messages.E066));
                        return View(model);
                    }

                    FormsAuthentication.SetAuthCookie(user.UserAccount, false);
                    SetLoginUser(user);

                    // Check if password has been expired
                    int effective_month = this._service.GetPasswordEffectiveMonth(GetLoginUser().CompanyCode);
                    if (effective_month == 0)
                    {
                        user.Is_expired_password = false;
                    }

                    if (user.Is_expired_password)
                    {
                        ViewBag.PASSWORD_EXPIRED = string.Format(Messages.I005, effective_month);
                        return View(model);
                    }

                    return RedirectToAction("Index", "PMS08001");
                }
                else
                {
                    // Check if the input UserAccount is existed
                    int? userId = this._service.IsExistedUserId(model.CompanyCode, model.UserAccount);
                    if (userId != null)
                    {
                        // Get list of invalid user from session
                        var _listInvalidUser = Session[Constant.SESSION_INVALID_LOGIN_USER] as List<UserLoginInvalid>;
                        bool isnew = true;

                        if (_listInvalidUser != null)
                        {
                            // get the limit of input password times
                            int limtInput = this._service.GetLimitInputPassword(model.CompanyCode);

                            foreach (var invalidUser in _listInvalidUser)
                            {
                                if (invalidUser.CompanyCode == model.CompanyCode && invalidUser.UserId == userId)
                                {
                                    isnew = false;
                                    invalidUser.InvalidCount++;

                                    if (invalidUser.InvalidCount >= limtInput)
                                    {
                                        // reach the limit input times, lock password
                                        this._service.LockUser(model.CompanyCode, (int)userId);
                                        ModelState.AddModelError("", Messages.E013);
                                        return View(model);
                                    }
                                }
                            }
                        }
                        else
                        {
                            _listInvalidUser = new List<UserLoginInvalid>();
                        }

                        ModelState.AddModelError("", string.Format(Messages.E007, "入力された内容", "アカウント"));
                        if (isnew)
                        {
                            _listInvalidUser.Add(new UserLoginInvalid(model.CompanyCode, (int)userId));
                        }

                        // save list of invalid user to session
                        Session[Constant.SESSION_INVALID_LOGIN_USER] = _listInvalidUser;
                    }
                    else
                    {
                        ModelState.AddModelError("", String.Format(Resources.Messages.E007, "入力された内容", "アカウント"));
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Function logout of system
        /// </summary>
        /// <returns>Logout</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Login", "PMS01001");
        }

        /// <summary>
        /// Function Password Reissue
        /// </summary>
        /// <returns>View Password Reissue</returns>
        [AllowAnonymous]
        public ActionResult PasswordReissue()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            ViewData["passwordReissueSucess"] = TempData["passwordReissueSucess"];

            return View();
        }

        /// <summary>
        /// Function Password Reissue[POST]
        /// </summary>
        /// <param name="model">PasswordReissueViewModel</param>
        /// <returns>View Password Reissue</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult PasswordReissue(PasswordReissueViewModel model)
        {
            bool isError = true;

            if (ModelState.IsValid)
            {
                var user = _service.CheckEmail(model.Email);
                if (user.Count > 0)
                {
                    if (user.Count == 1)
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            DateTime now = Utility.GetCurrentDateTime();
                            string param_value = SafePassword.GetSha256(model.Email + user[0].company_code + now.ToString("yyyy/MM/dd HH:mm:ss"));
                            int count = _service.UpdatePasswordResetManagement(model.Email, param_value, now, user[0].company_code);
                            //Sent mail new password
                            if (count > 0)
                            {
                                var objSentMail = new SentMailAuto();
                                if (objSentMail.SentMail(model.Email, param_value, user[0].user_account) > 0)
                                {
                                    transaction.Complete();
                                    TempData["PasswordReissueSucess"] = Resources.Messages.I001;
                                    isError = false;
                                }
                                else
                                {
                                    ModelState.AddModelError("", Resources.Messages.E012);
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData[TEMPDATA_EMAIL] = new TmpValues() { Email = model.Email };
                        return RedirectToAction("InputCompanyCode", "PMS01001");
                    }
                }
                else
                {
                    ModelState.AddModelError("", String.Format(Resources.Messages.E007, "メールアドレス", "メールアドレス"));
                }
            }
            if (!isError)
            {
                return RedirectToAction("PasswordReissue", "PMS01001");
            }
            return View(model);
        }

        /// <summary>
        /// Function input company code
        /// </summary>
        /// <returns>Input Company Code View</returns>
        [AllowAnonymous]
        public ActionResult InputCompanyCode()
        {
            if (!TempData.ContainsKey(TEMPDATA_EMAIL))
                return RedirectToAction("PasswordReissue", "PMS01001");
            var tmp = TempData[TEMPDATA_EMAIL] as TmpValues;
            var model = new CompanyCodeViewModel();
            model.Email = tmp.Email;
            return View(model);
        }

        /// <summary>
        /// Function input company code[POST]
        /// </summary>
        /// <param name="model">CompanyCodeViewModel</param>
        /// <returns>Input Company Code view</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult InputCompanyCode(CompanyCodeViewModel model)
        {
            bool isError = true;

            if (ModelState.IsValid)
            {
                var user = _service.CheckEmail(model.Email, model.CompanyCode);
                if (user != null)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        DateTime now = Utility.GetCurrentDateTime();
                        string param_value = SafePassword.GetSha256(model.Email + user.company_code + now.ToString("yyyy/MM/dd HH:mm:ss"));
                        int count = _service.UpdatePasswordResetManagement(model.Email, param_value, now, user.company_code);
                        //Sent mail new password
                        if (count > 0)
                        {
                            var objSentMail = new SentMailAuto();
                            if (objSentMail.SentMail(model.Email, param_value, user.user_account) > 0)
                            {
                                transaction.Complete();
                                TempData["PasswordReissueSucess"] = Resources.Messages.I001;
                                isError = false;
                            }
                            else
                            {
                                ModelState.AddModelError("", Resources.Messages.E012);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", String.Format(Resources.Messages.E007, "企業コード", "企業コード"));
                }
            }
            if (!isError)
            {
                return RedirectToAction("InputCompanyCode", "PMS01001");
            }
            return View(model);
        }

        /// <summary>
        /// Function Set New Password
        /// </summary>
        /// <param name="infor">infor</param>
        /// <returns>Set new password View</returns>
        [AllowAnonymous]
        public ActionResult SetNewPassword(string infor)
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            var model = new SetNewPasswordViewModel();
            bool flag = true;
            try
            {
                var passwordReset = _service.GetPasswordResetInfo(infor);
                if (passwordReset != null)
                {
                    var user = _service.CheckEmail(passwordReset.mail_address, passwordReset.company_code);
                    if (user != null)
                    {
                        if (Utility.GetCurrentDateTime() < passwordReset.apply_time.AddHours(_service.GetReissueMailEffectiveTime(user.company_code)))
                        {
                            model.UserId = user.user_sys_id;
                            model.Email = passwordReset.mail_address;
                            model.PasswordLockTarget = user.password_lock_target;
                            model.CompanyCode = user.company_code;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            catch
            {
                flag = false;
            }

            if (!flag)
            {
                return View("ErrorResetPassword");
            }

            return View(model);
        }

        /// <summary>
        /// Function Set New Password[POST]
        /// </summary>
        /// <param name="model">SetNewPasswordViewModel</param>
        /// <returns>Set New Password view</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetNewPassword(SetNewPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.NewPasswordConfirm)
                {
                    ModelState.AddModelError("", string.Format(Resources.Messages.E048));
                }
                else
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        if (_service.UpdatePassword(model.UserId, SafePassword.GetSaltedPassword(model.NewPassword), model.PasswordLockTarget, model.CompanyCode) > 0)
                        {
                            if (_service.DeletePasswordResetInfo(model.Email, model.CompanyCode) > 0)
                            {
                                transaction.Complete();
                                FormsAuthentication.SignOut();
                                Session.Clear();
                                ViewBag.MessageSuccess = String.Format(Resources.Messages.I017);
                            }
                        }
                    }
                }
            }

            return View("SetNewPassword", model);
        }

        /// <summary>
        /// Function Create a object SetNewPasswordViewModel
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="password_lock_target">password_lock_target</param>
        /// <param name="email">email</param>
        /// <returns>Set Password View</returns>
        private SetNewPasswordViewModel MakeSetNewPasswordViewModel(int userId, string password_lock_target, string email)
        {
            var model = new SetNewPasswordViewModel();
            model.UserId = userId;
            model.PasswordLockTarget = password_lock_target;
            model.Email = email;

            return model;
        }
    }
}
