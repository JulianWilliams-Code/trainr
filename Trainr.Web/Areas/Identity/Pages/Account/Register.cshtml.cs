using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Trainr.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Trainr.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepo iApplicationUserRepo;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, IApplicationUserRepo applicationUserRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this.iApplicationUserRepo = applicationUserRepo;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "FirstName")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "LastName")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Phone")]
            public string Phone { get; set; }

            [Required]
            [Display(Name = "UserRole")]
            public string UserRole { get; set; }

            [Display(Name = "Choose Sport Type")]
            public string sportType { get; set; }

            [Display(Name = "Choose Position")]
            public string AthletePosition { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                ApplicationUser user; //variable

                if (Input.UserRole == "Athlete")
                {

                    Athlete athlete = iApplicationUserRepo.FindAthleteSportTypeAndPosition(Input.sportType, Input.AthletePosition);

                    user = new Athlete(Input.FirstName, Input.LastName, Input.sportType, Input.Email, Input.Phone, Input.Password, Input.AthletePosition);

                }
                else if (Input.UserRole == "Trainer")
                {

                    Trainer trainer = iApplicationUserRepo.FindTrainerSportType(Input.sportType);

                    user = new Trainer(Input.FirstName, Input.LastName, Input.sportType, Input.Email, Input.Phone, Input.Password);

                }
                else
                {

                    user = new ApplicationUser(Input.FirstName, Input.LastName, Input.Email, Input.Phone, Input.Password);

                }
                //var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (Input.UserRole == "Athlete")
                    {
                        Athlete athlete = iApplicationUserRepo.FindAthleteSportTypeAndPosition(Input.sportType, Input.AthletePosition);
                        athlete.Id = user.Id;
                        await iApplicationUserRepo.UpdateAthleteSportTypeAndPositionAsync(athlete);

                    }
                    else if (Input.UserRole == "Trainer")
                    {
                        Trainer trainer = iApplicationUserRepo.FindTrainerSportType(Input.sportType);
                        trainer.Id = user.Id;
                        await iApplicationUserRepo.UpdateTrainerSportTypeAsync(trainer);

                    }

                    await _userManager.AddToRoleAsync(user, Input.UserRole);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
