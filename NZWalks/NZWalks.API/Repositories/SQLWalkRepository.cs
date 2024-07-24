using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();

            return walk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
             string? sortBy = null, Boolean? isAscending = true, int pageNumber = 1, int pageSize = 5)
        {
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();
            // Filtering
            Console.WriteLine("{0} {1}",filterOn,filterQuery);
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery)==false)
            {
                if (filterOn.Equals("Name",StringComparison.OrdinalIgnoreCase))
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
            }
            // Sorting
            if(string.IsNullOrWhiteSpace(sortBy)==false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    if (isAscending == true)
                        walks = walks.OrderBy(x => x.Name);
                    else walks = walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                    walks = isAscending == true ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
            }
            
            //Pagination
            walks = walks.Skip((pageNumber - 1) * pageSize).Take(pageSize);

           // var walkDomainModel = await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();

            //return walkDomainModel;

            return await walks.ToListAsync();

        }

        public async Task<Walk> GetByIdAsync(Guid Id)
        {
            var domainModel = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x=>x.ID== Id);

            return domainModel;

            
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var domainModel = await dbContext.Walks.FirstOrDefaultAsync(x=>x.ID== id);
            if (domainModel == null) return null;
            //update the values
            domainModel.Description = walk.Description;
            domainModel.Name = walk.Name;
            domainModel.LengthInKm = walk.LengthInKm;
            domainModel.DifficultyId = walk.DifficultyId;
            domainModel.RegionId = walk.RegionId;
            domainModel.WalkImageUrl = walk.WalkImageUrl;

            await dbContext.SaveChangesAsync();

            return domainModel;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var domainModel = dbContext.Walks.FirstOrDefault(x=>x.ID== id);
            if (domainModel == null) return null;

            dbContext.Walks.Remove(domainModel);
            await dbContext.SaveChangesAsync();
            return domainModel;
        }
    }
}
