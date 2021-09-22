using CoffeShop.Model;
using CoffeShop.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CoffeShop.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand HomeViewCommand { get; set; }
        public ICommand FoodTableCommand { get; set; }
        public ICommand FoodCommand { get; set; }
        public ICommand FoodCategoryCommand { get; set; }


        public HomeViewModel homeVM { get; set; }
        public FoodTableViewModel foodTableVM { get; set; }
        public FoodCategoryViewModel foodCategoryVM { get; set; }
        public FoodViewModel foodVM { get; set; }

        private object _currentView;
        public object currentView { get { return _currentView; } set { _currentView = value; OnPropertyChanged(); } }

        public MainWindowViewModel()
        {
            homeVM = new HomeViewModel();
            foodTableVM = new FoodTableViewModel();
            foodCategoryVM = new FoodCategoryViewModel();
            foodVM = new FoodViewModel();

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                currentView = homeVM;
                Isloaded = true;
                if (p == null)
                    return;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext != null)
                    p.Show();
                else 
                    return;


                var loginVM = loginWindow.DataContext as LoginWindowViewModel;

                if (loginVM.Islogin)
                {
                    p.Show();
                }
                else
                {
                    p.Close();
                }

            });

            HomeViewCommand = new RelayCommand<object>((p) => { return true; }, (p) => { currentView = homeVM; });
            FoodTableCommand = new RelayCommand<object>((p) => { return true; }, (p) =>{ currentView = foodTableVM; });
            FoodCategoryCommand = new RelayCommand<object>((p) => { return true; }, (p) =>{ currentView = foodCategoryVM; });
            FoodCommand = new RelayCommand<object>((p) => { return true; }, (p) => { currentView = foodVM; });

        }
    }
}
