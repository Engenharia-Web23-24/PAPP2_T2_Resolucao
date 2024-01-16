﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAP2T2.Data;
using PAP2T2.Models;

namespace PAP2T2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
              return _context.UnidadesCurriculares != null ? 
                          View(await _context.UnidadesCurriculares.Include(uc => uc.Inscricoes).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.UnidadesCurriculares'  is null.");
        }

        public IActionResult Inscrever()
        {
            ViewData["UCId"] = new SelectList(_context.UnidadesCurriculares, "Codigo", "Nome");
            return View();
        }

        public async Task<IActionResult> ListarInscritos(string id)
        {
            return PartialView(await _context.Inscricoes.Where(i => i.UCId == id).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Inscrever(Inscricao inscricao)
        {
            inscricao.Ano = DateTime.Now.Year;
            inscricao.Username = User.Identity.Name;
            _context.Add(inscricao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        private bool UnidadeCurricularExists(string id)
        {
          return (_context.UnidadesCurriculares?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
