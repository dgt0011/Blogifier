using Blogifier.Core.Providers;
using Blogifier.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blogifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnippetController : ControllerBase
    {
        private readonly ISnippetProvider _snippetProvider;

        public SnippetController(ISnippetProvider snippetProvider)
        {
            _snippetProvider = snippetProvider;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<TreeBranch>>> GetCategories()
        {
            return await _snippetProvider.GetSnippets();
        }

    }
}
