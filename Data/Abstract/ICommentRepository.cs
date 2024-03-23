using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Data.Abstract
{
    public interface ICommentRepository : IRepository<ProductComment>
    {
        Task<ProductComment> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<ProductComment>> GetCommentsForProductAsync(int productId);
        Task<IEnumerable<ProductComment>> GetCommentsByUserIdAsync(string userId);
        Task AddCommentAsync(ProductComment comment);
        Task UpdateCommentAsync(ProductComment comment);
        Task DeleteCommentAsync(int commentId);
    }
}
