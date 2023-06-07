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
        public InputCommandsView()
        {
            InitializeComponent();
            autoCompleteInputCommand();
            userCommands = new List<string>();
        }
        public Core Core { get; set; }
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
                    input += " " + parameters[j].Name;
                }
                autoCompleteStringCollection.Add(input);
            }
            txt_formInputCommand.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            txt_formInputCommand.AutoCompleteCustomSource = autoCompleteStringCollection;
            txt_formInputCommand.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
           
            txt_formInputCommand.Focus();
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

                // txt_inputCommandResult.AppendText(Environment.NewLine);
                //textBox.GetLineText()
                userCommands.Add(command);
                currentCommandIndex=0;
                txt_inputCommandResult.AppendText(command);
                txt_inputCommandResult.AppendText(Environment.NewLine);
                string result = Core.RunCommand(command);
                if (!string.IsNullOrEmpty(result))
                {
                    txt_inputCommandResult.AppendText(result);
                    txt_inputCommandResult.AppendText(Environment.NewLine);
                    txt_formInputCommand.Text = "";
                }
                txt_formInputCommand.Focus();
                // textBox.CaretIndex = textBox.Text.Length - 1;
              
            }
            //Need fix this
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (userCommands.Count == 0)
                    return;
                if (currentCommandIndex > 0)
                    currentCommandIndex--;
                txt_formInputCommand.Text = userCommands[currentCommandIndex];
                e.Handled = true;

            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (userCommands.Count == 0)
                    return;
                if (currentCommandIndex < userCommands.Count)
                    currentCommandIndex++;
                txt_formInputCommand.Text = userCommands[currentCommandIndex - 1];
                e.Handled = true;
            }

           


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txt_formInputCommand.Focus();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_formInputCommand.Focus();
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            txt_formInputCommand.Focus();
        }
        private void txt_inputCommandResult_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var wpfKey = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
            var winformModifiers = e.KeyboardDevice.Modifiers.ToWinforms();
            var winformKeys = (System.Windows.Forms.Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);
            txt_formInputCommand.Focus();
            TextBox_KeyUp(txt_formInputCommand, new System.Windows.Forms.KeyEventArgs(winformKeys));
            e.Handled = false;
        }

        private void txt_formInputCommand_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
           
            

        }

        private void UserControl_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txt_formInputCommand.Focus();
        }
    }
  
}
