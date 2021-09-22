using CoffeShop.Model;
using CoffeShop.View.Categories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoffeShop.ViewModel
{
    public class FoodCategoryViewModel : BaseViewModel
    {
        public AddCategory AddCategory;
        public UpdateCategory UpdateCategory;

        private ObservableCollection<FoodCategory> _categoryList;
        public ObservableCollection<FoodCategory> categoryList { get => _categoryList; set { _categoryList = value; OnPropertyChanged(); } }

        private FoodCategory _SelectedItem;
        public FoodCategory SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    Name = SelectedItem.Name;
                    Note = SelectedItem.Note; 
                }
            }
        }
        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Note { get; set; }
        public string Note { get => _Note; set { _Note = value; OnPropertyChanged(); } }

        #region command
        public ICommand ButtonAddCMD { get; set; }
        public ICommand ButtonUpdateCMD { get; set; }
        public ICommand CloseAddCMD { get; set; }
        public ICommand CloseUpdateCMD { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        #endregion
        public FoodCategoryViewModel()
        {
            categoryList = new ObservableCollection<FoodCategory>(dataProvider.Ins.DB.FoodCategories);
            //Cửa sổ thêm danh mục
            ButtonAddCMD = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                AddCategory = new AddCategory();
                AddCategory.ShowDialog();
            });

            CloseAddCMD = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                AddCategory.Close();
            });

            //Cửa sổ sửa danh mục
            ButtonUpdateCMD = new RelayCommand<Window>((p) => 
            {
                if (SelectedItem == null)
                    return false;
                else
                    return true; 
            }, (p) =>
            {
                UpdateCategory = new UpdateCategory();
                UpdateCategory.ShowDialog();
            });

            CloseUpdateCMD = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                UpdateCategory.Close();
            });

            //Thêm danh mục mới
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (Name == null)
                    return false;
                return true;
                
            }, (p) =>
            {
                var userLogin = dataProvider.Ins.DB.Accounts.Where(x => x.Online == true).SingleOrDefault();
                var category = new FoodCategory() {Id = Guid.NewGuid().ToString(), Name = Name,Note = Note,NameUserAdd = userLogin.UserName ,DateAdd = DateTime.Now};
                
                dataProvider.Ins.DB.FoodCategories.Add(category);
                dataProvider.Ins.DB.SaveChanges();
                categoryList.Add(category);
                AddCategory.Close();
            });

            //Sửa danh mục
            UpdateCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                var userLogin = dataProvider.Ins.DB.Accounts.Where(x => x.Online == true).SingleOrDefault();
                var Object = dataProvider.Ins.DB.FoodCategories.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Object.Name = Name;
                Object.Note = Note;
                Object.NameUserChange = userLogin.UserName;
                Object.DateChange = DateTime.Now;
                dataProvider.Ins.DB.SaveChanges();
                UpdateCategory.Close();
            });
        }
    }
}
