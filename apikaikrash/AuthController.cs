using apikaikrash;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ShoesContext _context;
    public AuthController(ShoesContext context) => _context = context;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _context.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
        if (user == null)
            return Unauthorized("Неверный логин или пароль");
        return Ok(user);
    }
}

public class LoginModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}
