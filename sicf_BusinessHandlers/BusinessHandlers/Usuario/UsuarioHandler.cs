using AutoMapper;
using sicf_BusinessHandlers.BusinessHandlers.Compartido;
using sicf_DataBase.Repositories;
using sicf_DataBase.Repositories.Usuario;
using sicf_Models.Dto.Solicitudes;
using sicf_Models.Dto.Usuario;
using sicf_Models.Utility;
using sicfExceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sicf_BusinessHandlers.BusinessHandlers.Usuario
{
    public class UsuarioHandler: IUsuarioHandler
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;
        private ISendgridNotificaciones _IsendgridNotificaciones;

        public UsuarioHandler(IUnitofWork unitofWork, IMapper mapper, ISendgridNotificaciones IsendgridNotificaciones) {

            _unitofWork = unitofWork;
            _mapper = mapper;
            _IsendgridNotificaciones = IsendgridNotificaciones;
        }

        public bool IsUserPerfil(long userID, string codPefil) {

            try
            {
                return   _unitofWork.UsuarioRepository.IsUserPerfil(userID, codPefil);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        
        }

        public async Task CrearUsuario(CrearUsuarioDTO data)
        {
            try

            {
                var dataUsu = await _unitofWork.UsuarioRepository.CrearUsuario(data);

                await _unitofWork.UsuarioRepository.UsuarioAsignacionComisaria(dataUsu.Item1,data.Idcomisaria);

                await _unitofWork.UsuarioRepository.AsignarPerfiles(dataUsu.Item1 , data.perfiles, data.Idcomisaria);

                //await _unitofWork.UsuarioRepository.AgregarhistorialContrasena(dataUsu.Item1, dataUsu.Item2);

                _IsendgridNotificaciones.EnviarContrasena(data.correoElectronico , dataUsu.Item3);


            }
            catch (Exception ex) {

                throw new Exception(ex.Message);
            }
        }

        
        public async Task<bool> ActualizarUsuario(UsuarioDTO data)
        {
            try
            {
                return await _unitofWork.UsuarioRepository.ActualizarUsuario(data);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioDTO> ConsultarUsuario(long userID)
        {
            try
            {
                return await _unitofWork.UsuarioRepository.ConsultarUsuario(userID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioDTO> ConsultarUsuarioPorCorreo(string email)
        {
            try
            {
                return await _unitofWork.UsuarioRepository.ConsultarUsuarioPorCorreo(email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UsuarioPerfilesDTO>> ListarUsuarios(int idComisaria)
        {
            try
            {

                return await _unitofWork.UsuarioRepository.ListarUsuarios(idComisaria);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> ComisariaUsuario(string email)
        {
            try
            {
                var response= await _unitofWork.UsuarioRepository.ComisariaUsuario(email);
                return response;
            }
            catch (Exception ex) {

                throw new Exception(ex.Message);
            }
        }

        public ResponseListaPaginada ValidarUsuario(RequestCiudadano requestCiudadano)
        {
            try
            {
                ResponseListaPaginada responseListaPaginada = new ResponseListaPaginada();

                List<string> errors = new List<string>();

                if (string.IsNullOrWhiteSpace(requestCiudadano.nombre_ciudadano) && string.IsNullOrWhiteSpace(requestCiudadano.apellido_ciudadano) && string.IsNullOrWhiteSpace(requestCiudadano.numero_documento))
                    errors.Add("Al menos Nombre y Apellido del Ciudadano o número de documento del ciudadano debe ser diligenciado");


                if (!string.IsNullOrWhiteSpace(requestCiudadano.nombre_ciudadano) && string.IsNullOrWhiteSpace(requestCiudadano.apellido_ciudadano))
                    errors.Add("Por favor ingrese el apellido del ciudadano, una vez que se introduce el nombre, se debe introducir el apellido");

                if (string.IsNullOrWhiteSpace(requestCiudadano.nombre_ciudadano) && !string.IsNullOrWhiteSpace(requestCiudadano.apellido_ciudadano))
                    errors.Add("Por favor ingrese el nombre del ciudadano, una vez que se introduce el apellido, se debe introducir  el nombre");

                responseListaPaginada.DatosPaginados = errors;
                responseListaPaginada.TotalRegistros = errors.Count;

                return responseListaPaginada;
            }
            catch (ControledException ex)
            {
                //loggerManager.EscribirLogger(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Se lanza una excepción en el metodo consultarVehiculos del controlador RutaSeleccionadaController, se lanza la excepción: " + ex.Message, listarVehiculosDtoParam);
                throw new ControledException(Convert.ToInt32(ex.RespuestaApi.Status));
            }
            catch (Exception ex)
            {
                //loggerManager.EscribirLogger(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Se lanza una excepción en el metodo consultarVehiculos del controlador RutaSeleccionadaController, se lanza la excepción: " + ex.Message, listarVehiculosDtoParam);
                throw new ControledException(ex.HResult);
            }
        }

        public ResponseListaPaginada GetUsuario(RequestCiudadano requestCiudadano)
        {
            //// se realiza insercion en bitacora

            try
            {
                return _unitofWork.UsuarioRepository.ObtenerUsuario(requestCiudadano);
            }
            catch (ControledException ex)
            {

                throw new ControledException(Convert.ToInt32(ex.RespuestaApi.Status));
            }
            catch (Exception ex)
            {
                throw new ControledException(ex.HResult);
            }
        }
    }
}
