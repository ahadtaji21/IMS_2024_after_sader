using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;


namespace jrs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class MenusController : ControllerBase{
        private readonly GeneralContext _context;

        public MenusController(GeneralContext context){
            _context = context;
        }

        // GET: api/Menu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetMenu(){
            var menus = await _context.Menu
                .Include(x => x.InverseParentMenu)
                .Where(x => x.ParentMenuId.HasValue == false)
                .ToListAsync();
            return menus;
            
            // var menus = _context.Set<Menu>()
            //     .Include(x => x.InverseParentMenu)
            //     .Where(x => x.ParentMenuId.HasValue == false)
            //     .Select(f => //f
            //     new Menu {
            //         Id = f.Id,
            //         Descr = f.Descr,
            //         Url = f.Url,
            //         ParentMenuId = f.ParentMenuId,
            //         LabelKey = f.LabelKey,
            //         IconCode = f.IconCode,
            //         InverseParentMenu = f.InverseParentMenu,
            //         // ParentMenu = f.ParentMenu
            //     }
            //     ).ToList();

            // return menus;

            /*
            List<Node> temp = _db.nodes.Include(x => x.subNodes).Where(x => x.ParentNode == null).Select(f => new Node { id = f.id, name = f.name, ParentId = f.ParentId, subNodes = f.subNodes }).ToList();
            return Content(JsonConvert.SerializeObject(new { data = get_all(temp) },Formatting.Indented),"application/json");
            */
        }

        // GET: api/Menu/E63DE8EA-5729-4534-94CF-DB43721B32AF
        [HttpGet("{userGuid}")]
        public async Task<ActionResult<IEnumerable<Menu>>> GetMenuForUser(Guid userGuid)
        {
            var menus = await _context.Set<Menu>()
                .FromSqlInterpolated($"SELECT * FROM dbo.IMS_UF_MENU_FOR_USER({userGuid})")
                // .Include(x => x.InverseParentMenu)
                // .Where(x => x.ParentMenuId.HasValue == false)
                .ToListAsync();

            //Remove leaf menu from the root list
            menus = menus.Where(menu => menu.ParentMenuId.HasValue == false).ToList();

            return menus;
        }
    }
}

