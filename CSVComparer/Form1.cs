using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSVReaderWriter;
namespace CSVComparer {
   public partial class Form1 : Form {
      private string CSVFilePath1;
      private string CSVFilePath2;
      private List<string> CSVFile1Headers;
      private List<string> CSVFile2Headers;
      private List<List<string>> RecordsOfCSVFile1;
      private List<List<string>> RecordsOfCSVFile2;
      private string CSVOutputFile;
      public Form1(string args) {
         InitializeComponent();
         if (!string.IsNullOrEmpty(args)) {
            var files = args.ToString().Split(',');
            CSVFilePath1 = files[0].ToString();
            CSVFilePath2 = files[1].ToString();
            CSVOutputFile = files[2].ToString();
            button2_Click(null, null);
            button3_Click(null, null);
            button1_Click(null, null);
         }
      }
      public Form1() {
         InitializeComponent();
      }

      /// <summary>
      /// Upload First CSV File
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button2_Click(object sender, EventArgs e) {
         try {
            label2.Visible = false;
            string filePath = string.Empty;
            if (string.IsNullOrEmpty(CSVFilePath1)) {
               OpenFileDialog openFileD1 = new OpenFileDialog();
               openFileD1.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*;";// "CSV files (*.csv)";
               openFileD1.Multiselect = false;
               if (openFileD1.ShowDialog() == DialogResult.OK) {
                  label1.Visible = true;
                  string ext = Path.GetExtension(openFileD1.FileName.Trim());
                  if (ext.ToLower() != ".csv") {
                     MessageBox.Show("Please upload a CSV file");
                     return;
                  }
                  filePath = openFileD1.FileName.Trim();
               }
               else {
                  return;
               }
            }
            else {
               filePath = CSVFilePath1;
            }
            using (CsvReader reader = new CsvReader(filePath, Encoding.Default)) {
               RecordsOfCSVFile1 = new List<List<string>>();
               while (reader.ReadNextRecord())
                  RecordsOfCSVFile1.Add(reader.Fields);
               if (RecordsOfCSVFile1.Count > 1) {
                  CsvFile csvFile = CreateCsvFile(RecordsOfCSVFile1[0], RecordsOfCSVFile1[1]);
                  CSVFile1Headers = csvFile.Headers;
               }
            }
            if (RecordsOfCSVFile1.Count > 0) {
               lblTRecords1.Text = (RecordsOfCSVFile1.Count - 1).ToString();
               DataTable dtTable = ConvertListToDataTable(RecordsOfCSVFile1.Skip(1).ToList());
               dataGridView1.DataSource = dtTable.DefaultView;
            }
            label1.Text = "Processed";
            label1.Visible = true;
         }
         catch (Exception ex) {
         }
      }

      /// <summary>
      /// Upload Second CSV File
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button3_Click(object sender, EventArgs e) {
         try {
            label2.Visible = false;
            string filePath = string.Empty;
            if (string.IsNullOrEmpty(CSVFilePath2)) {
               OpenFileDialog openFileD1 = new OpenFileDialog();
               openFileD1.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*;";// "CSV files (*.csv)";
               openFileD1.Multiselect = false;
               if (openFileD1.ShowDialog() == DialogResult.OK) {
                  string ext = Path.GetExtension(openFileD1.FileName.Trim());
                  if (ext.ToLower() != ".csv") {
                     MessageBox.Show("Please upload a CSV file");
                     return;
                  }
                  filePath = openFileD1.FileName.Trim();
               }
               else {
                  return;
               }
            }
            else {
               filePath = CSVFilePath2;
            }

            label2.Visible = true;
            using (CsvReader reader = new CsvReader(filePath, Encoding.Default)) {
               RecordsOfCSVFile2 = new List<List<string>>();
               while (reader.ReadNextRecord())
                  RecordsOfCSVFile2.Add(reader.Fields);
               if (RecordsOfCSVFile2.Count > 1) {
                  CsvFile csvFile = CreateCsvFile(RecordsOfCSVFile2[0], RecordsOfCSVFile2[1]);
                  CSVFile2Headers = csvFile.Headers;
               }
            }

            if (RecordsOfCSVFile2.Count > 0) {
               lblTRecords2.Text = (RecordsOfCSVFile2.Count - 1).ToString();
               DataTable dtTable = ConvertListToDataTable(RecordsOfCSVFile2.Skip(1).ToList());
               dataGridView2.DataSource = dtTable.DefaultView;
            }
            label2.Text = "Processed";
         }
         catch (Exception ex) {

         }
      }

      /// <summary>
      /// Create a new CSV file from the result
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button1_Click(object sender, EventArgs e) {
         try {
            List<string> diffOfHeaders = new List<string>();
            if (CSVFile2Headers.Count <= CSVFile1Headers.Count)
               diffOfHeaders = CSVFile1Headers.Except(CSVFile2Headers).ToList();
            else
               diffOfHeaders = CSVFile2Headers.Except(CSVFile1Headers).ToList();

            List<List<string>> results = new List<List<string>>();

            if (RecordsOfCSVFile1.Count <= RecordsOfCSVFile2.Count) {
               for (int i = 0; i <= RecordsOfCSVFile2.Count - 1; i++) {
                  if (i <= RecordsOfCSVFile1.Count - 1) {
                     for (int k = 0; k <= RecordsOfCSVFile2[i].Count - 1; k++) {
                        if (RecordsOfCSVFile2[i][k] != RecordsOfCSVFile1[i][k]) {
                           results.Add(RecordsOfCSVFile2[i]);
                        }
                     }
                  }
                  else {
                     results.Add(RecordsOfCSVFile2[i]);
                  }
               }
            }

            /*
            if (RecordsOfCSVFile2.Count <= RecordsOfCSVFile1.Count) {
               for (int i = 0; i <= RecordsOfCSVFile1.Count - 1; i++) {
                  if (i < RecordsOfCSVFile2.Count) {
                     var resultOfRecords = RecordsOfCSVFile2[i].Except(RecordsOfCSVFile1[i]).Union(RecordsOfCSVFile1[i].Except(RecordsOfCSVFile2[i])).ToList();
                     if (resultOfRecords.Count > 0) {
                        results.Add(RecordsOfCSVFile1[i]);
                     }
                  }
                  else {
                     results.Add(RecordsOfCSVFile1[i]);
                  }
               }

               for (int i = 0; i <= RecordsOfCSVFile2.Count - 1; i++) {
                  if (i < RecordsOfCSVFile1.Count) {
                     var resultOfRecords = RecordsOfCSVFile1[i].Except(RecordsOfCSVFile2[i]).ToList();
                     if (resultOfRecords.Count > 0) {
                        results.Add(RecordsOfCSVFile2[i]);
                     }
                  }
                  else {
                     results.Add(RecordsOfCSVFile2[i]);
                  }
               }
            }
            else {
               for (int i = 0; i <= RecordsOfCSVFile2.Count - 1; i++) {
                  if (i < RecordsOfCSVFile1.Count) {
                     var resultOfRecords = RecordsOfCSVFile2[i].Except(RecordsOfCSVFile1[i]).ToList();
                     if (resultOfRecords.Count > 0) {
                        results.Add(RecordsOfCSVFile1[i]);
                     }
                  }
                  else {
                     results.Add(RecordsOfCSVFile2[i]);

                  }
               }
            }
            */
            if (results.Count == 0) {
               MessageBox.Show("There is no difference between the two files being compared");
               return;
            }
            DataTable dtDifferenceTable = ConvertListToDataTable(results);
            dataGridView3.DataSource = dtDifferenceTable.DefaultView;
            CsvWriter cw = new CsvWriter();

            string fileName = CSVOutputFile;
            //var t = new Thread((ThreadStart)(() =>
            //{
            //   SaveFileDialog saveFileDialogue = new SaveFileDialog();
            //   saveFileDialogue.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*;";// "CSV files (*.csv)";
            //   if (saveFileDialogue.ShowDialog() == DialogResult.Cancel)
            //      return;

            //   fileName = saveFileDialogue.FileName;
            //}));

            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();
            //t.Join();
            if (!string.IsNullOrEmpty(fileName)) {
               cw.WriteCsv(dtDifferenceTable, fileName, Encoding.Default);
               MessageBox.Show("Data Exported");
            }
         }
         catch (Exception) {
            throw;
         }
      }

      private CsvFile CreateCsvFile(List<string> headers, List<string> fields) {
         CsvFile csvFile = new CsvFile();
         headers.ForEach(header => csvFile.Headers.Add(header));
         CsvRecord record = new CsvRecord();
         fields.ForEach(field => record.Fields.Add(field));
         csvFile.Records.Add(record);
         return csvFile;
      }

      private DataTable ConvertListToDataTable(List<List<string>> list) {
         // New table.
         DataTable table = new DataTable();
         table.TableName = "CSVTable";
         // Get max columns.
         int columns = 0;
         columns = list[0].Count;

         // Add columns.
         for (int i = 0; i <= CSVFile1Headers.Count - 1; i++) {
            table.Columns.Add(new DataColumn(CSVFile1Headers[i].ToString()));
         }

         // Add rows.
         foreach (List<string> listString in list) {
            object[] prams = new object[list[0].Count];
            for (int i = 0; i <= CSVFile1Headers.Count - 1; i++) {
               prams[i] = listString[i].ToString();
            }
            table.Rows.Add(prams);
         }
         return table;
      }
   }
}
