using SportsStore.Domain.Entities;
namespace SportsStore.Domain.Abstract
{
    public interface IOrderProcessor
    {
        void ProcessOrder(Cart cart, OrderHeader orderHeader, string userId, string userEmail);
    }
}

