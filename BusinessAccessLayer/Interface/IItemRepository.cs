using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Shared;
using Shared.Models;


namespace BusinessLogic.Interface
{
    public interface IItemRepository
    {
        Task<PagedList<ItemViewModel>> GetDataAsync(PaginationFilters filters);

        Task<ItemViewModel?> GetDataByIdAsync(int id);

        Task<ItemViewModel> CreateDataAsync(ItemViewModel item);

        Task<ItemViewModel> UpdateDataAsync(ItemViewModel item);

        Task<ItemViewModel> DeleteDataAsync(int id);

        Task<int> CheckDuplicateName(string name, int id, int categoryId);

        Task<bool> BalkUpload(List<ItemViewModel> data);

    }
}
