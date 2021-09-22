using CoffeShop.Model;
using CoffeShop.View.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoffeShop.ViewModel
{
    public class FoodTableViewModel : BaseViewModel
    {
        private ObservableCollection<TableFood> _TableFood;
        public ObservableCollection<TableFood> TableFood { get => _TableFood; set { _TableFood = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }

        public ICommand OrderButton { get; set; }
        public FoodTableViewModel()
        {
            TableFood = new ObservableCollection<TableFood>(dataProvider.Ins.DB.TableFoods);
            OrderButton = new RelayCommand<Model.TableFood>((t) => { return true; }, (t) =>
            {
                SetTables setTable = new SetTables();
                var setTableVM = setTable.DataContext as PayTablesViewModel;
                setTableVM.tb = t;
                setTableVM.LoadFood();
                setTable.ShowDialog();
                
            });
        }
    }
}
