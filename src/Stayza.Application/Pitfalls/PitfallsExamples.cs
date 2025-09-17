namespace Stayza.Application.Pitfalls;

public class PitfallsExamples
{
    #region Pifall 1 : Leaking Domain logic into Application layer

    // public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orderRepo;
    //     private readonly ICustomerRepository _customerRepo;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var customer = await _customerRepo.GetByIdAsync(request.CustomerId);
    //
    //         // ❌ Business rule: customers must be active to place an order
    //         if (!customer.IsActive)
    //             throw new InvalidOperationException("Inactive customers cannot place orders");
    //
    //         // ❌ Business rule: order must have at least one item
    //         if (!request.Items.Any())
    //             throw new InvalidOperationException("Cannot place empty order");
    //
    //         var order = new Order(request.CustomerId, request.Items);
    //         await _orderRepo.SaveAsync(order);
    //         return order.Id;
    //     }
    // }

    #endregion
    #region Pitfal 1 : Solution

    // public class Customer : Entity
    // {
    //     public bool IsActive { get; private set; }
    //
    //     public Order PlaceOrder(List<OrderItem> items)
    //     {
    //         if (!IsActive)
    //             throw new InvalidOperationException("Inactive customers cannot place orders");
    //
    //         if (items == null || !items.Any())
    //             throw new InvalidOperationException("Cannot place empty order");
    //
    //         return new Order(Id, items);
    //     }
    // }
    // public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly ICustomerRepository _customerRepo;
    //     private readonly IOrderRepository _orderRepo;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var customer = await _customerRepo.GetByIdAsync(request.CustomerId);
    //         var order = customer.PlaceOrder(request.Items); // ✅ domain enforces rules
    //         await _orderRepo.SaveAsync(order);
    //         return order.Id;
    //     }
    // }
    

    #endregion
    
    #region Pitfall 2 : God service
    
    // public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    // {
    //     private readonly AppDbContext _db;
    //
    //     public CreateOrderHandler(AppDbContext db)
    //     {
    //         _db = db;
    //     }
    //
    //     public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    //     {
    //         // ❌ Business rules in handler
    //         if (request.Items.Count == 0)
    //             throw new InvalidOperationException("Order must have at least one item.");
    //
    //         if (request.Items.Sum(i => i.Price * i.Quantity) < 10)
    //             throw new InvalidOperationException("Minimum order total is 10 EUR.");
    //
    //         // ❌ Mapping done in handler
    //         var order = new Order
    //         {
    //             Id = Guid.NewGuid(),
    //             CustomerName = request.CustomerName,
    //             Items = request.Items.Select(i => new OrderItem { ... }).ToList(),
    //             Status = "Pending"
    //         };
    //
    //         // ❌ Persistence logic inline
    //         _db.Orders.Add(order);
    //         await _db.SaveChangesAsync(ct);
    //
    //         return order.Id;
    //     }
    // }
    
    
    #endregion
    #region Pitfall 2 : Solution

    // public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //
    //     public CreateOrderHandler(IOrderRepository orders)
    //     {
    //         _orders = orders;
    //     }
    //
    //     public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    //     {
    //         // Orchestration only
    //         var order = Order.Create(request.CustomerName, request.Items);
    //
    //         await _orders.AddAsync(order, ct);
    //
    //         return order.Id;
    //     }
    // }
    //
    // public class Order : AggregateRoot
    // {
    //     private readonly List<OrderItem> _items = new();
    //
    //     private Order(string customerName, IEnumerable<OrderItemDto> items)
    //     {
    //         if (!items.Any())
    //             throw new DomainRuleViolation("Order must have at least one item.");
    //
    //         if (items.Sum(i => i.Price * i.Quantity) < 10)
    //             throw new DomainRuleViolation("Minimum order total is 10 EUR.");
    //
    //         Id = Guid.NewGuid();
    //         CustomerName = customerName;
    //         _items.AddRange(items.Select(i => new OrderItem(i.ProductId, i.Price, i.Quantity)));
    //     }
    //
    //     public static Order Create(string customerName, IEnumerable<OrderItemDto> items)
    //         => new Order(customerName, items);
    // }

    #endregion

    #region Pitfall 3 : Write during read

    // public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken ct)
    // {
    //     var messages = await _db.Messages
    //         .Where(m => m.UserId == request.UserId)
    //         .ToListAsync(ct);
    //
    //     foreach (var m in messages)
    //         m.Seen = true; // ❌ mutation in query
    //
    //     await _db.SaveChangesAsync(ct);
    //
    //     return messages.Select(m => new MessageDto(...));
    // }

    #endregion
    #region Pitfall 3 : Solution 1 - Split into 2

    // var messages = await _mediator.Send(new GetMessagesQuery(userId));
    // await _mediator.Send(new MarkMessagesAsSeenCommand(userId, messages.Select(m => m.Id)));

    #endregion
    #region Pitfall 3 : Solution 2 - Implicit command triggering
    // public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken ct)
    // {
    //     var messages = await _db.Messages
    //         .Where(m => m.UserId == request.UserId)
    //         .Select(m => new MessageDto(m.Id, m.Text, m.Seen))
    //         .ToListAsync(ct);
    //
    //     // Fire-and-forget or inline
    //     await _mediator.Send(new MarkMessagesAsSeenCommand(request.UserId, messages.Select(m => m.Id)), ct);
    //
    //     return messages;
    // }
    #endregion
    #region Pitfall 3 : Solution 3 - Eventual consistency using domain events
    // Samples already shown before
    #endregion

    #region Pitfall 4 : Overusing Transactions Across Aggregates
    // public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //     private readonly ICustomerRepository _customers;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var order = new Order(request.CustomerId, request.Items);
    //         var customer = await _customers.GetByIdAsync(request.CustomerId);
    //
    //         // ❌ Trying to update two aggregates in one transaction
    //         customer.IncreaseOrderCount();
    //
    //         await _orders.SaveAsync(order, ct);
    //         await _customers.SaveAsync(customer, ct); // problematic
    //
    //         return order.Id;
    //     }
    // }
    #endregion
    #region Pitfall 4 : Solution 1 - One aggregate per command + Domain events
    //
    // // Order aggregate
    // public class Order : AggregateRoot
    // {
    //     public void Place()
    //     {
    //         // business rules
    //         DomainEvents.Raise(new OrderPlaced(Id, CustomerId));
    //     }
    // }
    //
    // // Command handler
    // public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var order = new Order(request.CustomerId, request.Items);
    //         order.Place();
    //
    //         await _orders.SaveAsync(order, ct); // only this aggregate
    //         return order.Id;
    //     }
    // }
    //
    // // Event handler updates the customer asynchronously
    // public class OrderPlacedHandler : INotificationHandler<OrderPlaced>
    // {
    //     private readonly ICustomerRepository _customers;
    //
    //     public async Task Handle(OrderPlaced @event, CancellationToken ct)
    //     {
    //         var customer = await _customers.GetByIdAsync(@event.CustomerId);
    //         customer.IncreaseOrderCount();
    //         await _customers.SaveAsync(customer, ct);
    //     }
    // }

    #endregion
    #region Pitfall 4 : Solution 2 - Use Sagas / Process Managers for complex flows

    // public class OrderSaga
    // {
    //     public async Task Handle(OrderPlaced @event)
    //     {
    //         // orchestrates multiple aggregates
    //         var customer = await _customerRepo.GetByIdAsync(@event.CustomerId);
    //         customer.IncreaseOrderCount();
    //         await _customerRepo.SaveAsync(customer);
    //
    //         var invoice = new Invoice(@event.OrderId);
    //         await _invoiceRepo.SaveAsync(invoice);
    //     }
    // }

    #endregion

    #region Pitfall 5: Distributed transactions across multiple DbContexts

    // Caveat if all of the DbContexts are for the same database should be fine to use below approach.
    // If you need to enroll a second database, then a Distributed Transaction Coordinator is required, which
    // needs to be installed and setup on the sql server machine, and then allow firewall rules and network routes to and from the client machine
    // (DTC needs to be able to reach back to the client directly as well).
    
    // [HttpGet(Name = "Register")]
    // public int[] Get(int quantity = 1)
    // {
    //     var transaction = orderCtx.Database.BeginTransaction();
    //     stockCtx.Database.UseTransaction(transaction.GetDbTransaction());
    //
    //     try
    //     {
    //         orderCtx.OrderCounters.Find(1)!.Value += quantity;
    //         stockCtx.StockCounters.Find(1)!.Value -= quantity;
    //
    //         orderCtx.SaveChanges();
    //         if (quantity >= 10)
    //             throw new ApplicationException("Something went wrong.");
    //         stockCtx.SaveChanges();
    //
    //         transaction.Commit();
    //     }
    //     catch (Exception)
    //     {
    //         transaction.Rollback();
    //     }
    //
    //     // ...
    // }

    #endregion

    #region Pitfall 6: Skipping transaction boundaries

    // public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //     private readonly ICustomerRepository _customers;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var order = new Order(request.CustomerId, request.Items);
    //         await _orders.SaveAsync(order, ct);
    //
    //         var customer = await _customers.GetByIdAsync(request.CustomerId);
    //         customer.IncreaseOrderCount();
    //         await _customers.SaveAsync(customer, ct); // separate operation
    //
    //         return order.Id;
    //     }
    // }

    #endregion
    #region Pitfall 6: Solution 1 - Use DB transaction per command
    
    // public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    // {
    //     using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    //
    //     var order = new Order(request.CustomerId, request.Items);
    //     await _orders.SaveAsync(order, ct);
    //
    //     var customer = await _customers.GetByIdAsync(request.CustomerId);
    //     customer.IncreaseOrderCount();
    //     await _customers.SaveAsync(customer, ct);
    //
    //     await transaction.CommitAsync(ct);
    //     return order.Id;
    // }

    #endregion
    #region Pitfall 6: Solution 2 - EfContext / UoW

    // public class UnitOfWork : IUnitOfWork
    // {
    //     private readonly AppDbContext _db;
    //
    //     public Task CommitAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    // }
    //
    // public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    // {
    //     var order = new Order(request.CustomerId, request.Items);
    //     _orders.Add(order);
    //
    //     var customer = await _customers.GetByIdAsync(request.CustomerId);
    //     customer.IncreaseOrderCount();
    //
    //     await _unitOfWork.CommitAsync(ct);
    //     return order.Id;
    // }

    #endregion

    #region Pitfall 7: Synchronous integration / Ignoring events

    // public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //     private readonly IEmailService _emailService;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var order = new Order(request.CustomerId, request.Items);
    //     
    //         await _orders.SaveAsync(order, ct);
    //         
    //         // ❌ Application Layer directly triggers side effects before finishing transaction.
    //         await _emailService.SendOrderConfirmation(order.CustomerId, order.Id);
    //     
    //         return order.Id;
    //     }
    // }

    #endregion
    #region Pitfall 7: Solution - Asynchronously integrate via events

    // public class Order : AggregateRoot
    // {
    //     public void PlaceOrder(List<OrderItem> items)
    //     {
    //         if (!items.Any())
    //             throw new DomainException("Cannot place empty order.");
    //
    //         _items.AddRange(items);
    //         DomainEvents.Raise(new OrderPlaced(Id, CustomerId));
    //     }
    // }
    //
    // public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
    // {
    //     private readonly IOrderRepository _orders;
    //     private readonly IDomainEventDispatcher _dispatcher;
    //
    //     public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    //     {
    //         var order = new Order();
    //         order.PlaceOrder(request.Items);
    //
    //         await _orders.SaveAsync(order, ct);
    //
    //         // ✅ Dispatch domain events raised by aggregate here or in interceptors.
    //         await _dispatcher.DispatchAsync(order.DomainEvents, ct);
    //
    //         return order.Id;
    //     }
    // }
    //
    // public class SendOrderConfirmationHandler : INotificationHandler<OrderPlaced>
    // {
    //     private readonly IEmailService _emailService;
    //
    //     public async Task Handle(OrderPlaced @event, CancellationToken ct)
    //     {
    //         await _emailService.SendOrderConfirmation(@event.CustomerId, @event.OrderId);
    //     }
    // }

    #endregion
}