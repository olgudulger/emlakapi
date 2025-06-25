using DataAccess.Abstract;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class RefreshTokenDal : GenericRepository<RefreshToken>, IRefreshTokenDal
    {
        public RefreshTokenDal(DbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }
    }
} 