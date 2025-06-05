using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Stores;
using ShopTex.Infrastructure.Shared;
using ShopTex.Models;

namespace ShopTex.Infrastructure.Stores;

public class StoreRepository : BaseRepository<Store,StoreId>,IStoreRepository
{
    public StoreRepository(DatabaseContext context) : base(context.Store)
    {
        
    }

    public async Task<Store> FindByName(string name)
    {
        return await _objs.Where(s => name.Equals(s.Name)).FirstOrDefaultAsync();
    }
}