using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.DbPersistance
{
public class DapperContext
{
    private readonly IConfiguration _config;
    public DapperContext(IConfiguration config) => _config = config;

    //todo: remove hardcoded string, go IOptions
    public IDbConnection CreateConnection() =>
        new SqlConnection(_config.GetConnectionString("HariShop")); // Your connection string name  
}
}
