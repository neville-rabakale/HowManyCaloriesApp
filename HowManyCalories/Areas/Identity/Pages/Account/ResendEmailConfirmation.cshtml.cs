﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HowManyCalories.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnviroment;

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostEnviroment = hostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            public string Name { get; set; }

        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            // insert email template here
            //Path to file
            var pathtoFile = _hostEnviroment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
                + Path.DirectorySeparatorChar.ToString() + "Confirm_Account_Registration.html";
            var subject = " How Many Calories - Email Confirmation";
            string HtmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathtoFile))
            {
                HtmlBody = streamReader.ReadToEnd();
            }
            //{0}:Subject, {1}:DateTime, {2}:Name, {3}:Email, {4}:Message, {5}:callbackURL 

            string Message = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
            string messageBody = string.Format(HtmlBody,
                subject,
                String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                user.Email,
                user.Email,
                Message,
                callbackUrl
                );


            await _emailSender.SendEmailAsync(Input.Email, subject, messageBody);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }


    }
}
