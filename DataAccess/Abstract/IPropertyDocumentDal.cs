using Entity.Models;

namespace DataAccess.Abstract
{
    public interface IPropertyDocumentDal : IGenericRepository<PropertyDocument>
    {
        Task<List<PropertyDocument>> GetDocumentsByPropertyAsync(int propertyId);
        Task<PropertyDocument?> GetDocumentByPathAsync(string documentPath);
    }
} 