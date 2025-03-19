using Microsoft.AspNetCore.Mvc;

namespace ChessServer.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public abstract class BaseController : Controller
{
}