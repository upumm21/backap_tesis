using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepositorio;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public async Task<List<UsuarioDTO>> lista()
        {
            try 
            {
                var queryUsuario = await _usuarioRepositorio.Consultar();
                var listaUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuario);
            } catch {
                throw;
            }
        }

        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await _usuarioRepositorio.Consultar(u =>
                    u.Correo == correo &&
                    u.Clave == clave
                );

                if (queryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("El usuario no Existe");

                Usuario devolverUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<SesionDTO>(devolverUsuario);

            }
            catch
            {
                throw;
            }
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var usuarioCreado = await _usuarioRepositorio.Crear(_mapper.Map<Usuario>(modelo));

                if(usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear");

                var query = await _usuarioRepositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);

                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuarioCreado);

            }
            catch {
                throw;
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            var usurioModelo = _mapper.Map<Usuario>(modelo);

            var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == usurioModelo.IdUsuario);

            if(usuarioEncontrado == null )
                throw new TaskCanceledException("El usuario no existe");

            usuarioEncontrado.NombreCompleto = usuarioEncontrado.NombreCompleto;
            usuarioEncontrado.Correo = usurioModelo.Correo;
            usuarioEncontrado.IdRol = usurioModelo.IdRol;
            usuarioEncontrado.Clave = usurioModelo.Clave;
            usuarioEncontrado.EsActivo = usurioModelo.EsActivo;

            bool respuesta = await _usuarioRepositorio.Editar(usuarioEncontrado);

            if(!respuesta)
                throw new TaskCanceledException("El se puede editar");
            return respuesta;
        }

        public async Task<bool> Eliminar(int id)
        {
            var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == id);

            if(usuarioEncontrado == null)
                     throw new TaskCanceledException("El se puede editar");

            bool respuesta = await _usuarioRepositorio.Eliminar(usuarioEncontrado);
            if (!respuesta)
                throw new TaskCanceledException("El se puede eliminar");
            return respuesta;
        }

        
    }
}
