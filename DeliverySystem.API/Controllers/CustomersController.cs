using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CustomerResponse>> GetAll()
    {
        var customers = _customerRepository.GetAll();
        return Ok(customers.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<CustomerResponse> GetById(Guid id)
    {
        var customer = _customerRepository.GetById(id);
        if (customer == null)
            return NotFound(new { message = $"Customer with ID {id} not found." });

        return Ok(MapToResponse(customer));
    }

    [HttpPost]
    public ActionResult<CustomerResponse> Create([FromBody] CreateCustomerRequest request)
    {
        var address = new Address(request.Address.Street, request.Address.City,
            request.Address.PostalCode, request.Address.Country);

        var customer = new Customer(request.Name, request.Email, request.Phone, address);
        _customerRepository.Add(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, MapToResponse(customer));
    }

    [HttpPut("{id:guid}")]
    public ActionResult<CustomerResponse> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = _customerRepository.GetById(id);
        if (customer == null)
            return NotFound(new { message = $"Customer with ID {id} not found." });

        customer.SetName(request.Name);
        customer.SetEmail(request.Email);
        customer.SetPhone(request.Phone);

        var address = new Address(request.Address.Street, request.Address.City,
            request.Address.PostalCode, request.Address.Country);
        customer.SetAddress(address);

        _customerRepository.Update(customer);
        return Ok(MapToResponse(customer));
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        var customer = _customerRepository.GetById(id);
        if (customer == null)
            return NotFound(new { message = $"Customer with ID {id} not found." });

        _customerRepository.Delete(id);
        return NoContent();
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            new AddressDto(
                customer.Address.Street,
                customer.Address.City,
                customer.Address.PostalCode,
                customer.Address.Country));
    }
}
