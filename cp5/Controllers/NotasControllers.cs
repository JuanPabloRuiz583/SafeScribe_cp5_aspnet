using cp5.DTOS;
using cp5.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cp5.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotasController : ControllerBase
    {
        private static List<Note> _notes = new();

        private int GetUserId() => int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        private string GetUserRole() => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult CriarNota(NoteCreateDto dto)
        {
            var note = new Note
            {
                Id = _notes.Count + 1,
                Title = dto.Title,
                Content = dto.Content,
                UserId = GetUserId()
            };

            _notes.Add(note);
            return Ok(note);
        }

        [HttpGet("{id}")]
        public IActionResult ObterNota(int id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note == null)
                return NotFound("Nota não encontrada.");

            var userId = GetUserId();
            var role = GetUserRole();

            if (role != UserRoles.Admin && note.UserId != userId)
                return Forbid("Você não tem permissão para acessar esta nota.");

            return Ok(note);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarNota(int id, NoteUpdateDto dto)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note == null)
                return NotFound("Nota não encontrada.");

            var userId = GetUserId();
            var role = GetUserRole();

            if (role != UserRoles.Admin && note.UserId != userId)
                return Forbid("Você não pode alterar esta nota.");

            note.Title = dto.Title;
            note.Content = dto.Content;
            return Ok(note);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeletarNota(int id)
        {
            var note = _notes.FirstOrDefault(n => n.Id == id);
            if (note == null)
                return NotFound("Nota não encontrada.");

            _notes.Remove(note);
            return Ok("Nota deletada com sucesso.");
        }
    }
}
