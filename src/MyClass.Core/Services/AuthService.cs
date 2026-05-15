using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;
using MyClass.Core.Options;
using System.Security.Cryptography;

namespace MyClass.Core.Services;

public sealed class AuthService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IOptions<TeacherOptions> teacherOptions) : IAuthService
{

    public LoginState? Current { get; private set; }

    public event Action? Changed;

    public void Set(LoginState? state)
    {
        Current = state;
        Changed?.Invoke();
    }

    public async Task<Result<LoginState>> LoginAsync(
        string userName,
        string password,
        bool isTeacher,
        string classCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.Trim();
        var normalizedClassCode = classCode.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUserName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(normalizedClassCode))
        {
            return Result<LoginState>.Failure("Username, password, and class are required.");
        }

        var teacher = teacherOptions.Value;
        var validTeacher =
            string.Equals(normalizedUserName, teacher.UserName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(password, teacher.Password, StringComparison.Ordinal);

        if (validTeacher)
        {
            return Result<LoginState>.Success(new LoginState(normalizedUserName, true, normalizedClassCode, "Teacher"));
        }

        if (isTeacher)
        {
            return Result<LoginState>.Failure("Invalid username or password.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var students = await GetStudents(dbContext, normalizedClassCode);
     
        var student = students.SingleOrDefault(s => s.UserName.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase));          
        if (student is null || !VerifyPassword(password, student.PasswordHash))
        {
            return Result<LoginState>.Failure("Invalid username or password.");
        }

        if (!student.IsActive)
        {
            student.IsActive = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<LoginState>.Success(new LoginState(student.UserName, false, normalizedClassCode, student.DisplayName));
    }

    public async Task<Result<LoginState>> RegisterStudentAsync(
        string userName,
        string firstName,
        string lastName,
        string password,
        string classCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.Trim();
        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();
        var normalizedClassCode = classCode.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUserName) ||
            string.IsNullOrWhiteSpace(normalizedFirstName) ||
            string.IsNullOrWhiteSpace(normalizedLastName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(normalizedClassCode))
        {
            return Result<LoginState>.Failure("First name, last name, username, password, and class are required.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currentClass = await dbContext.Classes
            .SingleOrDefaultAsync(@class => @class.Code == normalizedClassCode, cancellationToken);

        if (currentClass is null)
        {
            return Result<LoginState>.Failure("A valid class is required before registration.");
        }

        var students = await GetStudents(dbContext, currentClass.Code);
        var duplicateExists = students
            .Any(student => student.UserName.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            return Result<LoginState>.Failure("That username is already registered for this class.");
        }

        var student = new Student
        {
            ClassId = currentClass.Id,
            UserName = normalizedUserName,
            FirstName = normalizedFirstName,
            LastName = normalizedLastName,
            DisplayName = $"{normalizedFirstName} {normalizedLastName}",
            PasswordHash = HashPassword(password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Students.Add(student);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Result<LoginState>.Failure("That username is already registered for this class.");
        }

        return Result<LoginState>.Success(new LoginState(student.UserName, false, normalizedClassCode, student.DisplayName));
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return $"pbkdf2-sha256:{100_000}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split(':');

        if (parts.Length != 4 ||
            parts[0] != "pbkdf2-sha256" ||
            !int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]);
        var expectedKey = Convert.FromBase64String(parts[3]);
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    private async Task<List<Student>> GetStudents(ApplicationDbContext dbContext, string classCode)
    {
        return await dbContext.Students
            .Where(s => s.Class.Code == classCode)
            .ToListAsync();
    }

}


