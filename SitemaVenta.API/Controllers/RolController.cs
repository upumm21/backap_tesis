using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//refencias
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SitemaVenta.API.Utilidad;

namespace SitemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolServio;

        public RolController(IRolService rolServio)
        {
            _rolServio = rolServio;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<RolDTO>>();
            try
            {
                rsp.status = true;
                rsp.value = await _rolServio.Lista();
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);

        }
    }
}
