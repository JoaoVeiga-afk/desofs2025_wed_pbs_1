using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Stores;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;

namespace ShopTex.Infrastructure.Stores;

public class StoreRepository : BaseRepository<Store, StoreId>, IStoreRepository
{
    public StoreRepository(DatabaseContext context) : base(context.Store)
    {

    }
    
    public async Task<Store?> FindById(string storeId)
    {
        return await _objs.FirstOrDefaultAsync(s => s.Id.AsString() == storeId);
    }
}