using System.Text.RegularExpressions;
using Domain;

namespace Application.Orders.Validators;

public class CreateOrderRequestValidator : ICreateOrderRequestValidator
{
    public bool TryValidate(Customer customer,
                            Address billingAddress,
                            Address shippingAddress,
                            out IDictionary<string, string> errors)
    {
        errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(customer.Name))
            errors.Add(nameof(customer.Name), "Name is required");

        if (!Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)) // There should be validation to ensure email address is lowercase
            errors.Add(nameof(customer.Email), "Email is not valid");

        if (customer.Created > DateTime.Now) // should this be UTCNow?
            errors.Add(nameof(customer.Created), "Customer cannot be from the future");

        if (string.IsNullOrWhiteSpace(billingAddress.LineOne))
            errors.Add(nameof(billingAddress.LineOne), "First address line is required"); // Should this message be more specific that it is a billing address issue?

        if (string.IsNullOrWhiteSpace(billingAddress.PostCode))
            errors.Add(nameof(billingAddress.PostCode), "PostCode is required");  // Should this message be more specific that it is a billing address issue?

        if (!Regex.IsMatch(billingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$", // Could move this regex to a constant file, could also add a custom validaiton attribute to look the post code up and validate it
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(billingAddress.PostCode), "Postcode is not valid");

        if (string.IsNullOrWhiteSpace(shippingAddress.LineOne))
            errors.Add(nameof(shippingAddress.LineOne), "First address line is required");  // Should this message be more specific that it is a shipping address issue?

        if (string.IsNullOrWhiteSpace(shippingAddress.PostCode))
            errors.Add(nameof(shippingAddress.PostCode), "PostCode is required");  // Should this message be more specific that it is a shipping address issue?

        if (!Regex.IsMatch(shippingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(shippingAddress.PostCode), "Postcode is not valid");  // Should this message be more specific that it is a shippingAddress issue?

        return !errors.Any();
    }
}