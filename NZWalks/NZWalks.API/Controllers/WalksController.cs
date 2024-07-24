using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Runtime.CompilerServices;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper,IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        




        //Create Walk
        //Post: /api/walks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            //model validation
            if( ! ModelState.IsValid) return BadRequest(ModelState);
            // Map addWalkRequestDto to Domain Model
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walkDomainModel);

            //Map Domain model to WalkDTO

            return Ok(mapper.Map<WalkDto2>(walkDomainModel));


        }

        //Get Walks
        //GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=5
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll([FromQuery]string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy , [FromQuery] Boolean? isAscending, [FromQuery] int pageNumber=1 , [FromQuery] int pageSize=100)
        {
            var domainModel  = await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending??true,pageNumber,pageSize);

            //Create an exception
            throw new Exception("this is a new Exception");

            return Ok( mapper.Map<List<WalkDto>>(domainModel));
         }
        //Get Walk by id
        // GET: /api/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetByIdAsync(id);
            if(walkDomainModel == null) return NotFound();
            //map domain model to dto

            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        //Update a Walk
        //PUT:/api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto walkDto )
        {
           var WalkdomainModel = mapper.Map<Walk>(walkDto);
           var domainModel = await walkRepository.UpdateAsync(id,WalkdomainModel);
            if(domainModel == null) return NotFound();

            //mapping to dto
            return Ok(mapper.Map<WalkDto>(domainModel));
            

        }

        //Delete a Walk
        //Delete: /api/walks/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var domainModel = await walkRepository.DeleteAsync(id);
            if(domainModel == null) return NotFound();

            // map to dto
            return Ok(mapper.Map<WalkDto>(domainModel));
           

        }
    
    }
}
