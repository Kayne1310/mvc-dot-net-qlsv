using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using son.Data;
using son.Models;

namespace son.Controllers
{
    public class StudentsController : Controller
    {
        private readonly sonContext _context;

        public StudentsController(sonContext context)
        {
            _context = context;
        }

        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Student model)
        {

                var student = _context.Students.FirstOrDefault(t => t.Username == model.Username);
                if (student == null || student.Password != model.Password)
                {
                    // Xác thực thất bại, đặt thông báo lỗi vào ViewBag và hiển thị lại form đăng nhập
                    ViewBag.ErrorMessage = "Invalid username or password.";
                    return View("Login", model);

                }
                else
                {
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == student.RoleId);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, student.StudentId.ToString()),
                        new Claim(ClaimTypes.Name, student.Username),
                        new Claim(ClaimTypes.Role, "Students"),
                        new Claim("Email", student.Email ?? string.Empty),
                        new Claim("Address", student.Address ?? string.Empty)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("HomePage", "Students");
                }
                return View();

        }
        #endregion

        #region Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Students");
            }

            var studentId = int.Parse(userId);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound();
            }

            var model = new Student
            {
                StudentId = student.StudentId,
                Name = student.Name,
                Address = student.Address ?? string.Empty,
                PhoneNumber = student.PhoneNumber,
                Email = student.Email ?? string.Empty,
                MajorName = student.MajorName,
                Username = student.Username,
            };
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", student.RoleId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Student model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // lấy dữ liệu thông tin khi đăng nhập thành công
            if (userId == null)
            {
                return RedirectToAction("Login", "Students");
            }

            var studentId = int.Parse(userId);
            if (studentId != model.StudentId)
            {
                return NotFound();
            }
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
                if (student == null)
                {
                    return NotFound();
                }
                student.Name = model.Name;
                student.Address = model.Address ?? string.Empty; // Xử lý giá trị null
                student.PhoneNumber = model.PhoneNumber;
                student.MajorName = model.MajorName;
                student.Email = model.Email ?? string.Empty;     // Xử lý giá trị null

                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("HomePage", "Students");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Unable to save changes. The student was updated or deleted by another user.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving changes: {ex.Message}");
            }
            // Nếu có lỗi xảy ra, cần điền lại dữ liệu cho model để hiển thị lại view
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", model.RoleId);

            return View(model);
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Students");
        }
        #endregion

        #region Change password

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            {
                return View();
            }

            // Get the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "Home"); // Or another action for unauthorized access
            }

            var student = await _context.Students.FindAsync(int.Parse(userId));
            if (student == null)
            {
                return NotFound();
            }

            // Validate old password
            if (student.Password != oldPassword)
            {
                return View();
            }

            // Validate new password and confirmation
            if (newPassword != confirmNewPassword)
            {
                return View();
            }

            // Update password
            student.Password = newPassword;
            _context.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("HomePage", "Students");
        }

        #endregion

        [HttpGet]
        public IActionResult HomePage()
        {
            return View();
        }
    }
}
