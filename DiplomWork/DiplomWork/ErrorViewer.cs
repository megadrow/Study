


namespace DiplomWork
{
    public static class ErrorViewer
    {
        private const string progName = "RTS-Sim";

        public static void ShowError(string msg)
        {
            System.Windows.Forms.MessageBox.Show(
                msg, 
                progName + ":Ошибка", 
                System.Windows.Forms.MessageBoxButtons.OK, 
                System.Windows.Forms.MessageBoxIcon.Error);
        }

        public static void ShowError(System.Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, 
                progName + ":Ошибка", 
                System.Windows.Forms.MessageBoxButtons.OK, 
                System.Windows.Forms.MessageBoxIcon.Error);
        }

        public static void ShowInfo(string msg)
        {
            System.Windows.Forms.MessageBox.Show(
               msg,
                progName + ":Информация",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }
    }
}
