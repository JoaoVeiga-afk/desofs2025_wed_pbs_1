using ShopTex.Domain.Shared;

namespace ShopTex.Domain.Stores;

public interface IStoreRepository : IRepository<Store, StoreId>
{
    public Task<Store?> FindById(string storeId);
}