using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeShop.Model
{
    public class dataProvider
    {
        private static dataProvider _ins;
        public static dataProvider Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new dataProvider();
                return _ins;
            }
            set
            {
                _ins = value;
            }
        }

        public CoffeeShopEntities DB { get; set; }
        private dataProvider()
        {
            DB = new CoffeeShopEntities();
        }
    }
}
