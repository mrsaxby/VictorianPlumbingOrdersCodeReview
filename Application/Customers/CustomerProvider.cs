using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Customers;

public class CustomerProvider(IRepository<Customer> customerRepo,
                              IUnitOfWork unitOfWork) : ICustomerProvider
{
    private readonly IRepository<Customer> _customerRepo = customerRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Customer GetCustomer(CustomerDto request)
    {
        // This could lead to a mismatch as there is no validation to ensure emailaddress is entered as lowercase

        /* EXAMPLE BUG REPORTING
         
            Description:
            The customer's email address cannot be located in the database

            Initial investigation:
            There is no validation to prevent an email address from being entered uppercase.
            Alternatively rather than preventing upper case letters being entered apply formatting to the input to convert it into the expected format

            Expected Result:
            Test@Googlemail.com => test@googlemail.com

            Actual Result:
            Test@Googlemail.com => Test@Googlemail.com

            Steps to re-produce
            1. Navigate to locahost 
            2. If the test user does not exist in the database. 
	            2.1 Create one by following these steps:
	            2.2 Using the API (insert URL)
	            2.3 Insert this JSON body and submit
		            (Some invalid JSON	

            3. Run the application and call the GET orders API
        */

        var customer = _customerRepo.Get(x => x.Email == request.EmailAddress.ToLowerInvariant())
                                    .SingleOrDefault();

        if (customer is not null)
            return customer;


        // method doesn't make sense to create a new user here
        customer = new Customer(request.EmailAddress,
                                request.CustomerName,
                                request.PhoneNumber);

        customerRepo.Insert(customer);

        _unitOfWork.Save();

        return customer;
    }
}