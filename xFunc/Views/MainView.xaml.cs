﻿// Copyright 2012 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using xFunc.Logics;
using xFunc.Logics.Exceptions;
using xFunc.Logics.Expressions;
using xFunc.Maths;
using xFunc.Maths.Exceptions;
using xFunc.Maths.Expressions;
using xFunc.Presenters;
using xFunc.ViewModels;

namespace xFunc.Views
{

    public partial class MainView : Fluent.RibbonWindow, IMainView
    {

        private MainPresenter mainPresenter;
        private MathTabPresenter mathPresenter;
        private LogicTabPresenter logicPresenter;
        private GraphsTabPresenter graphsPresenter;
        private TruthTableTabPresenter truthTablePresenter;

        public static RoutedCommand DegreeCommand = new RoutedCommand();
        public static RoutedCommand RadianCommand = new RoutedCommand();
        public static RoutedCommand GradianCommand = new RoutedCommand();

        public MainView()
        {
            InitializeComponent();

            mathExpressionBox.Focus();

            this.mainPresenter = new MainPresenter(this);
            this.mathPresenter = new MathTabPresenter(this);
            this.logicPresenter = new LogicTabPresenter(this);
            this.graphsPresenter = new GraphsTabPresenter(this);
            this.truthTablePresenter = new TruthTableTabPresenter(this);

            degreeButton.IsChecked = true;
        }

        private void DergeeButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            this.radianButton.IsChecked = false;
            this.gradianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Degree;
            this.degreeButton.IsChecked = true;
        }

        private void RadianButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            this.degreeButton.IsChecked = false;
            this.gradianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Radian;
            this.radianButton.IsChecked = true;
        }

        private void GradianButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            this.degreeButton.IsChecked = false;
            this.radianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Gradian;
            this.gradianButton.IsChecked = true;
        }

        private void AndleButtons_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            if (tabControl.SelectedItem == mathTab)
                args.CanExecute = true;
            else
                args.CanExecute = false;
        }

        private void InsertChar_Click(object o, RoutedEventArgs args)
        {
            var tag = ((Button)o).Tag.ToString();
            TextBox tb = null;
            if (tabControl.SelectedItem == mathTab)
                tb = mathExpressionBox;
            else if (tabControl.SelectedItem == logicTab)
                tb = logicExpressionBox;
            else if (tabControl.SelectedItem == graphsTab)
                tb = graphExpressionBox;

            var prevSelectionStart = tb.SelectionStart;
            tb.Text = tb.Text.Insert(prevSelectionStart, tag);
            tb.SelectionStart = prevSelectionStart + tag.Length;
            tb.Focus();
        }

        private void InsertFunc_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = null;
            if (tabControl.SelectedItem == mathTab)
                tb = mathExpressionBox;
            else if (tabControl.SelectedItem == logicTab)
                tb = logicExpressionBox;
            else if (tabControl.SelectedItem == graphsTab)
                tb = graphExpressionBox;

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(").Insert(prevSelectionStart + prevSelectionLength + func.Length + 1, ")");
                tb.SelectionStart = prevSelectionStart + func.Length + prevSelectionLength + 2;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func + "()");
                tb.SelectionStart = prevSelectionStart + func.Length + 1;
            }

            tb.Focus();
        }

        private void InsertInv_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = null;
            if (tabControl.SelectedItem == mathTab)
                tb = mathExpressionBox;
            else if (tabControl.SelectedItem == logicTab)
                tb = logicExpressionBox;
            else if (tabControl.SelectedItem == graphsTab)
                tb = graphExpressionBox;

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, "(").Insert(prevSelectionStart + prevSelectionLength + 1, ")" + func);
                tb.SelectionStart = prevSelectionStart + prevSelectionLength + func.Length + 2;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func);
                tb.SelectionStart = prevSelectionStart + func.Length;
            }

            tb.Focus();
        }

        private void InsertDoubleArgFunc_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = null;
            if (tabControl.SelectedItem == mathTab)
                tb = mathExpressionBox;
            else if (tabControl.SelectedItem == logicTab)
                tb = logicExpressionBox;
            else if (tabControl.SelectedItem == graphsTab)
                tb = graphExpressionBox;

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(").Insert(prevSelectionStart + prevSelectionLength + func.Length + 1, ", )");
                tb.SelectionStart = prevSelectionStart + func.Length + prevSelectionLength + 3;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(, )");
                tb.SelectionStart = prevSelectionStart + func.Length + 1;
            }

            tb.Focus();
        }

        private void GenerateTruthTable(IEnumerable<ILogicExpression> exps, LogicParameterCollection parameters)
        {
            truthTableGridView.Columns.Clear();

            truthTableGridView.Columns.Add(new GridViewColumn
            {
                Header = "#",
                DisplayMemberBinding = new Binding("Index")
            });
            for (int i = 0; i < parameters.Count; i++)
            {
                truthTableGridView.Columns.Add(new GridViewColumn
                {
                    Header = parameters[i],
                    DisplayMemberBinding = new Binding(string.Format("VarsValues[{0}]", i))
                });
            }
            for (int i = 0; i < exps.Count() - 1; i++)
            {
                truthTableGridView.Columns.Add(new GridViewColumn
                {
                    Header = exps.ElementAt(i),
                    DisplayMemberBinding = new Binding(string.Format("Values[{0}]", i))
                });
            }
            if (exps.Count() != 0)
                truthTableGridView.Columns.Add(new GridViewColumn
                {
                    Header = exps.ElementAt(exps.Count() - 1),
                    DisplayMemberBinding = new Binding("Result")
                });
        }

        private void mathExpressionBox_KeyUp(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrWhiteSpace(mathExpressionBox.Text))
            {
                try
                {
                    mathPresenter.Add(mathExpressionBox.Text);
                    statusBox.Text = string.Empty;
                }
                catch (MathLexerException mle)
                {
                    statusBox.Text = mle.Message;
                }
                catch (MathParserException mpe)
                {
                    statusBox.Text = mpe.Message;
                }
                catch (DivideByZeroException dbze)
                {
                    statusBox.Text = dbze.Message;
                }
                catch (ArgumentNullException ane)
                {
                    statusBox.Text = ane.Message;
                }
                catch (ArgumentException ae)
                {
                    statusBox.Text = ae.Message;
                }
                catch (FormatException fe)
                {
                    statusBox.Text = fe.Message;
                }
                catch (OverflowException oe)
                {
                    statusBox.Text = oe.Message;
                }
                catch (KeyNotFoundException)
                {
                    statusBox.Text = "The variable not found.";
                }
                catch (IndexOutOfRangeException)
                {
                    statusBox.Text = "Perhaps, variables have entered incorrectly.";
                }
                catch (InvalidOperationException ioe)
                {
                    statusBox.Text = ioe.Message;
                }
                catch (NotSupportedException)
                {
                    statusBox.Text = "This operation is not supported.";
                }

                mathExpressionBox.Text = string.Empty;
            }
        }

        private void logicExpressionBox_KeyUp(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrWhiteSpace(logicExpressionBox.Text))
            {
                try
                {
                    logicPresenter.Add(logicExpressionBox.Text);
                    statusBox.Text = string.Empty;
                }
                catch (LogicLexerException lle)
                {
                    statusBox.Text = lle.Message;
                }
                catch (LogicParserException lpe)
                {
                    statusBox.Text = lpe.Message;
                }
                catch (DivideByZeroException dbze)
                {
                    statusBox.Text = dbze.Message;
                }
                catch (ArgumentNullException ane)
                {
                    statusBox.Text = ane.Message;
                }
                catch (ArgumentException ae)
                {
                    statusBox.Text = ae.Message;
                }
                catch (FormatException fe)
                {
                    statusBox.Text = fe.Message;
                }
                catch (OverflowException oe)
                {
                    statusBox.Text = oe.Message;
                }
                catch (KeyNotFoundException)
                {
                    statusBox.Text = "The variable not found.";
                }
                catch (IndexOutOfRangeException)
                {
                    statusBox.Text = "Perhaps, variables have entered incorrectly.";
                }
                catch (InvalidOperationException ioe)
                {
                    statusBox.Text = ioe.Message;
                }
                catch (NotSupportedException)
                {
                    statusBox.Text = "This operation is not supported.";
                }

                logicExpressionBox.Text = string.Empty;
            }
        }

        private void graphExpBox_KeyUp(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrWhiteSpace(graphExpressionBox.Text))
            {
                try
                {
                    graphsPresenter.Add(graphExpressionBox.Text);
                    statusBox.Text = string.Empty;
                }
                catch (MathLexerException mle)
                {
                    statusBox.Text = mle.Message;
                }
                catch (MathParserException mpe)
                {
                    statusBox.Text = mpe.Message;
                }
                catch (DivideByZeroException dbze)
                {
                    statusBox.Text = dbze.Message;
                }
                catch (ArgumentNullException ane)
                {
                    statusBox.Text = ane.Message;
                }
                catch (ArgumentException ae)
                {
                    statusBox.Text = ae.Message;
                }
                catch (FormatException fe)
                {
                    statusBox.Text = fe.Message;
                }
                catch (OverflowException oe)
                {
                    statusBox.Text = oe.Message;
                }
                catch (KeyNotFoundException)
                {
                    statusBox.Text = "The variable not found.";
                }
                catch (IndexOutOfRangeException)
                {
                    statusBox.Text = "Perhaps, variables have entered incorrectly.";
                }
                catch (InvalidOperationException ioe)
                {
                    statusBox.Text = ioe.Message;
                }
                catch (NotSupportedException)
                {
                    statusBox.Text = "This operation is not supported.";
                }

                graphExpressionBox.Text = string.Empty;
            }
        }

        private void truthTableExpressionBox_KeyUp(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrWhiteSpace(truthTableExpressionBox.Text))
            {
                try
                {
                    truthTablePresenter.Generate(truthTableExpressionBox.Text);
                    GenerateTruthTable(truthTablePresenter.Expressions, truthTablePresenter.Parameters);
                    truthTableList.ItemsSource = truthTablePresenter.Table;

                    statusBox.Text = string.Empty;
                }
                catch (LogicLexerException lle)
                {
                    statusBox.Text = lle.Message;
                }
                catch (LogicParserException lpe)
                {
                    statusBox.Text = lpe.Message;
                }
                catch (DivideByZeroException dbze)
                {
                    statusBox.Text = dbze.Message;
                }
                catch (ArgumentNullException ane)
                {
                    statusBox.Text = ane.Message;
                }
                catch (ArgumentException ae)
                {
                    statusBox.Text = ae.Message;
                }
                catch (FormatException fe)
                {
                    statusBox.Text = fe.Message;
                }
                catch (OverflowException oe)
                {
                    statusBox.Text = oe.Message;
                }
                catch (KeyNotFoundException)
                {
                    statusBox.Text = "The variable not found.";
                }
                catch (IndexOutOfRangeException)
                {
                    statusBox.Text = "Perhaps, variables have entered incorrectly.";
                }
                catch (InvalidOperationException ioe)
                {
                    statusBox.Text = ioe.Message;
                }
                catch (NotSupportedException)
                {
                    statusBox.Text = "This operation is not supported.";
                }
            }
        }

        private void graphsList_SelectionChanged(object o, SelectionChangedEventArgs args)
        {
            if (graphsList.SelectedIndex >= 0)
                plot.Expression = graphsList.Items[graphsList.SelectedIndex] as IMathExpression;
            else
                plot.Expression = null;
        }

        private void tabControl_SelectionChanged(object o, SelectionChangedEventArgs args)
        {
            if (tabControl.SelectedItem == logicTab)
            {
                numberToolBar.Visibility = Visibility.Collapsed;
                standartToolBar.Visibility = Visibility.Collapsed;
                trigonometricToolBar.Visibility = Visibility.Collapsed;
                hyperbolicToolBar.Visibility = Visibility.Collapsed;
                constantsMathToolBar.Visibility = Visibility.Collapsed;
                additionalToolBar.Visibility = Visibility.Collapsed;

                standartLogicToolBar.Visibility = Visibility.Visible;
                constantsLogicToolBar.Visibility = Visibility.Visible;
            }
            else
            {
                numberToolBar.Visibility = Visibility.Visible;
                standartToolBar.Visibility = Visibility.Visible;
                trigonometricToolBar.Visibility = Visibility.Visible;
                hyperbolicToolBar.Visibility = Visibility.Visible;
                constantsMathToolBar.Visibility = Visibility.Visible;
                additionalToolBar.Visibility = Visibility.Visible;

                standartLogicToolBar.Visibility = Visibility.Collapsed;
                constantsLogicToolBar.Visibility = Visibility.Collapsed;
            }
        }

        private void aboutButton_Click(object o, RoutedEventArgs args)
        {
            AboutView aboutView = new AboutView() { Owner = this };
            aboutView.ShowDialog();
        }

        private void removeMath_Click(object o, RoutedEventArgs args)
        {
            var item = ((Button)o).Tag as MathWorkspaceItemViewModel;

            mathPresenter.Remove(item);
        }

        private void removeLogic_Click(object o, RoutedEventArgs args)
        {
            var item = (o as Button).Tag as LogicWorkspaceItemViewModel;

            logicPresenter.Remove(item);
        }

        private void removeGraph_Click(object o, RoutedEventArgs args)
        {
            var item = (o as Button).Tag as IMathExpression;

            graphsPresenter.Remove(item);
        }

        public IEnumerable<MathWorkspaceItemViewModel> MathExpressions
        {
            set
            {
                mathExpsListBox.ItemsSource = value;
                mathExpsListBox.ScrollIntoView(value.LastOrDefault());
            }
        }

        public IEnumerable<IMathExpression> Graphs
        {
            set
            {
                graphsList.ItemsSource = value;
                graphsList.SelectedIndex = value.Count() - 1;
            }
        }

        public IEnumerable<LogicWorkspaceItemViewModel> LogicExpressions
        {
            set
            {
                logicExpsListBox.ItemsSource = value;
                logicExpsListBox.ScrollIntoView(value.LastOrDefault());
            }
        }

    }

}
