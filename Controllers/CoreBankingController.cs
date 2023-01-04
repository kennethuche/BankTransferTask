using BankTransferTask.Core.Entities;
using BankTransferTask.Core.Models.Payloads;
using BankTransferTask.Core.Models.Resources;
using BankTransferTask.Core.Services.Bank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BankTransferTask.Controllers
{
    [ApiController]
    [Route("api/v1/core-banking")]
  //  [Authorize]
    public class CoreBankingController : BaseController
    {
        private readonly IBankService bankService;
        public CoreBankingController(IBankService bankService)
        {
            this.bankService = bankService;
        }

        /// <summary>
        /// Get List of All Banks
        /// </summary>
        /// <returns></returns>
        [HttpGet("banks")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(429), ProducesResponseType(415)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ListResource<ListBanksResource?>>> ListBankAsync()
        {
            var response = await bankService.Listbanks();
            return HandleResponse(response);


        }



        /// <summary>
        /// Validate A Bank Account
        /// <param name="payload"></param>
        /// </summary>
        /// <returns></returns>
        [HttpPost("validateBankAccount")]
        [ProducesResponseType(200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResource<ValidateBankAccountResource?>>> ValidateBankAccountAsync(
            [FromBody] ValidateBankAccountPayload payload)
        {
            var response = await bankService.ValidateBankAccount(payload.AccountNumber.ToString(),payload.BankCode);
            
            return HandleResponse(response);


        }

        /// <summary>
        /// Initiate Bank Transfer
        /// <param name="payload"></param>
        /// </summary>
        /// <returns></returns>
        [HttpPost("bankTransfer")]
        [ProducesResponseType(200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResource<TranscationDetail?>>> BankTransferAsync(
            [FromBody] BankTransferPayload payload)
        {
          
            var response = await bankService.BankTransfer(payload);

            return HandleResponse(response);


        }


        /// <summary>
        /// Send OTP
        /// <param name="otp"></param>
        /// <param name="transfer_code"></param>
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendOtp")]
        [ProducesResponseType(200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResource<TranscationDetail?>>> SendOtpAsync(
            string otp, string transfer_code)
        {

            var response = await bankService.VerifyOtp(otp,transfer_code);

            return HandleResponse(response);


        }



        /// <summary>
        /// Get Transcation Details
        /// <param name="reference"></param>
        /// </summary>
        /// <returns></returns>
        [HttpGet("transcation/{reference}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(429), ProducesResponseType(415)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResource<TranscationDetailsResource?>>> GetTranscationDetailsAsync(string reference)
        {
            var response = await bankService.GetTranscationDetails(reference);
            return HandleResponse(response);


        }
    }
}
