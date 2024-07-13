using Azure;
using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sicf_BusinessHandlers.BusinessHandlers.Usuario;
using sicf_Models.Dto.Usuario;
using sicfServicesApi.Utility;
using static sicf_Models.Constants.Constants;
using System.Net;
using sicf_Models.Dto.Solicitudes;
using sicf_Models.Utility;
using sicfExceptions.Exceptions;
using FluentValidation;
using sicf_BusinessHandlers.BusinessHandlers.Solicitudes;

namespace sicfServicesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize]
    public class UsuarioController : BaseController
    {

        private IUsuarioHandler usuarioHandler;
        private readonly ISolicitudesHandler _solicitudesHandler;
        private readonly IValidator<RequestDatosInvolucrado> _validator;

        public UsuarioController(IUsuarioHandler usuarioHandler, ISolicitudesHandler solicitudesHander, IValidator<RequestDatosInvolucrado> validator)
        {
            this.usuarioHandler = usuarioHandler;
            _solicitudesHandler = solicitudesHander;
            _validator = validator;
        }


        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario(CrearUsuarioDTO data)
        {
            try
            {
                await usuarioHandler.CrearUsuario(data);

                return CustomResult(Message.Ok, "Usuario Creado", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {


                return CustomResult(Message.ErrorInterno, ex.Message, HttpStatusCode.BadRequest);

            }

        }


        [HttpPost("ActualizarUsuarios")]
        public async Task<IActionResult> ActualizarUsuarios(UsuarioDTO data)
        {
            try
            {
                await usuarioHandler.ActualizarUsuario(data);

                return CustomResult(Message.Ok, "Usuario Actualizado", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return CustomResult(Message.ErrorInterno, ex.Message, HttpStatusCode.BadRequest);
            }

        }


        [HttpGet("GetUsuario/{userID}")]
        public async Task<IActionResult> getUsuario(long userID)
        {
            try
            {
                var ListaUsuarios = await usuarioHandler.ConsultarUsuario(userID);

                return CustomResult(Message.Ok, ListaUsuarios, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return CustomResult(Message.ErrorInterno, ex.Message, HttpStatusCode.BadRequest);
            }

        }


        [HttpGet("ListarUsuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            try
            {
                var quest = Context.GetToken(HttpContext);

                var comisaria = await usuarioHandler.ComisariaUsuario(quest.usuario);
                var ListaUsuarios = await usuarioHandler.ListarUsuarios(comisaria);

                return CustomResult(Message.Ok, ListaUsuarios, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {


                return CustomResult(Message.ErrorInterno, ex.Message, HttpStatusCode.BadRequest);

            }
        }

        [HttpGet("ListarUsuarios/{idComisaria}")]
        public async Task<IActionResult> ListarUsuariosComisaria(int idComisaria)
        {
            try
            {
                var ListaUsuarios = await usuarioHandler.ListarUsuarios(idComisaria);

                return CustomResult(Message.Ok, ListaUsuarios, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {


                return CustomResult(Message.ErrorInterno, ex.Message, HttpStatusCode.BadRequest);

            }


        }


        [HttpPost]
        [Route("consultarUsuario")]
        //[Authorize]
        public IActionResult ConsultarUsuario(RequestCiudadano requestCiudadano)
        {
            try
            {
                ResponseListaPaginada response = new ResponseListaPaginada();
                response = usuarioHandler.ValidarUsuario(requestCiudadano);

                if (response.TotalRegistros > 0)
                    return CustomResult(Message.ErrorRequest, response, HttpStatusCode.BadRequest);

                response = usuarioHandler.GetUsuario(requestCiudadano);

                return CustomResult(Message.Ok, response, HttpStatusCode.OK);

            }
            catch (ControledException ex)
            {
                return CustomResult(Message.ErrorInterno, Message.ErrorGenerico, HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                return CustomResult(Message.ErrorInterno, Message.ErrorGenerico, HttpStatusCode.InternalServerError);
            }
        }
    }
}
