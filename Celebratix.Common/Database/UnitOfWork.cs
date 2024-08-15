using Celebratix.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using IsolationLevel = System.Data.IsolationLevel;

namespace Celebratix.Common.Database
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed;
        private int _layer;
        private IDbContextTransaction? _transaction;
        public CelebratixDbContext DbContext { get; }

        public UnitOfWork(CelebratixDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ITransactionScope CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
        {
            if (_layer == 0)
                _transaction = DbContext.Database.BeginTransaction(isolationLevel);
            DbContext.SaveChanges();
            _transaction!.CreateSavepoint(_layer.ToString());
            var ts = new TransactionScope(this);
            _layer++;
            return ts;
        }

        public async Task<ITransactionScope> CreateTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead, CancellationToken cancellationToken = default)
        {
            if (_layer == 0)
                _transaction = await DbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            await _transaction!.CreateSavepointAsync(_layer.ToString(), cancellationToken);
            var ts = new TransactionScope(this);
            _layer++;
            return ts;
        }

        ~UnitOfWork() => Dispose(false);

        private void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    DbContext.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void DisposeScope(bool scopeCompleted)
        {
            DbContext.SaveChanges();

            _layer--;
            if (!scopeCompleted)
            {
                if (_layer == 0)
                {
                    _transaction!.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
                else
                    _transaction!.RollbackToSavepoint(_layer.ToString());
            }
            else if (_layer == 0)
            {
                try
                {
                    _transaction!.Commit();
                    _transaction.Dispose();
                    _transaction = null;
                }
                catch (Exception)
                {
                    _transaction!.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                    throw;
                }
            }
            else
                _transaction!.ReleaseSavepoint(_layer.ToString());
        }

        private async Task DisposeScopeAsync(bool scopeCompleted)
        {
            await DbContext.SaveChangesAsync();

            _layer--;
            if (!scopeCompleted)
            {
                if (_layer == 0)
                {
                    await _transaction!.RollbackAsync();
                    _transaction!.Dispose();
                    _transaction = null;
                }
                else
                    await _transaction!.RollbackToSavepointAsync(_layer.ToString());
            }
            else if (_layer == 0)
            {
                try
                {
                    await _transaction!.CommitAsync();
                    _transaction.Dispose();
                    _transaction = null;
                }
                catch (Exception)
                {
                    await _transaction!.RollbackAsync();
                    _transaction.Dispose();
                    _transaction = null;
                    throw;
                }
            }
            else
                await _transaction!.ReleaseSavepointAsync(_layer.ToString());
        }

        private sealed class TransactionScope : ITransactionScope, IDisposable, IAsyncDisposable
        {
            public bool _hasBeenCompleted;
            public int _layer;
            private UnitOfWork _unitOfWork;
            private bool _disposed;

            public TransactionScope(UnitOfWork unitOfWork)
            {
                _hasBeenCompleted = false;
                _unitOfWork = unitOfWork;
                _layer = unitOfWork._layer;
                _disposed = false;
            }

            public void Complete()
            {
                _hasBeenCompleted = true;
                if (_layer == 0)
                    Dispose();
            }

            public async Task CompleteAsync()
            {
                _hasBeenCompleted = true;
                if (_layer == 0)
                    await DisposeAsync();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _unitOfWork.DisposeScope(_hasBeenCompleted);
                }
            }

            public async ValueTask DisposeAsync()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    await _unitOfWork.DisposeScopeAsync(_hasBeenCompleted);
                }
            }
        }
    }
}
