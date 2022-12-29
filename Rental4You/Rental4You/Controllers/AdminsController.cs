using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Rental4You.Data;
using Rental4You.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Rental4You.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private object command;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
              return View(await _context.Admin.ToListAsync());
        }

        // GET: Admins/Details/5
            public async Task<IActionResult> GraficoVendas()
            {
                return View();           
        }



        [HttpPost]
            // POST: Cursos/GraficoVendas/5
            public async Task<IActionResult> GetDadosVendas()
            {
            //dados de exemplo
            DateTime currentdate= DateTime.Now;
            int count = _context.Reservation.Where(o => o.ActualDate >= currentdate.AddDays(-30)).Count();
            int countM = _context.Reservation.Where(o => o.ActualDate >= currentdate.AddMonths(-12)).Count();
            int countC = _context.Users.Where(a => a.ActualTime >= currentdate.AddMonths(-12)).Count();
            List<object> dados = new List<object>();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Reservas", System.Type.GetType("System.String"));
                    dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));
                    DataRow dr = dt.NewRow();
                    dr["Reservas"] = "Numero de Reservas Diárias";
                    dr["Quantidade"] = count;
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["Reservas"] = "Numero de Reservas Mensais";
                    dr["Quantidade"] = countM;
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["Reservas"] = "Numero de Clientes Mensais";
                    dr["Quantidade"] = countC;
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
            foreach (DataColumn dc in dt.Columns)
                    {
                        List<object> x = new List<object>();
                        x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                        dados.Add(x);
                    }
            return Json(dados);
            }

    public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Admin == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
            }
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
          return _context.Admin.Any(e => e.Id == id);
        }
    }
}
