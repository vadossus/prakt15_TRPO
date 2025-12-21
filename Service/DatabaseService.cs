using prakt15_TRPO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prakt15_TRPO.Service
{
    public class DatabaseService
    {
        private EStoreContext _context;
        public EStoreContext Context => _context;

        private static DatabaseService _instance;
        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DatabaseService();
                return _instance;
            }
        }

        private DatabaseService()
        {
            _context = new EStoreContext();
        }
    }
}
