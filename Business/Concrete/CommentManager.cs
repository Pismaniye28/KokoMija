using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;

namespace Bussines.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CommentManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductComment>> GetCommentsForProductAsync(int productId)
        {
            return await _unitOfWork.Comment.GetCommentsForProductAsync(productId);
        }

        public async Task AddCommentAsync(ProductComment comment)
        {
            // You might want to implement validation and moderation logic here.
            await _unitOfWork.Comment.AddCommentAsync(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCommentAsync(ProductComment comment)
        {
            // Implement logic for updating a comment, if needed.
            _unitOfWork.Comment.Update(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task ApproveCommentAsync(int commentId)
        {
            var comment = await _unitOfWork.Comment.GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                comment.ModerationStatus = ProductComment.CommentModerationStatus.Approved;
                _unitOfWork.Comment.Update(comment);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task RejectCommentAsync(int commentId)
        {
            var comment = await _unitOfWork.Comment.GetCommentByIdAsync(commentId);
            if (comment != null)
            {
                comment.ModerationStatus = ProductComment.CommentModerationStatus.Rejected;
                _unitOfWork.Comment.Update(comment);
                await _unitOfWork.SaveAsync();
            }
        }

        public bool Validation(ProductComment entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductComment>> GetCommentsByUserIdAsync(string userId)
        {
            return await _unitOfWork.Comment.GetCommentsByUserIdAsync(userId);
        }
    }
}
