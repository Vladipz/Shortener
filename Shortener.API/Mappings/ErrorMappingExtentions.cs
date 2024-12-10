using FluentValidation;

namespace Shortener.API.Mappings
{
    public static class ErrorMappingExtentions
    {
        public static Dictionary<string, string[]> ToErrorsList(this ValidationException ex)
        {
            return ex.Errors
                .GroupBy(static x => x.PropertyName)
                .ToDictionary(static g => g.Key, static g => g.Select(static x => x.ErrorMessage).ToArray());
        }
    }
}