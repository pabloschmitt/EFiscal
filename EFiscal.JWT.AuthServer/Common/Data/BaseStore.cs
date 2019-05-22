using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Common.Data
{
    public class BaseStore<TEntity, TKey, TDbContext> : IDisposable
        where TEntity : class, IBaseEntity<TKey>
        where TKey : IEquatable<TKey>
        where TDbContext : DbContext

    {
        public TDbContext Context { get; private set; }

        public BaseStore(TDbContext context)
        {
            Context = context;
        }

        public bool AutoSaveChanges { get; set; } = true;

        private async Task SaveChanges(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        #region Dispose
        private bool _disposed;
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        public void Dispose() => _disposed = true;
        #endregion

        #region ID Normalizations
        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public virtual string ConvertIdToString(TKey id)
        {
            if (id.Equals(default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }
        #endregion

        #region Common CRUD-FILTER Operations
        public async virtual Task<OperationResult> CreateAsync(TEntity o, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            Context.Add(o);
            await SaveChanges(cancellationToken);
            return OperationResult.Success;
        }

        public async virtual Task<OperationResult> UpdateAsync(TEntity o, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            Context.Attach(o);
            Context.Update(o);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failed("Error de Concurrencia");
            }
            return OperationResult.Success;
        }

        public async virtual Task<OperationResult> DeleteAsync(TEntity o, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            Context.Remove(o);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failed("Error de Concurrencia");
            }
            return OperationResult.Success;
        }

        public virtual Task<string> GetIdAsync(TEntity o, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            return Task.FromResult(ConvertIdToString(o.Id));
        }

        public virtual Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var objetId = ConvertIdFromString(id);
            return Entities.FirstOrDefaultAsync(e => e.Id.Equals(objetId), cancellationToken);
        }

        public virtual Task<TEntity> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {


            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Entities.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);
        }

        public virtual async Task<List<TEntity>> FindAsync(
            Func<TEntity, bool> condition = null,
            Func<TEntity, object> orderBy = null, ServiceFindOrderBy isDesc = ServiceFindOrderBy.Asc,
            int pageIndex = 0, int pageSize = 0, CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            #region Construccion del Query
            List<IEnumerable<TEntity>> data = new List<IEnumerable<TEntity>>();

            // Add filter is needed
            if (condition != null)
            {
                data.Add(Entities.Where(condition));
            }
            else
            {
                data.Add(Entities);
            }

            // Calculate records count with filter
            var count = data.Last<IEnumerable<TEntity>>().Count();

            // Apply orders
            if (orderBy != null)
            {
                if (isDesc != ServiceFindOrderBy.Desc)
                {
                    data.Add(data.Last<IEnumerable<TEntity>>().OrderBy(orderBy));
                }
                else
                {
                    data.Add(data.Last<IEnumerable<TEntity>>().OrderByDescending(orderBy));
                }
            }
            #endregion

            #region Pagination Logic
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            pageSize = pageSize == 0 ? count : pageSize;

            var final_query = data.Last<IEnumerable<TEntity>>()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize == 0 ? count : pageSize);

            #endregion

            return await final_query.AsQueryable<TEntity>().ToListAsync(cancellationToken);
        }


        #endregion

        public virtual IQueryable<TEntity> Entities => Context.Set<TEntity>();

    }
}
