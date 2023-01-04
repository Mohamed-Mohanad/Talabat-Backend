using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Dtos;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers
{
    [Authorize]
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        [HttpGet("GetBasketById")]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketByIdAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }
        [HttpPost("UpdateBasket")]
        public async Task<ActionResult<CustomerBasketDto>> UpdateBasket(CustomerBasketDto basket)
        {
            var customerBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket);
            return Ok(updatedBasket);
        }
        [HttpDelete("DeleteBasket")]
        public async Task<bool> DeleteBasket(string id)
            => await _basketRepository.DeleteBasketAsync(id);
    }
}
