using DatabaseLib;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
//using System.Windows.Forms;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace WpfAppServer
{
    /// <summary>
    /// Interaction logic for ReportArchive.xaml
    /// </summary>
    public partial class ReportArchive : Window
    {
        public ObservableCollection<CheckBoxList> TheList { get; set; }
        DataTable tableView = new DataTable();
        DataTable tableTotal = new DataTable();
        DataTable tableTotalHour = new DataTable();
        DataTable tableTotalDay = new DataTable();
        string col = ""; int NuCol = 0;

        public class CheckBoxList
        {
            public string TheText { get; set; }
            public bool IsSelected { get; set; }
            public string Details
            {
                get
                {
                    return String.Format("{0} and {1}", this.IsSelected, this.TheText);
                }
            }
        }

        public ReportArchive()
        {
            InitializeComponent();
            this.SetWindowState();
            TheList = new ObservableCollection<CheckBoxList>();
            ListBox1.DataContext = this;
            ListBox2.DataContext = this;
            ListBox3.DataContext = this;
            ListBox4.DataContext = this;
            ListBox5.DataContext = this;

            ComboBox1.SelectedIndex = 4;
            ComboBox2.SelectedIndex = 4;
            ComboBox3.SelectedIndex = 4;
            ComboBox4.SelectedIndex = 4;

            string CheckTable = "SELECT * FROM SaveDataHistory;";
            using (var reader = DataHelper.Instance.ExecuteDataTable(CheckTable))
            {
                foreach (DataColumn column in reader.Columns)
                {
                    if (column.ColumnName == "Ngay") { }
                    else if (column.ColumnName == "myId") { }
                    else if (column.ColumnName == "NgayDouble") { }
                    else
                    {
                        TheList.Add(new CheckBoxList { IsSelected = true, TheText = column.ColumnName });
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.SaveWindowState();
        }

        #region "Tổng"
        public void ViewDataTotal()
        {
            List<string> ColumTotalLocal = new List<string>();
            DataTable tableRead = new DataTable();
            object[] dataObjects;
            double TOTAL = 0;

            try
            {
                ColumTotalLocal.Clear();
                NuCol = 0;
                tableTotal = new DataTable();
                tableTotal.Clear();
                var selecteds = TheList.Where(itemS => itemS.IsSelected == true);
                foreach (var obj in selecteds)
                {
                    ColumTotalLocal.Add(obj.TheText);
                    NuCol++;
                }

                TextBox2.Text = NuCol.ToString();
                dataObjects = new object[NuCol + 1];
                // tạo chuỗi dữ liệu trong khoảng ngày chọn từ datepicker
                string sql = "SELECT Round(Max(" + ColumTotalLocal[0] + ")-Min(" + ColumTotalLocal[0] + "),0)" + " FROM SaveDataHistory where "
                            + " Ngay>='" + ConvertDatePicker(Picker21, "dd/MM/yyyy") + " 00:00' and Ngay<='" + ConvertDatePicker(Picker22, "dd/MM/yyyy") + " 23:59'";
                // union all lấy ra dữ liệu các cột có thể trùng lặp của 2 select
                for (int i = 1; i < NuCol; i++)
                {
                    sql += " union all SELECT Round(Max(" + ColumTotalLocal[i] + ")-Min(" + ColumTotalLocal[i] + "),0)" + " FROM SaveDataHistory where "
                            + " Ngay>='" + ConvertDatePicker(Picker21, "dd/MM/yyyy") + " 00:00' and Ngay<='" + ConvertDatePicker(Picker22, "dd/MM/yyyy") + " 23:59'";
                }

                using (var reader = DataHelper.Instance.ExecuteDataTable(sql))
                {
                    tableRead = reader;

                    for (int i = 0; i < ColumTotalLocal.Count; i++)
                    {
                        tableTotal.Columns.Add(ColumTotalLocal[i] + " (Kwh)");

                        if (tableRead.Rows[i][0].ToString() != "")
                        {
                            dataObjects[i] = tableRead.Rows[i][0];
                            TOTAL += double.Parse(tableRead.Rows[i][0].ToString());
                        }
                        else
                        {
                            dataObjects[i] = 0;
                        }
                    }
                    tableTotal.Columns.Add("TOTAL (Kwh)");
                    dataObjects[ColumTotalLocal.Count] = TOTAL;
                    tableTotal.Rows.Add(dataObjects);
                    DataGrid2.ItemsSource = tableTotal.DefaultView;
                }
            }
            catch
            {
                MessageBox.Show("Chọn laị dữ liệu đúng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally { }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ViewDataTotal();
        }

        private void ButtonPDF2_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_PDF(tableTotal, "A4", "Report", "From: " + ConvertDatePicker(Picker21, "dd/MM/yyyy") + " To: " + ConvertDatePicker(Picker22, "dd/MM/yyyy"));
        }
        private void ButtonExcel2_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_Excel(tableTotal, "Report", "Từ " + ConvertDatePicker(Picker21, "dd/MM/yyyy") + " đến " + ConvertDatePicker(Picker22, "dd/MM/yyyy"));
        }
        #endregion

        #region "Theo Giờ"

        public void ViewDataTheogio()
        {
            List<string> ColumTotalLocal = new List<string>();
            DataSet dset = new DataSet();
            object[] dataObjects;
            dset.Reset();

            try
            {
                ColumTotalLocal.Clear();
                NuCol = 0;
                tableTotalHour = new DataTable();
                tableTotalHour.Clear();
                var selecteds = TheList.Where(itemS => itemS.IsSelected == true);
                foreach (var obj in selecteds)
                {
                    ColumTotalLocal.Add(obj.TheText);
                    NuCol++;
                }

                TextBox3.Text = NuCol.ToString();
                dataObjects = new object[NuCol + 2];

                for (int j = 0; j < ColumTotalLocal.Count; j++)
                {
                    string sql = "SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                          "Ngay<='" + ConvertDatePicker(Picker31, "yyyy-MM-dd") + " 00:00'";

                    for (int i = 0; i < 24; i++)
                    {
                        if (i < 10)
                        {
                            sql += " union all SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                                    "Ngay<='" + ConvertDatePicker(Picker31, "yyyy-MM-dd") + " 0" + Convert.ToString(i) + ":59'";
                        }
                        else if (i >= 10)
                        {
                            sql += " union all SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                                    "Ngay<='" + ConvertDatePicker(Picker31, "yyyy-MM-dd") + " " + Convert.ToString(i) + ":59'";
                        }
                    }

                    using (var reader = DataHelper.Instance.ExecuteDataTable(sql))
                    {
                        dset.Tables.Add(reader);
                    }
                }

                //Tạo cột cho bảng dữ liệu
                tableTotalHour.Columns.Add("Ngày");
                for (int i = 0; i < ColumTotalLocal.Count; i++)
                {
                    tableTotalHour.Columns.Add(ColumTotalLocal[i] + " (Kwh)");
                }
                tableTotalHour.Columns.Add("TOTAL (Kwh)");

                //Tìm số lượng dữ liệu trống
                int[] jNull = new int[dset.Tables.Count];
                for (int i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        if (dset.Tables[j].Rows[i][0].ToString() == "" || dset.Tables[j].Rows[i][0].ToString() == "0")
                        {
                            jNull[j]++;
                        }
                    }
                }

                //Gán dữ liệu vào mảng 2 chiều
                double[,] ArrayTest = new double[dset.Tables[0].Rows.Count, dset.Tables.Count];
                for (int i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        if (dset.Tables[j].Rows[i][0].ToString() == "" || dset.Tables[j].Rows[i][0].ToString() == "0")
                        {
                            if (jNull[j] == dset.Tables[j].Rows.Count)
                            {
                                ArrayTest[i, j] = 0;
                            }
                            else
                            {
                                ArrayTest[i, j] = double.Parse(dset.Tables[j].Rows[jNull[j]][0].ToString());
                            }
                        }
                        else
                        {
                            ArrayTest[i, j] = double.Parse(dset.Tables[j].Rows[i][0].ToString());
                        }
                    }
                }

                //Xuất dữ liệu ra bảng
                for (int i = 0; i < dset.Tables[0].Rows.Count - 1; i++)
                {
                    double TOTAL = 0;
                    if (i < 10)
                    {
                        dataObjects[0] = ConvertDatePicker(Picker31, "yyyy/MM/dd") + " 0" + Convert.ToString(i) + ":00";
                    }
                    else if (i >= 10)
                    {
                        dataObjects[0] = ConvertDatePicker(Picker31, "yyyy/MM/dd") + " " + Convert.ToString(i) + ":00";
                    }

                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        dataObjects[j + 1] = Math.Round(ArrayTest[i + 1, j] - ArrayTest[i, j], 3);
                        TOTAL += Math.Round(ArrayTest[i + 1, j] - ArrayTest[i, j], 3);
                    }
                    dataObjects[dset.Tables.Count + 1] = TOTAL;
                    tableTotalHour.Rows.Add(dataObjects);
                    DataGrid3.ItemsSource = tableTotalHour.DefaultView;
                }
            }
            catch
            {
                MessageBox.Show("Chọn laị dữ liệu đúng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally { }
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ViewDataTheogio();
        }

        private void ButtonPDF3_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_PDF(tableTotalHour, "A4", "Report", "Day " + ConvertDatePicker(Picker31, "dd/MM/yyyy"));
        }
        private void ButtonExcel3_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_Excel(tableTotalHour, "Report", "Ngày " + ConvertDatePicker(Picker31, "dd/MM/yyyy"));
        }

        #endregion

        #region "Theo Ngày"

        public void ViewDataTheoNgay()
        {
            List<string> ColumTotalLocal = new List<string>();
            DataSet dset = new DataSet();
            object[] dataObjects;
            dset.Reset();

            try
            {
                ColumTotalLocal.Clear();
                NuCol = 0;
                tableTotalDay = new DataTable();
                tableTotalDay.Clear();
                var selecteds = TheList.Where(itemS => itemS.IsSelected == true);
                foreach (var obj in selecteds)
                {
                    ColumTotalLocal.Add(obj.TheText);
                    NuCol++;
                }

                TextBox4.Text = NuCol.ToString();
                dataObjects = new object[NuCol + 2];

                int numberDate = 28;
                if (ConvertDatePicker(Picker41, "MM") == "01" || ConvertDatePicker(Picker41, "MM") == "03" || ConvertDatePicker(Picker41, "MM") == "05" || ConvertDatePicker(Picker41, "MM") == "07" || ConvertDatePicker(Picker41, "MM") == "08" || ConvertDatePicker(Picker41, "MM") == "10" || ConvertDatePicker(Picker41, "MM") == "12")
                {
                    numberDate = 31;
                }
                else if (ConvertDatePicker(Picker41, "MM") == "04" || ConvertDatePicker(Picker41, "MM") == "06" || ConvertDatePicker(Picker41, "MM") == "09" || ConvertDatePicker(Picker41, "MM") == "11")
                {
                    numberDate = 30;
                }
                else if (int.Parse(ConvertDatePicker(Picker41, "yyyy")) % 4 == 0 && ConvertDatePicker(Picker41, "MM") == "02")
                {
                    numberDate = 29;
                }
                else
                {
                    numberDate = 28;
                }

                for (int j = 0; j < ColumTotalLocal.Count; j++)
                {
                    string sql = "SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                          "Ngay<='" + ConvertDatePicker(Picker41, "yyyy") + "-" + ConvertDatePicker(Picker41, "MM") + "-01" + " 06:00'";

                    for (int i = 2; i <= numberDate; i++)
                    {
                        if (i < 10)
                        {
                            sql += " union all SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                                    "Ngay<='" + ConvertDatePicker(Picker41, "yyyy") + "-" + ConvertDatePicker(Picker41, "MM") + "-0" + Convert.ToString(i) + " 06:00'";
                        }
                        else if (i >= 10)
                        {
                            sql += " union all SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                                    "Ngay<='" + ConvertDatePicker(Picker41, "yyyy") + "-" + ConvertDatePicker(Picker41, "MM") + "-" + Convert.ToString(i) + " 06:00'";
                        }
                    }

                    sql += " union all SELECT Max(" + ColumTotalLocal[j] + ") FROM SaveDataHistory where " +
                                                    "Ngay<='" + ConvertDatePicker(Picker41, "yyyy") + "-" + ConvertDatePicker(Picker41, "MM") + "-" + Convert.ToString(numberDate) + " 23:59'";

                    using (var reader = DataHelper.Instance.ExecuteDataTable(sql))
                    {
                        dset.Tables.Add(reader);
                    }
                }

                //Tạo cột cho bảng dữ liệu
                tableTotalDay.Columns.Add("Ngày");
                for (int i = 0; i < ColumTotalLocal.Count; i++)
                {
                    tableTotalDay.Columns.Add(ColumTotalLocal[i] + " (Kwh)");
                }
                tableTotalDay.Columns.Add("TOTAL (Kwh)");

                //Tìm số lượng dữ liệu trống
                int[] jNull = new int[dset.Tables.Count];
                for (int i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        if (dset.Tables[j].Rows[i][0].ToString() == "" || dset.Tables[j].Rows[i][0].ToString() == "0")
                        {
                            jNull[j]++;
                        }
                    }
                }

                //Gán dữ liệu vào mảng 2 chiều
                double[,] ArrayTest = new double[dset.Tables[0].Rows.Count, dset.Tables.Count];
                for (int i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        if (dset.Tables[j].Rows[i][0].ToString() == "" || dset.Tables[j].Rows[i][0].ToString() == "0")
                        {
                            if (jNull[j] == dset.Tables[j].Rows.Count)
                            {
                                ArrayTest[i, j] = 0;
                            }
                            else
                            {
                                ArrayTest[i, j] = double.Parse(dset.Tables[j].Rows[jNull[j]][0].ToString());
                            }
                        }
                        else
                        {
                            ArrayTest[i, j] = double.Parse(dset.Tables[j].Rows[i][0].ToString());
                        }
                    }
                }

                //Xuất dữ liệu ra bảng
                for (int i = 1; i < dset.Tables[0].Rows.Count; i++)
                {
                    double TOTAL = 0;
                    if (i < 10)
                    {
                        dataObjects[0] = ConvertDatePicker(Picker41, "yyyy") + "/" + ConvertDatePicker(Picker41, "MM") + "/0" + Convert.ToString(i) + " 06:00";
                    }
                    else if (i >= 10)
                    {
                        dataObjects[0] = ConvertDatePicker(Picker41, "yyyy") + "/" + ConvertDatePicker(Picker41, "MM") + "/" + Convert.ToString(i) + " 06:00";
                    }

                    for (int j = 0; j < dset.Tables.Count; j++)
                    {
                        dataObjects[j + 1] = Math.Round(ArrayTest[i, j] - ArrayTest[i - 1, j], 3);
                        TOTAL += Math.Round(ArrayTest[i, j] - ArrayTest[i - 1, j], 3);
                    }
                    dataObjects[dset.Tables.Count + 1] = TOTAL;
                    tableTotalDay.Rows.Add(dataObjects);
                    DataGrid4.ItemsSource = tableTotalDay.DefaultView;
                }
            }
            catch
            {
                MessageBox.Show("Chọn laị dữ liệu đúng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally { }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ViewDataTheoNgay();
        }

        private void ButtonPDF4_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_PDF(tableTotalDay, "A4", "Report", "Month " + ConvertDatePicker(Picker41, "MM/yyyy"));
        }

        private void ButtonExcel4_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_Excel(tableTotalDay, "Report", "Tháng " + ConvertDatePicker(Picker41, "MM/yyyy"));
        }
        #endregion

        #region "Report"

        public string ConvertDatePicker(DatePicker value, string Type)
        {
            DateTime? selectedDate = value.SelectedDate;

            if (selectedDate.HasValue)
            {
                return selectedDate.Value.ToString(Type, System.Globalization.CultureInfo.InvariantCulture);
            }

            return DateTime.Now.ToString(Type, System.Globalization.CultureInfo.InvariantCulture);
        }

        public void ExportTOTAL_Excel(DataTable dataGridView, string tieude, string ngay)
        {
            //foreach (Process proc in Process.GetProcessesByName("EXCEL")) { proc.Kill(); }

            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = true;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;

            // Tạo phần đầu nếu muốn nếu có cột tổng: for (int i = 1; i <= dataGridView.Columns.Count + 1; i++)
            for (int i = 1; i <= dataGridView.Columns.Count; i++)
            {
                worksheet.Columns[i].ColumnWidth = 25;
            }
            for (int i = 1; i <= dataGridView.Rows.Count + 2; i++)
            {
                worksheet.Rows[i].RowHeight = 22;
            }

            Microsoft.Office.Interop.Excel.Range PhanDau1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1];
            Microsoft.Office.Interop.Excel.Range PhanDau2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, dataGridView.Columns.Count]; //nếu có cột tổng: + 1
            Microsoft.Office.Interop.Excel.Range PhanDau = worksheet.get_Range(PhanDau1, PhanDau2);
            PhanDau.MergeCells = true;
            PhanDau.Value2 = tieude;
            PhanDau.Font.Bold = true;
            PhanDau.Font.Name = "Times New Roman";
            PhanDau.Font.Size = "18";
            PhanDau.Font.ColorIndex = 30;
            PhanDau.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            Microsoft.Office.Interop.Excel.Range PhanTime1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, 1];
            Microsoft.Office.Interop.Excel.Range PhanTime2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, dataGridView.Columns.Count]; //nếu có cột tổng: + 1
            Microsoft.Office.Interop.Excel.Range PhanTime = worksheet.get_Range(PhanTime1, PhanTime2);
            PhanTime.MergeCells = true;
            PhanTime.Value2 = ngay; // "Từ " + dateTimePicker01.Value.ToString("dd/MM/yyyy HH:mm:ss") + " đến " + dateTimePicker02.Value.ToString("dd/MM/yyyy HH:mm:ss");
            PhanTime.Font.Bold = false;
            PhanTime.Font.Name = "Times New Roman";
            PhanTime.Font.Size = "12";
            PhanTime.Font.ColorIndex = 26;
            PhanTime.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            Microsoft.Office.Interop.Excel.Range PhanData1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, 1];
            Microsoft.Office.Interop.Excel.Range PhanData2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[dataGridView.Rows.Count + 3, dataGridView.Columns.Count];
            Microsoft.Office.Interop.Excel.Range PhanData = worksheet.get_Range(PhanData1, PhanData2);
            PhanData.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            PhanData.Font.Bold = false;
            PhanData.Font.Name = "Times New Roman";
            PhanData.Font.Size = "12";

            Microsoft.Office.Interop.Excel.Range Vung1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1];
            Microsoft.Office.Interop.Excel.Range Vung2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[dataGridView.Rows.Count + 3, dataGridView.Columns.Count];
            Microsoft.Office.Interop.Excel.Range Vung = worksheet.get_Range(Vung1, Vung2);
            Vung.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;

            //data
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                worksheet.Cells[3, i + 1] = dataGridView.Columns[i].ColumnName;
            }
            //worksheet.Cells[3, dataGridView.Columns.Count + 1] = "TỔNG";

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (dataGridView.Rows[i][j] != null)
                    {
                        worksheet.Cells[i + 4, j + 1] = dataGridView.Rows[i][j].ToString();
                    }
                    else
                    {
                        worksheet.Cells[i + 4, j] = "";
                    }
                }
            }

            Microsoft.Office.Interop.Excel.Range Sum1 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, 1];
            Microsoft.Office.Interop.Excel.Range Sum2 = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, dataGridView.Columns.Count + 1];
            Microsoft.Office.Interop.Excel.Range Sum = worksheet.get_Range(Sum1, Sum2);
            //worksheet.Cells[dataGridView.Rows.Count + 2, dataGridView.Columns.Count + 1] = app.WorksheetFunction.Sum(Sum);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
        }

        public void ExportTOTAL_PDF(DataTable dataGridView, string SizePape, string tieude, string ngay)
        {
            iTextSharp.text.Font Times = FontFactory.GetFont("Times");
            // Creating iTextSharp Table from DataTable data
            PdfPTable pdfTable = new PdfPTable(dataGridView.Columns.Count);
            pdfTable.DefaultCell.Padding = 10;
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfTable.DefaultCell.BorderWidth = 1;

            PdfPCell cellName = new PdfPCell(new Phrase(tieude, Times));
            cellName.BackgroundColor = new iTextSharp.text.BaseColor(128, 255, 255);
            cellName.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cellName.Colspan = dataGridView.Columns.Count;
            pdfTable.AddCell(cellName);

            PdfPCell cellNgay = new PdfPCell(new Phrase(ngay, Times));
            cellNgay.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 128);
            cellNgay.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cellNgay.Colspan = dataGridView.Columns.Count;
            pdfTable.AddCell(cellNgay);

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                PdfPCell cellHeader = new PdfPCell(new Phrase(dataGridView.Columns[i].ColumnName, Times));
                cellHeader.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                cellHeader.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                pdfTable.AddCell(cellHeader);
            }

            // Add data
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (dataGridView.Rows[i][j] != null)
                    {
                        PdfPCell cellData = new PdfPCell(new Phrase(dataGridView.Rows[i][j].ToString(), Times));
                        cellData.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                        cellData.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                        pdfTable.AddCell(cellData);
                    }
                }
            }

            // Exporting to PDF
            SaveFileDialog path = new SaveFileDialog();
            path.Title = "Export Setting";
            path.Filter = "Text file (pdf)|*.pdf|All file (*.*)|*.*";

            iTextSharp.text.Rectangle Size = PageSize.A4;
            if (SizePape == "A0") { Size = PageSize.A0; }
            if (SizePape == "A1") { Size = PageSize.A1; }
            if (SizePape == "A2") { Size = PageSize.A2; }
            if (SizePape == "A3") { Size = PageSize.A3; }
            if (SizePape == "A4") { Size = PageSize.A4; }
            if (SizePape == "A5") { Size = PageSize.A5; }

            if (path.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(path.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(Size, 10f, 10f, 10f, 10f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            col = "myId,Ngay"; NuCol = 0;
            var selecteds = TheList.Where(itemS => itemS.IsSelected == true);
            foreach (var obj in selecteds)
            {
                col += "," + obj.TheText;
                NuCol++;
            }

            string CheckTable = "SELECT " + col + " FROM SaveDataHistory" + " where " + " Ngay>='" + ConvertDatePicker(Picker11, "yyyy-MM-dd") + " 00:00' and Ngay<='" + ConvertDatePicker(Picker12, "yyyy-MM-dd") + " 23:59'";
            using (var reader = DataHelper.Instance.ExecuteDataTable(CheckTable))
            {
                tableView = reader;
                DataGrid1.ItemsSource = reader.DefaultView;
                TextBox1.Text = NuCol.ToString();
            }
        }

        private void ButtonPDF_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_PDF(tableView, "A4", "Report", "From " + ConvertDatePicker(Picker11, "yyyy/MM/dd") + " To " + ConvertDatePicker(Picker12, "yyyy/MM/dd"));
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportTOTAL_Excel(tableView, "Report", "Từ " + ConvertDatePicker(Picker11, "yyyy/MM/dd") + " đến " + ConvertDatePicker(Picker12, "yyyy/MM/dd"));
        }
        #endregion

        #region "Biểu đồ"

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            col = "myId,Ngay"; NuCol = 0;
            var selecteds = TheList.Where(itemS => itemS.IsSelected == true);
            foreach (var obj in selecteds)
            {
                col += "," + obj.TheText;
                NuCol++;
            }

            string CheckTable = "SELECT " + col + " FROM SaveDataHistory" + " where " + " Ngay>='" + ConvertDatePicker(Picker51, "yyyy-MM-dd") + " 00:00' and Ngay<='" + ConvertDatePicker(Picker52, "yyyy-MM-dd") + " 23:59'";
            using (var reader = DataHelper.Instance.ExecuteDataTable(CheckTable))
            {
                tableView = reader;
                DataGrid4.ItemsSource = reader.DefaultView;
                TextBox5.Text = NuCol.ToString();
            }
        }

        #endregion
    }
}
