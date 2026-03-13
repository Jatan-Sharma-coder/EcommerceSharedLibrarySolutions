using EcommerceSharedLibrary.Responses;
using Microsoft.AspNetCore.Http.Features;
using System.Linq.Expressions;

namespace EcommerceSharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Response> CreateAsync (T entity);  
        Task<Response> UpdateAsync (T entity);  
        Task<Response> DeleteAsync (T entity);  
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync (int id);
        Task<T> GetByAsync (Expression<Func<T, bool>> predicate);
    }
}
