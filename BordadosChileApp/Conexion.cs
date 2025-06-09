    using System.Data.SqlClient;

namespace BordadosChileApp
{
    public class Conexion
    {
        public static SqlConnection Conectar()
        {
            // Esta es la cadena de conexión a tu base de datos
            string cadena = "Data Source=DESKTOP-RREDH95\\SQLEXPRESS;Initial Catalog=BordadosChileDB;Integrated Security=True;";
            SqlConnection cn = new SqlConnection(cadena);
            return cn;
        }
    }
}
