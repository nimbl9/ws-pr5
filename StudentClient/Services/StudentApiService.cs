using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using StudentClient.Models;

namespace StudentClient.Services;

public class StudentApiService : IStudentApiService
{
    private readonly IHttpClientFactory _factory;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public StudentApiService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient GetClient()
    {
        return _factory.CreateClient("StudentApi");
    }

    private static ApiResult<T> Error<T>(HttpStatusCode statusCode)
    {
        var code = (int)statusCode;

        var message = statusCode switch
        {
            HttpStatusCode.BadRequest => "Сервер отклонил данные. Проверьте введённые значения.",
            HttpStatusCode.NotFound => "Студент с указанным ID не найден.",
            HttpStatusCode.InternalServerError => "На сервере произошла внутренняя ошибка.",
            HttpStatusCode.Unauthorized => "Необходима авторизация.",
            HttpStatusCode.Forbidden => "Недостаточно прав для выполнения операции.",
            _ => $"Сервер вернул ошибку: {code}."
        };

        return ApiResult<T>.Fail(message, code);
    }

    private static ApiResult<T> NetworkError<T>()
    {
        return ApiResult<T>.Fail("Не удалось подключиться к Web API. Убедитесь, что StudentApi запущен.");
    }

    public async Task<ApiResult<List<Student>>> GetAllAsync()
    {
        try
        {
            using var response = await GetClient().GetAsync("api/students");

            if (response.StatusCode != HttpStatusCode.OK)
                return Error<List<Student>>(response.StatusCode);

            var students = await response.Content.ReadFromJsonAsync<List<Student>>(_jsonOptions);

            return ApiResult<List<Student>>.Ok(
                students ?? new List<Student>(),
                "Список студентов загружен.",
                (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return NetworkError<List<Student>>();
        }
        catch (TaskCanceledException)
        {
            return ApiResult<List<Student>>.Fail("Время ожидания ответа сервера истекло.");
        }
        catch (JsonException)
        {
            return ApiResult<List<Student>>.Fail("Не удалось обработать ответ сервера.");
        }
    }

    public async Task<ApiResult<Student>> GetByIdAsync(int id)
    {
        try
        {
            using var response = await GetClient().GetAsync($"api/students/{id}");

            if (response.StatusCode != HttpStatusCode.OK)
                return Error<Student>(response.StatusCode);

            var student = await response.Content.ReadFromJsonAsync<Student>(_jsonOptions);

            return student is null
                ? ApiResult<Student>.Fail("Сервер не вернул данные студента.")
                : ApiResult<Student>.Ok(student, "Данные студента загружены.", (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return NetworkError<Student>();
        }
        catch (TaskCanceledException)
        {
            return ApiResult<Student>.Fail("Время ожидания ответа сервера истекло.");
        }
        catch (JsonException)
        {
            return ApiResult<Student>.Fail("Не удалось обработать ответ сервера.");
        }
    }

    public async Task<ApiResult<Student>> CreateAsync(Student student)
    {
        try
        {
            using var response = await GetClient().PostAsJsonAsync("api/students", student);

            if (response.StatusCode != HttpStatusCode.Created
                && response.StatusCode != HttpStatusCode.OK)
                return Error<Student>(response.StatusCode);

            Student? created = null;

            try
            {
                created = await response.Content.ReadFromJsonAsync<Student>(_jsonOptions);
            }
            catch (JsonException)
            {
                created = null;
            }

            return ApiResult<Student>.Ok(
                created ?? student,
                "Студент успешно добавлен.",
                (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return NetworkError<Student>();
        }
        catch (TaskCanceledException)
        {
            return ApiResult<Student>.Fail("Время ожидания ответа сервера истекло.");
        }
    }

    public async Task<ApiResult<Student>> UpdateAsync(int id, Student student)
    {
        try
        {
            using var response = await GetClient().PutAsJsonAsync($"api/students/{id}", student);

            if (response.StatusCode != HttpStatusCode.OK
                && response.StatusCode != HttpStatusCode.NoContent)
                return Error<Student>(response.StatusCode);

            Student? updated = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    updated = await response.Content.ReadFromJsonAsync<Student>(_jsonOptions);
                }
                catch (JsonException)
                {
                    updated = null;
                }
            }

            return ApiResult<Student>.Ok(
                updated ?? student,
                "Данные студента успешно изменены.",
                (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return NetworkError<Student>();
        }
        catch (TaskCanceledException)
        {
            return ApiResult<Student>.Fail("Время ожидания ответа сервера истекло.");
        }
    }

    public async Task<ApiResult<bool>> DeleteAsync(int id)
    {
        try
        {
            using var response = await GetClient().DeleteAsync($"api/students/{id}");

            if (response.StatusCode != HttpStatusCode.OK
                && response.StatusCode != HttpStatusCode.NoContent)
                return Error<bool>(response.StatusCode);

            return ApiResult<bool>.Ok(true, "Студент успешно удалён.", (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return NetworkError<bool>();
        }
        catch (TaskCanceledException)
        {
            return ApiResult<bool>.Fail("Время ожидания ответа сервера истекло.");
        }
    }
}
