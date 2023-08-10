﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EUSL_WebApp;
using EUSL_WebApp.Models;

namespace EUSL_WebApp.Pages
{
    public class PlayersModel : PageModel
    {
        private readonly EUSL_WebApp.EuslContext _context;

        public PlayersModel(EUSL_WebApp.EuslContext context)
        {
            _context = context;
        }

        public IEnumerable<Player> Player { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Players != null)
            {
                Player = await _context.Players
                .Include(p => p.Nationality).ToListAsync();
            }
        }
    }
}