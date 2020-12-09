﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cryptography.Core;
using Cryptography.Core.Ciphers;
using Cryptography.Core.Enums;

namespace Cryptography.WinFormsApp
{
    public partial class CryptoApp : Form
    {
        private CipherFactory cipherFactory;
        
        public CryptoApp()
        {
            InitializeComponent();
            
            cipherFactory = new CipherFactory();
            
            cipherFactory.RegisterCipher(new Blowfish());
            cipherFactory.RegisterCipher(new IDEA());
            cipherFactory.RegisterCipher(new RC5());
            cipherFactory.RegisterCipher(new Twofish());
        }

        private void CryptoApp_Load(object sender, EventArgs e)
        {
            foreach (var availableCipher in cipherFactory.GetAvailableCiphers())
            {
                CipherAlgorithmBox.Items.Add(availableCipher);
            }

            CipherAlgorithmBox.SelectedItem = CipherAlgorithmBox.Items[0];
            
            foreach (InputType type in Enum.GetValues(typeof(InputType)))
            {
                TextTypeBox.Items.Add(type.ToString());
            }
            
            TextTypeBox.SelectedItem = TextTypeBox.Items[0];
            
            AppleModeButton();
        }
        
        private void CipherAlgorithmBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cipherFactory.SelectCipher(CipherAlgorithmBox.Text);
        }
        
        private void TextTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!Enum.TryParse(TextTypeBox.Text, out InputType parsed))
            {
                // TODO: show error message box
            }

            cipherFactory.TextType = parsed;
            UpdateBits(InputText, InputBits);
            UpdateBits(KeyText, KeyBits); 
        }
        
        private void InputText_TextChanged(object sender, EventArgs e)
        {
            UpdateBits(InputText, InputBits);
        }

        private void KeyText_TextChanged(object sender, EventArgs e)
        {
            UpdateBits(KeyText, KeyBits);
        }
        
        private void OutputText_TextChanged(object sender, EventArgs e)
        {
            UpdateBits(OutputText, OutputBits);
        }

        private void CipherMode_Click(object sender, EventArgs e)
        {
            SwitchModeButton();
        }

        private void CalculateBtn_Click(object sender, EventArgs e)
        {
            CipherResult result = cipherFactory.RunCipher(InputText.Text, KeyText.Text);

            if (result.HasParsingErrors())
            {
                ShowErrorMessage("Parse Error", "There is an error in the format of your text and/or key!");
                return;
            }

            if (result.HasInvalidInputAndKey())
            {
                ShowErrorMessage("Invalid Input", "text and/or key does not conform to the requirements of the cipher!");
                return;
            }

            if (!result.HasOutput())
            {
                ShowErrorMessage("Cipher Error", "Hmmmm... there was an error generating the output for your cipher!");
                return;
            }

            OutputText.Text = result.OutputText;
        }
    }
}