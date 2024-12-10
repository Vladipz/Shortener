using Microsoft.AspNetCore.Identity;

namespace Shortener.BLL.Mappings;

public static class IdentityResultMappingExtensions
{
    public static Dictionary<string, List<string>> ToValidationErrorsGrouped(this IdentityResult result)
    {
        return result.Errors
            .GroupBy(static e => GetFieldFromErrorCode(e.Code)) // Групуємо помилки за полем, взятим з коду
            .ToDictionary(
                static g => g.Key, // Ключ - це поле, з якого
                static g => g.Select(static e => e.Description).ToList()); // Опис помилки як список
    }

    private static string GetFieldFromErrorCode(string errorCode)
    {
        // Тут визначаємо, якому полю відповідає код помилки
        return errorCode switch
        {
            "PasswordTooShort" => "Password",
            "PasswordRequiresNonAlphanumeric" => "Password",
            "DuplicateUserName" => "UserName",
            "InvalidEmail" => "Email",
            _ => "General" // Якщо поле невідоме, відносимо до загальної категорії
        };
    }
}