using System.Security.Cryptography;
using System.Text;

namespace Domain;

public class Order
{
    // This could be apart if the main constructor if needed, though doesn't look used? 
    private Order()
    {
        OrderItems = new HashSet<OrderItem>();
    }

    public Order(Customer customer,
                 Address shippingAddress,
                 Address billingAddress)
        : this() 
    {
        CustomerId = customer.CustomerId;
        Customer = customer;

        ShippingAddressId = shippingAddress.AddressId;
        ShippingAddress = shippingAddress;

        BillingAddressId = billingAddress.AddressId;
        BillingAddress = billingAddress;

        Created = DateTime.Now;
        LastModified = DateTime.Now;

        OrderNumber = GenerateOrderNumber();
    }

    public int OrderId { get; private set; }
    public string OrderNumber { get; private set; }

    public DateTime Created { get; private set; }
    public DateTime LastModified { get; private set; }

    public decimal TotalPrice { get; private set; }

    // you can get these properties from the objects below, could these be removed?
    public int CustomerId { get; private set; } 
    public int BillingAddressId { get; private set; }
    public int ShippingAddressId { get; private set; }

    public Customer Customer { get; private set; }
    public Address BillingAddress { get; private set; }
    public Address ShippingAddress { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; }

    private string GenerateOrderNumber()
    {
        // Th8e ShippingAddress.LineTwo is nullable which may cause errors, should it be ShippingAddress.PostCode
        var seed =
            $"{Customer.Email}|{BillingAddress.LineOne}{BillingAddress.PostCode}|{ShippingAddress.LineOne}{ShippingAddress.LineTwo}|{Created}";

        using var md5 = MD5.Create();

        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));

        var hashGuid = new Guid(bytes);

        // not sure its an issue as it should still be unique but when the milenium ticks over 3000 % 1000 == 0
        return $"ORD-{Created.Year % 1000}{Created.Month}-{hashGuid.GetHashCode()}";
    }

    public void UpdateItems(IDictionary<Variant, int> orderItems)
    {
        // might be useful for readability to state the params with the variant: x.Key quantity: x.Value
        var orderedItems = orderItems.Select(x => new OrderItem(this,
                                                                x.Key,
                                                                x.Value))
                                     .ToList();
        // Loop isn't required
        // OrderItems = orderedItems;
        foreach (var item in orderedItems)
            OrderItems.Add(item);

        // Is x.Value quantity? could this be reworked to use object property names?
        TotalPrice = orderItems.Select(x => x.Key.Price * x.Value).Sum();
    }

    // This method could accept an address object
    public void UpdateShippingAddress(string lineOne,
                                      string lineTwo,
                                      string lineThree,
                                      string postCode)
    {
        ShippingAddress.Update(lineOne,
                               lineTwo,
                               lineThree,
                               postCode);
    }
}