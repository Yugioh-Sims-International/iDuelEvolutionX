﻿using iDuel_EvolutionX.Service;
using iDuel_EvolutionX.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iDuel_EvolutionX.Model
{
    public class CardControl : Image , INotifyPropertyChanged
    {
        public int id;
        public CardInfo info;               //卡片信息
        public string curAtk;               //当前攻击力
        public string curDef;               //当前防守力
        public BitmapImage backImage;       //卡背
        public BitmapImage originalImage;   //卡图
        private Status status;               //攻守正背
        private Location curLocation;        //当前位置（具体位置+所在层次）
        public List<SignTextBlock> signs;

        public event PropertyChangedEventHandler PropertyChanged;

        public Status Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                showImg();
            }
        }

        public string CurAtk
        {
            get
            {
                return curAtk == null || curAtk.Trim().Equals("") ? null : curAtk + "/" + curDef;
                
            }

            set
            {
                curAtk = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurAtk"));
                }
            }
        }

        public string CurDef
        {
            get
            {
                return curAtk + "/" + curDef;
            }

            set
            {
                curDef = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurAtk"));

                }
            }
        }


        public CardControl(BitmapImage backImg)
        {
            
            this.backImage = backImg;
            this.Status = Status.BACK_ATK;

            Width = 56;//设置卡片宽高
            Height = 81;


            RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotate = new RotateTransform();
            ScaleTransform scale = new ScaleTransform();
            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(rotate);
            this.RenderTransform = group;

            signs = new List<SignTextBlock>();
            
            

            this.ContextMenuOpening += CardUI_ContextMenuOpening;
            this.MouseWheel += CardUI_MouseWheel;
        }

        #region 弹出菜单操作

        private void CardUI_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //屏蔽右键弹出
            e.Handled = true;
        }

        private void CardUI_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.ContextMenu == null)
            {
                e.Handled = true;
                return;
            }

            if (e.Delta > 0)
            {

                
                this.ContextMenu.PlacementTarget = this;
                this.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                this.ContextMenu.IsOpen = true;
                switch (status)
                {
                    case Status.FRONT_ATK:
                    case Status.BACK_ATK:
                        this.ContextMenu.VerticalOffset = 0;
                        this.ContextMenu.HorizontalOffset = -(this.ContextMenu.ActualWidth - this.Width) / 2;
                        break;
                    case Status.FRONT_DEF:
                    case Status.BACK_DEF:
                        this.ContextMenu.HorizontalOffset = this.Width;
                        this.ContextMenu.VerticalOffset = -(this.ContextMenu.ActualWidth - this.Height) / 2;
                        break;
                    default:
                        break;
                }
                
                
            }
           
            ///关闭事件则在菜单初始化所注册

        }

        #endregion

        #region 初始化卡片信息

        public void initCardInfo(CardInfo info)
        {
            this.info = info;
            this.CurAtk = info.atk;
            this.CurDef = info.def;
            setImg(info.id);
            
        }

        #endregion

        #region 卡图操作

        /// <summary>
        /// 初始化卡图相关
        /// </summary>
        /// <param name="id">卡片ID</param>
        private void setImg(string id)
        {

            //1.从本地读
            string str = System.IO.Directory.GetCurrentDirectory() + "\\image\\" + id + ".jpg";
            if (System.IO.File.Exists(str))
            {
                //BitmapImage image = new BitmapImage(new Uri(str, UriKind.Absolute));
                originalImage = new BitmapImage(new Uri(str, UriKind.Absolute));

            }
            //2.加载默认种类卡图

            //3.从网络读

        }

        /// <summary>
        /// 显示图片
        /// </summary>
        public void showImg()
        {
            
            switch (Status)
            {
                case Status.FRONT_ATK:
                    Source = originalImage;
                    break;
                case Status.FRONT_DEF:
                    Source = originalImage;
                    break;
                case Status.BACK_ATK:
                    Source = backImage;
                    break;
                case Status.BACK_DEF:
                    Source = backImage;
                    break;
                default:
                    break;
            }

        }

        #endregion

        #region 状态操作

        /// <summary>
        /// 正面攻击表示
        /// </summary>
        public void set2FrontAtk()
        {
            Status = Status.FRONT_ATK;
            showImg();
            setAngle2zero();
        }

        /// <summary>
        /// 正面防守表示
        /// </summary>
        public void set2FrontDef()
        {
            Status = Status.FRONT_DEF;
            showImg();
            
        }

        /// <summary>
        /// 背面防守表示
        /// </summary>
        public void set2BackDef()
        {
            Status = Status.BACK_DEF;
            showImg();
        }

        /// <summary>
        /// 背面攻击表示
        /// </summary>
        public void set2BackAtk()
        {
            Status = Status.BACK_ATK;
            showImg();
            setAngle2zero();
        }

        /// <summary>
        /// 重置攻击力
        /// </summary>
        public void reSetAtk()
        {
            CurAtk = info.atk;
            CurDef = info.def;
        }

        #endregion

        #region 卡片操作

        /// <summary>
        /// 清除指示物
        /// </summary>
        public void clearSigns()
        {
            Panel parent;
            foreach (SignTextBlock item in signs)
            {
                parent = item.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(item);
                }
                
            }
            signs.Clear();
        }

        /// <summary>
        /// 脱离父控件
        /// </summary>
        /// <returns>操作结果</returns>
        public bool getAwayFromParents()
        {
            if (this.Parent == null)
            {
                return true;
            }

            Panel parent = this.Parent as Panel;
            parent.Children.Remove(this);
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            foreach (SignTextBlock item in signs)
            {
                parent = item.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(item);
                }
                
            }
            return false;
            
        }

        /// <summary>
        /// 竖直居中于父控件
        /// </summary>
        public void centerAtVerticalInParent()
        {
            Canvas parent = this.Parent as Canvas;
            if (parent == null)
            {
                return;
            }

            double top = this.ActualHeight == double.NaN || this.ActualHeight == 0 ? (parent.ActualHeight - this.Height) / 2.0 : (parent.ActualHeight - this.ActualHeight) / 2.0;
            double left = this.ActualWidth == double.NaN || this.ActualHeight == 0 ? (parent.ActualWidth - this.Width) / 2.0 : (parent.ActualWidth - this.ActualWidth) / 2.0;
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);
            setAngle2zero();

        }

        

        /// <summary>
        /// 横向居中于父控件
        /// </summary>
        public void centerAtHorizontalInParent()
        {
            Canvas parent = this.Parent as Canvas;
            if (parent == null)
            {
                return;
            }

            double top = this.ActualHeight == double.NaN || this.ActualHeight == 0 ? (parent.ActualHeight - this.Height) / 2.0 : (parent.ActualHeight - this.ActualHeight) / 2.0;
            double left = this.ActualWidth == double.NaN || this.ActualHeight == 0 ? (parent.ActualWidth - this.Width) / 2.0 : (parent.ActualWidth - this.ActualWidth) / 2.0;
            Canvas.SetTop(this, top);
            Canvas.SetLeft(this, left);

            TransformGroup tfg = this.RenderTransform as TransformGroup;
            RotateTransform rt = tfg.Children[1] as RotateTransform;
            rt.Angle = -90;

        }

        private void setAngle2zero()
        {
            TransformGroup tfg = this.RenderTransform as TransformGroup;
            RotateTransform rt = tfg.Children[1] as RotateTransform;
            rt.Angle = 0;
        }

        #endregion
    }


}
