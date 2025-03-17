using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Repository;

namespace Shoppping_Jewelry.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new { User = u, RoleName = r.Name }).ToListAsync();
            return View(usersWithRoles);
        }

        public IActionResult UserList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Khai báo biến roles một lần duy nhất ở đầu phương thức
            var roles = await _roleManager.Roles.ToListAsync();

            if (ModelState.IsValid)
            {
                // Cập nhật các thông tin cơ bản
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                // Lấy role hiện tại của user
                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                var currentRole = currentRoles.FirstOrDefault();

                // Lấy role mới từ RoleId
                var newRole = await _roleManager.FindByIdAsync(user.RoleId);
                if (newRole == null)
                {
                    ModelState.AddModelError(string.Empty, "Vai trò mới không tồn tại.");
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    return View(existingUser);
                }

                // Kiểm tra nếu role có thay đổi
                if (currentRole != newRole.Name)
                {
                    // Xóa role cũ
                    if (!string.IsNullOrEmpty(currentRole))
                    {
                        var removeResult = await _userManager.RemoveFromRoleAsync(existingUser, currentRole);
                        if (!removeResult.Succeeded)
                        {
                            AddIdentityErrors(removeResult);
                            ViewBag.Roles = new SelectList(roles, "Id", "Name");
                            return View(existingUser);
                        }
                    }

                    // Thêm role mới
                    var addResult = await _userManager.AddToRoleAsync(existingUser, newRole.Name);
                    if (!addResult.Succeeded)
                    {
                        AddIdentityErrors(addResult);
                        ViewBag.Roles = new SelectList(roles, "Id", "Name");
                        return View(existingUser);
                    }
                }

                // Cập nhật thông tin user
                var updateUser = await _userManager.UpdateAsync(existingUser);
                if (updateUser.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updateUser);
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    return View(existingUser);
                }
            }

            // Sử dụng lại biến roles đã khai báo
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            TempData["error"] = "Đã có lỗi xảy ra";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n ", errors);
            return View(existingUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash); //Tạo User
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email); //Tìm User dựa vào Email
                    var userId = createUser.Id; // Lấy UserId
                    var role = _roleManager.FindByIdAsync(user.RoleId); //Lây RoleId
                    //gán quyền  
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = "Model đang có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Đã xóa role người dùng thành công";
            return RedirectToAction("Index");
        }
    }
}
