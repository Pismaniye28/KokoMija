using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCoreCommentRepository : EfCoreGenericRepository<ProductComment>, ICommentRepository
    {
        public EfCoreCommentRepository(DbContext ctx) : base(ctx)
        {
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }
   public async Task<ProductComment> GetCommentByIdAsync(int commentId)
        {
            return await ShopContext.Comments.FindAsync(commentId);
        }

        public async Task<IEnumerable<ProductComment>> GetCommentsForProductAsync(int productId)
        {
            return await ShopContext.Comments
                .Where(comment => comment.ProductId == productId)
                .ToListAsync();
        }

        public async Task AddCommentAsync(ProductComment comment)
        {
            ShopContext.Comments.Add(comment);
            await ShopContext.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(ProductComment comment)
        {
            ShopContext.Entry(comment).State = EntityState.Modified;
            await ShopContext.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                ShopContext.Comments.Remove(comment);
                await ShopContext.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<ProductComment>> GetRepliesByParentCommentIdAsync(int parentCommentId)
        {
            return await ShopContext.Comments
                .Where(comment => comment.ParentCommentId == parentCommentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductComment>> GetCommentsByUserIdAsync(string userId)
        {
            return await ShopContext.Comments
                .Where(comment => comment.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductComment>> GetPendingCommentsAsync()
        {
            return await ShopContext.Comments
                .Where(comment => comment.ModerationStatus == ProductComment.CommentModerationStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductComment>> GetApprovedCommentsAsync()
        {
            return await ShopContext.Comments
                .Where(comment => comment.ModerationStatus == ProductComment.CommentModerationStatus.Approved)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductComment>> GetRejectedCommentsAsync()
        {
            return await ShopContext.Comments
                .Where(comment => comment.ModerationStatus == ProductComment.CommentModerationStatus.Rejected)
                .ToListAsync();
        }

        public async Task ApproveCommentAsync(int commentId)
        {
            var comment = await GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                comment.ModerationStatus = ProductComment.CommentModerationStatus.Approved;
                await ShopContext.SaveChangesAsync();
            }
        }

        public async Task RejectCommentAsync(int commentId)
        {
            var comment = await GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                comment.ModerationStatus = ProductComment.CommentModerationStatus.Rejected;
                await ShopContext.SaveChangesAsync();
            }
        }
        

    }
}