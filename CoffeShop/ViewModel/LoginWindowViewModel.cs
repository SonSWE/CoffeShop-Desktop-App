using CoffeShop.Model;
using CoffeShop.View.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoffeShop.ViewModel
{
    public class LoginWindowViewModel : BaseViewModel
    {
        public bool Islogin { get; set; }

        private bool _IsChecked;

        public bool IsChecked { get => _IsChecked; set { _IsChecked = value; OnPropertyChanged(); } }

        private string _UserName;
        public string UserName{ get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; set; }
        public LoginWindowViewModel()
        {
            if (Properties.Settings.Default.User != string.Empty)
            {
                UserName = Properties.Settings.Default.User;
                Password = Properties.Settings.Default.Pass;
            }
            Islogin = false;
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); }); 
        }
        void Login(Window p)
        {
            if (p == null)
                return;
            string passEncode = MD5Hash(Base64Encode(Password));
            int accCount = dataProvider.Ins.DB.Accounts.Where(x => x.UserName == UserName && x.Password == passEncode).Count();

            if(IsChecked && accCount > 0)
            {
                Properties.Settings.Default.User = UserName;
                Properties.Settings.Default.Pass = Password;
                Properties.Settings.Default.Save();
            } 

            if(!IsChecked && accCount <= 0)
            {
                Properties.Settings.Default.User = null;
                Properties.Settings.Default.Pass = null;
                Properties.Settings.Default.Save();
            }

            if(accCount > 0)
            {
                Islogin = true;
                var userLogin = dataProvider.Ins.DB.Accounts.Where(x => x.UserName == UserName).SingleOrDefault();
                userLogin.Online = true;
                dataProvider.Ins.DB.SaveChanges();
                p.Close();
            }  
            else
            {
                Islogin = false;
                ErrorLogin er = new ErrorLogin();
                er.ShowDialog();
            }
            
        }
        public static string Base64Encode(string plainText)
        {
            if (plainText == null)
                return "x";
            
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
            
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
