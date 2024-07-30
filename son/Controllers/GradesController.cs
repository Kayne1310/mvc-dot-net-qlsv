using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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


        // GET: Grades/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Grades/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId");
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId");
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GradeId,GradeStudent,CourseId,ClassId,StudentId")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", grade.StudentId);
            return View(grade);
        }

        // GET: Grades/Edit/5
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", grade.StudentId);
            return View(grade);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GradeId,GradeStudent,CourseId,ClassId,StudentId")] Grade grade)
        {
            if (id != grade.GradeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", grade.ClassId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", grade.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "StudentId", grade.StudentId);
            return View(grade);
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

        private bool GradeExists(int id)
        {
            return _context.Grades.Any(e => e.GradeId == id);
        }
    }
}
