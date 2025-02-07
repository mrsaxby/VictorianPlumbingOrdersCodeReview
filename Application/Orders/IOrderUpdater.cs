using Client.Dtos;

namespace Application.Orders;

public interface IOrderUpdater
{
    // Is there a benefit to having the Reader/Updater/Creator as seperate interfaces?
    OrderDto UpdateOrder(UpdateOrderRequestDto request);
}