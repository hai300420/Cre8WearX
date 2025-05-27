using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using _3_Repository.Repository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.IRepository;
using Repository.Repository;

namespace Service
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ClothesCusShopContext _context;
        private IProductRepository _productRepository;
        private IUserRepository _userRepository;
        private IFeedbackRepository _feedbackRepository;
        private IRoleRepository _roleRepository;
        private INotificationRepository _notificationRepository;
        private ICategoryRepository _categoryRepository;
        private ICustomizeProductRepository _customizeProductRepository;
        private IDesignElementRepository _designElementRepository;
        private IDesignAreaRepository _designAreaRepository;
        private IOrderRepository _orderRepository;
        private IOrderStageRepository _orderStageRepository;
        private IDbContextTransaction _transaction;
        public UnitOfWork(ClothesCusShopContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                _categoryRepository ??= new CategoryRepository(_context);
                return _categoryRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                _productRepository ??= new ProductRepository(_context);
                return _productRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }
        public INotificationRepository NotificationRepository
        {
            get
            {
                return _notificationRepository ??= new NotificationRepository(_context);
            }
        }

        public IFeedbackRepository FeedbackRepository
        {
            get
            {
                _feedbackRepository ??= new FeedbackRepository(_context);
                return _feedbackRepository;
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                _roleRepository ??= new RoleRepository(_context);
                return _roleRepository;
            }
        }

        public ICustomizeProductRepository CustomizeProductRepository
        {
            get
            {
                _customizeProductRepository ??= new CustomizeProductRepository(_context);
                return _customizeProductRepository;
            }
        }

        public IDesignElementRepository DesignElementRepository
        {
            get
            {
                _designElementRepository ??= new DesignElementRepository(_context);
                return _designElementRepository;
            }
        }
        public IDesignAreaRepository DesignAreaRepository
        {
            get
            {
                _designAreaRepository ??= new DesignAreaRepository(_context);
                return _designAreaRepository;
            }
        }

        public IOrderRepository OrderRepository
        {
            get { return _orderRepository ??= new OrderRepository(_context); }
        }

        public IOrderStageRepository OrderStageRepository
        {
            get { return _orderStageRepository ??= new OrderStageRepository(_context); }
        }

        // Xóa dòng này đi:
        // object IUnitOfWork.CategoryRepository => throw new NotImplementedException();

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving changes to database", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }
        // Transaction methods
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

    }

}
