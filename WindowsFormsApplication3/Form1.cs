using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        TableContext db = new TableContext();
        private List<Table> table;

        //link to WikiPedia
        private string baseUrl = "https://ru.wikipedia.org";

        //List of elements for each group of elements
        private List<string> halfMetal = new List<string>() { "B", "Si", "Ge", "As", "Sb", "Te", "Po" };
        private List<string> noMetal = new List<string>() { "H", "C", "N", "O", "P", "S", "Se" };
        private List<string> semiconductor = new List<string>() { "Al", "Ga", "In", "Sn", "Tl", "Pb", "Bi" };
        private List<string> halogen = new List<string>() { "F", "Cl", "Br", "I", "At" };
        private List<string> inertGase = new List<string>() { "He", "Ne", "Ar", "Kr", "Xe", "Rn" };
        private List<string> alkaline = new List<string>() { "Li", "Na", "K", "Rb", "Cs", "Fr" };
        private List<string> alkalineEarth = new List<string>() { "Be", "Mg", "Ca", "Sr", "Ba", "Ra" };
        private List<string> transitional = new List<string>() { "Sc", "Ti", "V", "Cr" ,"Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg" };
        private List<string> lanthanide = new List<string>() { "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu" };
        private List<string> actinide = new List<string>() { "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr" };
        public Form1()
        {
            
            //Function to get all elements in Mendeleev table and then write to DataBase
            //Call once 

            //GetElementFromPage();
            
            //Get data from DataBase

            table = db.Table.ToList();
            InitializeComponent();

            //Add style for our table  
            AddBorder();


            //For each button add function, which show all information about element 
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null && b.Name!="Made")
                {
                    b.Click +=
                        (object sender, EventArgs e) =>
                        {
                            //Find elements, which was clicked
                            Table temp = FindElement(b.Text);
                            // If element found, show information
                            if (temp != null)
                            {
                                Name1.Text = temp.Name;
                                Make.Text = temp.FullName;
                                Number.Text = temp.Id.ToString();
                                Mass.Text = temp.AtomicMass.ToString();
                                textBox1.Text = temp.About.ToString();
                                if (temp.Valence==null)
                                {
                                    label31.Text = "Не известная";
                                }
                                else
                                {
                                    label31.Text = temp.Valence;
                                }
                                label33.Text = temp.Configuration;
                            }
                            //If element not, show message with problem
                            else
                            {
                                textBox1.Text = "Этот элемент наша программа ещё не изучила(" + "\n";
                                Name1.Text = "";
                                Make.Text = "";
                                Number.Text = "";
                                Mass.Text = "";
                                label31.Text = "";
                                label33.Text = "";
                            }
                        };
                }
            }
        }
        //
        public Table FindElement(string name)
        {
            //Search element in DataBase
            Table first = table.Where(p => p.Name.ToString().Replace(" ", string.Empty) == name).FirstOrDefault();
            return first;
        }

        //Add style for button
        public void AddBorder()
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    b.Cursor = System.Windows.Forms.Cursors.Hand;
                    b.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    b.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                }
            }
        }

        //Function to get Mendeleev table from WikiPedia
        public void GetElementFromPage()
        {
            //Regex to find atomiac mass
            Regex regex = new Regex("([0-9]*[,])?[0-9]+");
            
            //Mendellev table page
            string Url = "https://ru.wikipedia.org/wiki/%D0%9F%D0%B5%D1%80%D0%B8%D0%BE%D0%B4%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B0%D1%8F_%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D0%B0_%D1%85%D0%B8%D0%BC%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D1%85_%D1%8D%D0%BB%D0%B5%D0%BC%D0%B5%D0%BD%D1%82%D0%BE%D0%B2";
           
            HtmlWeb web = new HtmlWeb();

            HtmlAgilityPack.HtmlDocument doc = web.Load(Url);

            //Get data for main group of elements 
            for (int i = 2; i < 9; i++)
            {
                for (int j = 1; j < 19; j++)
                {
                    GetOneElement(i, j, doc, regex);
                }
            }
             
            //Get data for special group of elements
            for (int i = 11; i < 13; i++)
            {
                for (int j = 1; j < 16; j++)
                {
                    GetOneElement(i, j, doc, regex);
                }
            }
        }
        private void GetOneElement(int i,int j,HtmlAgilityPack.HtmlDocument doc, Regex regex)
        {
            //Get current element from table
            var currentElement = doc.DocumentNode.SelectNodes("//*[@class=\"standard\"]").First().ChildNodes.Where(p => p.Name == "tr").ToArray()[i].ChildNodes.Where(p => p.Name == "td").ToArray()[j];
            int num;
            //check to get cell with element
            var number = Int32.TryParse(currentElement.InnerText.Split(new char[] { '\n' })[0], out num);
            if (number)
            {
                Table current = new Table();
                var linkToFullPageAboutElement = currentElement.ChildNodes.Where(p => p.Name == "a").First().Attributes["href"].Value;
                string newUrl = baseUrl + linkToFullPageAboutElement;
                var href = currentElement.InnerText.Split(new char[] { '\n' });
                HtmlWeb web1 = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc1 = web1.Load(newUrl);
                var informationAboutElement = doc1.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/p").First().InnerText.ToString() + doc1.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/p").Skip(1).First().InnerText.ToString();
                var a = doc1.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div/table").First().ChildNodes.Where(p => p.Name == "tr").ToArray()[1].ChildNodes.Where(p => p.Name == "td").First().InnerText;
                var configurationFromWikipedia = doc1.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div/table").First().ChildNodes.Where(p => p.Name == "tr").ToArray()[2].ChildNodes.Where(p => p.Name == "td").First().InnerHtml;
                var cleanConfiguration = configurationFromWikipedia.Replace("</sup>", " ").Replace("<sup>", "^");

                current.Id = Convert.ToInt32(href[0]);
                current.Name = href[1];
                current.FullName = href[2];
                current.About = informationAboutElement.Replace("&#160;", "");
                current.Configuration = cleanConfiguration;
                Match match = regex.Match(a);
                var v = match.ToString();
                var b = Convert.ToDouble(match.ToString());
                current.AtomicMass = Convert.ToDouble(match.ToString());

                //Check element in DataBase
                if (db.Table.Where(p => p.Id == current.Id).FirstOrDefault() == null)
                {
                    db.Table.Add(current);
                    db.SaveChanges();
                }
                else
                {
                    var currElement = db.Table.Where(p => p.Id == current.Id).FirstOrDefault();
                    currElement.Configuration = cleanConfiguration;
                    db.Entry<Table>(currElement).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        //Make some style for some group of elements
        private void label23_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (halfMetal.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.DarkSeaGreen;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }
        //Return style to initial state
        private void label23_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }

        //Make some style for some group of elements
        private void label22_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (noMetal.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Lime;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label22_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }

        private void label20_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (halogen.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Turquoise;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }
        private void label20_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }

        private void label21_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (inertGase.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.DeepSkyBlue;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label21_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }

        private void label24_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (alkaline.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Orange;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label24_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }
        private void label26_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (alkalineEarth.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Yellow;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label26_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }
        private void label27_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (transitional.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Thistle;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }
                }
            }
        }

        private void label27_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }
        private void label28_MouseHover(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (semiconductor.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.LightSeaGreen;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label28_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }
        private void label29_MouseHover(object sender, EventArgs e)
        {

            link1.BackColor = System.Drawing.Color.DarkOrange;
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {

                    if (lanthanide.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.DarkOrange;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link2.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label29_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }
        private void label30_MouseHover(object sender, EventArgs e)
        {

            link2.BackColor = System.Drawing.Color.Plum;
            foreach (var item in this.Controls)
            {
                Button b = item as Button;
                if (b != null)
                {
                    if (actinide.Contains(b.Text))
                    {
                        b.BackColor = System.Drawing.Color.Plum;
                    }
                    else
                    {
                        b.BackColor = System.Drawing.Color.DarkGray;
                        link1.BackColor = System.Drawing.Color.DarkGray;
                    }

                }
            }
        }

        private void label30_MouseLeave(object sender, EventArgs e)
        {
            DrawButtonBack();
        }

        //Use for return to initial style in table 
        private void DrawButtonBack()
        {
            this.button1.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button2.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button3.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button4.BackColor = System.Drawing.Color.Gold;
            this.button5.BackColor = System.Drawing.Color.Gold;
            this.button6.BackColor = System.Drawing.Color.Gold;
            this.button7.BackColor = System.Drawing.Color.Gold;
            this.button8.BackColor = System.Drawing.Color.Gold;
            this.button9.BackColor = System.Drawing.Color.Gold;
            this.button10.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button12.BackColor = System.Drawing.Color.Gold;
            this.button13.BackColor = System.Drawing.Color.Gold;
            this.button14.BackColor = System.Drawing.Color.Gold;
            this.button15.BackColor = System.Drawing.Color.Gold;
            this.button16.BackColor = System.Drawing.Color.Gold;
            this.button17.BackColor = System.Drawing.Color.Gold;
            this.button18.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button19.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button20.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button21.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button22.BackColor = System.Drawing.Color.DodgerBlue;
            this.button23.BackColor = System.Drawing.Color.DodgerBlue;
            this.button24.BackColor = System.Drawing.Color.DodgerBlue;
            this.button25.BackColor = System.Drawing.Color.DodgerBlue;
            this.button26.BackColor = System.Drawing.Color.DodgerBlue;
            this.button27.BackColor = System.Drawing.Color.DodgerBlue;
            this.button28.BackColor = System.Drawing.Color.DodgerBlue;
            this.button29.BackColor = System.Drawing.Color.DodgerBlue;
            this.button30.BackColor = System.Drawing.Color.DodgerBlue;
            this.button31.BackColor = System.Drawing.Color.DodgerBlue;
            this.button32.BackColor = System.Drawing.Color.Gold;
            this.button33.BackColor = System.Drawing.Color.Gold;
            this.button34.BackColor = System.Drawing.Color.Gold;
            this.button35.BackColor = System.Drawing.Color.DodgerBlue;
            this.button36.BackColor = System.Drawing.Color.Gold;
            this.button37.BackColor = System.Drawing.Color.Gold;
            this.button38.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button39.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button40.BackColor = System.Drawing.Color.DodgerBlue;
            this.button41.BackColor = System.Drawing.Color.DodgerBlue;
            this.button42.BackColor = System.Drawing.Color.DodgerBlue;
            this.button43.BackColor = System.Drawing.Color.DodgerBlue;
            this.button44.BackColor = System.Drawing.Color.DodgerBlue;
            this.button45.BackColor = System.Drawing.Color.Gold;
            this.button46.BackColor = System.Drawing.Color.DodgerBlue;
            this.button47.BackColor = System.Drawing.Color.DodgerBlue;
            this.button48.BackColor = System.Drawing.Color.DodgerBlue;
            this.button49.BackColor = System.Drawing.Color.DodgerBlue;
            this.button50.BackColor = System.Drawing.Color.Gold;
            this.button51.BackColor = System.Drawing.Color.DodgerBlue;
            this.button52.BackColor = System.Drawing.Color.DodgerBlue;
            this.button53.BackColor = System.Drawing.Color.DodgerBlue;
            this.button54.BackColor = System.Drawing.Color.DodgerBlue;
            this.button55.BackColor = System.Drawing.Color.YellowGreen;
            this.button56.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button57.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button58.BackColor = System.Drawing.Color.Gold;
            this.button59.BackColor = System.Drawing.Color.Gold;
            this.button60.BackColor = System.Drawing.Color.Gold;
            this.button61.BackColor = System.Drawing.Color.Gold;
            this.button62.BackColor = System.Drawing.Color.Gold;
            this.button63.BackColor = System.Drawing.Color.DodgerBlue;
            this.button64.BackColor = System.Drawing.Color.DodgerBlue;
            this.button65.BackColor = System.Drawing.Color.DodgerBlue;
            this.button66.BackColor = System.Drawing.Color.DodgerBlue;
            this.button67.BackColor = System.Drawing.Color.Gold;
            this.button68.BackColor = System.Drawing.Color.Gold;
            this.button69.BackColor = System.Drawing.Color.Gold;
            this.button70.BackColor = System.Drawing.Color.Gold;
            this.button71.BackColor = System.Drawing.Color.Gold;
            this.button72.BackColor = System.Drawing.Color.Gold;
            this.button73.BackColor = System.Drawing.Color.DodgerBlue;
            this.button74.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button75.BackColor = System.Drawing.Color.PaleVioletRed;
            this.button76.BackColor = System.Drawing.Color.OliveDrab;
            this.button77.BackColor = System.Drawing.Color.DodgerBlue;
            this.button78.BackColor = System.Drawing.Color.DodgerBlue;
            this.button79.BackColor = System.Drawing.Color.DodgerBlue;
            this.button80.BackColor = System.Drawing.Color.DodgerBlue;
            this.button81.BackColor = System.Drawing.Color.DodgerBlue;
            this.button82.BackColor = System.Drawing.Color.DodgerBlue;
            this.button83.BackColor = System.Drawing.Color.DodgerBlue;
            this.button84.BackColor = System.Drawing.Color.YellowGreen;
            this.button85.BackColor = System.Drawing.Color.YellowGreen;
            this.button86.BackColor = System.Drawing.Color.YellowGreen;
            this.button87.BackColor = System.Drawing.Color.YellowGreen;
            this.button88.BackColor = System.Drawing.Color.YellowGreen;
            this.button89.BackColor = System.Drawing.Color.YellowGreen;
            this.button90.BackColor = System.Drawing.Color.YellowGreen;
            this.button91.BackColor = System.Drawing.Color.YellowGreen;
            this.button92.BackColor = System.Drawing.Color.YellowGreen;
            this.button93.BackColor = System.Drawing.Color.YellowGreen;
            this.button94.BackColor = System.Drawing.Color.OliveDrab;
            this.button95.BackColor = System.Drawing.Color.OliveDrab;
            this.button96.BackColor = System.Drawing.Color.OliveDrab;
            this.button97.BackColor = System.Drawing.Color.OliveDrab;
            this.button98.BackColor = System.Drawing.Color.OliveDrab;
            this.button99.BackColor = System.Drawing.Color.OliveDrab;
            this.button100.BackColor = System.Drawing.Color.OliveDrab;
            this.button101.BackColor = System.Drawing.Color.OliveDrab;
            this.button102.BackColor = System.Drawing.Color.OliveDrab;
            this.button103.BackColor = System.Drawing.Color.OliveDrab;
            this.button104.BackColor = System.Drawing.Color.OliveDrab;
            this.button105.BackColor = System.Drawing.Color.OliveDrab;
            this.button106.BackColor = System.Drawing.Color.OliveDrab;
            this.button107.BackColor = System.Drawing.Color.OliveDrab;
            this.button108.BackColor = System.Drawing.Color.YellowGreen;
            this.button109.BackColor = System.Drawing.Color.YellowGreen;
            this.button110.BackColor = System.Drawing.Color.YellowGreen;
            this.button111.BackColor = System.Drawing.Color.YellowGreen;
            link1.BackColor = System.Drawing.Color.YellowGreen;
            link2.BackColor = System.Drawing.Color.OliveDrab;
        }

        private void Made_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }

        private void label32_Click(object sender, EventArgs e)
        {

        }
    }
}
   