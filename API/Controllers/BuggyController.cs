using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BuggyController : BaseController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnAuthorized(){
            return Unauthorized();
        }
        [HttpGet("badrequest")]
        public IActionResult GetBadRequest(){
            return BadRequest("Not a good request");
        }
        [HttpGet("notfound")]
        public IActionResult GetNotFound(){
            return NotFound();
        }
        [HttpGet("internalerror")]
        public IActionResult GetInternalError(){
            throw new Exception("This is a test expception");
        }
        [HttpPost("validationerror")]
        public IActionResult GetValidationError(CreateProductDto product){
            return Ok();
        }
    }
}