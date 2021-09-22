using CoffeShop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeShop.ViewModel
{
    public class ShowBillViewModel : BaseViewModel
    {
        private double _TotalUnpaid;
        private ObservableCollection<Model.BillInfo> _BillInfoList;

        public double TotalUnpaid { get =>_TotalUnpaid; set { _TotalUnpaid = value; OnPropertyChanged(); } }
        public ObservableCollection<Model.BillInfo> BillInfoList { get => _BillInfoList; set { _BillInfoList = value; OnPropertyChanged(); } }
        public ShowBillViewModel()
        {
        }

        public void LoadBill( Model.Bill bill)
        {
            TotalUnpaid = 0;
            BillInfoList = new ObservableCollection<Model.BillInfo>(dataProvider.Ins.DB.BillInfoes.Where(x=>x.IdBill == bill.Id));

            foreach(var infor in BillInfoList)
            {
                TotalUnpaid += (Double)infor.TotalPrice;
            }    
        }
    }
}
