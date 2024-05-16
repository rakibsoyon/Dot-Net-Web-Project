using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Shared.Models;


namespace BusinessLogic.Interface
{
    public interface ICategoryRepository
    {
        Task<JqueryDataTablesPagedResults<CategoryViewModel>> GetDataAsync(JqueryDataTablesParameters table);
        Task<CategoryViewModel> GetDataByIdAsync(int id);
        Task CreateDataAsync(CategoryViewModel ct);
        Task UpdateDataAsync(CategoryViewModel ct);
        Task DeleteDataAsync(int id);
        Task<int> CheckDuplicateName(string name, int id);

        Task<List<CategoryViewModel>> GetAllCategoriesAsync();

        Task<List<CategoryViewModel>> GetCategories();

        Task<bool> BalkUpload(List<CategoryViewModel> data);

    }
}
