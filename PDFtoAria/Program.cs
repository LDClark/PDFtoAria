using System;
using VMS.TPS.Common.Model.API;

namespace PDFtoAria
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (Application app = Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            } 
        }

        static void Execute(Application app)
        {
            var user = app.CurrentUser;
            var mainView = new MainView();
            var viewModel = new MainViewModel(user);
            mainView.DataContext = viewModel;
            mainView.Title = "PDFtoAria";
            mainView.ShowDialog();
        }
    }
}
