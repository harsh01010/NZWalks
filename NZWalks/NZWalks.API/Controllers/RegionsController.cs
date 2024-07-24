using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext,IRegionRepository regionRepository,
            IMapper mapper,
            ILogger<RegionsController>logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }



        // GET ALL REGIONS
        //https://localhost:portnumber/api/regions

        [HttpGet]
        [Authorize(Roles ="Reader")]
       public async Task<IActionResult> GetAll()
        {
            try
            {
                //throw new Exception("This is custom exception");
                //GET data from database -> domain model
                //var regionsDomain = await dbContext.Regions.ToListAsync();
                var regionsDomain = await regionRepository.GetAllAsync();
                //Map domain models to the DTOs
                //var regionsDto = new List<RegionDto>();

                /*
                foreach(var region in regionsDomain) 
                 {
                    regionsDto.Add(new RegionDto()
                    {
                        Id = region.Id,
                        Code = region.Code,
                        Name = region.Name,
                        RegionImageUrl = region.RegionImageUrl
                    });

                }
                */
                var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
                logger.LogInformation($"Data: {JsonSerializer.Serialize(regionsDomain)}");
                //Return DTOs
                return Ok(regionsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,ex.Message);
                throw;
            }

        }
        // GET SINGLE REGION BY ID
        // GET:https://localhost:pn/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById( [FromRoute] Guid id)
        {
            // dbContext.Regions.Where(curr => curr.Id == id).ToList();
            // var region = dbContext.Regions.Find(id);
            //Get region domain model from the database
            // var region = await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id== id);
            var region = await regionRepository.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound(); // 404
            }
            //map to dto
            /*
            var regionDto = new RegionDto()
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                RegionImageUrl = region.RegionImageUrl
            };
            */
            var regionDto = mapper.Map<RegionDto>(region);
            //return the region dto
            if (regionDto == null)
            {
                return NotFound(); // 404
            }
            return Ok(regionDto);
        }

        // POST To create new region
        //POST: https://localhost:pn/api/regions

        [HttpPost]
        [ValidateModel]
        public async  Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Model Validation
          // if( ! ModelState.IsValid) return BadRequest(ModelState);
            // map or convert dto to domain mode
            /*
            var regionDomainModel = new Region()
            {
                Id = Guid.NewGuid(),
                Code = addRegionRequestDto.Code ,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };
            */
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);
            //use Domain Model to create Region
           // await dbContext.Regions.AddAsync(regionDomainModel);
            //await dbContext.SaveChangesAsync();
           regionDomainModel= await regionRepository.CreateAsync(regionDomainModel);

            //map domain model back to dto
            /*
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };
            */
            var regionDto = mapper.Map<AddRegionRequestDto>(regionDomainModel);
            return CreatedAtAction(nameof(GetById), new {id=regionDomainModel.Id},regionDto);
        }


        //Update region
        //PUT: https://localhost:pn//api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto  )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //check if region exists

            /*
            var regionDomainModel= await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
           if(regionDomainModel == null)
            {
                return NotFound();
            }

          //  Map Dto to domain model

           regionDomainModel.Code = updateRegionRequestDto.Code;
           regionDomainModel.Name = updateRegionRequestDto.Name;
           regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl; 
            
            await dbContext.SaveChangesAsync();
            */

            // DTO to domain model
            /*
            var regionDomainModel = new Region
            {
                Code = updateRegionRequestDto.Code,
                Name = updateRegionRequestDto.Name,
                RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            };
            */
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            if (regionDomainModel == null)
                return NotFound();

            //Convert Domain Model to DTO
            /*
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };
            */
            var regionDto = mapper.Map<UpdateRegionRequestDto>(regionDomainModel);
            return Ok(regionDto); 
        
        }


        //Delete a Region
        //DELETE: https://localhost:pn//api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id == id);
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                    return NotFound();
            }


            //dbContext.Regions.Remove(regionDomainModel);
          // await dbContext.SaveChangesAsync();

            //Return deleted Region back(optional)
            var regionDto  = new RegionDto
            { Id = regionDomainModel.Id,
            Name=regionDomainModel.Name,
            Code=regionDomainModel.Code,
            RegionImageUrl=regionDomainModel.RegionImageUrl};
            return Ok(regionDto);
        }
    }
}
