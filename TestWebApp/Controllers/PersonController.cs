using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TestWebApp.Application;
using TestWebApp.Domain.PersonManagement;
using TestWebApp.Helpers;
using TestWebApp.Interfaces;
using TestWebApp.Models.Person;
using TestWebApp.Resources;

namespace TestWebApp.Controllers
{
    [ApiController]
    public class PersonController : Controller
    {
        public IStringLocalizer _translations;
        private readonly UnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public PersonController(IStringLocalizer<Translations> translations, UnitOfWork unitOfWork, IFileService fileService)
        {
            _translations = translations;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("persons")]
        public async Task<IActionResult> Index(string search, int page = 0, int pageSize = 10, string sortBy = "id", SortOrder sortOrder = SortOrder.Asc)
        {
            var persons = await _unitOfWork.Repository<Person>().Get<Person>(
                x => !string.IsNullOrEmpty(search) ? x.Name.Contains(search) || x.LastName.Contains(search) || x.PersonalId.Contains(search) : true,
                x => x.OrderBy($"{sortBy} {sortOrder}"),
                null,
                page,
                pageSize)
                    .ToListAsync();

            var (total, filtered) = await _unitOfWork.Repository<Person>().CountAsync(
                                x =>
                                    !string.IsNullOrEmpty(search) ?
                                    x.Name.Contains(search) || x.LastName.Contains(search) || x.PersonalId.Contains(search) :
                                    true);

            return Ok(new
            {
                data = persons,
                total = total,
                filtered = filtered,
                page = page,
                pageSize = pageSize,
                orderBy = sortBy,
            });
        }

        [HttpGet("person/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var person = await _unitOfWork.Repository<Person>().GetByIdAsync(id);
            return Ok(person);
        }

        [HttpPost("person")]
        public async Task<IActionResult> Create([FromBody] PersonModel model)
        {
            var newPerson = new Person
            {
                Name = model.Name,
                CityId = model.CityId,
                DOB = model.DOB,
                LastName = model.LastName,
                PersonalId = model.PersonalId,
                PhoneNumbers = model.PhoneNumbers?.Select(x => new PersonPhone
                {
                    Number = x.Number,
                    Type = x.Type
                }).ToList(),
                Sex = model.Sex
            };

            await _unitOfWork.Repository<Person>().CreateAsync(newPerson);

            await _unitOfWork.SaveChangesAsync();

            return Created($"person/{newPerson.Id}", newPerson);
        }

        [HttpPatch("person/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PersonModel model)
        {
            var person = await _unitOfWork.Repository<Person>().GetByIdAsync(id);

            if (person == null)
            {
                return NotFound(model);
            }

            person.Name = model.Name;
            person.CityId = model.CityId;
            person.DOB = model.DOB;
            person.LastName = model.LastName;
            person.PersonalId = model.PersonalId;
            person.PhoneNumbers = person.PhoneNumbers?.Concat(model.PhoneNumbers?.Select(x => new PersonPhone
            {
                Number = x.Number,
                Type = x.Type
            })).ToList();

            person.Sex = model.Sex;

            _unitOfWork.Repository<Person>().Update(person);

            await _unitOfWork.SaveChangesAsync();

            return Ok(person);
        }

        [HttpPatch("person/{id}/image")]
        public async Task<IActionResult> AddImage([FromRoute] int id, IFormFile image)
        {
            var allowed = new string[] { "image/png", "image/jpg", "image/jpeg" };
            if (image != null && allowed.Contains(image.ContentType.ToLower()))
            {
                using var stream = new MemoryStream();
                await image.CopyToAsync(stream);
                var response = await _fileService.SaveFileAsync(stream, Path.GetExtension(image.FileName));

                var person = await _unitOfWork.Repository<Person>().GetByIdAsync(id);

                if (person == null)
                {
                    BadRequest("not found");
                }

                if (!string.IsNullOrEmpty(person.ImageUrl))
                {
                    _fileService.DeleteFile(person.ImageUrl);
                }

                person.ImageUrl = response;

                await _unitOfWork.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(_translations["image should not be empty"]);
        }

        [HttpDelete("person/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var person = await _unitOfWork.Repository<Person>().GetByIdAsync(id);
            if (person == null)
            {
                return NotFound(_translations["person not found"]);
            }

            _fileService.DeleteFile(person.ImageUrl);

            await _unitOfWork.Repository<Person>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("person/{id}/relations")]
        public async Task<IActionResult> GetRelations([FromRoute] int id)
        {
            var relations = (await _unitOfWork.Repository<Person>().GetByIdAsync(id))
                .Relatives.Select(x => new { x.Person, x.Relation });

            return Ok(relations);
        }

        [HttpPost("person/{id}/relation/")]
        public async Task<IActionResult> AddRelation([FromRoute] int id, [FromBody] RelationModel model)
        {
            await _unitOfWork.Repository<PersonRelation>()
                    .CreateAsync(new PersonRelation() { PersonId = id, Relation = model.Relation, RelativeId = model.RelativeId });

            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("person/{id}/relation/{relativeId}")]
        public async Task<IActionResult> DeleteRelation([FromRoute] int id, [FromRoute] int relativeId)
        {
            await _unitOfWork.Repository<PersonRelation>()
                    .DeleteAsync(new { PersonId = id, RelativeId = relativeId });

            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("person/{id}/phonenumber/{phoneId}")]
        public async Task<IActionResult> DeletePhoneNumber([FromRoute] int id, [FromRoute] int phoneId)
        {
            var phoneNumber = await _unitOfWork.Repository<PersonPhone>().Get<object>(x => x.PersonId == id && x.Id == phoneId).FirstOrDefaultAsync();
            if (phoneNumber != null)
            {
                _unitOfWork.Repository<PersonPhone>().Delete(phoneNumber);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(_translations["Phone does not exists"]);
        }
    }
}
