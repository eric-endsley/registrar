using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Registrar.Models;
using System.Collections.Generic;
using System.Linq;

namespace Registrar.Controllers
{
    public class StudentsController : Controller
    {
        private readonly RegistrarContext _db;
        public StudentsController(RegistrarContext db)
        {
          _db = db;
        }
        public ActionResult Index()
        {
          return View(_db.Students.ToList()); // refers to List on Index View page to pass x data to said view
        }

        public ActionResult Create()
        {
          ViewBag.CourseId = new SelectList(_db.Courses, "CourseId", "CourseName");
          return View();
        }

        [HttpPost]
        public ActionResult Create(Student student, int CourseId)
        {
          _db.Students.Add(student);
          if (CourseId != 0)
          {
            _db.StudentCourse.Add(new StudentCourse() { CourseId = CourseId, StudentId = student.StudentId });
          }
          _db.SaveChanges();
          return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
          var thisStudent = _db.Students
            .Include(student => student.Courses)
            .ThenInclude(join => join.Course)
            .FirstOrDefault(student => student.StudentId == id);
          return View(thisStudent);
        }

        public ActionResult AddCourse(int id)
        {
          var thisStudent = _db.Students.FirstOrDefault(student => student.StudentId == id);
          ViewBag.CourseId = new SelectList(_db.Courses, "CourseId", "CourseName");
          return View(thisStudent);
        }

        [HttpPost]
        public ActionResult AddCourse(Student student, int CourseId)
        {
            if (CourseId != 0)
            {
              var returnedJoin = _db.StudentCourse
                .Any(join => join.CourseId == CourseId && join.StudentId == student.StudentId);
              if (!returnedJoin)
              {
                _db.StudentCourse.Add(new StudentCourse() { CourseId = CourseId, StudentId = student.StudentId });
              }
            }
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = student.StudentId });
        }

        [HttpPost]
        public ActionResult DeleteJoin(int studentId, int studentCourseId) // Line 46 in StudentsController.cs is `Student student`
        {
          var joinEntry = _db.StudentCourse.FirstOrDefault(entry => entry.StudentCourseId == studentCourseId);
          _db.StudentCourse.Remove(joinEntry);
          _db.SaveChanges();
          return RedirectToAction("Details", new { id = studentId });
        }

        public ActionResult Delete(int studentId)
        {
          var thisStudent = _db.Students.FirstOrDefault(student => student.StudentId == studentId);
          return View(thisStudent);
        }

//will check if a relationship exists already and prevent duplicates
//      [HttpPost]
//      public ActionResult AddCategory(Item item, int CategoryId)
//      {
//      if (CategoryId != 0)
//   // Check if CategoryId is valid
//      {
//     var returnedJoin = _db.CategoryItem
//       .Any(join => join.ItemId == item.ItemId && join.CategoryId == CategoryId);
//     // Check if "Any" of this relationship exists, returns a bool
//     if (!returnedJoin) {
//     // if the returnedJoin for that relationship if false, then add the relationship
//       _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
//     }
//   }
//   _db.SaveChanges();
//   return RedirectToAction("Index");
// }
    }
}