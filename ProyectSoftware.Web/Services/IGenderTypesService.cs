using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Core;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Helpers;
using static System.Collections.Specialized.BitVector32;
using System.Collections.Generic;

namespace ProyectSoftware.Web.Services
{

    public interface IGenderTypesService
    {
        Task<Response<GenderType>> CreateAsync(GenderType model);
        public Task<Response<List<GenderType>>> GetListAsync();
        public Task<Response<GenderType>> GetOneAsync(int id);

        public Task<Response<GenderType>> EditAsync(GenderType model);

        public Task<Response<GenderType>> DeleteAsync(int id);

        //public Task<Response<GenderType>> ToggleGenderTypeAsync(ToggleGenderTypeRequest request);
    }


    // Implementación del servicio de Sections.
    public class GenderTypeServices : IGenderTypesService
    {
        private readonly DataContext _context; // Campo privado para el contexto de datos.

        // Constructor que inyecta el contexto de datos.
        public GenderTypeServices(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<GenderType>> CreateAsync(GenderType model)
        {
            try
            {
                GenderType genderType = new GenderType()
                {
                    GenderName = model.GenderName,
                };
                await _context.AddAsync(genderType);
                await _context.SaveChangesAsync();

                return ResponseHelper<GenderType>.MakeResponseSuccess(genderType, "Gender name created");
               
            }catch(Exception ex){

                return ResponseHelper<GenderType>.MakeResponseSuccess(ex.Message);
            }
        }

        // Implementación del método para obtener la lista de Sections de manera asíncrona.
        public async Task<Response<List<GenderType>>> GetListAsync()
        {
            try
            {
                // Obtiene la lista de autores de la base de datos de manera asíncrona.
                List<GenderType> list = await _context.GenderTypes.ToListAsync();

                // Crea una respuesta exitosa con la lista obtenida.
                return ResponseHelper<List<GenderType>>.MakeResponseSuccess(list, "Gender name created");

               
            }
            catch (Exception ex)
            {
                // En caso de excepción, crea una respuesta de error con el mensaje de la excepción.
                return ResponseHelper<List<GenderType>>.MakeResponseSuccess(ex.Message);
            }
        }

        public async Task<Response<GenderType>> GetOneAsync(int id)
        {
            try
            {
                GenderType? genderType = await _context.GenderTypes.FirstOrDefaultAsync(s => s.Id == id);
                if (genderType is null)
                {
                    return ResponseHelper<GenderType>.MakeResponseFail($"La sección con id '{id}' no existe.");
                }
                return ResponseHelper<GenderType>.MakeResponseSuccess(genderType);

            }
            catch (Exception ex)
            {
                return ResponseHelper<GenderType>.MakeResponseFail(ex);
            }


        }

        public async Task<Response<GenderType>> EditAsync(GenderType model)
        {
            try
            {
                _context.GenderTypes.Update(model);
                await _context.SaveChangesAsync();

                return ResponseHelper<GenderType>.MakeResponseSuccess(model, "Sección editada con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<GenderType>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<GenderType>> DeleteAsync(int id)
        {
            try
            {
                GenderType? genderType = await _context.GenderTypes.FirstOrDefaultAsync(s => s.Id == id);

                if (genderType is null)
                {
                    return ResponseHelper<GenderType>.MakeResponseFail($"La sección con id '{id}' no existe.");
                };
                _context.GenderTypes.Remove(genderType);
                await _context.SaveChangesAsync();

                return ResponseHelper<GenderType>.MakeResponseSuccess("Sección eliminada con éxito");

            }
            catch (Exception ex)
            {
                return ResponseHelper<GenderType>.MakeResponseFail(ex);
            }
        }

        
        //public async Task<Response<GenderType>> ToggleGenderTypeAsync(ToggleGenderTypeRequest request)
        //{
        //    try
        //    {
        //        GenderType? model = await _context.GenderTypes.FindAsync(request.Id);

        //        if (model == null)
        //        {
        //            return ResponseHelper<GenderType>.MakeResponseFail($"No existe seeción con id '{request.Id}'");
        //        }




        //        _context.GenderTypes.Update(model);
        //        await _context.SaveChangesAsync();

        //        return ResponseHelper<GenderType>.MakeResponseSuccess("Sección Actualizada con éxito");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseHelper<GenderType>.MakeResponseFail(ex);
        //    }

        //}

    }


}
      
    

