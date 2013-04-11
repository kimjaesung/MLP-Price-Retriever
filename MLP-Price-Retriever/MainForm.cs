using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace MLP_Price_Retriever
{
    public struct MLPEntry
    {
        public string mlpTicker;
        public string mlpName;
        public DateTime mlpStartDate;
        public string mlpType;
    }

    public struct MLPThreadEntry
    {
        public string mlpTicker;
        public string mlpName;
        public DateTime mlpStartDate;
        public string mlpType;
        public MLPThreadState mlpState;
        public string sPrices;
        public string sDistributions;
    }

    public enum MLPThreadState
    {
        Initialized,
        DownloadingPrices,
        PricesReady,
        WritingPrices,
        Complete
    }

    public partial class AppForm : Form
    {
        private const string sUrlStart = "http://ichart.finance.yahoo.com/table.csv?s=";
        private const string sUrlPrice = "g=d";
        private const string sUrlDist = "g=v";
        
        private const int nPriceColStart = 2;
        private const int nPriceRowStart = 12;
        private const int nDistColStart = 11;
        private const int nDistRowStart = 12;

        private volatile Workbook xlWb;
        private volatile List<MLPEntry> pEntries;
        private volatile List<MLPThreadEntry> pThreadEntries;


        public void DownloadThread()
        {
            for (int i = 0; i < pThreadEntries.Count; i++)
            {
                WebClient lclWc = new WebClient();
                MLPThreadEntry lclEntry = pThreadEntries[i];
                lclEntry.mlpState = MLPThreadState.DownloadingPrices;
                
                lock (pThreadEntries)
                {
                    pThreadEntries[i] = lclEntry;
                }
                string status = lclEntry.mlpTicker + " [" + ((stripProgressBar.Value + 1) / 2).ToString() + "/" + pThreadEntries.Count.ToString() + "]";
                UpdateStatus(status + " downloading prices");

                string lclMLPName = lclEntry.mlpTicker;
                DateTime lclMLPStartDate = lclEntry.mlpStartDate;

                lclEntry.sPrices = lclWc.DownloadString(GenUrl(lclMLPName, lclMLPStartDate, DateTime.Today, true));
                lclEntry.sDistributions = lclWc.DownloadString(GenUrl(lclMLPName, lclMLPStartDate, DateTime.Today, false));
                lclEntry.mlpState = MLPThreadState.PricesReady;
                
                lock (pThreadEntries)
                {
                    pThreadEntries[i] = lclEntry;                    
                }
                UpdateStatus(status + " downloading prices completed!");
            }
        }

        public void WriteThread()
        {
            for (int i = 0; i < pThreadEntries.Count; i++)
            {
                while (pThreadEntries[i].mlpState != MLPThreadState.PricesReady) { Thread.Sleep(0); }
                MLPThreadEntry lclEntry = pThreadEntries[i];
                string status = lclEntry.mlpTicker + " [" + ((stripProgressBar.Value + 1) / 2).ToString() + "/" + pThreadEntries.Count.ToString() + "]";
                lclEntry.mlpState = MLPThreadState.WritingPrices;
                lock (pThreadEntries)
                {
                    pThreadEntries[i] = lclEntry;
                }
                UpdateStatus(status + " writing prices to worksheet");

                #region write-to-xl

                Worksheet lclWs = xlWb.Worksheets.Add();
                lclWs.Cells.Style.Font.Size = 8;
                lclWs.Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                lclWs.Name = lclEntry.mlpTicker;

                lclWs.Cells[3, 2].Value2 = "Ticker";
                lclWs.Cells[3, 3].Value2 = "Name";
                lclWs.Cells[3, 4].Value2 = "Sector";
                lclWs.Cells[4, 2].Value2 = lclEntry.mlpTicker;
                lclWs.Cells[4, 3].Value2 = lclEntry.mlpName;
                lclWs.Cells[4, 4].Value2 = lclEntry.mlpType;

                int row = nPriceRowStart;
                int col = nPriceColStart;

                string[] lclPriceRows = lclEntry.sPrices.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string lclPriceRow in lclPriceRows)
                {

                    string[] lclPriceElems = lclPriceRow.Split(new char[] { ',' });
                    foreach (string lclPriceElem in lclPriceElems)
                        lclWs.Cells[row, col++].Value2 = lclPriceElem;
                    col = nPriceColStart;
                    row++;
                    UpdateStatus(status + " wrote " + (row - nPriceRowStart).ToString() + " of " + lclPriceRows.Length.ToString() + " prices");
                }

                    //stripProgressBar.PerformStep();

                row = nDistRowStart;
                col = nDistColStart;

                string[] lclDistRows = lclEntry.sDistributions.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string lclDistRow in lclDistRows)
                {
                    string[] lclDistElems = lclDistRow.Split(new char[] { ',' });
                    foreach (string lclDistElem in lclDistElems)
                        lclWs.Cells[row, col++].Value2 = lclDistElem;
                    col = nDistColStart;
                    row++;
                    UpdateStatus(status + " - wrote " + (row - nDistRowStart).ToString() + " of " + lclDistRows.Length.ToString() + " distributions");
                }

                //stripProgressBar.PerformStep();
                
                #endregion

                // do work

                lclEntry.mlpState = MLPThreadState.Complete;
                lock (pThreadEntries)
                {
                    pThreadEntries[i] = lclEntry;
                }
                UpdateStatus(status + " writing prices to worksheet completed");
            }
        }


        public AppForm()
        {
            InitializeComponent();
        }

        private void UpdateStatus(string inMsg)
        {
            //lock (stripLabel)
            //{
            //    //stripLabel.Text = inMsg.Trim();
            //}
        }

        public void PasteToDGV(Object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (MLPGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
                {
                    DataObject o = MLPGridView.GetClipboardContent();
                    Clipboard.SetDataObject(o);
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                DataObject o = (DataObject)Clipboard.GetDataObject();
                if (o.GetDataPresent(DataFormats.Text))
                {
                    int row = MLPGridView.CurrentCell.RowIndex;
                    string[] selectedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                    if (selectedRows == null || selectedRows.Length == 0)
                        return;
                    foreach (string selectedRow in selectedRows)
                    {
                        if (row >= MLPGridView.Rows.Count)
                            MLPGridView.Rows.Add();
                        try
                        {
                            DataGridViewRow dgvRow = (DataGridViewRow)MLPGridView.Rows[0].Clone();
                            string[] data = Regex.Split(selectedRow, "\t");
                            int col = MLPGridView.CurrentCell.ColumnIndex;

                            foreach (string ob in data)
                            {
                                if (col >= MLPGridView.Columns.Count)
                                    break;
                                if (ob != null)
                                {
                                    
                                    dgvRow.Cells[col].Value = Convert.ChangeType(ob, MLPGridView[col, row].ValueType);

                                    //MLPGridView[col, row].Value = Convert.ChangeType(ob, MLPGridView[col, row].ValueType);
                                }
                                col++;
                            }
                            MLPGridView.Rows.Add(dgvRow);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        row++;
                        MLPGridView.Refresh();
                    }
                }
            }
        }

        private void RightClickOnDGV(Object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MLPGridView.Rows.Clear();
            }
            
        }

        private void GetPricesButton_Click(object sender, EventArgs e)
        {
            MLPGridView.Enabled = false;
            SetPathButton.Enabled = false;
            GetPricesButton.Enabled = false;
            this.Refresh();

            pThreadEntries = new List<MLPThreadEntry>();

            for (int i = 0; i < MLPGridView.RowCount; i++)
            {
                if (Convert.ToString(MLPGridView.Rows[i].Cells["mlpTicker"].Value).Trim() != "")
                {
                    MLPThreadEntry lclEntry = new MLPThreadEntry();
                    lclEntry.mlpName = Convert.ToString(MLPGridView.Rows[i].Cells["mlpName"].Value);
                    lclEntry.mlpTicker = Convert.ToString(MLPGridView.Rows[i].Cells["mlpTicker"].Value);
                    lclEntry.mlpType = Convert.ToString(MLPGridView.Rows[i].Cells["mlpType"].Value);
                    lclEntry.mlpStartDate = Convert.ToDateTime(MLPGridView.Rows[i].Cells["mlpStartDate"].Value);
                    lclEntry.mlpState = MLPThreadState.Initialized;
                    //DateTime.FromOADate(Convert.ToInt32(MLPGridView.Rows[i].Cells["mlpStartDate"].Value));
                    pThreadEntries.Add(lclEntry);
                }
            }

            stripProgressBar.Minimum = 1;
            stripProgressBar.Maximum = pThreadEntries.Count * 2;
            stripProgressBar.Value = 1;
            stripProgressBar.Step = 1;

            UpdateStatus("started");

            Microsoft.Office.Interop.Excel.Application lclExcel = new Microsoft.Office.Interop.Excel.Application();
            xlWb = lclExcel.Workbooks.Add();

            BackgroundWorker bgwDownload = new BackgroundWorker();
            bgwDownload.WorkerReportsProgress = true;
            bgwDownload.ProgressChanged += new ProgressChangedEventHandler(UpdateStatusEvent_Handler);
            bgwDownload.DoWork += new DoWorkEventHandler(DownloadPricesWorker_Handler);

            Thread downloadThread = new Thread(new ThreadStart(DownloadThread));            
            Thread writeThread = new Thread(new ThreadStart(WriteThread));

            downloadThread.Start();
            writeThread.Start();

            while (downloadThread.IsAlive && writeThread.IsAlive)
                Thread.Sleep(0);

            SetPathButton.Enabled = true;

            MLPGridView.Enabled = true;
            GetPricesButton.Enabled = true;
            UpdateStatus("Excel sheet ready to be saved");
        }

        private void DownloadPricesWorker_Handler(Object sender, DoWorkEventArgs e)
        {

        }

        private void UpdateStatusEvent_Handler(Object sender, ProgressChangedEventArgs e)
        {
            stripProgressBar.Value += 1;            
        }
        
        private void OLD_GetPricesButton_Click(object sender, EventArgs e)
        {
            MLPGridView.Enabled = false;
            SetPathButton.Enabled = false;   
            GetPricesButton.Enabled = false;
            this.Refresh();
            
            pEntries = new List<MLPEntry>();

            for (int i = 0; i < MLPGridView.RowCount; i++)
            {
                if (Convert.ToString(MLPGridView.Rows[i].Cells["mlpTicker"].Value).Trim() != "")
                {
                    MLPEntry lclEntry = new MLPEntry();
                    lclEntry.mlpName = Convert.ToString(MLPGridView.Rows[i].Cells["mlpName"].Value);
                    lclEntry.mlpTicker = Convert.ToString(MLPGridView.Rows[i].Cells["mlpTicker"].Value);
                    lclEntry.mlpType = Convert.ToString(MLPGridView.Rows[i].Cells["mlpType"].Value);
                    lclEntry.mlpStartDate = Convert.ToDateTime(MLPGridView.Rows[i].Cells["mlpStartDate"].Value);
                        //DateTime.FromOADate(Convert.ToInt32(MLPGridView.Rows[i].Cells["mlpStartDate"].Value));
                    pEntries.Add(lclEntry);
                }
            }

            stripProgressBar.Minimum = 1;
            stripProgressBar.Maximum = pEntries.Count * 2;
            stripProgressBar.Value = 1;
            stripProgressBar.Step = 1;

            UpdateStatus("started");

            WebClient lclWc = new WebClient();
            Microsoft.Office.Interop.Excel.Application lclExcel = new Microsoft.Office.Interop.Excel.Application();
            xlWb = lclExcel.Workbooks.Add();
            for (int i = xlWb.Worksheets.Count; i >= 2; i--)
                xlWb.Worksheets[i].Delete();
            xlWb.Worksheets[1].Name = "summary";
            xlWb.Worksheets[1].Cells.Style.Font.Size = 8;
            xlWb.Worksheets[1].Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

            foreach (MLPEntry lclEntry in pEntries)
            {
                string status = lclEntry.mlpTicker + " [" + ((stripProgressBar.Value + 1) / 2).ToString() + "/" + pEntries.Count.ToString() + "]";
                UpdateStatus(status);

                string lclMLPName = lclEntry.mlpTicker;
                DateTime lclMLPStartDate = lclEntry.mlpStartDate;

                string lclPrices = lclWc.DownloadString(GenUrl(lclMLPName, lclMLPStartDate, DateTime.Today, true));
                string lclDists = lclWc.DownloadString(GenUrl(lclMLPName, lclMLPStartDate, DateTime.Today, false));

                UpdateStatus(status + " prices downloaded");

                Worksheet lclWs = xlWb.Worksheets.Add();
                lclWs.Cells.Style.Font.Size = 8;
                lclWs.Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                lclWs.Name = lclMLPName;

                lclWs.Cells[3, 2].Value2 = "Ticker";
                lclWs.Cells[3, 3].Value2 = "Name";
                lclWs.Cells[3, 4].Value2 = "Sector";
                lclWs.Cells[4, 2].Value2 = lclEntry.mlpTicker;
                lclWs.Cells[4, 3].Value2 = lclEntry.mlpName;
                lclWs.Cells[4, 4].Value2 = lclEntry.mlpType;

                int row = nPriceRowStart;
                int col = nPriceColStart;

                string[] lclPriceRows = lclPrices.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                
                foreach (string lclPriceRow in lclPriceRows)
                {
                    
                    string[] lclPriceElems = lclPriceRow.Split(new char[] { ',' });
                    foreach (string lclPriceElem in lclPriceElems)
                        lclWs.Cells[row, col++].Value2 = lclPriceElem;
                    col = nPriceColStart;
                    row++;
                    UpdateStatus(status + " wrote " + (row - nPriceRowStart).ToString() + " of " + lclPriceRows.Length.ToString() + " prices");
                }

                stripProgressBar.PerformStep();

                row = nDistRowStart;
                col = nDistColStart;

                string[] lclDistRows = lclDists.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string lclDistRow in lclDistRows)
                {
                    string[] lclDistElems = lclDistRow.Split(new char[] { ',' });
                    foreach (string lclDistElem in lclDistElems)
                        lclWs.Cells[row, col++].Value2 = lclDistElem;
                    col = nDistColStart;
                    row++;
                    UpdateStatus(status + " - wrote " + (row - nDistRowStart).ToString() + " of " + lclDistRows.Length.ToString() + " distributions");
                }

                stripProgressBar.PerformStep();
            }

            //xlWb.SaveAs(@"c:\temp\test.xlsx");
            //xlWb.Close();

            SetPathButton.Enabled = true;

            MLPGridView.Enabled = true;
            GetPricesButton.Enabled = true;
            UpdateStatus("Excel sheet ready to be saved");
        }

        private string GenUrl(string inMLPName, DateTime inStartDate, DateTime inEndDate, bool isPrice)
        {
            return sUrlStart + inMLPName + @"&a=" + (inStartDate.Month - 1).ToString() + @"&b=" + inStartDate.Day.ToString() + @"&c=" + inStartDate.Year.ToString()
                + @"&d=" + (inEndDate.Month - 1).ToString() + @"&e=" + inEndDate.Day.ToString() + @"&f=" + inEndDate.Year.ToString()
                + @"&" + ((isPrice)? sUrlPrice : sUrlDist) + @"&ignore=.csv";
            ////http://ichart.finance.yahoo.com/table.csv?s=ACMP&a=6&b=29&c=2010&d=3&e=8&f=2013&g=v&ignore=.csv
        }

        private void SetPathButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog fsd = new SaveFileDialog();
            fsd.Filter = @"Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            Guid tmpGuid = System.Guid.NewGuid();
            string tmpPath = @"c:\temp\" + tmpGuid.ToString() + @".xlsx";
            try
            {
                if (fsd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (xlWb != null)
                    {
                        xlWb.SaveAs(tmpPath);
                        xlWb.Close();
                    }
                    File.Copy(tmpPath, fsd.FileName, true);
                    File.Delete(tmpPath);
                    UpdateStatus("saved file to " + fsd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            SetPathButton.Enabled = false;
        }
    }
}
