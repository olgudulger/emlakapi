using DataAccess.Abstract;
using DataAccess.Context;
using Entity.Models;

namespace DataAccess.Concrete;

public class NeighborhoodDal : GenericRepository<Neighborhood>, INeighborhoodDal
{
    public NeighborhoodDal(EmlakDbContext context) : base(context)
    {
    }
} 