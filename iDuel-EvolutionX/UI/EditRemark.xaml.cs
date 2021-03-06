﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iDuel_EvolutionX.UI
{
    public delegate void EditRemarkDelegate(String remark);

    /// <summary>
    /// EditRemark.xaml 的交互逻辑
    /// </summary>
    public partial class EditRemark : Window
    {
        public EditRemarkDelegate sendResult;

        public EditRemark()
        {
            InitializeComponent();
            tb_remark.Focus();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sendResult(tb_remark.Text);
            this.Close();
        }
    }
}
