using AuthApp.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Controllers
{
    public class Account : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signInManager;

       

        public Account(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)  // Added validation for model state**
            {
                return View(registerVM);  // Return the view if the model is not valid
            }

            var Newuser = new IdentityUser
            {
                UserName = registerVM.Username,
                Email = registerVM.Email
            };

            var adduser = await userManager.CreateAsync(Newuser, registerVM.Password);

            if (adduser.Succeeded)
            {
                // Optional: You can implement email confirmation logic here if necessary

                return RedirectToAction("Index", "Home");
            }

            // Added error handling for failed user creation**
            foreach (var error in adduser.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description); // Show the error messages in the view
            }

            return View(registerVM);  // Return the view with errors
        }

        public IActionResult AddRoles()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddRoles")]
        public async Task<IActionResult> AddRoles(AddRoleVM addRoleVM)
        {
            if (!ModelState.IsValid)
            {
                return View(addRoleVM);
            }
            var newrole = new IdentityRole
            {
                //Id = addRoleVM.Id,
                Name = addRoleVM.RoleName
            };

            var roleExist = await roleManager.RoleExistsAsync(newrole.ToString());
            if (!roleExist)
            {
                var createdrole = await roleManager.CreateAsync(new IdentityRole(newrole.Name));
                if (createdrole.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(addRoleVM);

        }

        // Display roles and Users
        [HttpGet]
        public async  Task<IActionResult> RoleList()
        {
            var totalroles = await roleManager.Roles.ToListAsync();
            return View(totalroles);
        }
        [Authorize]
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var totalUsers = await userManager.Users.ToListAsync();
            var userRoles = new List<UserWithRolesVM>();

            foreach (var user in totalUsers)
            {
                var roles = await userManager.GetRolesAsync(user); // Fetch roles for the user
                userRoles.Add(new UserWithRolesVM
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Roles = string.Join(" ",roles) // Join roles into a single string
                });
            }

            return View(userRoles);
        }



        // Assign Roles to users
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            // Find the user by their ID
            var exist = await userManager.FindByIdAsync(id);

            if (exist == null) // Check if the user exists
            {
                // If user does not exist, show a not found message or redirect
                return NotFound($"User with ID {id} not found.");
            }

            // Get all available roles
            var roles = roleManager.Roles.Select(r => r.Name).ToList();

            // Prepare the ViewModel
            var user = new RoleAssignVm
            {
                Username = exist.UserName,
                AvailableRoles = roles
               
            };

            return View(user);
        }

        [HttpPost]
        [ActionName("AssignRole")]
        public async Task<IActionResult> AssignRole(RoleAssignVm vm)
        {
            var exist = await userManager.FindByIdAsync(vm.Id);
            if (exist == null)
            {
                return NotFound("Username not exist ");
            }

            
            var userrole = vm.SelectedRole;

            var addedrole = await userManager.AddToRoleAsync(exist, userrole);

            if (addedrole.Succeeded)
            {
                // Redirect to a success page or show a success message
                return RedirectToAction("Index", "Home");
            }

            // If there are errors, add them to the model state
            foreach (var error in addedrole.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);




        }
        public async Task<IActionResult> Login(LoginVm vm)
        {
            var signinResult = await signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);

            if (signinResult != null && signinResult.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(vm.ReturnUrl))
                {
                    return Redirect(vm.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            return View();

           
        }
        

    }
}
