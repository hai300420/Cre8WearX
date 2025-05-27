    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Linq.Expressions;
using System.Text;
    using System.Threading.Tasks;

    namespace Repository
    {
        public interface IGenericRepository<T> where T : class
        {
            IQueryable<T> GetAll();

            IQueryable<T> GetAllWithInclude(string entity);

            IQueryable<T> GetAllWithTwoInclude(string entity1, string entity2);

        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        void Create(T entity);

            Task<int> CreateAsync(T entity);

            void Update(T entity);

            Task<int> UpdateAsync(T entity);

            bool Remove(T entity);

            Task<bool> RemoveAsync(T entity);

            T GetById(int id);

            Task<T> GetByIdAsync(int id);

            T GetByName(string name);

            Task<T> GetByNameAsync(string name);
        }
    }
