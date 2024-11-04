using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IhorsApi.Controllers;

public record PetEntity(Guid Id, string Name, int Age, string Type);

public record PetCreateRequest(
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    [RegularExpression("^[A-Za-z]+(?: [IVXLCDM]+)?$", ErrorMessage = "Name must contain latin letters in upper or lower case and can contain Rome numbers in the end of the name.")]
        string Name, 
    [Required]
    [Range(0, 30)]
        int Age, 
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
        string Type);

public record PetUpdateRequest(
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    [RegularExpression("^[A-Za-z]+(?: [IVXLCDM]+)?$", ErrorMessage = "Name must contain latin letters in upper or lower case and can contain Rome numbers in the end of the name.")]
        string Name, 
    [Required]
    [Range(0, 30)]
        int Age);

[ApiController]
[Route("api/[controller]")]
public class PetController : ControllerBase
{
    private static List<PetEntity> _db = new(100);

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_db);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var pet = _db.FirstOrDefault(p => p.Id == id);

        if (pet is not null)
        {
            return Ok(pet);
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult Create([FromBody] PetCreateRequest request)
    {
        var pet = new PetEntity(Guid.NewGuid(), request.Name, request.Age, request.Type);

        if (_db.Count == 100)
        {
            var first = _db[0];
            _db.Remove(first);
        }
        
        _db.Add(pet);

        return Ok(pet);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] PetUpdateRequest request)
    {
        var pet = _db.FirstOrDefault(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        var updatedPet = pet with { Name = request.Name, Age = request.Age };

        _db.Remove(pet);
        _db.Add(updatedPet);

        return Ok(updatedPet);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var pet = _db.FirstOrDefault(p => p.Id == id);

        if (pet is not null)
        {
            _db.Remove(pet);

            return Ok(pet);
        }

        return NotFound();
    }
}