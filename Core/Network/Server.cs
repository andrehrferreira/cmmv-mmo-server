public partial class Server
{
    public static Server? _Instrance;
    public static Server Instance {
        get
        {
            if (_Instrance == null)            
                _Instrance = new Server();
                  
            return _Instrance;
        }
    }
} 