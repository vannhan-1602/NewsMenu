using Infrastructure.ReadModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Infrastructure.Options;
using Application.Interfaces;
using Application.DTOs;


namespace Infrastructure.Repositories
{
    public class NewsMongoReadRepository : INewsReadRepository
    {
        private readonly IMongoCollection<NewsReadModel> _collection;

        public NewsMongoReadRepository(IOptions<MongoDbOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.DatabaseName);
            _collection = db.GetCollection<NewsReadModel>("news");

            
            var indexModel = new CreateIndexModel<NewsReadModel>(
                Builders<NewsReadModel>.IndexKeys.Ascending(n => n.IsPublished));
            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task<NewsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var doc = await _collection
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync(ct);

            return doc == null ? null : MapToDto(doc);
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var docs = await _collection
                .Find(n => n.IsPublished)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);

            return docs.Select(MapToDto);
        }

       
        public async Task UpsertAsync(NewsDto news, CancellationToken ct = default)
        {
            var doc = new NewsReadModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Summary = news.Summary,
                IsPublished = news.IsPublished,
                CreatedAt = news.CreatedAt,
                Menus = news.Menus.Select(m => new MenuReadModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Slug = m.Slug,
                }).ToList(),
            };

            var filter = Builders<NewsReadModel>.Filter.Eq(n => n.Id, news.Id);
            var opts = new ReplaceOptions { IsUpsert = true };
            await _collection.ReplaceOneAsync(filter, doc, opts, ct);
        }
        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            
            await _collection.DeleteOneAsync(x => x.Id == id, ct);

        }

        private static NewsDto MapToDto(NewsReadModel doc) => new(
            doc.Id,
            doc.Title,
            doc.Content,
            doc.Summary,
            doc.IsPublished,
            doc.CreatedAt,
            doc.Menus.Select(m => new MenuDto(m.Id, m.Name, m.Slug, 0)).ToList()
        );
    }
}
