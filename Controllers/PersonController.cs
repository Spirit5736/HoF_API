using Microsoft.AspNetCore.Mvc;
using HoF_API.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HoF_API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonContext _db;

        public PersonController(PersonContext db)
        {
            _db = db;
        }

        [HttpGet("persons")]
        public ActionResult<List<Person>> GetPersons()
        {
            return Ok(_db.Persons.Include(p => p.Skills));
        }

        [HttpGet("person/{id}")]
        public ActionResult<Person> GetPerson(long id)
        {
            return Ok(_db.Persons.Include(p => p.Skills).FirstOrDefault(p => p.Id == id));
        }

        [HttpPost("person")]
        public ActionResult CreatePerson(Person person)
        {
            _db.Persons.Add(person);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("person/{id}")]
        public ActionResult UpdatePerson(long id, Person person)
        {
            if (id != person.Id)
                BadRequest();

            if (person.Skills.Where(s => s.Id == 0).Any())
                _db.Skills.AddRange(person.Skills.Where(s => s.Id == 0));

            if (person.Skills.Where(s => s.Id != 0).Any())
                _db.Skills.UpdateRange(person.Skills.Where(s => s.Id != 0));

            var skills = _db.Skills.Where(s => s.PersonId == id).ToList();
            var skillsToDelete = skills.Where(s => !person.Skills.Contains(s));
            if (skillsToDelete.Any())
                _db.Skills.RemoveRange(skillsToDelete);
            _db.Entry(person).State = EntityState.Modified;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("person/{id}")]
        public ActionResult RemovePerson(long id)
        {
            var person = _db.Persons.Find(id);
            _db.Persons.Remove(person);
            _db.SaveChangesAsync();
            return NoContent();
        }
    }
}