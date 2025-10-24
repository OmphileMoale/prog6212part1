using Microsoft.Win32;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ContractClaim.Pages
{
    public partial class SubmitClaimPage : Page
    {
        private string[] uploadedFiles;

        public SubmitClaimPage()
        {
            InitializeComponent();

            // Set default date
            DpClaimMonth.SelectedDate = DateTime.Now;

            // Update total when inputs change
            TbHoursWorked.TextChanged += UpdateEstimatedTotal;
            TbHourlyRate.TextChanged += UpdateEstimatedTotal;
        }

        // Allow only numbers and decimal
        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d*\.?\d*$");
        }

        // Update the estimated total
        private void UpdateEstimatedTotal(object sender, TextChangedEventArgs e)
        {
            double hours = 0;
            double rate = 0;

            double.TryParse(TbHoursWorked.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out hours);
            double.TryParse(TbHourlyRate.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out rate);

            double total = hours * rate;
            TbEstimatedTotal.Text = total.ToString("C2", CultureInfo.GetCultureInfo("en-ZA"));
        }

        // Upload files
        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Documents (*.pdf;*.doc;*.docx;*.jpg;*.png;*.xlsx)|*.pdf;*.doc;*.docx;*.jpg;*.png;*.xlsx"
            };

            if (ofd.ShowDialog() == true)
            {
                uploadedFiles = ofd.FileNames;
                TbFilesSelected.Text = $"{uploadedFiles.Length} file(s) selected";
            }
        }

        // Submit claim
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbHoursWorked.Text) ||
                string.IsNullOrWhiteSpace(TbHourlyRate.Text) ||
                string.IsNullOrWhiteSpace(TbModule.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string status = RbPending.IsChecked == true ? "Pending Review" : "Pre-Approved";
            string totalAmount = TbEstimatedTotal.Text;

            MessageBox.Show($"Claim submitted!\nModule: {TbModule.Text}\nTotal: {totalAmount}\nStatus: {status}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset form
            TbHoursWorked.Text = "";
            TbHourlyRate.Text = "850";
            TbModule.Text = "";
            TbNotes.Text = "";
            uploadedFiles = null;
            TbFilesSelected.Text = "";
            RbPending.IsChecked = true;

            UpdateEstimatedTotal(null, null);

            TbSuccessMessage.Visibility = Visibility.Visible;
        }

        // Cancel and go back
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}


