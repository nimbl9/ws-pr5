using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private static readonly object Locker = new();

    private static readonly List<Student> Students =
    [
        new Student { Id = 1, FullName = "Иван Иванов", Group = "SE-301", Course = 3, Email = "ivan@example.com" },
        new Student { Id = 2, FullName = "Анна Петрова", Group = "SE-302", Course = 3, Email = "anna@example.com" },
        new Student { Id = 3, FullName = "Максим Ким", Group = "SE-301", Course = 3, Email = "maxim@example.com" }
    ];

    private static int _nextId = 4;

    [HttpGet]
    public ActionResult<List<Student>> GetAll()
    {
        lock (Locker)
        {
            return Ok(Students.Select(CopyStudent).ToList());
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> GetById(int id)
    {
        lock (Locker)
        {
            var student = Students.FirstOrDefault(x => x.Id == id);

            if (student is null)
                return NotFound(new { message = "Студент с указанным ID не найден." });

            return Ok(CopyStudent(student));
        }
    }

    [HttpPost]
    public ActionResult<Student> Create(Student student)
    {
        lock (Locker)
        {
            student.Id = _nextId++;
            Students.Add(CopyStudent(student));

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, CopyStudent(student));
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Student updated)
    {
        lock (Locker)
        {
            var student = Students.FirstOrDefault(x => x.Id == id);

            if (student is null)
                return NotFound(new { message = "Студент с указанным ID не найден." });

            student.FullName = updated.FullName;
            student.Group = updated.Group;
            student.Course = updated.Course;
            student.Email = updated.Email;

            return Ok(CopyStudent(student));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (Locker)
        {
            var student = Students.FirstOrDefault(x => x.Id == id);

            if (student is null)
                return NotFound(new { message = "Студент с указанным ID не найден." });

            Students.Remove(student);

            return NoContent();
        }
    }

    private static Student CopyStudent(Student student)
    {
        return new Student
        {
            Id = student.Id,
            FullName = student.FullName,
            Group = student.Group,
            Course = student.Course,
            Email = student.Email
        };
    }
}
