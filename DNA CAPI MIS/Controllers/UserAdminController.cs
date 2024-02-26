using DNA_CAPI_MIS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DNA_CAPI_MIS.DAL;

namespace DNA_CAPI_MIS.Controllers
{
    //[Authorize(Roles = "Admin,Customer")]
    public class UsersAdminController : Controller
    {
        ProjectContext db = new ProjectContext();
        public UsersAdminController()
        {
          
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DealerIndex()
        {
            List<int> ids = db.CustomerUser.Select(x => x.UserID).ToList();                      
            var users = await UserManager.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
            List<CustomerUser> dealerName = db.CustomerUser.Where(x => ids.Contains(x.UserID)).ToList();

            ViewBag.dealerName = dealerName;
            return View(users);
        }


        [Authorize(Roles = "Survey Admin, Admin")]
        public async Task<ActionResult> DealerUserIndex()
        {
            if (User.IsInRole("Admin"))
            {
                var users = await UserManager.Users.ToListAsync();
                return View(users);
            } 
            else
            {
                int userid = Convert.ToInt32(User.Identity.GetUserId());
                CustomerBranch CusBranch = GetCustBranch();
                List<int> ids = db.CustomerUser.Where(x => x.CustomerBranchID == CusBranch.ID && x.UserID != userid).Select(x => x.UserID).ToList();

                var users = await UserManager.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
                return View(users);
            }
        }

        //
        // GET: /Users/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int id)
        {
            if (id > 0)
            {
                // Process normally:
                var user = await UserManager.FindByIdAsync(id);
                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                return View(user);
            }
            // Return Error:
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DetailsDealer(int id)
        {
            if (id > 0)
            {
                // Process normally:
                var user = await UserManager.FindByIdAsync(id);

                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                return View(user);
            }
            // Return Error:
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> DetailsDealerUser(int id)
        {
            if (id > 0)
            {
                // Process normally:
                var user = await UserManager.FindByIdAsync(id);

                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                return View(user);
            }
            // Return Error:
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }


        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateDealer()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
            //ViewBag.City = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.City).Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

            ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");
            ViewBag.Region = new SelectList(db.CustomerRegion.Select(x => new { x.ID, x.Name }).ToList(), "ID", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateDealer(RegisterViewModel userViewModel, string Dealer, string City, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                selectedRoles = new string[] { "Customer", "Surveyor", "Survey Designer", "Survey Admin", "Field Supervisor", "Project Manager", "QC" };
                CustomerBranch CustBranc = new CustomerBranch();
                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    try
                    {

                        CustBranc.Name = Dealer;
                        CustBranc.IsActive = true;
                        CustBranc.DisplayOrder = 1;
                        CustBranc.CustomerID = 1;
                        CustBranc.CityID = Convert.ToInt32(City);
                        db.CustomerBranch.Add(CustBranc);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }
                    try
                    {
                        CustomerUser CustUser = new CustomerUser();
                        CustUser.CustomerBranchID = CustBranc.ID;
                        CustUser.UserID = user.Id;
                        db.CustomerUser.Add(CustUser);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                            ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                    ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                    return View();

                }
                return RedirectToAction("DealerIndex");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
            ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

            return View();
        }

        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCustomerDealer()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
            //ViewBag.City = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.City).Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

            //ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");
            //ViewBag.Region = new SelectList(db.CustomerRegion.Select(x => new { x.ID, x.Name }).ToList(), "ID", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCustomerDealer(RegisterViewModel userViewModel, int Dealerid, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                selectedRoles = new string[] { "Customer", "Surveyor", "Survey Designer", "Survey Admin", "Field Supervisor", "Project Manager", "QC" };
                CustomerBranch CustBranc = new CustomerBranch();
                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    try
                    {
                        CustomerUser CustUser = new CustomerUser();
                        CustUser.CustomerBranchID = Dealerid;
                        CustUser.UserID = user.Id;
                        db.CustomerUser.Add(CustUser);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                            //ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                    //ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                    return View();

                }
                return RedirectToAction("DealerIndex");
            }
            ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
           
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
            //ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

            return View();
        }


        // GET: /Users/CreateDealerUser
        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> CreateDealerUser()
        {
            int id = 0;
            if (int.TryParse(User.Identity.GetUserId(), out id))
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }


                var userRoles = await UserManager.GetRolesAsync(user.Id);
                //ViewBag.RoleId = new SelectList(userRoles.ToList(), "Name", "Name");
                ViewBag.RoleId = userRoles.Where(x => x != "Customer").ToList();
                //return View(new EditUserViewModel()
                //{
                //    Id = user.Id,
                //    Email = user.Email,
                //    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                //    {
                //        Selected = userRoles.Contains(x.Name),
                //        Text = x.Name,
                //        Value = x.Name
                //    })
                //});
            }
            //Get the list of Roles
            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }


        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> CreateDealerUser(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            int id = 0;
            if (int.TryParse(User.Identity.GetUserId(), out id))
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);
                ViewBag.RoleId = userRoles.Where(x => x != "Customer").ToList();
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                CustomerBranch CustBranc = new CustomerBranch();
                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    try
                    {
                        CustomerUser CustUser = new CustomerUser();
                        CustomerBranch Custb = GetCustBranch();
                        int BranchId = 0;
                        BranchId = Custb == null ? -1 : Custb.ID;
                        if (BranchId > 0)
                        {
                            CustUser.CustomerBranchID = BranchId;
                            CustUser.UserID = user.Id;
                            db.CustomerUser.Add(CustUser);
                            db.SaveChanges();
                        }
                        else
                        {

                        }

                    }
                    catch (Exception e)
                    {

                    }
                    if (selectedRoles != null)
                    {

                        List<string> selectedRoleslst = new List<string>();
                        selectedRoleslst = selectedRoles.ToList();
                        selectedRoleslst.Add("Customer");

                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoleslst.ToArray());
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                            ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");

                    //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
                    ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

                    return View();

                }
                return RedirectToAction("DealerUserIndex");
            }
            //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            //ViewBag.Dealers = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.Name, x.ID }).ToList(), "ID", "Name");
            //ViewBag.City = new SelectList(db.City.Select(x => new { x.ID, x.Name }).Distinct().ToList(), "ID", "Name");

            return View();
        }

        public CustomerBranch GetCustBranch()
        {
            string Userid = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(Userid))
            {
                int UserId = Convert.ToInt32(Userid);
                //var Currentuser = UserManager.FindById(Convert.ToInt32(Userid));
                CustomerUser Cuser = db.CustomerUser.Where(x => x.UserID == UserId).FirstOrDefault();
                if (Cuser != null)
                {
                    return Cuser.CustomerBranch;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            ////await UserManager.FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            //var
            //ApplicationUser users = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        //
        // GET: /Users/Edit/1
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditDealer(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                CustomerBranch BranchName = db.CustomerUser.Where(x => x.UserID == id).Single().CustomerBranch;
                ViewBag.Branch = BranchName.Name;
                ViewBag.City = new SelectList(db.City.Distinct().ToList(), "ID", "Name",BranchName.City.ID);

                ViewBag.Region = new SelectList(db.CustomerRegion.Select(x => new { x.ID, x.Name }).ToList(), "ID", "Name");
                ViewBag.SelectedRegion = db.RDXCustomerRegion.Where(x => x.CityID == BranchName.CityID).First().CityID;
                
                    var userRoles = await UserManager.GetRolesAsync(user.Id);
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditDealer([Bind(Include = "Email,Id")] EditUserViewModel editUser, string Dealer, string City, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = new string[] { "Customer", "Surveyor", "Survey Designer", "Survey Admin", "Field Supervisor", "Project Manager", "QC" };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                CustomerBranch CustBranc = new CustomerBranch();
                CustomerUser CustUser = db.CustomerUser.Where(x => x.UserID == editUser.Id).SingleOrDefault();
                CustUser.CustomerBranch.CityID = Convert.ToInt32(City);
                CustUser.CustomerBranch.Name = Dealer;
                db.SaveChanges();
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("DealerIndex");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> EditDealerUser(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                List<string> selectedRoles = new List<string> { "Customer", "Surveyor", "Survey Designer", "Survey Admin", "Field Supervisor", "Project Manager", "QC" };

                userRoles = userRoles.ToList();
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    RolesList = RoleManager.Roles.Where(x => x.Name != "Customer" && selectedRoles.Contains(x.Name)).ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> EditDealerUser([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                List<string> selectedRoleslst = new List<string>();
                selectedRoleslst = selectedRole.ToList();
                selectedRoleslst.Add("Customer");

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRoleslst.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("DealerUserIndex");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }




        //GET: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);

                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDealer(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> DeleteDealerUser(int id)
        {
            if (id > 0)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var user = await UserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    var result = await UserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }

        [HttpPost, ActionName("DeleteDealer")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmedDealer(int id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var user = await UserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    var result = await UserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("DealerIndex");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }

        [HttpPost, ActionName("DeleteDealerUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Survey Admin")]
        public async Task<ActionResult> DeleteConfirmedDealerUser(int id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var user = await UserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    var result = await UserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("DealerUserIndex");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }


        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Import()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.FailedUsers = "";
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Import(string usersToImport, string userPassword, params string[] selectedRoles)
        {
            List<string> users = new List<string>();
            List<string> failedUsers = new List<string>();
            string errMessage = "";

            try
            {
                string[] users1 = usersToImport.Split('\n');
                foreach (string u1 in users1)
                {
                    if (u1.Contains(','))
                    {
                        string[] users2 = u1.Trim().Split(',');
                        foreach (string u2 in users2)
                        {
                            users.Add(u2.Trim());
                        }
                    }
                    else
                    {
                        users.Add(u1.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.AdditionalErrorMessage = "Unrecognized list of User IDs, please separate the users by a new line or comma";
                ViewBag.FailedUsers = string.Join(",", failedUsers);
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                return View();
            }

            foreach (string nuser in users)
            {
                string newuser = nuser.Trim();
                if (newuser.Length > 0)
                {
                    if (!newuser.Contains('@'))
                    {
                        newuser = newuser + "@dna.com.sa";
                    }
                    var user = new ApplicationUser { UserName = newuser, Email = newuser };
                    var adminresult = await UserManager.CreateAsync(user, nuser.Trim() + userPassword);

                    //Add User to the selected Roles 
                    if (adminresult.Succeeded)
                    {
                        if (selectedRoles != null)
                        {
                            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        }
                    }
                    else
                    {
                        failedUsers.Add(nuser);
                        errMessage = string.Join(",", adminresult.Errors);
                        break;
                    }
                }
            }

            ViewBag.AdditionalErrorMessage = errMessage;
            ViewBag.FailedUsers = string.Join(",", failedUsers);
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

    }
}
