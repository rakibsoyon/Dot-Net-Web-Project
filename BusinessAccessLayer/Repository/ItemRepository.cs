using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.Interface;
using Dapper;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Models;
using System.Data;


namespace BusinessLogic.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly DapperContext _dapperContext;

        public ItemRepository(ApplicationDbContext context, IConfigurationProvider mappingConfiguration, DapperContext dapperContext)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _dapperContext = dapperContext;
        }

        public async Task<PagedList<ItemViewModel>> GetDataAsync(PaginationFilters filters)
        {

            var query = _context.Items.Include(i => i.Category)
                                       .ProjectTo<ItemViewModel>(_mappingConfiguration)
                                       .OrderByDescending(o => o.Id)
                                       .AsNoTracking()
                                       .AsQueryable();

            return await PagedList<ItemViewModel>.CreateAsync(query, filters.Page, filters.PageSize);
        }

        public async Task<ItemViewModel?> GetDataByIdAsync(int id)
        {
            var item = await _context.Items.Include(i => i.Category)
                                           .ProjectTo<ItemViewModel>(_mappingConfiguration)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (item is null) { return null;}
            
            return _mappingConfiguration.CreateMapper().Map<ItemViewModel>(item);
        }

        public async Task<ItemViewModel> CreateDataAsync(ItemViewModel model)
        {
            var entity = _mappingConfiguration.CreateMapper().Map<Item>(model);
            await _context.Items.AddAsync(entity);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<ItemViewModel> UpdateDataAsync(ItemViewModel model)
        {
            var entity = await _context.Items.AsNoTracking()
                                           .SingleOrDefaultAsync(x => x.Id.Equals(model.Id));

            entity = _mappingConfiguration.CreateMapper().Map(model, entity);

            _context.Items.Update(entity!);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<ItemViewModel> DeleteDataAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);

            _context.Items.Remove(item!);
            await _context.SaveChangesAsync();
            return _mappingConfiguration.CreateMapper().Map<ItemViewModel>(item); ;
        }

        public async Task<int> CheckDuplicateName(string name, int id, int categoryId)
        {
            return await _context.Items
                                 .AsQueryable()
                                 .Where(x => x.Name.ToLower() == name.ToLower() && x.Id != id && x.CategoryId == categoryId)
                                 .CountAsync();
        }

        public async Task<bool> BalkUpload(List<ItemViewModel> itemViewModels)
        {

            try
            {
                var insertQuery = "INSERT INTO Items (Name,Unit,Quantity,CategoryId) VALUES (@Name,@Unit,@Quantity,@CategoryId)";
                using (var connection = _dapperContext.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var item in itemViewModels)
                        {

                            var count = await CheckDuplicateName(item.Name, item.Id,item.CategoryId);

                            if (count == 0)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("Name", item.Name, DbType.String);
                                parameters.Add("Unit", item.Unit, DbType.Int32);
                                parameters.Add("Quantity", item.Quantity, DbType.Int32);
                                parameters.Add("CategoryId", item.CategoryId, DbType.Int32);
                                await connection.ExecuteAsync(insertQuery, parameters, transaction: transaction);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
