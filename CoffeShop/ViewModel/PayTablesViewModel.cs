using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CoffeShop.Model;
using System.Windows.Input;
using CoffeShop.View.Tables;

namespace CoffeShop.ViewModel
{
    public class PayTablesViewModel : BaseViewModel
    {
        //
        private object _CurentView;
        private string _NameSearch;
        private Model.FoodCategory _SelectedCategory;
        private ObservableCollection<Model.Food> _FoodList;
        private ObservableCollection<Model.FoodCategory> _FoodCategoryList;
        //
        public Model.TableFood tb { get; set; }
        public object CurentView { get => _CurentView; set { _CurentView = value; OnPropertyChanged(); } }
        public string NameSearch { get => _NameSearch; set { _NameSearch = value; OnPropertyChanged(); } }
        public Model.FoodCategory SelectedCategory { get => _SelectedCategory; set { _SelectedCategory = value; OnPropertyChanged(); } }
        //
        public ObservableCollection<Model.Food> FoodList { get => _FoodList; set { _FoodList = value; OnPropertyChanged(); } }
        public ObservableCollection<Model.FoodCategory> FoodCategoryList { get => _FoodCategoryList; set { _FoodCategoryList = value; OnPropertyChanged(); } }


        #region paid
        private int _Count = 1;
        private double _TotalBill;
        private Model.Bill _NewBill;
        private ObservableCollection<BillInfo> _CartList;
        public PaidViewMdel paidVM { get; set; }
        public int Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }
        public double TotalBill { get => _TotalBill; set { _TotalBill = value; OnPropertyChanged(); } }
        public ObservableCollection<Model.BillInfo> CartList { get => _CartList; set { _CartList = value; OnPropertyChanged(); } }
        public Bill NewBill { get => _NewBill; set { _NewBill = value; OnPropertyChanged(); } }

        #endregion

        #region Unpaid
        private ObservableCollection<Model.Bill> _BillList;

        public UnpaidViewModel UnpaidVM { get; set; }
        public ObservableCollection<Model.Bill> BillList { get => _BillList; set { _BillList = value; OnPropertyChanged(); } }

        #endregion

        #region Command
        public ICommand PaidCMD { get; set; }
        public ICommand UnpaidCMD { get; set; }
        public ICommand SearchCMD { get; set; }
        public ICommand AddFoodToCartCMD { get; set; }
        public ICommand PlusCountCMD { get; set; }
        public ICommand SubtractionCountCMD { get; set; }
        public ICommand PayCMD { get; set; }
        public ICommand SelectedCMD { get; set; }

        #endregion
        public PayTablesViewModel()
        {
            PaidCMD = new RelayCommand<object>((p) => { return true; }, (p) => { CurentView = paidVM; });
            UnpaidCMD = new RelayCommand<object>((p) => { return true; }, (p) => { CurentView = UnpaidVM; LoadUnpaid(); });
            PlusCountCMD = new RelayCommand<BillInfo>((b) => { return true; }, (b) => { PlusFood(b); });
            SubtractionCountCMD = new RelayCommand<BillInfo>((b) => { return true; }, (b) => { SubtractinFood(b); });
            AddFoodToCartCMD = new RelayCommand<Food>((f) => { return true; }, (f) => { AddCartList(f); });
            PayCMD = new RelayCommand<object>((p) => { return true; }, (p) => { Pay(); });
            SelectedCMD = new RelayCommand<Bill>((b) => { return true; }, (b) => { ShowBilInfo(b); });
            SearchCMD = new RelayCommand<object>((p) => { return true; }, (p) => { Search(); });
        }
        #region Load
        public void LoadFood()
        {
            FoodCategory category = new FoodCategory() { Name = "Tất cả" };
            SelectedCategory = category;

            paidVM = new PaidViewMdel();
            UnpaidVM = new UnpaidViewModel();
            CurentView = paidVM;

            FoodCategoryList = new ObservableCollection<FoodCategory>(dataProvider.Ins.DB.FoodCategories);
            FoodCategoryList.Insert(0, category);
            FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IsOutOfStock == false));

            LoadPaid();

        }
        
        public void LoadPaid()
        {
            NewBill = new Bill() { Id = Guid.NewGuid().ToString(), IdTable = tb.Id, DateCheckOut = DateTime.Now, TableFood = tb };
            CartList = new ObservableCollection<BillInfo>();
        }
        public void LoadUnpaid()
        {
            BillList = new ObservableCollection<Bill>(dataProvider.Ins.DB.Bills.Where(x => x.TableFood.Id == tb.Id));
        }
        public void ShowBilInfo(Bill bill)
        {
            ShowBillWindow BillInfo = new ShowBillWindow();
            var BillInfoVM = BillInfo.DataContext as ShowBillViewModel;
            BillInfoVM.LoadBill(bill);
            BillInfo.ShowDialog();
        }

        #endregion

        #region Order
        public void AddCartList(Food food)
        {
            bool find = false;
            Count = 1;
            BillInfo billInfo = new BillInfo() { IdBill = NewBill.Id, IdFood = food.Id, Count = Count, Food = food, TotalPrice = Count * food.OutputPrice };

            foreach (var c in CartList)
                if (c.Food.Name == billInfo.Food.Name)
                {
                    foreach (var infor in CartList.Where(x => x.Food.Name == billInfo.Food.Name))
                    {
                        infor.Count++;
                        infor.TotalPrice = infor.Count * infor.Food.OutputPrice;

                        break;
                    }

                    find = true;
                    break;
                }

            if (!find)
                CartList.Add(billInfo);

            CalTotalPrice();
        }
        public void PlusFood(BillInfo billInfo)
        {
            foreach (var infor in CartList)
                if (infor.Food.Name == billInfo.Food.Name)
                {
                    infor.Count++;
                    infor.TotalPrice = infor.Count * infor.Food.OutputPrice;
                    CalTotalPrice();
                    return;
                }
        }
        public void SubtractinFood(BillInfo billInfo)
        {
            foreach (var infor in CartList)
                if (infor.Food.Name == billInfo.Food.Name)
                    if (infor.Count <= 1)
                    {
                        CartList.Remove(infor);
                        CalTotalPrice();
                        return;
                    }
                    else
                    {
                        infor.Count--;
                        infor.TotalPrice = infor.Count * infor.Food.OutputPrice;
                        CalTotalPrice();
                        return;
                    }
        }
        public void Pay()
        {
            dataProvider.Ins.DB.Bills.Add(NewBill);
            dataProvider.Ins.DB.SaveChanges();

            foreach (var infor in CartList)
            {
                infor.Id = Guid.NewGuid().ToString();
                dataProvider.Ins.DB.BillInfoes.Add(infor);
            }
            dataProvider.Ins.DB.SaveChanges();
            LoadPaid();
        }
        public void CalTotalPrice()
        {
            TotalBill = 0;
            foreach (var info in CartList)
            {
                TotalBill += (double)info.TotalPrice;
            }
        }
        #endregion

        #region method
        public void Search()
        {
            if (NameSearch == null || NameSearch == "")
            {
                if (SelectedCategory.Name == "Tất cả")
                {
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IsOutOfStock == false));
                }
                else
                {
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IsOutOfStock == false && x.IdCategory == SelectedCategory.Id));
                }
            }
            else
            {
                if (SelectedCategory.Name == "Tất cả")
                {
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IsOutOfStock == false && x.Name.Contains(NameSearch) ));
                }
                else
                {
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IsOutOfStock == false && x.IdCategory == SelectedCategory.Id && x.Name.Contains(NameSearch)));
                }
            }
        }
        #endregion
    }
}
