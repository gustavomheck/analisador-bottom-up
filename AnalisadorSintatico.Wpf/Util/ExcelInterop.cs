using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace AnalisadorSintatico.Wpf.Util
{
    public class ExcelInterop : IDisposable
    {
        private Excel.Application _xlApp;
        private Excel.Workbook _xlWorkbook;
        private Excel.Range _xlRange;
        private Excel._Worksheet _xlWorksheet;
        private List<List<string>> _tabelaLSR;

        public List<List<string>> LerTabela(string caminho)
        {
            _xlApp = new Excel.Application();
            _xlWorkbook = _xlApp.Workbooks.Open(caminho);
            _xlWorksheet = _xlWorkbook.Sheets[1];
            _xlRange = _xlWorksheet.UsedRange;
            _tabelaLSR = new List<List<string>>();

            int rowCount = _xlRange.Rows.Count;
            int colCount = _xlRange.Columns.Count;

            for (int i = 2; i <= rowCount; i++)
            {
                _tabelaLSR.Add(new List<string>());

                for (int j = 2; j <= colCount; j++)
                {
                    if (_xlRange.Cells[i, j] != null && _xlRange.Cells[i, j].Value2 != null)
                    {
                        _tabelaLSR[_tabelaLSR.Count - 1].Add(_xlRange.Cells[i, j].Value2.ToString());
                    }
                    else
                    {
                        _tabelaLSR[_tabelaLSR.Count - 1].Add("");
                    }
                }
            }

            return _tabelaLSR;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(_xlRange);
            Marshal.ReleaseComObject(_xlWorksheet);
            _xlWorkbook.Close();
            Marshal.ReleaseComObject(_xlWorkbook);
            _xlApp.Quit();
            Marshal.ReleaseComObject(_xlApp);
        }
    }
}
