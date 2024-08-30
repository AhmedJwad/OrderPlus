using OrderPlus.Backend.UnitsOfWork.Implementations;
using OrderPlus.Backend.UnitsOfWork.Interfaces;
using OrderPlus.Shared.DTOs;
using OrderPlus.Shared.Entites;
using OrderPlus.Shared.Enums;
using OrderPlus.Shared.Responses;

namespace OrderPlus.Backend.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly ITemporalOrdersUnitOfWork _temporalOrdersUnitOfWork;
        private readonly IProductsUnitOfWork _productsUnitOfWork;
        private readonly IOrdersUnitOfWork _ordersUnitOfWork;
        private readonly IKardexUnitOfWork _kardexUnitOfWork;

        public OrdersHelper(IUsersUnitOfWork usersUnitOfWork , ITemporalOrdersUnitOfWork temporalOrdersUnitOfWork,
            IProductsUnitOfWork productsUnitOfWork, IOrdersUnitOfWork ordersUnitOfWork, IKardexUnitOfWork
             kardexUnitOfWork)
        {
           _usersUnitOfWork = usersUnitOfWork;
           _temporalOrdersUnitOfWork = temporalOrdersUnitOfWork;
           _productsUnitOfWork = productsUnitOfWork;
           _ordersUnitOfWork = ordersUnitOfWork;
           _kardexUnitOfWork = kardexUnitOfWork;
        }
        public async Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO)
        {
           var user=await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Invalid user"
                };
            }
            var actionTemporalOrders = await _temporalOrdersUnitOfWork.GetAsync(email);
            if (!actionTemporalOrders.WasSuccess)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "There is no detail in the order"
                };
            }
            var temporalOrders=actionTemporalOrders.Result as List<TemporalOrder>;
            var response = await CheckInventoryAsync(temporalOrders!);
            if (!response.WasSuccess)
            {
                return response;
            }
            var order = new Order
            {
                Date = DateTime.UtcNow,
                User = user,
                Remarks = orderDTO.Remarks,
                OrderDetails = new List<OrderDetail>(),
                OrderStatus = OrderStatus.New,
                OrderType = orderDTO.OrderType,
            };
            if(orderDTO.OrderType==OrderType.PayOnLine)
            {
                order.OrderPayments=new List<OrderPayment>();
                order.OrderPayments.Add(new OrderPayment
                {
                    BankId = orderDTO.BankId,
                    Date = DateTime.UtcNow,
                    Reference = orderDTO.Reference,
                    Email = orderDTO.Email,
                    Value = orderDTO.Value,
                   
                });
            }
            foreach (var item in temporalOrders!)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    Product=item.Product,
                    Quantity=item.Quantity,
                    Description=item.Product!.Description,
                    Remarks=item.Remarks,
                    Price=item.Product!.Price,
                    Image=item.Product!.MainImage,
                    Name=item.Product!.Name,
                });
                var actionproduct=await _productsUnitOfWork.GetAsync(item.Product!.Id);
                if(actionproduct.WasSuccess)
                {
                    var product = actionproduct.Result;
                    if(product != null)
                    {
                        var kardexDTO = new KardexDTO
                        {
                            Date = order.Date,
                            ProductId = item.ProductId,
                            KardexType = KardexType.Order,
                            Cost = product.Cost,
                            Quantity = item.Quantity
                        };
                        await _kardexUnitOfWork.AddAsync(kardexDTO);
                    }
                }
                await _temporalOrdersUnitOfWork.DeleteAsync(item.Id);
            }
            await _ordersUnitOfWork.AddAsync(order);
            return response;
        }

        private async Task<ActionResponse<bool>> CheckInventoryAsync(List<TemporalOrder> temporalOrders)
        {
            var response= new ActionResponse<bool>() { WasSuccess=true};
            foreach (var temporalOrder in temporalOrders)
            {
                var actionProduct = await _productsUnitOfWork.GetAsync(temporalOrder.Product!.Id);
                if(!actionProduct.WasSuccess)
                {
                    response.WasSuccess = false;
                    response.Message= $"The product {temporalOrder.Product!.Id}, no longer available";
                    return response;
                }
                var product = actionProduct.Result;
                if(product == null)
                {
                    response.WasSuccess = false;
                    response.Message = $"The product {temporalOrder.Product!.Id}, no longer available";
                    return response;
                }
                if(product.Stock <temporalOrder.Quantity)
                {
                    response.WasSuccess = false;
                    response.Message = $"Sorry we do not have enough stock of the product {temporalOrder.Product!.Name}" +
                        $", to take your order. Please reduce the quantity or replace it with another.";
                    return response;
                }
               
            }
            return response;
        }
    }
}
