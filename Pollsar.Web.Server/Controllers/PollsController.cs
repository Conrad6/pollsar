using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Pollsar.Shared.Models;
using Pollsar.Web.Data;

namespace Name
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public partial class PollsController : Controller
    {
        private const string pageLinkFormat = "{0}://{1}/api/v1/polls?page={2}&size={3}";
        private readonly PollsarContext _pollsarContext;
        private readonly IMapper _mapper;

        public PollsController (PollsarContext pollsarContext, IMapper mapper)
        {
            _pollsarContext = pollsarContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseEntity<PollViewModel>>> GetPollsAsync ([FromQuery] int page = 0, [FromQuery] int size = 50)
        {
            if (page < 0) return BadRequest(new { error = "Page cannot be less than zero" });
            if (size <= 0) return BadRequest(new { error = "Page size cannot be less than or equal to zero" });

            var query = _pollsarContext.Polls
                .Include(p => p.Categories).ThenInclude(c => c.Category)
                .Include(p => p.Choices).IgnoreAutoIncludes()
                .Include(p => p.Creator)
                .Include(p => p.Tags).ThenInclude(p => p.Tag)
                .OrderByDescending(p => p.DateCreated)
                .ThenBy(p => p.LastUpdated);

            var polls = await query
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            var totalCount = await _pollsarContext.Polls.CountAsync();

            var totalPages = totalCount == 0 ? 0 : System.Math.Ceiling((double) totalCount / size);

            bool nextPageExists = await query.Skip((page + 1) * size).AnyAsync(), previousPageExists = page > 0;

            string nextPage = null, lastPage = null, previousPage = null;
            StringBuilder pageLinkBuilder = null;
            if (nextPageExists)
            {
                pageLinkBuilder ??= new StringBuilder();

                pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, page + 1, size);
                nextPage = pageLinkBuilder.ToString();
            }

            if (previousPageExists)
            {
                pageLinkBuilder ??= new StringBuilder();
                pageLinkBuilder.Clear();
                pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, page - 1, size);
                previousPage = pageLinkBuilder.ToString();
            }

            pageLinkBuilder ??= new StringBuilder();
            pageLinkBuilder.Clear();
            pageLinkBuilder.AppendFormat(pageLinkFormat, Request.Scheme, Request.Host, totalPages - 1, size);
            lastPage = pageLinkBuilder.ToString();

            var response = new ResponseEntity<PollViewModel>
            {

                Content = polls.Select(p => _mapper.Map<PollViewModel>(p)),
                Page = page,
                Size = size,
                TotalCount = totalCount,
                TotalPages = (int) totalPages,
                NextPage = nextPage,
                PreviousPage = previousPage,
                LastPage = lastPage
            };

            return Json(response);
        }
    }
}