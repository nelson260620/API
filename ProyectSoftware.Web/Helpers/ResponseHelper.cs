using ProyectSoftware.Web.Core;

namespace ProyectSoftware.Web.Helpers
{
    public static class ResponseHelper<T>
    {
        // Método para crear una respuesta de éxito con un modelo.
        public static Response<T> MakeResponseSuccess(T model)
        {
            // Devuelve una respuesta con éxito, mensaje estándar y el modelo proporcionado.
            return new Response<T>
            {
                IsSuccess = true,
                Message = "Tarea realizada con éxito",
                Result = model
            };
        }

        // Método para crear una respuesta de éxito con un modelo y un mensaje personalizado.
        public static Response<T> MakeResponseSuccess(T model, string message)
        {
            // Devuelve una respuesta con éxito, mensaje personalizado y el modelo proporcionado.
            return new Response<T>
            {
                IsSuccess = true,
                Message = message,
                Result = model
            };
        }

        // Método para crear una respuesta de éxito solo con un mensaje personalizado.
        public static Response<T> MakeResponseSuccess(string message)
        {
            // Devuelve una respuesta con éxito y mensaje personalizado.
            return new Response<T>
            {
                IsSuccess = true,
                Message = message
            };
        }

        // Método para crear una respuesta de fallo con una lista de errores.
        public static Response<object> MakeResponseFail(List<string> errors)
        {
            // Devuelve una respuesta de fallo, mensaje estándar y la lista de errores.
            return new Response<object>
            {
                IsSuccess = false,
                Message = "Error al generar la solicitud",
                Errors = errors
            };
        }

        // Método para crear una respuesta de fallo con una lista de errores y un mensaje personalizado.
        public static Response<T> MakeResponseFail(List<string> errors, string message)
        {
            // Devuelve una respuesta de fallo, mensaje personalizado y la lista de errores.
            return new Response<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        // Método para crear una respuesta de fallo a partir de una excepción.
        public static Response<T> MakeResponseFail(Exception ex)
        {
            // Crea una lista de errores con el mensaje de la excepción.
            List<string> errors = new List<string>
            {
                ex.Message
            };

            // Devuelve una respuesta de fallo, mensaje estándar y la lista de errores.
            return new Response<T>
            {
                IsSuccess = false,
                Message = "Error al generar la solicitud",
                Errors = errors
            };
        }

        // Método para crear una respuesta de fallo con un solo error.
        public static Response<T> MakeResponseFail(string error)
        {
            // Crea una lista de errores con el error proporcionado.
            List<string> errors = new List<string>
            {
                error
            };

            // Devuelve una respuesta de fallo, mensaje con el error y la lista de errores.
            return new Response<T>
            {
                IsSuccess = false,
                Message = error,
                Errors = errors
            };
        }
    }
}
