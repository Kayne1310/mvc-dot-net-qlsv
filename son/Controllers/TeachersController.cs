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
using Microsoft.AspNetCore.Authorization;

namespace son.Controllers
{
    public class TeachersController : Controller
    {
        private readonly sonContext _context;

        public TeachersController(sonContext context)
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
		public async Task<IActionResult> Login(Teacher model)
		{
				var teacher = _context.Teachers.FirstOrDefault(t => t.Username == model.Username);
				if (teacher == null || teacher.Password != model.Password)
				{
					// Xác thực thất bại, đặt thông báo lỗi vào ViewBag và hiển thị lại form đăng nhập
					ViewBag.ErrorMessage = "Invalid username or password.";
					return View("Login", model);

				}
				else
				{
					var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == teacher.RoleId);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.NameIdentifier, teacher.TeacherId.ToString()),
						new Claim(ClaimTypes.Name, teacher.Username),
						new Claim(ClaimTypes.Role, "Teachers"),
						new Claim("Email", teacher.Email ?? string.Empty),
						new Claim("Address", teacher.Address ?? string.Empty)
					};
					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
					await HttpContext.SignInAsync(claimsPrincipal);
					return RedirectToAction("DashBoard", "Teachers");
				}
				return View();

		}
		#endregion

		#region Register for Student
		[HttpGet]
		public IActionResult RegisterST()
		{
			return View(); 
		}

		[HttpPost]
		public async Task<IActionResult> RegisterST(Student model)
		{
            try
            {
                var existingStudent = _context.Students.FirstOrDefault(t => t.Username == model.Username);
                if (existingStudent != null)
                {
                    ViewBag.ErrorMessage = "Username already exists. Please choose a different username.";                
                    return View();
                }

				var student = new Student();
				student.Username = model.Username;
				student.Name = model.Name;
                student.Password = model.Password;
				student.Email = model.Email;
				student.Address = model.Address;
				student.PhoneNumber = model.PhoneNumber;	
                student.RoleId = model.RoleId;
                student.MajorName = model.MajorName;

                _context.Students.Add(student);
                _context.SaveChanges();
                TempData["ok"] = "Create Student Successful!";
                return RedirectToAction("DashBoard", "Teachers");
            }
            catch (Exception ex)
            {
                var mess = $"{ex.Message} shh";
            }
            return View();
        }
        #endregion

        #region Register for Teacher

        [HttpGet]
        public IActionResult RegisterTE()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTE(Teacher model)
        {
            try
            {
                var existingTeacher = _context.Teachers.FirstOrDefault(t => t.Username == model.Username);
                if (existingTeacher != null)
                {
                    ViewBag.ErrorMessage = "Username already exists. Please choose a different username.";
                    return View();
                }

                var teacher = new Teacher();
                teacher.Username = model.Username;
                teacher.Name = model.Name;
                teacher.Password = model.Password;
                teacher.Email = model.Email;
                teacher.Address = model.Address;
                teacher.PhoneNumber = model.PhoneNumber;
                teacher.RoleId = model.RoleId;
   

                _context.Teachers.Add(teacher);
                _context.SaveChanges();
                TempData["ok"] = "Create Student Successful!";
                return RedirectToAction("DashBoard", "Teachers");
            }
            catch (Exception ex)
            {
                var mess = $"{ex.Message} shh";
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
                return RedirectToAction("Login", "Teachers");
            }

            var teacherId = int.Parse(userId);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(s => s.TeacherId == teacherId);

            if (teacher == null)
            {
                return NotFound();
            }

            var model = new Teacher
            {
                TeacherId = teacher.TeacherId,
                Name = teacher.Name,
                Address = teacher.Address ?? string.Empty,
                PhoneNumber = teacher.PhoneNumber,
                Email = teacher.Email ?? string.Empty,
                Username = teacher.Username,
            };
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", teacher.RoleId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Teacher model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // lấy dữ liệu thông tin khi đăng nhập thành công
            if (userId == null)
            {
                return RedirectToAction("Login", "Teachers");
            }

            var teacherId = int.Parse(userId);
            if (teacherId != model.TeacherId)
            {
                return NotFound();
            }
            try
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(s => s.TeacherId == teacherId);
                if (teacher == null)
                {
                    return NotFound();
                }
                teacher.Name = model.Name;
                teacher.Address = model.Address ?? string.Empty; // Xử lý giá trị null
                teacher.PhoneNumber = model.PhoneNumber;
                teacher.Email = model.Email ?? string.Empty;     // Xử lý giá trị null

                _context.Update(teacher);
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
            return RedirectToAction("Login", "Teachers");
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

            var teacher = await _context.Teachers.FindAsync(int.Parse(userId));
            if (teacher == null)
            {
                return NotFound();
            }

            // Validate old password
            if (teacher.Password != oldPassword)
            {
                return View();
            }

            // Validate new password and confirmation
            if (newPassword != confirmNewPassword)
            {
                return View();
            }

            // Update password
            teacher.Password = newPassword;
            _context.Update(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction("DashBoard", "Teachers");
        }

        #endregion

        #region Management Teacher

        [HttpGet]
        public async Task<IActionResult> ManagementTE()
        {
            var teachers = await _context.Teachers
                .Select(e => new Teacher
                {
                    TeacherId = e.TeacherId,
                    Name = e.Name,
                    Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                    Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                    PhoneNumber = e.PhoneNumber,
                })
                .ToListAsync();
                return View(teachers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagementTE(string actionType)
        {
            switch (actionType)
            {
                case "list":
                    var teachers = await _context.Teachers
                        .Select(e => new Teacher
                        {
                            TeacherId = e.TeacherId,
                            Name = e.Name,
                            Address = e.Address,
                            Email = e.Email,
                            PhoneNumber = e.PhoneNumber,
                        })
                        .ToListAsync();
                    return View("ManagementTE", teachers); 

                default:
                    return BadRequest("Invalid action type"); 
            }

        }
        #endregion

        #region Edit Teacher

        [HttpGet]
        public async Task<IActionResult> EditTE(int id)
        {
            var teacher = await _context.Teachers
                .Include(s => s.Role)
                .Select(e => new Teacher
                {
                    TeacherId = e.TeacherId,
                    Name = e.Name,
                    Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                    Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                    PhoneNumber = e.PhoneNumber,
                })
                .FirstOrDefaultAsync(s => s.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            var model = new Teacher
            {
                TeacherId = teacher.TeacherId,
                Name = teacher.Name,
                Address = teacher.Address,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,

            };
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", teacher.RoleId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTE(int id, Teacher model)
        {
            try
            {
                var teacher = await _context.Teachers 
                    .Include(s => s.Role)
                    .Select(e => new Teacher
                    {
                        TeacherId = e.TeacherId,
                        Name = e.Name,
                        Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                        Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                        PhoneNumber = e.PhoneNumber,
                        Username = e.Username,
                        Password = e.Password,
                        RoleId = e.RoleId,
                    })
                    .FirstOrDefaultAsync(s => s.TeacherId == id);

                if (teacher == null)
                {
                    return NotFound();
                }

                teacher.Name = model.Name;
                teacher.Address = model.Address;
                teacher.Email = model.Email;
                teacher.PhoneNumber = model.PhoneNumber;

                _context.Update(teacher);
                await _context.SaveChangesAsync();

                return RedirectToAction("ManagementTE", "Teachers");
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

        #region Delete Teacher

        [HttpGet]
        public async Task<IActionResult> DeleteTE(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost, ActionName("DeleteTE")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedTE(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return RedirectToAction("Login", "Teachers");
            }

            // Set TeacherId to null in related classes
            var classes = await _context.Classes
                .Where(c => c.TeacherId == id)
                .ToListAsync();
            if (classes != null)
            {
                foreach (var classItem in classes)
                {
                    classItem.TeacherId = null;
                }
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManagementTE", "Teachers");
        }


        #endregion

        #region Management Student

        [HttpGet]
        public async Task<IActionResult> ManagementST()
        {
            var students = await _context.Students
                .Select(e => new Student
                {
                    StudentId = e.StudentId,
                    Name = e.Name,
                    Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                    Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                    PhoneNumber = e.PhoneNumber,
                    MajorName = e.MajorName,
                })
                .ToListAsync();
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagementST(string actionType)
        {
            switch (actionType)
            {
                case "list":
                    var students = await _context.Students
                        .Select(e => new Student
                        {
                            StudentId = e.StudentId,
                            Name = e.Name,
                            Address = e.Address,
                            Email = e.Email,
                            PhoneNumber = e.PhoneNumber,
                            MajorName= e.MajorName,
                        })
                        .ToListAsync();
                    return View("ManagementST", students);

                default:
                    return BadRequest("Invalid action type");
            }

        }

        #endregion

        #region Edit Student

        [HttpGet]
        public async Task<IActionResult> EditST(int id)
        {
            var student = await _context.Students
                .Include(s => s.Role)
                .Select(e => new Student
                {
                    StudentId = e.StudentId,
                    Name = e.Name,
                    Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                    Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                    PhoneNumber = e.PhoneNumber,
                    MajorName = e.MajorName,
                })
                .FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new Student
            {
                StudentId = student.StudentId,
                Name = student.Name,
                Address = student.Address,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                MajorName = student.MajorName,  
            };
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", student.RoleId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditST(int id, Student model)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Role)
                    .Select(e => new Student
                    {
                        StudentId = e.StudentId,
                        Name = e.Name,
                        Address = e.Address != null ? e.Address : string.Empty,  // Xử lý giá trị null
                        Email = e.Email != null ? e.Email : string.Empty,        // Xử lý giá trị null
                        PhoneNumber = e.PhoneNumber,
                        Username = e.Username,
                        Password = e.Password,
                        RoleId = e.RoleId,
                        MajorName = e.MajorName,
                    })
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                if (student == null)
                {
                    return NotFound();
                }

                student.Name = model.Name;
                student.Address = model.Address;
                student.Email = model.Email;
                student.PhoneNumber = model.PhoneNumber;
                student.MajorName = model.MajorName;

                _context.Update(student);
                await _context.SaveChangesAsync();

                return RedirectToAction("ManagementST", "Teachers");
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

        #endregion Stuen

        #region Delete Student

        [HttpGet]
        public async Task<IActionResult> DeleteST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("DeleteST")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedST(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction("Login", "Students");
            }

            var classes = await _context.Grades
                .Include(c => c.Student)
                .Where(c => c.StudentId == id)
                .ToListAsync();
            if (classes != null)
            {
               foreach (var classItem in classes)
                {
                    classItem.StudentId = null;
                }
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Delete Student Successfully!";
            return RedirectToAction("ManagementST", "Teachers");
        }


        #endregion

        [HttpGet]
        public IActionResult DashBoard()
        {
            return View();
        }
    }
}
