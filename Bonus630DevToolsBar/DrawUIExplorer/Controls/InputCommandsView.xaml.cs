using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Controls
{
    /// <summary>
    /// Interaction logic for InputCommandsView.xaml
    /// </summary>
    public partial class InputCommandsView : UserControl
    {
        System.Windows.Forms.AutoCompleteStringCollection autoCompleteStringCollection;
        List<string> userCommands;
        int currentCommandIndex = 0;

        public string Text { get { return txt_inputCommandResult.Text; } set { txt_inputCommandResult.Text = value; } }
        private Core core;
        public void Core(Core core)
        {
            this.core = core;
            core.InCorelChanged += (v) => { autoCompleteInputCommand(); };
        }
        public InputCommandsView()
        {
            InitializeComponent();
            txt_formInputCommand.TabStop = false;
            txt_formInputCommand.KeyUp += TextBox_KeyUp;
            txt_formInputCommand.KeyDown += TextBox_KeyDown;
            //txt_formInputCommand.Click += Txt_formInputCommand_Click;
           
            txt_formInputCommand.GotFocus += Txt_formInputCommand_GotFocus;
            txt_formInputCommand.LostFocus += Txt_formInputCommand_LostFocus;
            txt_formInputCommand.PreviewKeyDown += Txt_formInputCommand_PreviewKeyDown;
        
            autoCompleteInputCommand();
            userCommands = new List<string>();

        }
        private void autoCompleteInputCommand()
        {
            autoCompleteStringCollection = new System.Windows.Forms.AutoCompleteStringCollection();
            MethodInfo[] m = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < m.Length; i++)
            {
                string input = m[i].Name;
                ParameterInfo[] parameters = m[i].GetParameters();
                for (int j = 0; j < parameters.Length; j++)
                {
                    input += " [" + parameters[j].Name + "]";
                }
                autoCompleteStringCollection.Add(input);
            }
            this.Dispatcher.Invoke(new Action(() =>
            {
                txt_formInputCommand.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
                txt_formInputCommand.AutoCompleteCustomSource = autoCompleteStringCollection;
                txt_formInputCommand.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                txt_formInputCommand.Focus();
            }));
        }

 

        private void Txt_formInputCommand_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == System.Windows.Forms.Keys.Back)
            {
                string text = txt_formInputCommand.Text;

                if (autoCompleteStringCollection.Contains(text))
                {

                    txt_formInputCommand.SelectionStart = text.IndexOf(' ') +1;
                    txt_formInputCommand.SelectionLength = text.Length - txt_formInputCommand.SelectionStart ;
                    Debug.WriteLine("selection " + text.IndexOf(' '));
                    //txt_formInputCommand.Select(text.IndexOf(' '),text.Length);
                    //txt_formInputCommand.SelectAll();
                    
                }
            }
        }

        private void Txt_formInputCommand_LostFocus(object sender, EventArgs e)
        {
            Debug.WriteLine("Txt_formInputCommand LostFocus");
        }

        private void Txt_formInputCommand_GotFocus(object sender, EventArgs e)
        {
            Debug.WriteLine("Txt_formInputCommand GetFocus");
        }

        private void Txt_formInputCommand_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Txt_formInputCommand MouseClick");
            txt_formInputCommand.Focus();

        }

        private void TextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (userCommands.Count == 0)
                return;
            if (e.KeyCode == System.Windows.Forms.Keys.Tab)
            {

                e.Handled = true;
                e.SuppressKeyPress = true;

                string[] keyWords = txt_formInputCommand.Lines[txt_formInputCommand.GetLineFromCharIndex(txt_formInputCommand.SelectionStart)].Split(' ');

                int currentPosition = 0;
                for (int i = 0; i < keyWords.Length; i++)
                {
                    if (txt_formInputCommand.SelectionStart >= currentPosition && txt_formInputCommand.SelectionStart <= currentPosition + keyWords[i].Length)
                    {
                        currentPosition += keyWords[i].Length;
                        break;
                    }
                    currentPosition += keyWords[i].Length + 1;
                }

                int nextPosition = (currentPosition == txt_formInputCommand.TextLength) ? 0 : currentPosition;
                foreach (var keyWord in keyWords)
                {
                    if (nextPosition <= txt_formInputCommand.TextLength - keyWord.Length && txt_formInputCommand.Text.Substring(nextPosition, keyWord.Length) == keyWord)
                    {
                        txt_formInputCommand.Select(nextPosition, keyWord.Length);
                        txt_formInputCommand.ScrollToCaret();
                        break;
                    }
                    nextPosition += keyWord.Length + 1;
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {

                if (currentCommandIndex == 0)
                    return;
                e.Handled = true;
                e.SuppressKeyPress = true;
                Debug.WriteLine("Txt_formInputCommand Keydown UP ");

            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {

                if (currentCommandIndex == userCommands.Count - 1)
                    return;
                e.Handled = true;
                e.SuppressKeyPress = true;
                Debug.WriteLine("Txt_formInputCommand Keydown DOWN ");
            }
        }
        private void TextBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            Debug.WriteLine(textBox.Text.Length);
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string command = "";
                //command = textBox.GetLineText(textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex));
                command = textBox.Text;
                command.Trim(" ".ToCharArray());
                Debug.WriteLine(command);
                string result = core.RunCommand(command);

                // txt_inputCommandResult.AppendText(Environment.NewLine);
                //textBox.GetLineText()
                userCommands.Add(command);
                currentCommandIndex = 0;
                txt_inputCommandResult.AppendText(command);
                txt_inputCommandResult.AppendText(Environment.NewLine);

                if (!string.IsNullOrEmpty(result))
                {
                    txt_inputCommandResult.AppendText(result);
                    txt_inputCommandResult.AppendText(Environment.NewLine);
                    txt_formInputCommand.Text = "";
                }
                txt_inputCommandResult.Focus();
                txt_inputCommandResult.CaretIndex = txt_inputCommandResult.Text.Length;
                txt_inputCommandResult.ScrollToEnd();
                txt_formInputCommand.Focus();
                // textBox.CaretIndex = textBox.Text.Length - 1;

            }
            //Need fix this

            if (autoCompleteStringCollection.Contains(txt_formInputCommand.Text))
                return;

            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {

                if (userCommands.Count == 0)
                    return;
                if (currentCommandIndex > 0)
                {
                    currentCommandIndex--;
                    Debug.WriteLine("Txt_formInputCommand Keyup UP: " + userCommands[currentCommandIndex]);
                    txt_formInputCommand.Text = userCommands[currentCommandIndex];

                }

            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {

                if (userCommands.Count == 0)
                    return;
                if (currentCommandIndex < userCommands.Count - 1)
                {
                    currentCommandIndex++;
                    Debug.WriteLine("Txt_formInputCommand Keyup DOWN: " + userCommands[currentCommandIndex]);
                    txt_formInputCommand.Text = userCommands[currentCommandIndex];
                }
            }

        }


        private void txt_inputCommandResult_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var wpfKey = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
            var winformModifiers = e.KeyboardDevice.Modifiers.ToWinforms();
            var winformKeys = (System.Windows.Forms.Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);
            if (winformKeys == System.Windows.Forms.Keys.Enter)
            {
                var textbox = (sender as TextBox);
                int count = 0;
                int caret = textbox.CaretIndex;
                string text = "";
                for (int i = 0; i < textbox.LineCount; i++)
                {
                    if (caret > count && caret < count + textbox.GetLineLength(i))
                    {
                        text = textbox.GetLineText(i);
                        break;
                    }
                    count += textbox.GetLineLength(i);
                }
                if (!string.IsNullOrEmpty(text))
                {
                    txt_formInputCommand.Text = text;
                }
            }


            txt_formInputCommand.Focus();
            if(!autoCompleteStringCollection.Contains(txt_formInputCommand.Text))
                TextBox_KeyUp(txt_formInputCommand, new System.Windows.Forms.KeyEventArgs(winformKeys));
            e.Handled = false;
        }




    }

}
