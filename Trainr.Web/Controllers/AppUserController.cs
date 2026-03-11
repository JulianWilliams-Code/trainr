using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainr.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trainr.Web.Controllers
{
    public class AppUserController : Controller
    {
        private IApplicationUserRepo iApplicationUserRepo;
        private UserManager<ApplicationUser> userManager;

        public AppUserController(IApplicationUserRepo applicationUserRepo, UserManager<ApplicationUser> UserManager)
        {
            this.iApplicationUserRepo = applicationUserRepo;
            this.userManager = UserManager;
        }

        public string GetCurrentRoles(string Id)
        {
            return iApplicationUserRepo.GetCurrentRoles(Id);
        }

        public string GetAvailableRoles(string Id)
        {
            return iApplicationUserRepo.GetAvailableRoles(Id);
        }

        [HttpGet]
        public IActionResult AssignAppUsers()
        {
            ViewData["AppUsers"] = new SelectList(iApplicationUserRepo.ListAllApplicationUsers(), "Id", "fullName");

            return View();
        }
        [HttpPost]
        public IActionResult AssignAppUsers(string submitButton, string ddlAppUsers, List<string> availableRoles, List<string> currentRoles)

        {
            string userID = ddlAppUsers;
            ApplicationUser user = iApplicationUserRepo.FindUser(userID);


            if (submitButton == "AddRoles")
            {
                userManager.AddToRolesAsync(user, availableRoles).Wait();
            }
            else if (submitButton == "RemoveRoles")
            {
                userManager.RemoveFromRolesAsync(user, currentRoles).Wait();
            }

            ViewData["AppUsers"] = new SelectList(iApplicationUserRepo.ListAllApplicationUsers(), "Id", "fullName", userID);


            return View();
        }
    }
}