using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using son.Data;
using son.Models;
using son.ViewModels;

namespace son.Controllers
{
    public class ClassesController : Controller
    {
        private readonly sonContext _context;

        public ClassesController(sonContext context)
        {
            _context = context;
        }

        // GET: Classes
        [Authorize(Roles = "Teachers")]
        public async Task<IActionResult> Index()
        {
            var classes = _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Select(c => new ClassViewModel
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    Year = c.Year,
                    RoomName = c.RoomName,
                    CourseName = c.Course.CourseName,
                    TeacherName = c.Teacher.Name,
                    Semester = c.Semester

                })
                 .ToList();



            return View(classes);
        }


        // GET: Classes/Create
        [Authorize(Roles = "Teachers")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "TeacherId", "Name");
            return View();
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,CourseId,TeacherId,ClassName,Semester,Year,RoomName")] Class @class)
        {
           
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", @class.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "TeacherId", "Name", @class.TeacherId);
            return View(@class);
        }


        // GET: Classes/Edit/5
        [Authorize(Roles = "Teachers")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", @class.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "TeacherId", "Name", @class.TeacherId);
            return View(@class);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,CourseId,ClassName,TeacherId,Semester,Year,RoomName")] Class @class)
        {
            if (id != @class.ClassId)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.ClassId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", @class.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "TeacherId", "Name", @class.TeacherId);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", @class.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "TeacherId", "Name", @class.TeacherId);
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }
    }
}
