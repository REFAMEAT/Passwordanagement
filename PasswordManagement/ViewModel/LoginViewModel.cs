﻿using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using PasswordManagement.Backend;
using PasswordManagement.Backend.Login;
using PasswordManagement.Database.DbSet;
using PasswordManagement.Database.Model;
using PasswordManagement.File.Binary;
using PasswordManagement.File.Config;
using PasswordManagement.Logging;
using PasswordManagement.Model;
using PasswordManagement.Model.Interfaces;
using PasswordManagement.Model.Setting;
using PasswordManagement.View;
using PasswordManagement.ViewModel.Base;

namespace PasswordManagement.ViewModel
{
    public class LoginViewModel : NotifyPropertyChanged
    {
        private readonly ILogin iLogin;
        private ICommand buttonCommandLogin;
        private string userName;

        internal LoginViewModel(ILogin logonMethod)
        {
            iLogin.Initialize();

            // Fallback, if logon Method doesn't work
            if (!iLogin.InitSuccessful)
            {
                iLogin = new LocalLogin();
            }

            bool needFirstUser = iLogin.NeedFirstUser();

            if (!needFirstUser)
            {
                return;
            }

            USERDATA firstUser = AddUser.CreateUser(true);

            if (firstUser != null && Globals.UseDatabase)
            {
                DataSet<USERDATA> data = new DataSet<USERDATA>();
                data.Entities.Add(firstUser);
                data.SaveChanges();
            }
            else if (firstUser != null)
            {
                new BinaryHelper().Write(new BinaryData(firstUser.USUSERNAME, firstUser.USPASSWORD, firstUser.USSALT));
            }
        }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public ICommand ButtonCommandLogin => buttonCommandLogin ??= new Command(DoLogin);

        private void DoLogin(object obj)
        {
            if (!(obj is Login login)) return;

            string userId = iLogin.Validate(userName, login.passwordBox.Password);

            if (userId != null)
            {
                login.DialogResult = true;
                Globals.CurrentUserId = userId;
                login.Close();
            }
            else
            {
                login.userNameTextBox.Foreground = Brushes.Red;
                login.passwordBox.Foreground = Brushes.Red;
            }
        }
    }
}