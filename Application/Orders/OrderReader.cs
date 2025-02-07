using Application.Orders.Mappers;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderReader(
    IRepository<Order> orderRepo,
    IUnitOfWork unitOfWork)
    : IOrderReader
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto ReadOrder(string orderNumber)
    {
        // As there is not validaition you could pass any character in as a orderNumber
        var order = _orderRepo.Get(x => x.OrderNumber == orderNumber)
            // Are these just looking for their own properties? TODO (check this)
                              .Include(x => x.OrderItems)
                              .ThenInclude(x => x.Variant)
                              .ThenInclude(x => x.Product)
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .Include(x => x.BillingAddress)
                              .SingleOrDefault()
            ;

        // Is anything being saved here?
        // Is it to say that this product has been accessed from the db?, don't think this is needed
        _unitOfWork.Save();

        // There isn't a null check for the order which will make the mapper fall over

        /*
         * Thought: Should this method return the Order from the DB and create a seperate method that uses this and then returns the mapped order?
         * You could then add the null check at a higher level and then throw an exception
         */
        return new OrderDtoMapper().Map(order);
    }
}