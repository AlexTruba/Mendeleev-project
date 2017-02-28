using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form2 : Form
    {
        private List<Table> table = new List<Table>();
        public Form2()
        {
            InitializeComponent();
            using (TableContext db = new TableContext())
            {
                table = db.Table.ToList();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Check for send empty text
            if (textBox1.Text!= null)
            {
                //Try to find element in DataBase
                Table temp = table.Where(x => x.Name.ToLower() == textBox1.Text.ToLower()).FirstOrDefault();
                //If found, go next
                if (temp!=null)
                {
                    var b = temp.Valence;
                    //Try to calculate oxide 
                    if (temp.Valence!= null)
                    {
                        //Oxygen already used in oxide
                        if (temp.Name == "O") result.Text = "Этот элемент уже используется в оксиде!";
                        //Elements with 0 valence can not make oxide and fluorine also
                        else if (temp.Valence == "0" || temp.Name=="F") result.Text = "Невозможно создать такой оксид ";
                        //make oxide
                        else result.Text =  GetAllElements(temp.Valence, temp.Name);
                    }
                    else
                    {
                        result.Text = "Програма ещё не достаточно обучена!";
                    }
                }
                else
                {
                    result.Text = "Элемент не найден!";
                }
            }
            else
            {
                result.Text = "Введите элемент!";
            }
        }
        //send line with valence and name of element
        private string GetAllElements(string input,string name)
        {
            string result = "";
            //Gets all valence for this element
            var numbers = input.Split(new char[] { ',' }).Select(p =>p.Replace(" ",string.Empty)).ToList();
            //Make all possible oxide for current element 
            foreach (string item in numbers)
            {
                switch (item)
                {
                    case "1":
                        result += name + "\x2082" + "O" + "  ";
                        break;
                    case "2":
                        result += name + "O" + "  ";
                        break;
                    case "3":
                        result += name + "\x2082" + "O\x2083" + "  ";
                        break;
                    case "4":
                        result += name + "O\x2082" + "  ";
                        break;
                    case "5":
                        result += name + "\x2082" + "O\x2085" + "  ";
                        break;
                    case "6":
                        result += name + "O\x2083" + "  ";
                        break;
                    case "7":
                        result += name + "\x2082" + "O\x2087" + "  ";
                        break;
                    case "8":
                        result += name + "O\x2084" + "  ";;
                        break;
                }
            }
            return result;
        }

    }
}
