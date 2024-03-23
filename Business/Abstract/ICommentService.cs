using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Bussines.Abstract
{
    public interface ICommentService:IValidator<ProductComment>
        {
        Task<IEnumerable<ProductComment>> GetCommentsForProductAsync(int productId);
        Task AddCommentAsync(ProductComment comment);
        Task UpdateCommentAsync(ProductComment comment);
        Task ApproveCommentAsync(int commentId);
        Task RejectCommentAsync(int commentId);
        Task<IEnumerable<ProductComment>> GetCommentsByUserIdAsync(string userId);
    }
    
    
}