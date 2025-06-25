using System;
using Entity.Models;

namespace DataAccess.Abstract;

public interface IRefreshTokenDal:IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}
