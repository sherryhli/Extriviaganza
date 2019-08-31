using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Domain.Services.Communication;
using QbQuestionsAPI.Extensions;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetAsync(int id)
        {
            QbQuestion question = await _qbQuestionService.GetAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            QbQuestionResource resource = _mapper.Map<QbQuestion, QbQuestionResource>(question);

            return Ok(resource);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomAsync(int? level = null)
        {
            QbQuestion question = await _qbQuestionService.GetRandomAsync(level);

            if (question == null)
            {
                return NotFound();
            }

            QbQuestionResource resource = _mapper.Map<QbQuestion, QbQuestionResource>(question);
            return Ok(resource);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveQbQuestionResource[] resources)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            List<SaveQbQuestionResource> resourcesList = new List<SaveQbQuestionResource>(resources);
            List<QbQuestion> questions = _mapper.Map<List<SaveQbQuestionResource>, List<QbQuestion>>(resourcesList);

            List<QbQuestionResource> resultsResources = new List<QbQuestionResource>();

            foreach (QbQuestion question in questions)
            {
                QbQuestionResponse result = await _qbQuestionService.SaveAsync(question);

                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                resultsResources.Add(_mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion));
            }

            return Ok(resultsResources);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] SaveQbQuestionResource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            QbQuestion question = _mapper.Map<SaveQbQuestionResource, QbQuestion>(resource);
            QbQuestionResponse result = await _qbQuestionService.UpdateAsync(id, question);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            QbQuestionResource questionResource = _mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion);
            return Ok(questionResource);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            QbQuestionResponse result = await _qbQuestionService.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            QbQuestionResource questionResource = _mapper.Map<QbQuestion, QbQuestionResource>(result.QbQuestion);
            return Ok(questionResource);
        }
    }
}
