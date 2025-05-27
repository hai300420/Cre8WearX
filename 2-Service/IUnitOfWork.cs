using System.Threading.Tasks;
using _3_Repository.IRepository;
using Repository.IRepository;

namespace Service
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        IFeedbackRepository FeedbackRepository { get; }
        ICategoryRepository CategoryRepository { get; }  
        ICustomizeProductRepository CustomizeProductRepository { get; }
        IRoleRepository RoleRepository { get; }
        IDesignElementRepository DesignElementRepository { get; }
        IDesignAreaRepository DesignAreaRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderStageRepository OrderStageRepository { get; }

        Task<int> SaveChangesAsync();
        void Dispose();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
