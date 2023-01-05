using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using locacao_veiculos.Database;
using locacao_veiculos.Models;

namespace locacao_veiculos.Controllers
{
    public class PedidosController : Controller
    {
        private readonly LocacaoContext _context;

        public PedidosController(LocacaoContext context)
        {
            _context = context;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidosSql = _context.Pedidos.Join(
                _context.Clientes,
                ped => ped.ClienteId,
                cli => cli.Id,
                (ped, cli) => new {
                    PedidoId = ped.Id,
                    NomeCliente = cli.Nome,
                    CarroId = ped.CarroId
                }
            ).Join(
                _context.Carros,
                pedCli => pedCli.CarroId,
                carro => carro.Id,
                (pedCli, carro) => new {
                    PedidoId = pedCli.PedidoId,
                    NomeCliente = pedCli.NomeCliente,
                    NomeCarro = carro.Nome,
                    MarcaId = carro.MarcaId
                }
            ).Join(
                _context.Marcas,
                pedCliCarr => pedCliCarr.MarcaId,
                marca => marca.Id,
                (pedCliCarr, marca) => new {
                    PedidoId = pedCliCarr.PedidoId,
                    NomeCliente = pedCliCarr.NomeCliente,
                    NomeCarro = pedCliCarr.NomeCarro,
                    MarcaDoCarro = marca.Nome
                }
            ).ToQueryString();

            Console.WriteLine("==================[");
            Console.WriteLine(pedidosSql);
            Console.WriteLine("]==================");

            var locacaoContext = _context.Pedidos.Include(p => p.Carro).Include(p => p.Cliente).Include(p=> p.Carro.Marca );
            return View(await locacaoContext.ToListAsync());
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Carro)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidos/Create
        public IActionResult Create()
        {
            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Nome");
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,CarroId,DataLocacao")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var config = _context.Configuracoes.FirstOrDefault();
                var dias = config is not null ? config.DiasDeLocacao : 1;
                pedido.DataEntrega = pedido.DataLocacao.AddDays(dias);
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Nome", pedido.CarroId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pedido.ClienteId);
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Nome", pedido.CarroId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pedido.ClienteId);
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,CarroId,DataLocacao,DataEntrega")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
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
            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Nome", pedido.CarroId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pedido.ClienteId);
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Carro)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'LocacaoContext.Pedidos'  is null.");
            }
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
          return (_context.Pedidos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
