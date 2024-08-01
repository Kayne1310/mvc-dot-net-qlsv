using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using son.Data;
using son.Models;
using son.ViewModels;

namespace son.Controllers
{
    public class GradesController : Controller
    {
        private readonly sonContext _context;

        public GradesController(sonContext context)
        {
            _context = context;
        }

        // GET: Grades
        [Authorize(Roles = "Teachers")]
        public async Task<IActionResult> Index()
        {
            var sonContext = _context.Grades
                .Include(g => g.Class)
                .Include(g => g.Course)
                .Include(g => g.Student)
                .Select(g => new GradeViewModel
                {
                    GradeStudent = g.GradeStudent,
                    StudentName = g.Student.Name,
                    ClassName = g.Class.ClassName,
                    CourseName = g.Course.CourseName,
                    GradeId = g.GradeId,
                });
            return View(await sonContext.ToListAsync());
        }



        // GET: Grades/Create
        [Authorize(Roles = "Teachers")]
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "Name");
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GradeId,GradeStudent,ClassName,CourseName,Name,CourseId,ClassId,StudentId")] Grade grade)
        {

         
      
            _context.Add(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "Name", grade.StudentId);
            return View(grade);
        }

        // GET: Grades/Edit/5
        [Authorize(Roles = "Teachers")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "Name", grade.StudentId);
            return View(grade);
        }


        // POST: Grades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GradeId,GradeStudent,ClassName,CourseName,Name,CourseId,ClassId,StudentId")] Grade grade)
        {
            if (id != grade.GradeId)
            {
                return NotFound();
            }

           

            try
            {
                _context.Update(grade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GradeExists(grade.GradeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "Name", grade.StudentId);
            return RedirectToAction("Index", "Grades");
        }

        // GET: Grades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades
                .Include(g => g.Class)
                .Include(g => g.Course)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(m => m.GradeId == id);
            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Students")]
        public async Task<IActionResult> GradeStudent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Students");
            }
            var studentId = int.Parse(userId);
            var GradeStudent = await _context.Grades
                             .Include(g => g.Student)
                             .Include(g=>g.Class)
                             .Include(g=>g.Course)
                             .FirstOrDefaultAsync(m => m.GradeId == studentId);

            var model = new List<GradeViewModel>
         {
        new GradeViewModel
        {
            ClassName = GradeStudent.Class.ClassName,
            GradeStudent = GradeStudent.GradeStudent,
            CourseName = GradeStudent.Course.CourseName,
            GradeId = GradeStudent.GradeId,
            StudentName=GradeStudent.Student.Name,
            // You can add other necessary properties here
        }
             };
            ViewData["StudentId"] = GradeStudent.Student.Name;
            return View(model);
        }


        private bool GradeExists(int id)
        {
            return _context.Grades.Any(e => e.GradeId == id);
        }
    }
}
