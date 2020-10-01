using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccountOwnerServer.Controllers
{
    [Route("api/Owner")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repositoryWrapper;
        private IMapper _mapper;

        public OwnerController(ILoggerManager logger,
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper)
        {
            _logger = logger;
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllOwners()
        {
            try
            {
                var owners = _repositoryWrapper.Owner.GetAllOwners();
                _logger.LogInfo($"Returned all owners from database.");
                var ownersResult = _mapper.Map<IEnumerable<OwnerDto>>(owners);
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Somthing went wrong inside GetAllOwners action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "OwnerById")]
        public IActionResult GetOwnerById(Guid id)
        {
            try
            {
                var owner = _repositoryWrapper.Owner.GetOwnerById(id);

                if (owner == null)
                {
                    _logger.LogError($"owner with id : {id} has not been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id : {id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in GetOwnerById action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/account")]
        public IActionResult GetOwnerWithDetails(Guid Id)
        {
            try
            {
                var owner = _repositoryWrapper.Owner.GetOwnerWithDetails(Id);

                if (owner == null)
                {
                    _logger.LogError($"owner with id: {Id} has'nt been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with details for id: {Id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOwnerWithDetails action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateOwner([FromBody]OwnerForCreationDto owner)
        {
            try
            {
                if (owner == null)
                {
                    _logger.LogError($"owner object sent from client is null at : {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"owner object sent from client is null at : {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("Invalid model object");
                }

                var ownerEntity = _mapper.Map<Owner>(owner);

                _repositoryWrapper.Owner.CreateOwner(ownerEntity);
                _repositoryWrapper.Save();

                var createdOwner = _mapper.Map<OwnerDto>(ownerEntity);
                return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatedOwner action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOwner(Guid id, [FromBody]OwnerForUpdateDto owner)
        {
            try
            {
                if(owner == null)
                {
                    _logger.LogError("Owner object sent from client is null");
                    return BadRequest("Owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid owner object is sent from client.");
                    return BadRequest("Invalid model object");
                }

                var ownerEntity = _repositoryWrapper.Owner.GetOwnerById(id);
                if(ownerEntity == null)
                {
                    _logger.LogError($"owner with id: {id} has'nt been in db.");
                    return NotFound();
                }

                _mapper.Map(owner, ownerEntity);

                _repositoryWrapper.Owner.UpdateOwner(ownerEntity);
                _repositoryWrapper.Save();

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Somthing went wrong inside UpdateOwner action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(Guid id)
        {
            try
            {
                var owner = _repositoryWrapper.Owner.GetOwnerById(id);
                if (owner == null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if (_repositoryWrapper.Account.AccountsByOwner(id).Any())
                {
                    _logger.LogError($"Cannot delete owner with id: {id}. It has related accounts. Delete those accounts first");
                    return BadRequest("Cannot delete owner. It has related accounts. Delete those accounts first");
                }
                _repositoryWrapper.Owner.DeleteOwner(owner);
                _repositoryWrapper.Save();
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action : {ex.Message} ");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}