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
using System.Windows.Navigation;
using System.Windows.Shapes;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Presenters;
using xFunc.Resources;
using xFunc.ViewModels;

namespace xFunc.Views
{

    public partial class MathControl : UserControl, IMathView
    {

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(MathControl));

        private MathPresenter presenter;

        public MathControl()
        {
            InitializeComponent();
        }

        public MathControl(MathPresenter presenter)
        {
            this.presenter = presenter;

            InitializeComponent();
        }

        private void MathExpEnter()
        {
            try
            {
                presenter.Add(mathExpressionBox.Text);
                var count = mathExpsListBox.Items.Count;
                if (count > 0)
                    mathExpsListBox.ScrollIntoView(mathExpsListBox.Items[count - 1]);
                Status = string.Empty;
            }
            catch (MathLexerException mle)
            {
                Status = mle.Message;
            }
            catch (MathParserException mpe)
            {
                Status = mpe.Message;
            }
            catch (MathParameterIsReadOnlyException mpiroe)
            {
                Status = mpiroe.Message;
            }
            catch (DivideByZeroException dbze)
            {
                Status = dbze.Message;
            }
            catch (ArgumentNullException ane)
            {
                Status = ane.Message;
            }
            catch (ArgumentException ae)
            {
                Status = ae.Message;
            }
            catch (FormatException fe)
            {
                Status = fe.Message;
            }
            catch (OverflowException oe)
            {
                Status = oe.Message;
            }
            catch (KeyNotFoundException)
            {
                Status = Resource.VariableNotFoundExceptionError;
            }
            catch (IndexOutOfRangeException)
            {
                Status = Resource.IndexOutOfRangeExceptionError;
            }
            catch (InvalidOperationException ioe)
            {
                Status = ioe.Message;
            }
            catch (NotSupportedException)
            {
                Status = Resource.NotSupportedOperationError;
            }

            mathExpressionBox.Text = string.Empty;
        }

        private void mathExpressionBox_KeyUp(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrWhiteSpace(mathExpressionBox.Text))
            {
                MathExpEnter();
            }
        }

        private void removeMath_Click(object o, RoutedEventArgs args)
        {
            var item = ((Button)o).Tag as MathWorkspaceItemViewModel;

            presenter.Remove(item);
        }

        public string Status
        {
            get
            {
                return (string)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        public MathPresenter Presenter
        {
            get
            {
                return presenter;
            }
            set
            {
                presenter = value;
            }
        }

        public IEnumerable<MathWorkspaceItemViewModel> MathExpressions
        {
            set
            {
                mathExpsListBox.ItemsSource = value;
            }
        }

    }

}
