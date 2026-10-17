using Microsoft.AspNetCore.Mvc;
using StudentClient.Models;
using StudentClient.Services;

namespace StudentClient.Controllers;

public class StudentController : Controller
{
    private readonly IStudentApiService _studentApi;

    public StudentController(IStudentApiService studentApi)
    {
        _studentApi = studentApi;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _studentApi.GetAllAsync();
        ViewBag.Error = result.Success ? null : result.Message;
        ViewBag.StatusCode = result.StatusCode;
        ViewBag.Notice = TempData["Notice"];
        return View(result.Data ?? new List<Student>());
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return BadRequest("Некорректный ID студента.");

        var result = await _studentApi.GetByIdAsync(id);

        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return View("NotFoundStudent");

            ViewBag.Error = result.Message;
            ViewBag.StatusCode = result.StatusCode;
            return View("ApiError");
        }

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Student());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid)
            return View(student);

        var result = await _studentApi.CreateAsync(student);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(student);
        }

        TempData["Notice"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return BadRequest("Некорректный ID студента.");

        var result = await _studentApi.GetByIdAsync(id);

        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return View("NotFoundStudent");

            ViewBag.Error = result.Message;
            ViewBag.StatusCode = result.StatusCode;
            return View("ApiError");
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id <= 0)
            return BadRequest("Некорректный ID студента.");

        if (!ModelState.IsValid)
            return View(student);

        student.Id = id;
        var result = await _studentApi.UpdateAsync(id, student);

        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return View("NotFoundStudent");

            ModelState.AddModelError(string.Empty, result.Message);
            return View(student);
        }

        TempData["Notice"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("Некорректный ID студента.");

        var result = await _studentApi.DeleteAsync(id);
        TempData["Notice"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Error()
    {
        ViewBag.Error = "Произошла внутренняя ошибка приложения.";
        ViewBag.StatusCode = 500;
        return View("ApiError");
    }
}
