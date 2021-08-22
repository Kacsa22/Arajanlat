using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Arajanlat
{
    public partial class Form1 : Form
    {
        List<Tetel> tetelek = new List<Tetel>();
        List<ExportAdat> adatok = new List<ExportAdat>();
        public Form1()
        {
            InitializeComponent();       
        }

        //Event
        private void hozzaadBtn_Click(object sender, EventArgs e)
        {
            if (hozzaadBtn.Text.Equals("Hozzáadás")){
                if (!(formatumEllenoriz(nettoEgysegarBox, mennyisegBox)))
                {
                    if(!(ertekEllenoriz(nettoEgysegarBox, mennyisegBox)))
                    {
                        tetelek.Add(new Tetel(megnevezesBox.Text, reszletekBox.Text, Convert.ToDouble(nettoEgysegarBox.Text), Convert.ToDouble(mennyisegBox.Text),
                                       mennyisegiEgysegBox.Text, Convert.ToDouble(afaKulcsCbx.Text), tetelekTbl));
                        bemenetTorles();
                        osszesites();
                    }
                }
            }

            if (hozzaadBtn.Text.Equals("Módosítás"))
            {
                int rowIndex = tetelekTbl.CurrentCell.RowIndex;
                if (!(formatumEllenoriz(nettoEgysegarBox, mennyisegBox)))
                {
                    if (!(ertekEllenoriz(nettoEgysegarBox, mennyisegBox)))
                    {
                        tetelek[rowIndex].setMegnevezes(megnevezesBox.Text, rowIndex);
                        tetelek[rowIndex].setReszletek(reszletekBox.Text, rowIndex);
                        tetelek[rowIndex].setEgysegar(Convert.ToDouble(nettoEgysegarBox.Text), rowIndex);
                        tetelek[rowIndex].setMennyiseg(Convert.ToDouble(mennyisegBox.Text), rowIndex);
                        tetelek[rowIndex].setMennyisegiEgyseg(mennyisegiEgysegBox.Text, rowIndex);
                        tetelek[rowIndex].setAfaKulcs(Convert.ToDouble(afaKulcsCbx.Text), rowIndex);
                        bemenetTorles();
                        osszesites();
                    }
                }
            }
        }

        private void szerkesztBtn_Click(object sender, EventArgs e)
        {
            if (tetelekTbl.CurrentCell.RowIndex != -1)
            {
                DataGridViewRow row = tetelekTbl.Rows[tetelekTbl.CurrentCell.RowIndex];
                megnevezesBox.Text = Convert.ToString(row.Cells[0].Value);
                reszletekBox.Text = Convert.ToString(row.Cells[1].Value);
                nettoEgysegarBox.Text = Convert.ToString(row.Cells[2].Value);
                mennyisegBox.Text = Convert.ToString(row.Cells[3].Value);
                mennyisegiEgysegBox.Text = Convert.ToString(row.Cells[4].Value);
                afaKulcsCbx.Text = Convert.ToString(row.Cells[5].Value);

                hozzaadBtn.Text = "Módosítás";
            }
        }

        private void megseBtn_Click(object sender, EventArgs e)
        {
            bemenetTorles();
            hozzaadBtn.Text = "Hozzáadás";
        }

        private void torolBtn_Click(object sender, EventArgs e)
        {
            int rowIndex = tetelekTbl.CurrentCell.RowIndex;
            if (rowIndex != 0)
            {
                DataGridViewRow row = tetelekTbl.Rows[rowIndex];
                tetelekTbl.Rows.Remove(row);
                tetelek.RemoveAt(rowIndex);
                osszesites();
            }
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            if (!datumEllenoriz(dateTimePicker1))
            {
                adatok.Clear();
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                document.Pages[0].Orientation = PdfSharp.PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                exportFejlec(page.Width, page.Height);
                exportTetelek(page.Width, page.Height);
                exportOsszesites(page.Width, page.Height);

                foreach (ExportAdat i in adatok)
                {
                    gfx.DrawString(i.getSzoveg(), i.getFont(), i.getBrush(),
                                    new XRect(i.getX(), i.getY(), page.Width, page.Height), i.getFormat());
                }

                Stream myStream;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                //saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                        // Code to write the stream goes here.
                        String proba = saveFileDialog1.FileName;
                        document.Save(proba);
                }
            }
        }

        public void exportFejlec(Double pageWidth, Double pageHeight)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            XFont fontCim = new XFont("Segoe UI", 30);
            XFont fontAlCim = new XFont("Segoe UI", 18);
            XFont fontSzoveg = new XFont("Segoe UI", 12);
            XFont fontDatum = new XFont("Segoe UI", 12, XFontStyle.Bold);

            Double oszlop1 = 30;
            Double oszlop2 = 110;
            Double oszlop3 = ((pageWidth - 60) / 2) + 30;
            Double oszlop4 = oszlop3 + 80;

            XBrush brushFekete = XBrushes.Black;

            DateTime ma = DateTime.Today;
            DateTime ervenyes = dateTimePicker1.Value;

            adatok.Add(new ExportAdat("Árajánlat", fontCim, brushFekete, 0, 15, XStringFormat.TopCenter));
            adatok.Add(new ExportAdat("Árajánlat adó", fontAlCim, brushFekete, oszlop1, adatok[0].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Árajánlat kérő", fontAlCim, brushFekete, oszlop3, adatok[0].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Cégnév", fontSzoveg, brushFekete, oszlop1, adatok[1].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(cegnevKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[1].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Cégnév", fontSzoveg, brushFekete, oszlop3, adatok[1].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(cegnevBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[1].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Székhely", fontSzoveg, brushFekete, oszlop1, adatok[3].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(szekhelyKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[3].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Székhely", fontSzoveg, brushFekete, oszlop3, adatok[3].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(szekhelyBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[3].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Adószám", fontSzoveg, brushFekete, oszlop1, adatok[7].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(adoszamKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[7].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Adószám", fontSzoveg, brushFekete, oszlop3, adatok[7].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(adoszamBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[7].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Kapcs. tartó", fontSzoveg, brushFekete, oszlop1, adatok[11].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(kapcsKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[11].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Kapcs. tartó", fontSzoveg, brushFekete, oszlop3, adatok[11].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(kapcsBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[11].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Telefon", fontSzoveg, brushFekete, oszlop1, adatok[15].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(telKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[15].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Telefon", fontSzoveg, brushFekete, oszlop3, adatok[15].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(telBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[15].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("E-mail", fontSzoveg, brushFekete, oszlop1, adatok[19].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(mailKiallitoBox.Text, fontSzoveg, brushFekete, oszlop2, adatok[19].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("E-mail", fontSzoveg, brushFekete, oszlop3, adatok[19].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(mailBekeroBox.Text, fontSzoveg, brushFekete, oszlop4, adatok[19].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Árajánlat kelte", fontDatum, brushFekete, oszlop1, adatok[23].getAlja() + 10, XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(ma.ToString("D"), fontDatum, brushFekete, oszlop2 + 20, adatok[23].getAlja() + 10, XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Árajánlat érvényessége", fontDatum, brushFekete, oszlop3, adatok[23].getAlja() + 10, XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(ervenyes.ToString("D"), fontDatum, brushFekete, oszlop4 + 70, adatok[23].getAlja() + 10, XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Tételek", fontAlCim, brushFekete, oszlop1, adatok[27].getAlja() + 20, XStringFormat.TopLeft));
        }

        public void exportTetelek(Double pageWidth, Double pageHeight)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            XFont fontCim = new XFont("Segoe UI", 30);
            XFont fontAlCim = new XFont("Segoe UI", 18);
            XFont fontSzoveg = new XFont("Segoe UI", 12);
            XFont fontReszletek = new XFont("Segoe UI", 10, XFontStyle.Italic);

            XBrush brush = XBrushes.Black;

            Double oszlop1 = 30;
            Double oszlop2 = pageWidth - 30 -100 - 100 - 40 - 100 - 100 - 100 - 100 -30;
            Double oszlop3 = oszlop2 + 100;
            Double oszlop4 = oszlop3 + 100;
            Double oszlop5 = oszlop4 + 40;
            Double oszlop6 = oszlop5 + 100;
            Double oszlop7 = oszlop6 + 100;
            Double oszlop8 = oszlop7 + 100;
            Double oszlopReszletek = 60;
            int utolso = adatok.Count - 1;
            int sorSzam = 0;

            adatok.Add(new ExportAdat("Megnevezés", fontSzoveg, brush, oszlop1, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Nettó egységár", fontSzoveg, brush, oszlop2, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Mennyiség", fontSzoveg, brush, oszlop3, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("M.e.", fontSzoveg, brush, oszlop4, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Áfa kulcs", fontSzoveg, brush, oszlop5, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Nettó érték", fontSzoveg, brush, oszlop6, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Áfa érték", fontSzoveg, brush, oszlop7, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat("Bruttó érték", fontSzoveg, brush, oszlop8, adatok[utolso].getAlja(), XStringFormat.TopLeft));

            foreach (DataGridViewRow row in tetelekTbl.Rows)
            {
                sorSzam++;
                if ((sorSzam % 2) == 0)
                {
                    brush = XBrushes.Black;
                }
                else
                {
                    brush = XBrushes.DarkGray;
                }

                utolso = adatok.Count - 1;
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[0].Value), fontSzoveg, brush, oszlop1, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[2].Value), fontSzoveg, brush, oszlop2, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[3].Value), fontSzoveg, brush, oszlop3, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[4].Value), fontSzoveg, brush, oszlop4, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[5].Value), fontSzoveg, brush, oszlop5, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[6].Value), fontSzoveg, brush, oszlop6, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[7].Value), fontSzoveg, brush, oszlop7, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[8].Value), fontSzoveg, brush, oszlop8, adatok[utolso].getAlja(), XStringFormat.TopLeft));
                adatok.Add(new ExportAdat(Convert.ToString(row.Cells[1].Value), fontReszletek, brush, oszlopReszletek, adatok[utolso+1].getAlja(), XStringFormat.TopLeft));
            }
        }

        public void exportOsszesites(Double pageWidth, Double pageHeight)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            XFont fontSzoveg = new XFont("Segoe UI", 12, XFontStyle.Bold);

            XBrush brush = XBrushes.Black;

            Double oszlop1 = 30;
            Double oszlop2 = pageWidth - 30 - 100 - 100 - 40 - 100 - 100 - 100 - 100 - 30;
            Double oszlop3 = oszlop2 + 100;
            Double oszlop4 = oszlop3 + 100;
            Double oszlop5 = oszlop4 + 40;
            Double oszlop6 = oszlop5 + 100;
            Double oszlop7 = oszlop6 + 100;
            Double oszlop8 = oszlop7 + 100;
            Double oszlopReszletek = 60;
            int utolso = adatok.Count - 1;

            adatok.Add(new ExportAdat("Összesen", fontSzoveg, brush, oszlop1, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(Convert.ToString(nettoErtekOsszesenLbl.Text), fontSzoveg, brush, oszlop6, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(Convert.ToString(afaErtekOsszesenLbl.Text), fontSzoveg, brush, oszlop7, adatok[utolso].getAlja(), XStringFormat.TopLeft));
            adatok.Add(new ExportAdat(Convert.ToString(bruttoErtekOsszesenLbl.Text), fontSzoveg, brush, oszlop8, adatok[utolso].getAlja(), XStringFormat.TopLeft));
        }

        public Boolean datumEllenoriz(DateTimePicker picker)
        {
            Boolean hiba = false;
            TimeSpan t = picker.Value - DateTime.Now;
            if(t.TotalDays <= 0)
            {
                hiba = true;
            }
            return hiba;
        }

        public Boolean formatumEllenoriz(TextBox nettoEgysegarBox, TextBox mennyisegBox)
        {
            Boolean hiba = false;
            return hiba;
        }

        public Boolean ertekEllenoriz(TextBox nettoEgysegarBox, TextBox mennyisegBox)
        {
            Boolean hiba = false;
            return hiba;
        }

        public void bemenetTorles()
        {
            this.megnevezesBox.Text = "";
            this.reszletekBox.Text = "";
            this.nettoEgysegarBox.Text = "";
            this.mennyisegBox.Text = "";
            this.mennyisegiEgysegBox.Text = "";
        }

        public void osszesites()
        {
            Double nettoErtekOsszesen = 0, afaErtekOsszesen = 0, bruttoErtekOsszesen = 0;
            foreach(Tetel i in tetelek)
            {
                nettoErtekOsszesen += i.getNettoErtek();
                afaErtekOsszesen += i.getAfaErtek();
                bruttoErtekOsszesen += i.getBruttoErtek();
            }
            nettoErtekOsszesenLbl.Text = Convert.ToString(nettoErtekOsszesen) + " Ft";
            afaErtekOsszesenLbl.Text = Convert.ToString(afaErtekOsszesen) + " Ft";
            bruttoErtekOsszesenLbl.Text = Convert.ToString(bruttoErtekOsszesen) + " Ft";
        }

        //Class

        public class Tetel
        {
            String megnevezes, reszletek, mennyisegiEgyseg;
            Double egysegar, mennyiseg, nettoErtek, afaErtek, bruttoErtek, afaKulcs;
            DataGridView tabla;
            public Tetel(String megnevezes, String reszletek, Double egysegar, Double mennyiseg, String mennyisegiEgyseg, Double afaKulcs, DataGridView tabla)
            {
                this.megnevezes = megnevezes;
                this.reszletek = reszletek;
                this.egysegar = egysegar;
                this.mennyiseg = mennyiseg;
                this.mennyisegiEgyseg = mennyisegiEgyseg;
                this.afaKulcs = afaKulcs;
                this.nettoErtek = nettoErtekSzamol(this.egysegar, this.mennyiseg);
                this.afaErtek = afaErtekSzamol(this.nettoErtek, this.afaKulcs);
                this.bruttoErtek = bruttoErtekSzamol(this.nettoErtek, this.afaErtek);
                this.tabla = tabla;
                sorHozzaad();
            }

            public Double nettoErtekSzamol(Double egysegAr, Double mennyiseg)
            {
                return kerekit((egysegAr * mennyiseg), 2);
            }

            public Double afaErtekSzamol(Double nettoErtek, Double afaKulcs)
            {
                return kerekit((nettoErtek * (afaKulcs / 100)),2);
            }

            public Double bruttoErtekSzamol(Double nettoErtek, Double afaErtek)
            {
                return kerekit((nettoErtek + afaErtek),2);
            }

            public void sorHozzaad()
            {
                tabla.Rows.Add(this.megnevezes, this.reszletek, tagol(this.egysegar) + " Ft", tagol(this.mennyiseg),
                    this.mennyisegiEgyseg, this.afaKulcs + " %", tagol(this.nettoErtek) + " Ft", tagol(this.afaErtek) + " Ft", tagol(this.bruttoErtek) + " Ft");
            }

            public Double strtoDouble(String str)
            {
                return Convert.ToDouble(str);
            }

            public Double kerekit(Double szam, Double jegy)
            {
                szam = szam * Math.Pow(10, jegy);
                szam = Math.Round(szam);
                szam = szam / Math.Pow(10, jegy);
                return szam;
            }

            public String tagol(Double szam)
            {
                Double egesz = Math.Truncate(szam);
                String tagolt = Convert.ToString(szam);
                if (egesz < 1000)
                {
                    return Convert.ToString(szam);
                }
                else 
                {
                    int i = ((Convert.ToString(egesz).Length) % 3);
                    int hossz = Convert.ToString(egesz).Length;
                    while (i < hossz)
                    {
                        if (i != 0)
                        {
                            tagolt = tagolt.Insert(i, " ");
                            i += 4;
                        }
                        else
                        {
                            i += 3;
                        }
                    }
                    return tagolt;
                }
            }

            //Getters

            public String getMegnevezes()
            {
                return this.megnevezes;
            }

            public Double getEgysegar()
            {
                return this.egysegar;
            }

            public Double getMennyiseg()
            {
                return this.mennyiseg;
            }

            public String getMennyisegiEgyseg()
            {
                return this.mennyisegiEgyseg;
            }

            public Double getNettoErtek()
            {
                return this.nettoErtek;
            }

            public Double getAfaErtek()
            {
                return this.afaErtek;
            }

            public Double getBruttoErtek()
            {
                return this.bruttoErtek;
            }

            //Setters

            public void setMegnevezes(String megnevezes, int index)
            {
                this.megnevezes = megnevezes;
                this.tabla.Rows[index].Cells[0].Value = this.megnevezes;
            }

            public void setReszletek(String reszletek, int index)
            {
                this.reszletek = reszletek;
                this.tabla.Rows[index].Cells[1].Value = this.reszletek;
            }

            public void setEgysegar(Double egysegAr, int index)
            {
                this.egysegar = egysegAr;
                this.tabla.Rows[index].Cells[2].Value = tagol(this.egysegar);
                this.nettoErtek = nettoErtekSzamol(this.egysegar, this.mennyiseg);
                this.afaErtek = afaErtekSzamol(this.nettoErtek, this.afaKulcs);
                this.bruttoErtek = bruttoErtekSzamol(this.nettoErtek, this.afaErtek);
                this.tabla.Rows[index].Cells[6].Value = tagol(this.nettoErtek);
                this.tabla.Rows[index].Cells[7].Value = tagol(this.afaErtek);
                this.tabla.Rows[index].Cells[8].Value = tagol(this.bruttoErtek);
            }
            public void setMennyiseg(Double mennyiseg, int index)
            {
                this.mennyiseg = mennyiseg;
                this.tabla.Rows[index].Cells[3].Value = tagol(this.mennyiseg);
                this.nettoErtek = nettoErtekSzamol(this.egysegar, this.mennyiseg);
                this.afaErtek = afaErtekSzamol(this.nettoErtek, this.afaKulcs);
                this.bruttoErtek = bruttoErtekSzamol(this.nettoErtek, this.afaErtek);
                this.tabla.Rows[index].Cells[6].Value = tagol(this.nettoErtek);
                this.tabla.Rows[index].Cells[7].Value = tagol(this.afaErtek);
                this.tabla.Rows[index].Cells[8].Value = tagol(this.bruttoErtek);
            }

            public void setMennyisegiEgyseg(String mennyisegiEgyseg, int index)
            {
                this.mennyisegiEgyseg = mennyisegiEgyseg;
                this.tabla.Rows[index].Cells[4].Value = this.mennyisegiEgyseg;
            }

            public void setAfaKulcs(Double afaKulcs, int index)
            {
                this.afaKulcs = afaKulcs;
                this.tabla.Rows[index].Cells[5].Value = this.afaKulcs;
                this.nettoErtek = nettoErtekSzamol(this.egysegar, this.mennyiseg);
                this.afaErtek = afaErtekSzamol(this.nettoErtek, this.afaKulcs);
                this.bruttoErtek = bruttoErtekSzamol(this.nettoErtek, this.afaErtek);
                this.tabla.Rows[index].Cells[6].Value = tagol(this.nettoErtek);
                this.tabla.Rows[index].Cells[7].Value = tagol(this.afaErtek);
                this.tabla.Rows[index].Cells[8].Value = tagol(this.bruttoErtek);
            }

        }

        public class ExportAdat
        {
            String szoveg;
            XFont font;
            XBrush brush;
            Double x, y;
            XStringFormat format;
            Double alja;

            public ExportAdat(String szoveg, XFont font, XBrush brush, Double x, Double y, XStringFormat format)
            {
                this.szoveg = szoveg;
                this.font = font;
                this.brush = brush;
                this.x = x;
                this.y = y;
                this.format = format;
                this.alja = y + (this.font.Height * 1.2);
            }

            public String getSzoveg()
            {
                return this.szoveg;
            }

            public XFont getFont()
            {
                return this.font;
            }

            public XBrush getBrush()
            {
                return this.brush;
            }

            public Double getX()
            {
                return this.x;
            }

            public Double getY()
            {
                return this.y;
            }

            public XStringFormat getFormat()
            {
                return this.format;
            }

            public Double getAlja()
            {
                return this.alja;
            }
        }
    }
}
