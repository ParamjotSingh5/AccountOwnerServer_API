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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountOwnerServer.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repositoryWrapper;
        private IMapper _mapper;

        public AccountController(ILoggerManager logger,
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper)
        {
            _logger = logger;
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            try
            {
                var accounts = _repositoryWrapper.Account.GetAllAccounts();
                _logger.LogInfo($"Retuned all of the accounts");
                var accountsResult = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                return Ok(accountsResult);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllAccounts action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{Id}", Name ="accountById")]
        public IActionResult GetAccountById(Guid Id)
        {
            try
            {
                var account = _repositoryWrapper.Account.GetAccountById(Id);

                if(account == null)
                {
                    _logger.LogError($"account with id: {Id} has not been found inside db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned account with id : {Id}");

                    var accountResult = _mapper.Map<AccountDto>(account);
                    return Ok(accountResult);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAccountById action : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] AccountForCreationDto account)
        {
            try
            {
                if(account == null)
                {
                    _logger.LogError($"account object sent from client is null at: {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("account object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"account object sent from client is not invalid at: {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("account model is invalid");
                }

                if(_repositoryWrapper.Owner.GetOwnerById(account.ownerid) == null)
                {
                    _logger.LogError($"owner with id: {account.ownerid} has'nt been in db.");
                    return BadRequest($"owner with id: {account.ownerid} has'nt been in db.");
                }

                var accountEntity = _mapper.Map<Account>(account);

                accountEntity.DateCreated = Convert.ToDateTime( DateTime.Now.ToString("yyyy-mm-dd"));

                _repositoryWrapper.Account.CreateAccount(accountEntity);
                _repositoryWrapper.Save();

                var createdAccount = _mapper.Map<AccountDto>(accountEntity);
                return CreatedAtRoute("accountById", new { Id = createdAccount.Id }, createdAccount);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner actio: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(Guid id, [FromBody] AccountForUpdateDto account)
        {
            try
            {
                if(account == null)
                {
                    _logger.LogError($"account object sent from client is null at: {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("account object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"account object sent from client is not invalid at: {DateTime.Now.ToString("dd/mm/YYYY HH:MM:SS")}");
                    return BadRequest("account model is invalid");
                }
                var accountEntity = _repositoryWrapper.Account.GetAccountById(id);
                if(accountEntity == null)
                {
                    _logger.LogError($"account with id: {id} has'nt been in db.");
                    return NotFound();
                }

                _mapper.Map(account, accountEntity);

                _repositoryWrapper.Account.UpdateAccount(accountEntity);
                _repositoryWrapper.Save();

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner actio: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(Guid id)
        {
            try
            {
                var account = _repositoryWrapper.Account.GetAccountById(id);

                if(account == null)
                {
                    _logger.LogError($"Account with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repositoryWrapper.Account.DeleteAccount(account);
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