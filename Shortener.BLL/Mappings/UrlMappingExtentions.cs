using Shortener.BLL.Models;
using Shortener.DAL.Entities;

namespace Shortener.BLL.Mappings
{
    public static class UrlMappingExtentions
    {
        public static ShortUrlModel ToModel(this ShortUrl entity)
        {
            return new ShortUrlModel
            {
                Id = entity.Id,
                OriginalUrl = entity.OriginalUrl,
                ShortenedUrl = entity.ShortenedUrl,
                CreatedAt = entity.CreatedAt,
                CreatedById = entity.CreatedById,
                CreatedBy = entity.CreatedBy.UserName,
            };
        }

        public static ShortUrl ToEntity(this ShortUrlModel model)
        {
            return new ShortUrl
            {
                Id = model.Id,
                OriginalUrl = model.OriginalUrl,
                ShortenedUrl = model.ShortenedUrl,
                CreatedAt = model.CreatedAt,
                CreatedById = model.CreatedById,
            };
        }
    }
}