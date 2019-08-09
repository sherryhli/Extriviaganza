using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveQbQuestionResource[] resources)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var resourcesList = new List<SaveQbQuestionResource>(resources);
            var questions = _mapper.Map<List<SaveQbQuestionResource>, List<QbQuestion>>(resourcesList);

            List<QbQuestionResource> resultsResources = new List<QbQuestionResource>();

            foreach (var question in questions)
            {
                var result = await _qbQuestionService.SaveAsync(question);

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
