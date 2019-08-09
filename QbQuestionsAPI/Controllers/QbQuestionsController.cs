using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Extensions;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.Controllers
{
    [Route("/api/[controller]")]
    public class QbQuestionsController : Controller
    {
        private readonly IQbQuestionService _qbQuestionService;
        private readonly IMapper _mapper;

        public QbQuestionsController(IQbQuestionService qbQuestionService, IMapper mapper)
        {
            _qbQuestionService = qbQuestionService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<QbQuestionResource> GetAsync(int id)
        {
            var question = await _qbQuestionService.GetAsync(id);
            var resources = _mapper.Map<QbQuestion, QbQuestionResource>(question);

            return resources;
        }

        // TODO: Post array of questions
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveQbQuestionResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var question = _mapper.Map<SaveQbQuestionResource, QbQuestion>(resource);
            var result = await _qbQuestionService.SaveAsync(question);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var questionResource = _mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion);
            return Ok(questionResource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] SaveQbQuestionResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var question = _mapper.Map<SaveQbQuestionResource, QbQuestion>(resource);
            var result = await _qbQuestionService.UpdateAsync(id, question);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var questionResource = _mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion);
            return Ok(questionResource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _qbQuestionService.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            
            var questionResource = _mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion);
            return Ok(questionResource);
        }
    }
}