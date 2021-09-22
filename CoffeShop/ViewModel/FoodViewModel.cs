using CoffeShop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CoffeShop.View.Food;
using CoffeShop.MyExtentions;

namespace CoffeShop.ViewModel
{
    public class FoodViewModel : BaseViewModel
    {
        public UpdateFood UpdateFood;
        public AddFood AddFood;

        private ObservableCollection<Model.Food> _FoodList;
        public ObservableCollection<Model.Food> FoodList { get => _FoodList; set { _FoodList = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.FoodCategory> _FoodCategoryListForSearch;
        public ObservableCollection<Model.FoodCategory> FoodCategoryListForSearch { get => _FoodCategoryListForSearch; set { _FoodCategoryListForSearch = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.FoodCategory> _FoodCategoryList;
        public ObservableCollection<Model.FoodCategory> FoodCategoryList { get => _FoodCategoryList; set { _FoodCategoryList = value; OnPropertyChanged(); } }

        private Model.FoodCategory _SelectedCategory;
        public Model.FoodCategory SelectedCategory { get => _SelectedCategory; set { _SelectedCategory = value; OnPropertyChanged(); } }

        private Model.FoodCategory _SelectedCategoryForSearch;
        public Model.FoodCategory SelectedCategoryForSearch { get => _SelectedCategoryForSearch; set { _SelectedCategoryForSearch = value; OnPropertyChanged(); } }

        #region menber
        private BitmapImage _ImgBitmap;
        private string _ImgString;
        private string _Id;
        private string _IdCategory;
        private string _Name;
        private int _Stock;
        private double _InphutPrice;
        private double _OutputPrice;
        private string _Note;
        private string _IdImage;
        private string _NameSearch;
        public string ImgString { get => _ImgString; set { _ImgString = value; OnPropertyChanged(); } }
        public BitmapImage ImgBitmap { get => _ImgBitmap; set { _ImgBitmap = value; OnPropertyChanged(); } }
        private Nullable<bool> _IsOutOfStock;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public string IdCategory { get => _IdCategory; set { _IdCategory = value; OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        public int Stock { get => _Stock; set { _Stock = value; OnPropertyChanged(); } }
        public double InputPrice { get => _InphutPrice; set { _InphutPrice = value; OnPropertyChanged(); } }
        public double OutputPrice { get => _OutputPrice; set { _OutputPrice = value; OnPropertyChanged(); } }
        public string Note { get => _Note; set { _Note = value; OnPropertyChanged(); } }
        public string IdImage { get => _IdImage; set { _IdImage = value; OnPropertyChanged(); } }
        public Nullable<bool> IsOutOfStock { get => _IsOutOfStock; set { _IsOutOfStock = value; OnPropertyChanged(); } }
        public string NameSearch { get => _NameSearch; set { _NameSearch = value; OnPropertyChanged(); } }
        #endregion
        #region Command
        public ICommand ButtonAddCMD { get; set; }
        public ICommand CloseAddCMD { get; set; }
        public ICommand ButtonUpdateCMD { get; set; }
        public ICommand CloseUpdateCMD { get; set; }
        public ICommand LoadImgCMD { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCMD { get; set; }
        public ICommand CheckBoxCMD { get; set; }
        public ICommand SearchCMD { get; set; }
        #endregion

        public FoodViewModel()
        {
            LoadFood();
            //Cửa sổ thêm món mới
            ButtonAddCMD = new RelayCommand<Window>((p) => { return true; }, (p) =>{ AddFood = new AddFood(); AddFood.ShowDialog(); });

            CloseAddCMD = new RelayCommand<Window>((p) => { return true; }, (p) =>{ AddFood.Close(); });

            //Cửa sổ sửa món
            ButtonUpdateCMD = new RelayCommand<Model.Food>((f) => { return true; }, (f) => { OpenFormEditFood(f); });

            CloseUpdateCMD = new RelayCommand<Window>((p) => { return true; }, (p) => { UpdateFood.Close(); });

            LoadImgCMD = new RelayCommand<object>((p) => { return true; }, (p) => { LoadImageFormPC(); });

            AddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddNewFood(); });

            EditCMD = new RelayCommand<object>((p) => { return true; }, (p) =>{ EditFood(); });

            CheckBoxCMD = new RelayCommand<Model.Food>((f) => { return true; }, (f) => { dataProvider.Ins.DB.SaveChanges(); });

            SearchCMD = new RelayCommand<object>((p) => { return true; }, (p) => { Search(); });
        }
        public void LoadFood()
        {
            FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods);
            FoodCategoryList = new ObservableCollection<Model.FoodCategory>(dataProvider.Ins.DB.FoodCategories);
            FoodCategoryListForSearch = new ObservableCollection<Model.FoodCategory>(dataProvider.Ins.DB.FoodCategories);
            FoodCategory category = new FoodCategory() { Name = "Tất cả" };
            FoodCategoryListForSearch.Insert(0, category);
            SelectedCategoryForSearch = category;
        }
        #region method
        public void Search()
        {
            if (NameSearch == null || NameSearch == "")
            {
                if (SelectedCategoryForSearch.Name == "Tất cả")
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods);
                else
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IdCategory == SelectedCategoryForSearch.Id));
            }
            else
            {
                if (SelectedCategoryForSearch.Name == "Tất cả")
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.Name.Contains(NameSearch)));
                else
                    FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods.Where(x => x.IdCategory == SelectedCategoryForSearch.Id && x.Name.Contains(NameSearch)));
            }
        }
        public void OpenFormEditFood(Model.Food f)
        {
            UpdateFood = new UpdateFood();
            Id = f.Id;
            Name = f.Name;
            SelectedCategory = f.FoodCategory;
            InputPrice = f.InputPrice;
            OutputPrice = f.OutputPrice;
            Note = f.Note;
            Stock = f.Stock;
            IsOutOfStock = f.IsOutOfStock;
            IdImage = f.IdImage;
            ImgBitmap = MyExtention.Base64ToImageSource(f.Image.Data);
            ImgString = f.Image.Data;
            UpdateFood.ShowDialog();
        }

        public void LoadImageFormPC()
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog() { Filter = "Image files (*.jpg, *.png) | *.jpg; *.png" };
            openFile.CheckFileExists = true;
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImgString = MyExtention.ConvertImageToBase64(openFile.FileName);
                ImgBitmap = MyExtention.Base64ToImageSource(ImgString);
            }
        }

        public void AddNewFood()
        {
            Id = Guid.NewGuid().ToString();

            //Lưu ảnh
            var img = new Image() { Id = Guid.NewGuid().ToString(), Data = ImgString };
            dataProvider.Ins.DB.Images.Add(img);
            dataProvider.Ins.DB.SaveChanges();

            //Lưu món
            if (Stock > 0)
                IsOutOfStock = false;
            else
                IsOutOfStock = true;

            var food = new Model.Food() { Id = Id, IdCategory = SelectedCategory.Id, IdImage = img.Id, Name = Name, InputPrice = InputPrice, Stock = Stock, OutputPrice = OutputPrice, Note = Note, IsOutOfStock = IsOutOfStock };
            dataProvider.Ins.DB.Foods.Add(food);
            dataProvider.Ins.DB.SaveChanges();

            //Hiển thị món ăn
            FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods);

            //Thoát cửa sổ
            AddFood.Close();
        }

        public void EditFood()
        {
            var img = dataProvider.Ins.DB.Images.Where(x => x.Id == IdImage).SingleOrDefault();
            img.Data = ImgString;
            dataProvider.Ins.DB.SaveChanges();

            var food = dataProvider.Ins.DB.Foods.Where(x => x.Id == Id).SingleOrDefault();
            food.Name = Name;
            food.InputPrice = InputPrice;
            food.OutputPrice = OutputPrice;
            food.Stock = Stock;
            if (Stock > 0)
                food.IsOutOfStock = false;
            else
                food.IsOutOfStock = true;
            food.Note = Note;
            food.IdCategory = SelectedCategory.Id;
            dataProvider.Ins.DB.SaveChanges();

            FoodList = new ObservableCollection<Model.Food>(dataProvider.Ins.DB.Foods);

            UpdateFood.Close();
        }
        #endregion
    }

}
