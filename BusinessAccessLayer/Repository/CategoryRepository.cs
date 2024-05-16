using AutoMapper;
using AutoMapper.QueryableExtensions;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Infrastructure;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Interface;
using DataAccess;
using Domain;
using Shared.Models;
using Dapper;
using System.Data;

namespace BusinessLogic.Repository
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly DapperContext _dapperContext;


        public CategoryRepository(ApplicationDbContext context, IConfigurationProvider mappingConfiguration, DapperContext dapperContext)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
            _dapperContext = dapperContext;
        }


        public async Task<JqueryDataTablesPagedResults<CategoryViewModel>> GetDataAsync(JqueryDataTablesParameters table)
        {
            CategoryViewModel[] items = null;
            IQueryable<Category> query = _context.Categories.OrderByDescending(o => o.Id).AsNoTracking();
                                                   

            query = SearchOptionsProcessor<CategoryViewModel, Category>.Apply(query, table.Columns);
            query = SortOptionsProcessor<CategoryViewModel, Category>.Apply(query, table);

            var size = await query.CountAsync();

            if (table.Length > 0)
            {
                items = await query
                .Skip((table.Start / table.Length) * table.Length)
                .Take(table.Length)
                .ProjectTo<CategoryViewModel>(_mappingConfiguration)
                .ToArrayAsync();
            }
            else
            {
                items = await query
                .ProjectTo<CategoryViewModel>(_mappingConfiguration)
                .ToArrayAsync();
            }

            return new JqueryDataTablesPagedResults<CategoryViewModel>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<CategoryViewModel> GetDataByIdAsync(int id)
        {
            var item = await _context.Categories.AsNoTracking()
                                           .SingleOrDefaultAsync(x => x.Id.Equals(id));

            return _mappingConfiguration.CreateMapper().Map<CategoryViewModel>(item);
        }

        public async Task CreateDataAsync(CategoryViewModel model)
        {
         
            var entity = _mappingConfiguration.CreateMapper().Map<Category>(model);
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync(); 
            
        }

        public async Task UpdateDataAsync(CategoryViewModel model)
        {
            var entity = await _context.Categories.AsNoTracking()
                                           .SingleOrDefaultAsync(x => x.Id.Equals(model.Id));

            entity = _mappingConfiguration.CreateMapper().Map(model, entity);

            _context.Categories.Update(entity!);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDataAsync(int id)
        {
            var item = await _context.Categories.FindAsync(id);

            _context.Categories.Remove(item!);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CheckDuplicateName(string name, int id)
        {
            return await _context.Categories
                                 .AsQueryable()
                                 .Where(x => x.Name.ToLower() == name.ToLower() && x.Id != id)
                                 .CountAsync();
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            return  await _context.Categories
                .ProjectTo<CategoryViewModel>(_mappingConfiguration)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CategoryViewModel>> GetCategories()
        {

            var query = "SELECT * FROM Categories";

            using (var connection = _dapperContext.CreateConnection())
            {
                var items = await connection.QueryAsync<Category>(query);
                return items.Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
        }

        
        public async Task<bool> BalkUpload(List<CategoryViewModel> categoryViewModels) {

            try
            {
                var insertQuery = "INSERT INTO Categories (Name) VALUES (@Name)";
                using (var connection = _dapperContext.CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var item in categoryViewModels)
                        {

                            var count = await CheckDuplicateName(item.Name, item.Id);

                            if (count == 0)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("Name", item.Name, DbType.String);
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
