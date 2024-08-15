using System.Data;
using Celebratix.Common.Models;

namespace Celebratix.Common.Database
{
    public interface IUnitOfWork
    {
        CelebratixDbContext DbContext { get; }
        // ITransactionScope CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);
        /// <summary>
        /// Creates a transaction scope asynchronously (creating a new transaction if needed).
        /// <example>
        /// For example:
        /// <code>
        /// await using (var transactionScope = await _unitOfWork.CreateTransactionAsync()) {
        ///     ...
        ///     await transactionScope.CompleteAsync();
        /// }
        /// </code>
        /// </example>
        /// </summary>
        Task<ITransactionScope> CreateTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead, CancellationToken cancellationToken = default);
    }

    public interface ITransactionScope : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Mark transaction scope as complete, commiting all changes if this is the top level scope.
        /// If this function is not called before the scope is disposed all changes in this scope will be lost.
        /// Doing database changes after this function has been called, but before the scope has been disposed is undefined behavior.
        /// </summary>
        void Complete();
        /// <summary>
        /// Mark transaction scope as complete asynchronously, commiting all changes if this is the top level scope.
        /// If this function is not called before the scope is disposed all changes in this scope will be lost.
        /// Doing database changes after this function has been called, but before the scope has been disposed is undefined behavior.
        /// </summary>
        Task CompleteAsync();
    };
}
