namespace EnemyField
{
    using System;
    using System.Windows.Forms;

    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(
                args.Length == 0
                    ? new Form1()
                    : new Form1(int.Parse(args[0])));
        }
    }
}
