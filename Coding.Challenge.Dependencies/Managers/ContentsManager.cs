using Coding.Challenge.Dependencies.Database;
using Coding.Challenge.Dependencies.Models;

namespace Coding.Challenge.Dependencies.Managers
{
    public class ContentsManager : IContentsManager
    {
        private readonly IDatabase<Content?, ContentDto?> _database;
        public ContentsManager(IDatabase<Content?, ContentDto> database)
        {
            _database = database;
        }

        public async Task<IEnumerable<Content?>> GetManyContents()
        {
            return await _database.ReadAll();
        }

        public async Task<IEnumerable<Content?>> GetFilteredContents(string? title, string? genre)
        {
            var contents = await GetManyContents();

            if (!string.IsNullOrEmpty(title))
            {
                contents = contents.Where(c => c?.Title.Contains(title, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (!string.IsNullOrEmpty(genre))
            {
                contents = contents.Where(c => c?.GenreList.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)) ?? false);
            }

            return contents;
        }

        public async Task<Content?> CreateContent(ContentDto content)
        {
            return await _database.Create(content);
        }

        public async Task<Content?> GetContent(Guid id)
        {
            return await _database.Read(id);
        }

        public async Task<Content?> UpdateContent(Guid id, ContentDto content)
        {
            return await _database.Update(id, content);
        }

        public async Task<Guid> DeleteContent(Guid id)
        {
            return await _database.Delete(id);
        }

        public async Task<bool> AddGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _database.Read(id);

            if (content == null)
                return false;

            var updatedGenres = content.GenreList.Concat(genres).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var updatedContentDto = new ContentDto(
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                updatedGenres
            );

            var result = await _database.Update(id, updatedContentDto);
            return result != null;
        }

        public async Task<bool> RemoveGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _database.Read(id);

            if (content == null)
                return false;

            var updatedGenres = content.GenreList.Except(genres, StringComparer.OrdinalIgnoreCase).ToList();
            var updatedContentDto = new ContentDto(
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                updatedGenres
            );

            var result = await _database.Update(id, updatedContentDto);
            return result != null;
        }
    }
}
