using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Registrar.Models;
using System.Collections.Generic;
using System.Linq;

namespace Registrar.Controllers
{
    public class CoursesController : Controller
    {
        private readonly RegistrarContext _db;

        public CoursesController(RegistrarContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            List<Course> courseList = _db.Courses.ToList();
            courseList.Sort((x, y) => string.Compare(x.CourseNumber, y.CourseNumber));
            return View(courseList);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create (Course course)
        {
            _db.Courses.Add(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            var thisCourse = _db.Courses
                .Include(course => course.Students)
                .ThenInclude(join => join.Student)
                .FirstOrDefault(course => course.CourseId == id);
            return View(thisCourse);
        }

        public ActionResult AddStudent(int id)
        {
            var thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
            ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name");
            return View(thisCourse);
        }

        [HttpPost]
        public ActionResult AddStudent(Course course, int StudentId)
        {
            if (StudentId != 0)
            {
                var returnedJoin = _db.StudentCourse
                    .Any(join => join.StudentId == StudentId && join.CourseId == course.CourseId);
                if (!returnedJoin)
                {
                    _db.StudentCourse.Add(new StudentCourse() { StudentId = StudentId, CourseId = course.CourseId });
                }
            }
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = course.CourseId });
        }

        public ActionResult Delete(int id)
        {
            var thisCourse = _db.Courses.FirstOrDefault(courses => courses.CourseId == id);
            return View(thisCourse);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var thisCourse = _db.Courses.FirstOrDefault(courses => courses.CourseId == id);
            _db.Courses.Remove(thisCourse);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}