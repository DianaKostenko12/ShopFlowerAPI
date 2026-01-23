using AutoMapper;
using BLL.Services.OrderBouquets;
using BLL.Services.Orders;
using BLL.Services.Orders.Descriptors;
using DAL.Exceptions;
using DAL.Models.Orders;
using FlowerShopApi.Common.Extensions;
using FlowerShopApi.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopApi.Controllers
{
    [ApiController, Route("order")]
    public class OrderController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IOrderBouquetService _orderBouquetService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IOrderBouquetService orderBouquetService,IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _orderService = orderService;
            _orderBouquetService = orderBouquetService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            foreach (var orderResponse in orderResponses)
            {
                var orderBouquets = await _orderBouquetService.GetByOrderIdAsync(orderResponse.OrderId);
                orderResponse.Bouquets = _mapper.Map<List<BouquetDetails>>(orderBouquets);
            }

            return Ok(orderResponses);
        }

        [HttpGet("user/userId"), Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId()
        {
            int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
            try
            {
                var orders = await _orderService.GetOrdersByUserId(userId.Value);
                var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
                foreach (var orderResponse in orderResponses)
                {
                    var orderBouquets = await _orderBouquetService.GetByOrderIdAsync(orderResponse.OrderId);
                    orderResponse.Bouquets = _mapper.Map<List<BouquetDetails>>(orderBouquets);
                }

                return Ok(orderResponses);
            }
            catch (BusinessException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> AddOrderAsync([FromBody] CreateOrderDescriptor descriptor)
        {
            try
            {
                int? userId = _httpContextAccessor.HttpContext.User.GetUserId();
                await _orderService.AddOrderAsync(descriptor, userId.Value);
                return CreatedAtAction(nameof(GetOrders), new { userId = userId }, descriptor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{orderId}/status/{status}"), Authorize]
        public async Task<ActionResult> ChangeOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                await _orderService.ChangeOrderStatus(orderId, status);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
