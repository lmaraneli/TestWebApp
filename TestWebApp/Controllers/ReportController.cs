using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Application;
using TestWebApp.Infrastructure;
using TestWebApp.Domain.PersonManagement;
using TestWebApp.Helpers;
using TestWebApp.Resources;

namespace TestWebApp.Controllers
{
    [ApiController]
    public class ReportController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly TestWebAppDbContext _db;
        private readonly IStringLocalizer<Translations> _translations;

        public ReportController(UnitOfWork unitOfWork, TestWebAppDbContext db, IStringLocalizer<Translations> translations)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _translations = translations;
        }

        [HttpGet("report")]
        public async Task<IActionResult> Index()
        {
            var groupedRes = _unitOfWork.Repository<PersonRelation>()
                    .Get<string>()
                    .GroupBy(x => new { x.PersonId, x.Relation }, t => t.RelativeId)
                    .Select(x => new SelectHelper { PersonId = x.Key.PersonId, Relation = x.Key.Relation, Total = x.Count() });
            //on p.Id equals agg.PersonId into lj;

            var report =
                from p in _unitOfWork.Repository<Person>().Get<object>()
                    join agg in groupedRes on p.Id equals agg.PersonId into lj
                    from r in lj.DefaultIfEmpty()
                select new
                {
                    p.Id,
                    p.Name,
                    p.LastName,
                    p.Sex,
                    p.PersonalId,
                    p.DOB,
                    p.ImageUrl,
                    City = p.City.Name,
                    Relation = r == null ? (r.Relation ?? null) : r.Relation,
                    TotalRelatives = r == null ? (r.Total ?? null) : r.Total
                };

            return Ok(report);
        }
    }
}
